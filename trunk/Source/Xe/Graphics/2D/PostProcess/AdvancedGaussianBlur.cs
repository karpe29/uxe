using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Xe.Graphics2D.PostProcess
{
    public class AdvancedGaussianBlur : PostProcessEffect
	{
		#region Variables and Properties

		EffectParameter weightsParameter, offsetsParameter;
        EffectParameter bloomScaleParameter = null;

		int m_SampleCount;

		private float m_BlurAmount = 0;
        private readonly float[] Weights = null;
        private readonly Vector2[] Offsets = null;

        public float BlurAmount
        {
            get
            {
                return m_BlurAmount;
            }
            set
            {
                m_BlurAmount = value;
            }
        }

        public float BloomScale
        {
            get
            {
                return bloomScaleParameter.GetValueSingle();
            }
            set
            {
                bloomScaleParameter.SetValue(value);
            }
		}

		#endregion

        internal AdvancedGaussianBlur(GraphicsDevice graphicsDevice, ContentManager contentManager)
			:base (graphicsDevice,contentManager, "AdvancedGaussianBlur")
        {
			EffectParameter tmpParam = m_effect.Parameters["SampleCount"];
			m_SampleCount = tmpParam.GetValueInt32();

			Weights = new float[m_SampleCount];
			Offsets = new Vector2[m_SampleCount];
            
            weightsParameter = m_effect.Parameters["SampleWeights"];
            offsetsParameter = m_effect.Parameters["SampleOffsets"];

            bloomScaleParameter = m_effect.Parameters["BloomScale"];
        }

        internal override void Begin()
        {
            weightsParameter.SetValue(Weights);
            offsetsParameter.SetValue(Offsets);

			base.Begin();
        }

        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
		/// Called in PostProcessManger, once for V blur, once for H blur
        /// </summary>
        internal void SetBlurParameters(float dx, float dy)
        {
            // The first sample always has a zero offset.
            Weights[0] = ComputeGaussian(0);
            Offsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = Weights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < m_SampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                Weights[i * 2 + 1] = weight;
                Weights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                Offsets[i * 2 + 1] = delta;
                Offsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(Weights);
            offsetsParameter.SetValue(Offsets);
        }

        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        private float ComputeGaussian(float n)
        {
            float theta = m_BlurAmount + float.Epsilon;

            return theta = (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}

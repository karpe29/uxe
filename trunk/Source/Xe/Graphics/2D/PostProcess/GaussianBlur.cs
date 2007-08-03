using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Xe.Graphics2D.PostProcessing
{
    public class GaussianBlur
    {
        #region Effect Code
        private const string EffectCode1 = "// Pixel shader applies a one dimensional gaussian blur filter.\r\n"
            + "// This is to be used twice by the postprocess, first to\r\n"
            + "// blur horizontally, and then again to blur vertically.\r\n"
            + "sampler TextureSampler : register(s0);\r\n"
            + "float BloomScale = 2.0f;\r\n"
            + "#define SAMPLE_COUNT ";

        private const string EffectCode2 = "\r\n"
            + "float2 SampleOffsets[SAMPLE_COUNT];\r\n"
            + "float SampleWeights[SAMPLE_COUNT];\r\n"
            + "float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0\r\n"
            + "{\r\n"
            + "    float4 c = 0;\r\n"
            + "    // Combine a number of weighted image filter taps.\r\n"
            + "    for (int i = 0; i < SAMPLE_COUNT; i++)\r\n"
            + "    {\r\n"
            + "        c += tex2D(TextureSampler, texCoord + SampleOffsets[i]) * SampleWeights[i];\r\n"
            + "    }\r\n"
            + "    return c * BloomScale;\r\n"
            + "}\r\n"
            + "technique GaussianBlur\r\n"
            + "{\r\n"
            + "    pass Pass1\r\n"
            + "    {\r\n"
            + "        PixelShader = compile ps_2_0 PixelShader();\r\n"
            + "        ZEnable = false;\r\n"
            + "    }\r\n"
            + "}";
        #endregion

        EffectParameter weightsParameter, offsetsParameter;
        EffectParameter bloomScaleParameter = null;
        private int m_SampleCount = 15;
        private EffectPass GaussianBlurPass = null;
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

        Effect effect = null;

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

        internal GaussianBlur(GraphicsDevice graphicsDevice) : this(graphicsDevice, 13)
        {
        }
        internal GaussianBlur(GraphicsDevice graphicsDevice, int sampleCount)
        {
            m_SampleCount = sampleCount;

            Weights = new float[sampleCount];
            Offsets = new Vector2[sampleCount];

            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode1 + sampleCount + EffectCode2, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            effect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
            
            weightsParameter = effect.Parameters["SampleWeights"];
            offsetsParameter = effect.Parameters["SampleOffsets"];
            bloomScaleParameter = effect.Parameters["BloomScale"];

            GaussianBlurPass = effect.Techniques["GaussianBlur"].Passes[0];

            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        internal void Begin()
        {
            weightsParameter.SetValue(Weights);
            offsetsParameter.SetValue(Offsets);
            effect.Begin();
            GaussianBlurPass.Begin();
        }

        internal void End()
        {
            GaussianBlurPass.End();
            effect.End();
        }


        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            effect.Dispose();
        }

        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
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

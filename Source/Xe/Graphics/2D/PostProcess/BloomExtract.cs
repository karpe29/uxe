using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcess
{
    public sealed class BloomExtract
    {
        #region Effect Code
        private const string EffectCode = "// Pixel shader extracts the brighter areas of an image.\r\n"
            + "sampler TextureSampler : register(s0);\r\n"
            + "float Threshold = 0.25F;\r\n"
            + "float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0\r\n"
            + "{\r\n"
            + "    // Look up the original image color.\r\n"
            + "    float4 c = tex2D(TextureSampler, texCoord);\r\n"
            + "    // Adjust it to keep only values brighter than the specified threshold.\r\n"
            + "    return saturate((c - Threshold) / (1 - Threshold));\r\n"
            + "}\r\n"
            + "technique BrightExtract\r\n"
            + "{\r\n"
            + "    pass Pass1\r\n"
            + "    {\r\n"
            + "        PixelShader = compile ps_2_0 PixelShader();\r\n"
            + "        ZEnable = false;\r\n"
            + "    }\r\n"
            + "}";
        #endregion

        /// <summary>
        /// Bloom Extract Threshold, this value should be in [0.0 1.0] range. 
        /// </summary>
        public float Threshold
        {
            get
            {
                return ThresholdParameter.GetValueSingle();
            }
            set
            {
                ThresholdParameter.SetValue(value);
            }
        }

        private GraphicsDevice m_graphicsDevice = null;
        private Effect effect = null;
        private EffectParameter ThresholdParameter = null;
        private EffectPass BrightExtractPass = null;

        public BloomExtract(GraphicsDevice graphicsDevice)
        {
            m_graphicsDevice = graphicsDevice;
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            effect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
            ThresholdParameter = effect.Parameters["Threshold"];
            BrightExtractPass = effect.Techniques["BrightExtract"].Passes[0];

            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            effect.Dispose();
        }

        internal void Begin()
        {
            effect.Begin();
            BrightExtractPass.Begin();
        }

        internal void End()
        {
            BrightExtractPass.End();
            effect.End();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcess
{
    public class ToneMapping
    {
        #region Effect Code
        private const string EffectCode = "sampler TextureSampler : register(s0);\r\n"
            + "float exposure = 0.0;\r\n"
            + "float defog = 0;\r\n"
            + "float gamma = 1.0f / 2.2f;\r\n"
            + "float3 fogColor = { 1.0, 1.0, 1.0 };\r\n"
            + "float4 TonemapPS(float4 texcoord  : TEXCOORD0) : COLOR\r\n"
            + "{\r\n"
            + "     float3 c = tex2D(TextureSampler, texcoord);\r\n"
            + "  	c = max(0, c - defog * fogColor);\r\n"
            + "	    c *= pow(2.0f, exposure);\r\n"
            + "     // gamma correction - could use texture lookups for this\r\n"
            + "     c = pow(c, gamma);\r\n"
            + "     return float4(c.rgb, 1.0);\r\n"
            + "}\r\n"
            + "technique ToneMapping\r\n"
            + "{\r\n"
            + "	pass p0 \r\n"
            + "	{\r\n"
            + "		cullmode = none;\r\n"
            + "		ZEnable = false;\r\n"
            + "		AlphaBlendEnable = false;\r\n"
            + "		PixelShader = compile ps_2_0 TonemapPS();\r\n"
            + "	}\r\n"
            + "}";
        #endregion

        private Effect effect = null;
        private EffectPass ToneMappingPass = null;

        private EffectParameter exposureParameter = null;
        private EffectParameter defogParameter = null;
        private EffectParameter gammaParameter = null;


        public float Exposure
        {
            get
            {
                return exposureParameter.GetValueSingle();
            }
            set
            {
                exposureParameter.SetValue(value);
            }
        }
        public float DeFog
        {
            get
            {
                return defogParameter.GetValueSingle();
            }
            set
            {
                defogParameter.SetValue(value);
            }
        }
        public float Gamma
        {
            get
            {
                return gammaParameter.GetValueSingle();
            }
            set
            {
                gammaParameter.SetValue(value);
            }
        }

        internal ToneMapping(GraphicsDevice graphicsDevice)
        {
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            effect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
            ToneMappingPass = effect.Techniques["ToneMapping"].Passes[0];

            exposureParameter = effect.Parameters["exposure"];
            defogParameter = effect.Parameters["defog"];
            gammaParameter = effect.Parameters["gamma"];

            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            effect.Dispose();
        }

        internal void Begin()
        {
            effect.Begin();
            ToneMappingPass.Begin();
        }

        internal void End()
        {
            ToneMappingPass.End();
            effect.End();
        }
    }
}

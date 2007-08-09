using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcess
{
    public class Combine
    {
        #region Effect Code
        private const string EffectCode = 
            "sampler BloomSampler : register(s0);\r\n"
            + "sampler BaseSampler : register(s1);\r\n"
            + "\r\n"
            + "float Intensity1 = 1;\r\n"
            + "float Intensity2 = 1;\r\n"
            + "\r\n"
            + "float Saturation1 = 1;\r\n"
            + "float Saturation2 = 1;\r\n"
            + "\r\n"
            + "float3 SaturationColor = float3(0.3, 0.59, 0.11);\r\n"
            + "\r\n"
            + "\r\n"
            + "// Helper for modifying the saturation of a color.\r\n"
            + "float4 AdjustSaturation(float4 color, float saturation)\r\n"
            + "{\r\n"
            + "    // The constants 0.3, 0.59, and 0.11 are chosen because the\r\n"
            + "    // human eye is more sensitive to green light, and less to blue.\r\n"
            + "    float grey = dot(color, SaturationColor);\r\n"
            + "\r\n"
            + "    return lerp(grey, color, saturation);\r\n"
            + "}\r\n"
            + "\r\n"
            + "\r\n"
            + "float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0\r\n"
            + "{\r\n"
            + "    // Look up the bloom and original base image colors.\r\n"
            + "    float4 bloom = tex2D(BloomSampler, texCoord);\r\n"
            + "    float4 base = tex2D(BaseSampler, texCoord);\r\n"
            + "    \r\n"
            + "    // Adjust color saturation and intensity.\r\n"
            + "    bloom = AdjustSaturation(bloom, Saturation1) * Intensity1;\r\n"
            + "    base = AdjustSaturation(base, Saturation2) * Intensity2;\r\n"
            + "    \r\n"
            + "    // Darken down the base image in areas where there is a lot of bloom,\r\n"
            + "    // to prevent things looking excessively burned-out.\r\n"
            + "    base *= (1 - saturate(bloom));\r\n"
            + "    \r\n"
            + "    // Combine the two images.\r\n"
            + "    return base + bloom;\r\n"
            + "}\r\n"
            + "technique Combine\r\n"
            + "{\r\n"
            + "    pass Pass1\r\n"
            + "    {\r\n"
            + "        PixelShader = compile ps_2_0 PixelShader();\r\n"
            + "        ZEnable = false;\r\n"
            + "    }\r\n"
            + "}";
        #endregion

        public float Intensity1
        {
            get
            {
                return Intensity1Parameter.GetValueSingle();
            }
            set
            {
                Intensity1Parameter.SetValue(value);
            }
        }

        public float Intensity2
        {
            get
            {
                return Intensity2Parameter.GetValueSingle();
            }
            set
            {
                Intensity2Parameter.SetValue(value);
            }
        }
        public float Saturation1
        {
            get
            {
                return Saturation1Parameter.GetValueSingle();
            }
            set
            {
                Saturation1Parameter.SetValue(value);
            }
        }
        public float Saturation2
        {
            get
            {
                return Saturation2Parameter.GetValueSingle();
            }
            set
            {
                Saturation2Parameter.SetValue(value);
            }
        }

        public float SaturationColor
        {
            get
            {
                return SaturationColorParameter.GetValueSingle();
            }
            set
            {
                SaturationColorParameter.SetValue(value);
            }
        }

        private Effect effect = null;
        private EffectParameter Intensity1Parameter = null;
        private EffectParameter Intensity2Parameter = null;
        private EffectParameter Saturation1Parameter = null;
        private EffectParameter Saturation2Parameter = null;
        private EffectParameter SaturationColorParameter = null;

        private EffectPass CombinePass = null;

        internal Combine(GraphicsDevice graphicsDevice)
        {
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            effect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());

            Intensity1Parameter = effect.Parameters["Intensity1"];
            Intensity2Parameter = effect.Parameters["Intensity2"];
            Saturation1Parameter = effect.Parameters["Saturation1"];
            Saturation2Parameter = effect.Parameters["Saturation2"];
            SaturationColorParameter = effect.Parameters["SaturationColor"];

            CombinePass = effect.Techniques["Combine"].Passes[0];

            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        internal void Begin()
        {
            effect.Begin();
            CombinePass.Begin();
        }

        internal void End()
        {
            CombinePass.End();
            effect.End();
        }

        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            effect.Dispose();
        }
    }
}

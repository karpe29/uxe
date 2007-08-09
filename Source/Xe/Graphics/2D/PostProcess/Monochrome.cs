using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcess
{
    public class Monochrome
    {
        #region Effect Code
        private const string EffectCode = "sampler TextureSampler : register(s0);\r\n"
            + "float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0\r\n"
            + "{\r\n"
            + "    // Look up the original image color.\r\n"
            + "    return dot((float3)tex2D( TextureSampler, texCoord), float3(0.2125f, 0.7154f, 0.0721f));\r\n"
            + "}\r\n"
            + "technique Monochrome\r\n"
            + "{\r\n"
            + "    pass Pass1\r\n"
            + "    {\r\n"
            + "        PixelShader = compile ps_2_0 PixelShader();\r\n"
            + "        ZEnable = false;\r\n"
            + "    }\r\n"
            + "}";
        #endregion

        private Effect shaderEffect = null;
        private EffectPass MonochromePass = null;

        internal Monochrome(GraphicsDevice graphicsDevice)
        {
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            shaderEffect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
            MonochromePass = shaderEffect.Techniques["Monochrome"].Passes[0];

            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            shaderEffect.Dispose();
        }
        internal void Begin()
        {
            shaderEffect.Begin();
            MonochromePass.Begin();
        }

        internal void End()
        {
            MonochromePass.End();
            shaderEffect.End();
        }
    }
}

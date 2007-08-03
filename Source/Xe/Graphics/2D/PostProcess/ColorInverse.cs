using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcessing
{
    public class ColorInverse
    {
        #region Effect Code
        private const string EffectCode = "sampler TextureSampler : register(s0);\r\n"
            + "float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0\r\n"
            + "{\r\n"
            + "    // Look up the original image color.\r\n"
            + "    return 1.0F - tex2D(TextureSampler, texCoord);\r\n"
            + "}\r\n"
            + "technique ColorInverse\r\n"
            + "{\r\n"
            + "    pass Pass1\r\n"
            + "    {\r\n"
            + "        PixelShader = compile ps_2_0 PixelShader();\r\n"
            + "        ZEnable = false;\r\n"
            + "    }\r\n"
            + "}";
        #endregion

        private readonly Effect shaderEffect = null;
        private EffectPass ColorInversePass = null;

        internal ColorInverse(GraphicsDevice graphicsDevice)
        {
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            shaderEffect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
            ColorInversePass = shaderEffect.Techniques["ColorInverse"].Passes[0];
            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            shaderEffect.Dispose();
        }

        internal void Begin()
        {
            shaderEffect.Begin();
            ColorInversePass.Begin();
        }

        internal void End()
        {
            ColorInversePass.End();
            shaderEffect.End();
        }
    }
}

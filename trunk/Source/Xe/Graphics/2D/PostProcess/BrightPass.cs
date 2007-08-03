using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcessing
{
    public class BrightPass
    {
        #region Effect Code
        private const string EffectCode = "sampler TextureSampler : register(s0);\r\n"
		+ "float Luminance = 0.08f;\r\n"
		+ "static const float fMiddleGray = 0.18f;\r\n"
		+ "static const float fWhiteCutoff = 0.8f;\r\n"
		+ "float4 BrightPassFilter( in float2 Tex : TEXCOORD0 ) : COLOR0\r\n"
		+ "{\r\n"
		+ "    float3 ColorOut = tex2D(TextureSampler, Tex );\r\n"
		+ "    ColorOut *= fMiddleGray / ( Luminance + 0.001f );\r\n"
		+ "    ColorOut *= ( 1.0f + ( ColorOut / ( fWhiteCutoff * fWhiteCutoff ) ) );\r\n"
		+ "    ColorOut -= 5.0f;\r\n"
		+ "    ColorOut = max( ColorOut, 0.0f );\r\n"
		+ "    ColorOut /= ( 10.0f + ColorOut );\r\n"
		+ "    return float4( ColorOut, 1.0f );\r\n"
		+ "}\r\n"
        + "technique BrightPass\r\n"
		+ "{\r\n"
		+ "    pass p0\r\n"
		+ "    {\r\n"
		+ "        VertexShader = null;\r\n"
		+ "        PixelShader = compile ps_2_0 BrightPassFilter();\r\n"
		+ "        ZEnable = false;\r\n"
		+ "    }\r\n"
		+ "}";
        #endregion
        private Effect effect = null;
        private EffectPass BrightPassPass = null;

        internal BrightPass(GraphicsDevice graphicsDevice)
        {
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            effect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
            BrightPassPass = effect.Techniques["BrightPass"].Passes[0];

            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            effect.Dispose();
        }

        internal void Begin()
        {
            effect.Begin();
            BrightPassPass.Begin();
        }

        internal void End()
        {
            BrightPassPass.End();
            effect.End();
        }
    }
}

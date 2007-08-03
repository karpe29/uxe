using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcessing
{
    internal class NormalEdgeDetect
    {
        #region Effect Code
        private const string EffectCode = "sampler NormalSampler : register(s0);\r\n"
                + "float2 PixelKernel[4] =\r\n"
                + "{\r\n"
                + "    { 0,  1},\r\n"
                + "    { 1,  0},\r\n"
                + "    { 0, -1},\r\n"
                + "    {-1,  0}\r\n"
                + "};\r\n"
                + "float2 TexelKernel[4] =\r\n"
                + "{\r\n"
                + "    { 0,  1},\r\n"
                + "    { 1,  0},\r\n"
                + "    { 0, -1},\r\n"
                + "    {-1,  0}\r\n"
                + "};\r\n"
                + "float3 LuminanceConv = { 0.2125f, 0.7154f, 0.0721f };\r\n"
                + "float4 _PixelShader( float2 Tex : TEXCOORD0) : COLOR0\r\n"
                + "{\r\n"
                + "    float4 Orig = tex2D(NormalSampler, Tex );\r\n"
                + "    float4 Sum = 0;\r\n"
                + "    for( int i = 0; i < 4; i++ )\r\n"
                + "    {\r\n"
                + "        Sum += saturate( 1 - dot( Orig.xyz, tex2D(NormalSampler, Tex + TexelKernel[i] ).xyz ) );\r\n"
                + "    }\r\n"
                + "    return Sum;\r\n"
                + "}\r\n"
                + "technique NormalEdgeDetect\r\n"
                + "{\r\n"
                + "    pass p0\r\n"
                + "    {\r\n"
                + "        VertexShader = null;\r\n"
                + "        PixelShader = compile ps_2_0 _PixelShader();\r\n"
                + "        ZEnable = false;\r\n"
                + "    }\r\n"
                + "}";
        #endregion
        private Effect effect = null;
        private EffectPass NormalEdgeDetectPass = null;

        internal NormalEdgeDetect(GraphicsDevice graphicsDevice)
        {
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            effect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
            NormalEdgeDetectPass = effect.Techniques["NormalEdgeDetect"].Passes[0];

            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            effect.Dispose();
        }

        internal void Begin()
        {
            effect.Begin();
            NormalEdgeDetectPass.Begin();
        }

        internal void End()
        {
            NormalEdgeDetectPass.End();
            effect.End();
        }
    }
}

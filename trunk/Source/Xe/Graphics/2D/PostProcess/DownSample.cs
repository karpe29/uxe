using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcess
{
    public class DownSample
    {
        #region Effect Code
        private const string EffectCode = "sampler TextureSampler : register(s0);\r\n"
            + "float2 PixelCoordsDownFilter[16] =\r\n"
		    + "{\r\n"
		    + "    { 1.5,  -1.5 },\r\n"
		    + "    { 1.5,  -0.5 },\r\n"
		    + "    { 1.5,   0.5 },\r\n"
		    + "    { 1.5,   1.5 },\r\n"
		    + "    { 0.5,  -1.5 },\r\n"
		    + "    { 0.5,  -0.5 },\r\n"
		    + "    { 0.5,   0.5 },\r\n"
		    + "    { 0.5,   1.5 },\r\n"
		    + "    {-0.5,  -1.5 },\r\n"
		    + "    {-0.5,  -0.5 },\r\n"
		    + "    {-0.5,   0.5 },\r\n"
		    + "    {-0.5,   1.5 },\r\n"
		    + "    {-1.5,  -1.5 },\r\n"
		    + "    {-1.5,  -0.5 },\r\n"
		    + "    {-1.5,   0.5 },\r\n"
		    + "    {-1.5,   1.5 },\r\n"
		    + "};\r\n"
		    + "float2 TexelCoordsDownFilter[16]\r\n"
		    + "<\r\n"
		    + "    string ConvertPixelsToTexels = \"PixelCoordsDownFilter\";\r\n"
		    + ">;\r\n"
		    + "float4 DownFilter( in float2 Tex : TEXCOORD0 ) : COLOR0\r\n"
		    + "{\r\n"
		    + "    float4 Color = 0;\r\n"
		    + "    for (int i = 0; i < 16; i++)\r\n"
		    + "    {\r\n"
            + "        Color += tex2D(TextureSampler, Tex + PixelCoordsDownFilter[i].xy );\r\n"
		    + "    }\r\n"
		    + "    return Color / 16;\r\n"
		    + "}\r\n"
		    + "technique DownSample\r\n"
		    + "{\r\n"
		    + "    pass p0\r\n"
		    + "    {\r\n"
		    + "        VertexShader = null;\r\n"
		    + "        PixelShader = compile ps_2_0 DownFilter();\r\n"
		    + "        ZEnable = false;\r\n"
		    + "    }\r\n"
		    + "}";
        #endregion

        private readonly Effect shaderEffect = null;
        private EffectPass DownSamplePass = null;

        internal DownSample(GraphicsDevice graphicsDevice)
        {
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            shaderEffect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
            
            DownSamplePass = shaderEffect.Techniques["DownSample"].Passes[0];
            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            shaderEffect.Dispose();
        }

        internal void Begin()
        {
            shaderEffect.Begin();
            DownSamplePass.Begin();
        }

        internal void End()
        {
            DownSamplePass.End();
            shaderEffect.End();
        }
    }
}

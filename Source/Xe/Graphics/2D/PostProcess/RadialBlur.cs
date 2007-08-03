using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Xe.Graphics2D.PostProcessing
{
    public class RadialBlur
    {
        #region Effect Code
        private const string EffectCode = "sampler TextureSampler : register(s0);\r\n"
                + "float2 Center = { 0.5, 0.5 };\r\n"
                + "float BlurStart = 1.0f;\r\n"
                + "float BlurWidth = -0.1;\r\n"
                + "float4 PS_RadialBlur(float2 UV	: TEXCOORD0,\r\n"
                + "			   		uniform int nsamples\r\n"
                + "			   		) : COLOR\r\n"
                + "{\r\n"
                + "    UV -= Center;\r\n"
                + "    float4 c = 0;\r\n"
                + "    // this loop will be unrolled by compiler and the constants precalculated:\r\n"
                + "    for(int i=0; i<nsamples; i++) {\r\n"
                + "    	float scale = BlurStart + BlurWidth*(i/(float) (nsamples-1));\r\n"
                + "    	c += tex2D(TextureSampler, UV * scale + Center );\r\n"
                + "   	}\r\n"
                + "   	c /= nsamples;\r\n"
                + "    return c;\r\n"
                + "} \r\n"
                + "technique RadialBlur\r\n"
                + "{\r\n"
                + "    pass p0\r\n"
                + "    {\r\n"
                + "		cullmode = none;\r\n"
                + "		ZEnable = false;\r\n"
                + "		PixelShader  = compile ps_2_0 PS_RadialBlur(16);\r\n"
                + "    }\r\n"
                + "}";
#endregion

        private Effect effect = null;
        private EffectPass RadialBlurPass = null;
        private EffectParameter blurWidthParameter = null;
        private EffectParameter blurStartParameter = null;
        private EffectParameter centerParameter = null;

        public float BlurWidth
        {
            get
            {
                return blurWidthParameter.GetValueSingle();
            }
            set
            {
                blurWidthParameter.SetValue(value);
            }
        }

        public float BlurStart
        {
            get
            {
                return blurStartParameter.GetValueSingle();
            }
            set
            {
                blurStartParameter.SetValue(value);
            }
        }

        public Vector2 Center
        {
            get
            {
                return centerParameter.GetValueVector2();
            }
            set
            {
                centerParameter.SetValue(value);
            }
        }

        internal RadialBlur(GraphicsDevice graphicsDevice)
        {
            CompiledEffect ce = Effect.CompileEffectFromSource(EffectCode, null, null, CompilerOptions.None, Microsoft.Xna.Framework.TargetPlatform.Windows);
            effect = new Effect(graphicsDevice, ce.GetEffectCode(), CompilerOptions.None, new EffectPool());
            RadialBlurPass = effect.Techniques["RadialBlur"].Passes[0];

            blurWidthParameter = effect.Parameters["BlurWidth"];
            blurStartParameter = effect.Parameters["BlurStart"];
            centerParameter = effect.Parameters["Center"];

            graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
        }

        private void GraphicsDevice_Disposing(object sender, EventArgs e)
        {
            effect.Dispose();
        }

        internal void Begin()
        {
            effect.Begin();
            RadialBlurPass.Begin();
        }

        internal void End()
        {
            RadialBlurPass.End();
            effect.End();
        }
    }
}

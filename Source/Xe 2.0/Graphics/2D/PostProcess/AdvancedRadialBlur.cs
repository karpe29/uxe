using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Xe.Graphics2D.PostProcess
{
    public class AdvancedRadialBlur : PostProcessEffect
	{
		#region Variables and Properties

		private EffectParameter blurWidthParameter;
        private EffectParameter blurStartParameter;
        private EffectParameter centerParameter;

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

		#endregion

		public AdvancedRadialBlur(PostProcessManager manager)
			: base(manager, "AdvancedRadialBlur")
        {
            blurWidthParameter = m_effect.Parameters["BlurWidth"];
            blurStartParameter = m_effect.Parameters["BlurStart"];
            centerParameter = m_effect.Parameters["Center"];
        }
    }
}

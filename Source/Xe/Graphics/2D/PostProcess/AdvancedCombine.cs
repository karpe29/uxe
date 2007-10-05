using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Xe.Graphics2D.PostProcess
{
    public class AdvancedCombine : PostProcessEffect
	{
		#region Variables and Properties

		private EffectParameter Intensity1Parameter = null;
		private EffectParameter Intensity2Parameter = null;
		private EffectParameter Saturation1Parameter = null;
		private EffectParameter Saturation2Parameter = null;
		private EffectParameter SaturationColorParameter = null;
		
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

		#endregion

        internal AdvancedCombine(GraphicsDevice graphicsDevice, ContentManager contentManager)
			: base (graphicsDevice, contentManager, "AdvancedCombine")
        {
            Intensity1Parameter = m_effect.Parameters["Intensity1"];
            Intensity2Parameter = m_effect.Parameters["Intensity2"];
            Saturation1Parameter = m_effect.Parameters["Saturation1"];
            Saturation2Parameter = m_effect.Parameters["Saturation2"];
            SaturationColorParameter = m_effect.Parameters["SaturationColor"];

        }
    }
}

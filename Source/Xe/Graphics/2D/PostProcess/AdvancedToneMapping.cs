using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Xe.Graphics2D.PostProcess
{
    public class AdvancedToneMapping : PostProcessEffect
	{

		#region Variable and Properties

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

		#endregion

		public AdvancedToneMapping(GraphicsDevice graphicsDevice, ContentManager contentManager)
			: base (graphicsDevice, contentManager, "AdvancedToneMapping")
        {
            exposureParameter = m_effect.Parameters["exposure"];
            defogParameter = m_effect.Parameters["defog"];
            gammaParameter = m_effect.Parameters["gamma"];
        }
    }
}

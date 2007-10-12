using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Xe.Graphics2D.PostProcess
{
	public sealed class AdvancedSmartBlur : PostProcessEffect
	{
		private EffectParameter BlurAmountParameter = null;

		internal AdvancedSmartBlur(GraphicsDevice graphicsDevice, ContentManager contentManager)
			: base(graphicsDevice, contentManager, "AdvancedSmartBlur")
		{
			BlurAmountParameter = m_effect.Parameters["BlurAmount"];
		}

		public float BlurAmount
		{
			get
			{
				return BlurAmountParameter.GetValueSingle();
			}
			set
			{
				BlurAmountParameter.SetValue(value);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcess
{
	public class PostProcessEffect
	{
		
		private string m_name = null;

		public string Name { get { return m_name; } }

		protected Effect m_effect = null;
		protected EffectPass m_pass = null;

		internal PostProcessEffect(GraphicsDevice graphicsDevice, ContentManager contentManager, string effectName)
		{
			m_name = effectName;

			m_effect = contentManager.Load<Effect>(PostProcessManager.EFFECT_PATH + m_name);

			// NullReferenceException occurs here if (technique name != file name)
			m_pass = m_effect.Techniques[m_name].Passes[0];

			graphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);
		}

		internal virtual void GraphicsDevice_Disposing(object sender, EventArgs e)
		{
			m_effect.Dispose();
		}

		internal virtual void BeginPostProcess()
		{
			m_effect.Begin();
			m_pass.Begin();
		}

		internal virtual void EndPostProcess()
		{
			m_pass.End();
			m_effect.End();
		}
	}
}

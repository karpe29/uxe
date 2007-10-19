using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Xe.Graphics2D.PostProcess
{
	public class PostProcessEffect
	{
		protected PostProcessManager m_manager;

		private string m_name = null;

		public string Name { get { return m_name; } }

		protected Effect m_effect = null;
		protected EffectPass m_pass = null;

		public delegate PostProcessResult ApplyEffectDelegate(PostProcessEffect ppe, PostProcessResult result);

		public ApplyEffectDelegate ApplyEffect;

		internal PostProcessEffect(PostProcessManager manager, string effectName)
		{
			m_manager = manager;
			m_name = effectName;

			m_effect = m_manager.ContentManager.Load<Effect>(PostProcessManager.EFFECT_PATH + m_name);

			m_pass = m_effect.Techniques[0].Passes[0];

			m_manager.GraphicsDevice.Disposing += new EventHandler(GraphicsDevice_Disposing);

			ApplyEffect = new ApplyEffectDelegate(m_manager.ApplyEffect);
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

		internal void Update(GameTime gameTime)
		{
			for (int i = 0; i < m_effect.Parameters.Count; i++)
			{
				if (m_effect.Parameters[i].Name.ToLower().Contains("timer"))
					m_effect.Parameters[i].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
			}
		}
	}
}

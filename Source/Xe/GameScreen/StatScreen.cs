using System;
using System.Collections.Generic;
using System.Text;

using Xe;
using Xe.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;

namespace Xe.GameScreen
{
	class StatScreen : IGameScreen
	{
		private Stats m_stats;

		public StatScreen(GameScreenManager gameScreenManager) 
			: base(gameScreenManager, true)
		{
			m_stats = new Stats(gameScreenManager.Game, gameScreenManager.ContentManager);
			
			m_stats.Enabled = m_stats.Visible = false;
			
			m_stats.DrawOrder = 10001;
			m_stats.ForeColor = Color.WhiteSmoke;

			m_stats.Initialize();

			GameScreenManager.Ebi.KeyDown += new KeyDownHandler(ebi_KeyDown);

			GameScreenManager.Game.Components.Add(m_stats);
		}

		void ebi_KeyDown(object focus, KeyEventArgs k)
		{
			if (k.Key == Keys.F1)
			{
				this.m_stats.Enabled = !this.m_stats.Enabled;
				this.m_stats.Visible = !this.m_stats.Visible;
			}
		}

		#region IGameScreen Members

		public override bool IsBlockingUpdate
		{
			get { return false; }
		}

		public override bool IsBlockingDraw
		{
			get { return false; }
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;

using Xe;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;
using Xe.Graphics2D;
using Xe.Input;

namespace Xe.GameScreen
{
	class StatScreen : IGameScreen
	{
		private Stats m_stats;
		private SafeArea m_safeArea;

		public StatScreen(GameScreenManager gameScreenManager) 
			: base(gameScreenManager, true)
		{
			m_stats = new Stats(gameScreenManager.Game, XeGame.ContentManager);
			m_stats.Enabled = m_stats.Visible = false;
			m_stats.DrawOrder = 10*1000 + 2;
			m_stats.ForeColor = Color.Red;
			m_stats.Initialize();
			
			GameScreenManager.Game.Components.Add(m_stats);

			m_safeArea = new SafeArea(GameScreenManager.Game);
			m_safeArea.Enabled = m_safeArea.Visible = false;
			m_safeArea.DrawOrder = 10*1000 + 1;
			m_safeArea.Initialize();

			GameScreenManager.Game.Components.Add(m_safeArea);

			XeGame.Ebi.KeyDown += new KeyDownHandler(ebi_KeyDown);
		}

		void ebi_KeyDown(object focus, KeyEventArgs k)
		{
			if (k.Key == Keys.F1)
			{
				this.m_stats.Enabled = !this.m_stats.Enabled;
				this.m_stats.Visible = !this.m_stats.Visible;
			}

			if (k.Key == Keys.F2)
			{
				this.m_safeArea.Enabled = !this.m_safeArea.Enabled;
				this.m_safeArea.Visible = !this.m_safeArea.Visible;
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

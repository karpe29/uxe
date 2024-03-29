using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xe.Tools;
using Xe.Input;
using Xe.Gui;

namespace Xe.GameScreen
{
	class ConsoleScreen : IGameScreen
	{
		private Xe.Gui.Console m_console;
		private bool m_showConsole;

		public ConsoleScreen(GameScreenManager gameScreenManager) 
			: base(gameScreenManager, true)
		{
			m_console = new Xe.Gui.Console(this.GameScreenManager.Game, XeGame.ContentManager);
			
			m_console.DrawOrder = 10000;

			m_console.Initialize();

			XeGame.Ebi.KeyDown += new KeyDownHandler(ebi_KeyDown);

			this.GameScreenManager.Game.Components.Add(m_console);
		}

		void ebi_KeyDown(object focus, KeyEventArgs k)
		{
		    if (k.Key == Keys.F3)
		    {
				this.m_console.SetActive(!m_showConsole);
				m_showConsole = !m_showConsole;
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

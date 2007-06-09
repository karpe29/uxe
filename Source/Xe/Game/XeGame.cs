using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Content;

using Xe;
using Xe.Graphics2D;
using Xe.GUI;
using Xe.Input;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Xe
{
	public class XeGame : Microsoft.Xna.Framework.Game
	{
		#region Variables

		private GUIManager m_guiManager;
		private Xe.Input.Ebi m_ebi;

		private ContentManager m_contentManager;
		
		private GameScreenManager m_gameScreenManager;

		private StatScreen m_statScreen;
		private ConsoleScreen m_consoleScreen;
		private IntroScreen m_introScreen;

		protected GraphicsDeviceManager m_graphics;
		#endregion	

		public XeGame()
		{
			m_graphics = new GraphicsDeviceManager(this);

			m_contentManager = new ContentManager(this.Services);

			this.IsFixedTimeStep = false;
			this.TargetElapsedTime = new TimeSpan(1);
			this.IsMouseVisible = true;

			this.Window.Title = "Xe3D";
			this.Window.AllowUserResizing = true;
			
			this.m_graphics.SynchronizeWithVerticalRetrace = false;
			this.m_graphics.PreferredBackBufferWidth = 1024;
			this.m_graphics.PreferredBackBufferHeight = 768;
			this.m_graphics.PreferMultiSampling = true;
			
			m_ebi = new Ebi(this);
			m_ebi.UpdateOrder = 0;
			Components.Add(this.m_ebi);

			m_guiManager = new GUIManager(this);
			m_guiManager.UpdateOrder = 1000;
			m_guiManager.DrawOrder = 1000;
			Components.Add(this.m_guiManager);

			m_gameScreenManager = new GameScreenManager(this, m_contentManager);
			m_gameScreenManager.UpdateOrder = 500;
			m_gameScreenManager.DrawOrder = 500;
			Components.Add(this.m_gameScreenManager);
		}

		protected override void Initialize()
		{
			m_guiManager.LoadSettings(@"Content\XML\gui_Xe.xml");

			m_statScreen = new StatScreen(m_gameScreenManager);
			m_consoleScreen = new ConsoleScreen(m_gameScreenManager);

			m_introScreen = new IntroScreen(m_gameScreenManager);

			base.Initialize();
		}

		
		protected override bool BeginDraw()
		{
			m_graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
			return base.BeginDraw();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			GamePadState playerOneGamePadState = GamePad.GetState(PlayerIndex.One);
			GamePadState playerTwoGamePadState = GamePad.GetState(PlayerIndex.Two);
			KeyboardState keyboardState = Keyboard.GetState();

			if (playerOneGamePadState.Buttons.Back == ButtonState.Pressed ||
				playerTwoGamePadState.Buttons.Back == ButtonState.Pressed ||
				keyboardState.IsKeyDown(Keys.Escape))
				this.Exit();

			if (keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt))
				if (keyboardState.IsKeyDown(Keys.Enter))
				{
					if (this.m_graphics.IsFullScreen)
					{
						this.m_graphics.PreferredBackBufferWidth = 1024;
						this.m_graphics.PreferredBackBufferHeight = 768;
					}
					else
					{
						this.m_graphics.PreferredBackBufferWidth = 1920;
						this.m_graphics.PreferredBackBufferHeight = 1200;
					}
					this.m_graphics.ToggleFullScreen();
				}
		}
	}
}

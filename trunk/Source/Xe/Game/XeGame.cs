using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Content;

using Xe;
using Xe.Graphics2D;
using Xe.GUI;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Xe.Tools;
using Xe.Graphics2D.PostProcess;

namespace Xe
{
	public class XeGame : Microsoft.Xna.Framework.Game
	{
		#region Static Variables

		public static readonly string FONT_GUI = "BatmanForeverAlternate";

		public static readonly string FONT_DBG = "Perspective Sans";


		private static ContentManager s_contentManager;
		
		private static GameScreenManager s_gameScreenManager;

		private static GraphicsDevice s_device;

		private static PostProcessManager s_postProcessManager;

		#endregion	

		#region Static Properties

		public static ContentManager ContentManager
		{
			get { return s_contentManager; }
		}

		public static GUIManager GuiManager
		{
			get { return (GUIManager)ServiceHelper.Get<IGUIManagerService>(); }
		}

		public static Reporter Reporter
		{
			get { return (Reporter)ServiceHelper.Get<IReporterService>(); }
		}

		public static Ebi Ebi
		{
			get { return (Ebi)ServiceHelper.Get<IEbiService>(); }
		}

		public static Stats Stats
		{
			get { return ServiceHelper.Get<Stats>(); }
		}

		public static GameScreenManager GameScreenManager
		{
			get { return s_gameScreenManager; }
		}

		public static GraphicsDevice Device
		{
			get { return s_device; }
		}

		public static PostProcessManager PostProcessManager
		{
			get { return s_postProcessManager; }
		}

		#endregion

		#region Variables

		

		protected GraphicsDeviceManager m_graphics;

		private StatScreen m_statScreen;
		private ConsoleScreen m_consoleScreen;
		private IntroScreen m_introScreen;

		private GUIManager m_guiManager;

		private Reporter m_reporter;

		private Ebi m_ebi;

		#endregion 

		#region Properties

		#endregion


		public XeGame()
		{
			ServiceHelper.Game = this;

			m_graphics = new GraphicsDeviceManager(this);

			

			s_contentManager = new ContentManager(ServiceHelper.Services);

			this.IsFixedTimeStep = false;
			this.TargetElapsedTime = TimeSpan.FromMilliseconds(1);
			this.IsMouseVisible = true;

			this.Window.Title = "Xe3D";
			this.Window.AllowUserResizing = true;
			
			this.m_graphics.SynchronizeWithVerticalRetrace = false;
			this.m_graphics.PreferredBackBufferWidth = 1024;
			this.m_graphics.PreferredBackBufferHeight = 768;
			this.m_graphics.PreferMultiSampling = true;
			
			m_ebi = new Ebi(this);
			m_ebi.UpdateOrder = 0;
			Components.Add(m_ebi);

			m_guiManager = new GUIManager(this);
			m_guiManager.UpdateOrder = 1000;
			m_guiManager.DrawOrder = 1000;
			Components.Add(m_guiManager);

			m_reporter = new Reporter(this);
			m_reporter.UpdateOrder = 0;

			s_gameScreenManager = new GameScreenManager(this);
			s_gameScreenManager.UpdateOrder = 500;
			s_gameScreenManager.DrawOrder = 500;
			Components.Add(s_gameScreenManager);
		}
			
		protected override void Initialize()
		{
			s_device = m_graphics.GraphicsDevice;

			m_guiManager.LoadSettings(@"Content\XML\gui_Xe.xml");

			m_statScreen = new StatScreen(s_gameScreenManager);

			m_consoleScreen = new ConsoleScreen(s_gameScreenManager);

			m_introScreen = new IntroScreen(s_gameScreenManager);

			base.Initialize();
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
				s_postProcessManager = new PostProcessManager();
			}
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

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			s_postProcessManager.ApplyPostProcess();
		}
	}
}

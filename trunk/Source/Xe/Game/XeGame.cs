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
using System.Reflection;
using Xe.Input;

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

		public static InputHelper InputHelper
		{
			get { return (InputHelper)ServiceHelper.Get<InputHelper>(); }
		}


		public static IGUIManager GuiManager
		{
			get { return ServiceHelper.Get<IGUIManager>(); }
		}

		public static Reporter Reporter
		{
			get { return (Reporter)ServiceHelper.Get<IReporterService>(); }
		}

		public static IEbiService Ebi
		{
			get { return ServiceHelper.Get<IEbiService>(); }
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

		private InputHelper m_inputHelper;

		protected GraphicsDeviceManager m_graphics;

		private StatScreen m_statScreen;
		private ConsoleScreen m_consoleScreen;
		private IntroScreen m_introScreen;

		private GUIManager<SpriteRenderer> m_guiManager;

		private Reporter m_reporter;

		private Ebi<MKController> m_ebi;

		#endregion 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="percent">percent between 0 and 1</param>
		/// <returns>percent width of the screen</returns>
		public static int WitdhPercent(float percent)
		{
			return (int) (XeGame.Device.PresentationParameters.BackBufferWidth * MathHelper.Clamp(percent, 0, 1));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="percent">percent between 0 and 1</param>
		/// <returns>percent height of the screen</returns>
		public static int HeightPercent(float percent)
		{
			return (int) (XeGame.Device.PresentationParameters.BackBufferHeight * MathHelper.Clamp(percent, 0, 1));
		}


		public XeGame()
		{
			ServiceHelper.Game = this;

			m_graphics = new GraphicsDeviceManager(this);

			m_inputHelper = new InputHelper(this);
			m_inputHelper.UpdateOrder = 0;
			Components.Add(m_inputHelper);

			s_contentManager = new ContentManager(ServiceHelper.Services);

			IsFixedTimeStep = false;
			TargetElapsedTime = TimeSpan.FromMilliseconds(1);
			IsMouseVisible = true;

			Window.Title = "Xe3D v" + Assembly.GetCallingAssembly().GetName().Version.ToString();
			Window.AllowUserResizing = true;
			
			m_graphics.SynchronizeWithVerticalRetrace = false;
			m_graphics.PreferredBackBufferWidth = 1024;
			m_graphics.PreferredBackBufferHeight = 768;
			m_graphics.PreferMultiSampling = true;
			
			m_ebi = new Ebi<MKController> (this);
			m_ebi.UpdateOrder = 0;
			Components.Add(m_ebi);
			ServiceHelper.Add<IEbiService>(m_ebi);

			m_guiManager = new GUIManager<SpriteRenderer> (this, m_ebi);
			m_guiManager.UpdateOrder = 11*1000;
			m_guiManager.DrawOrder = 11*1000;
			Components.Add(m_guiManager);
			ServiceHelper.Add<IGUIManager>(m_guiManager);

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

			m_guiManager.LoadSettings(@"Content\XML\Xe_GUI.xml");

			s_postProcessManager = new PostProcessManager(this, m_graphics.GraphicsDevice, s_contentManager);
			s_postProcessManager.UpdateOrder = 10 * 1000;
			s_postProcessManager.DrawOrder = 10 * 1000;
			Components.Add(s_postProcessManager);

			base.Initialize();

			m_statScreen = new StatScreen(s_gameScreenManager);

			m_consoleScreen = new ConsoleScreen(s_gameScreenManager);

			m_introScreen = new IntroScreen(s_gameScreenManager);
		}

		protected override bool BeginDraw()
		{
			m_graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
			return base.BeginDraw();
		}

		protected override void Draw(GameTime gameTime)
		{
			//PostProcessResult r = s_postProcessManager.RetrieveFrameBuffer();
			//r = pp.ApplySmartBlur(r);
			//r = pp.ApplyGaussianBlurWithBloomH(r);
			//r = pp.ApplyGaussianBlurWithBloomV(r);
			//r = pp.ApplyMonochrome(r);

			//PostProcessResult s = pp.ApplyBloomExtract(r);
			//r = pp.CombineScreens(r, s);
			//r = pp.ApplyRadialBlur(r);
			//pp.Present(null, r);
			
			base.Draw(gameTime);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (InputHelper.KeyboardEscapeJustPressed)
				this.Exit();

			if (InputHelper.Keyboard.IsKeyDown(Keys.LeftAlt) || InputHelper.Keyboard.IsKeyDown(Keys.RightAlt))
				if (InputHelper.Keyboard.IsKeyDown(Keys.Enter))
				{
					this.m_graphics.ToggleFullScreen();
				}
		}
	}
}

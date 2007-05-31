using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using XeFramework.GUI;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XeFramework;

namespace XeFramework.GameScreen
{
	class MainMenuScreen : IGameScreen
	{
		XeFramework.GUI.Button buttonNewGame;
		XeFramework.GUI.Button buttonOptions;
		XeFramework.GUI.Button buttonQuit;

		public static Type BackgroundScreenType = typeof(MainBackgroundScreen2);

		public MainMenuScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, false)
		{
			// if we don't already have a backgroundScreen, create one
			if (this.GameScreenManager.CurrentGameScreen.GetType() != BackgroundScreenType)
				if (BackgroundScreenType.BaseType == typeof(IGameScreen))
					Activator.CreateInstance(BackgroundScreenType, this.GameScreenManager);

			// add it to the list of GameScreens
			this.GameScreenManager.AddGameScreen(this);
			
			buttonNewGame = new Button(GameScreenManager.Game, GameScreenManager.GuiManager);
			buttonNewGame.Text = "New Game";
			buttonNewGame.Width = 120;
			buttonNewGame.Height = 30;
			buttonNewGame.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonNewGame.Width / 2;
			buttonNewGame.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonNewGame.Height / 2;
			buttonNewGame.Click += new ClickHandler(buttonNewGame_Click);
			GameScreenManager.GuiManager.AddControl(buttonNewGame);

			buttonOptions = new Button(GameScreenManager.Game, GameScreenManager.GuiManager);
			buttonOptions.Text = "Options";
			buttonOptions.Width = 120;
			buttonOptions.Height = 30;
			buttonOptions.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 2 / 4 - buttonOptions.Width / 2;
			buttonOptions.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonOptions.Height / 2;
			buttonOptions.Click += new ClickHandler(buttonOptions_Click);
			GameScreenManager.GuiManager.AddControl(buttonOptions);

			buttonQuit = new Button(GameScreenManager.Game, GameScreenManager.GuiManager);
			buttonQuit.Text = "Quit";
			buttonQuit.Width = 120;
			buttonQuit.Height = 30;
			buttonQuit.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonQuit.Width / 2;
			buttonQuit.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonQuit.Height / 2;
			buttonQuit.Click += new ClickHandler(buttonQuit_Click);
			GameScreenManager.GuiManager.AddControl(buttonQuit);
		}

		void buttonOptions_Click(object sender, XeFramework.Input.MouseEventArgs args)
		{
			ExitScreen();

			OptionScreen o = new OptionScreen(GameScreenManager);
		}

		void buttonQuit_Click(object sender, XeFramework.Input.MouseEventArgs args)
		{
			ExitScreen();

			CreditsScreen c = new CreditsScreen(GameScreenManager);
		}

		void buttonNewGame_Click(object sender, XeFramework.Input.MouseEventArgs args)
		{
			ExitScreen();

			LevelSelectionScreen l = new LevelSelectionScreen(GameScreenManager);
		}

		protected override void Cleanup()
		{
			GameScreenManager.GuiManager.RemoveControl(buttonNewGame);
			buttonNewGame.Dispose();

			GameScreenManager.GuiManager.RemoveControl(buttonOptions);
			buttonOptions.Dispose();

			GameScreenManager.GuiManager.RemoveControl(buttonQuit);
			buttonQuit.Dispose();
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);
			if (loadAllContent)
			{
			}
			else
			{
				buttonNewGame.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonNewGame.Width / 2;
				buttonNewGame.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonNewGame.Height / 2;

				buttonOptions.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 2 / 4 - buttonOptions.Width / 2;
				buttonOptions.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonOptions.Height / 2;

				buttonQuit.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonQuit.Width / 2;
				buttonQuit.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonQuit.Height / 2;
			}
		}


		#region IGameScreen Members

		public override bool IsBlockingUpdate { get { return false; } }

		public override bool IsBlockingDraw { get { return false; } }

		#endregion
	}
}

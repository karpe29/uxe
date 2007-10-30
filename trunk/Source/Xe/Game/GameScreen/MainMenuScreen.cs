using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Xe.Gui;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xe;
using Xe.Tools;
using Xe.Input;

namespace Xe.GameScreen
{
	class MainMenuScreen : IGameScreen
	{
		Xe.Gui.Button buttonNewGame;
		Xe.Gui.Button buttonOptions;
		Xe.Gui.Button buttonQuit;

		public static Type BackgroundScreenType = typeof(MainBackgroundScreen);

		public MainMenuScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, false)
		{
			// if we don't already have a backgroundScreen, create one
			
			/*if (this.GameScreenManager.CurrentGameScreen.GetType() != BackgroundScreenType)
				if (BackgroundScreenType.BaseType == typeof(IGameScreen))
					Activator.CreateInstance(BackgroundScreenType, this.GameScreenManager);
			*/
			MainBackgroundScreen mbgs = new MainBackgroundScreen(this.GameScreenManager);

			// add it to the list of GameScreens
			this.GameScreenManager.AddGameScreen(this);

			buttonNewGame = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonNewGame.Text = "New Game";
			buttonNewGame.TextAlignVertical = TextAlignVertical.Center;
			buttonNewGame.IsDraggable = false;
			buttonNewGame.Width = 120;
			buttonNewGame.Height = 120;
			buttonNewGame.X = XeGame.WitdhPercent(0.25f) - buttonNewGame.Width / 2;
			buttonNewGame.Y = XeGame.HeightPercent(0.75f) - buttonNewGame.Height / 2;
			buttonNewGame.Click += new ClickHandler(buttonNewGame_Click);
			XeGame.GuiManager.Controls.Add(buttonNewGame);

			buttonOptions = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonOptions.Text = "Options";
			buttonOptions.Width = 120;
			buttonOptions.Height = 32;
			buttonOptions.X = XeGame.WitdhPercent(0.5f) - buttonOptions.Width / 2;
			buttonOptions.Y = XeGame.HeightPercent(0.75f) - buttonOptions.Height / 2;
			buttonOptions.Click += new ClickHandler(buttonOptions_Click);
			XeGame.GuiManager.Controls.Add(buttonOptions);

			buttonQuit = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonQuit.Text = "Quit";
			buttonQuit.Width = 120;
			buttonQuit.Height = 32;
			buttonQuit.X = XeGame.WitdhPercent(0.75f) - buttonQuit.Width / 2;
			buttonQuit.Y = XeGame.HeightPercent(0.75f) - buttonQuit.Height / 2;
			buttonQuit.Click += new ClickHandler(buttonQuit_Click);
			XeGame.GuiManager.Controls.Add(buttonQuit);

			SliderString n = new SliderString(gameScreenManager.Game, XeGame.GuiManager, SliderType.Horizontal);
			n.X = 100;
			n.Y = 100;
			List<string> l = new List<string>();
			l.Add("One player 1");
			l.Add("Two Players 2");
			l.Add("Three Players 3");
			n.Strings = l;
			n.Loopable = true;
			n.Width = 400;
			n.Index = 0;
			//n.IsDraggable = false;
			//n.Height = 50;
			XeGame.GuiManager.Controls.Add(n);

			CheckBox c = new CheckBox(gameScreenManager.Game, XeGame.GuiManager);
			c.Text = "cccc";
			c.TextAlign = TextAlign.Center;
			c.X = 100;
			c.Y = 200;
			c.Label.Width = 100;
			//c.Height = 30;
			XeGame.GuiManager.Controls.Add(c);

		}

		void buttonOptions_Click(object sender, MouseEventArgs args)
		{
			ExitScreen();

			OptionScreen o = new OptionScreen(GameScreenManager);
		}

		void buttonQuit_Click(object sender, MouseEventArgs args)
		{
			ExitScreen();

			CreditsScreen c = new CreditsScreen(GameScreenManager);
		}

		void buttonNewGame_Click(object sender, MouseEventArgs args)
		{
			ExitScreen();

			LevelSelectionScreen l = new LevelSelectionScreen(GameScreenManager);
		}

		protected override void Cleanup()
		{
			XeGame.GuiManager.Controls.Remove(buttonNewGame);
			buttonNewGame.Dispose();

			XeGame.GuiManager.Controls.Remove(buttonOptions);
			buttonOptions.Dispose();

			XeGame.GuiManager.Controls.Remove(buttonQuit);
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

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (InputHelper.Keyboard.IsKeyDown(Keys.Add))
				this.buttonNewGame.Height++;

			if (InputHelper.Keyboard.IsKeyDown(Keys.Subtract))
				this.buttonNewGame.Height--;
		}
	}
}

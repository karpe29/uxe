using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Xe.GUI;
using Xe.Tools;

namespace Xe.GameScreen
{
	class OptionScreen : IGameScreen
	{
		Xe.GUI.Button buttonBack;

		public OptionScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{
			buttonBack = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonBack.Text = "Back";
			buttonBack.Width = 120;
			buttonBack.Height = 30;
			buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
			buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;
			buttonBack.Click += new ClickHandler(buttonBack_Click);
			XeGame.GuiManager.AddControl(buttonBack);
		}

		void buttonBack_Click(object sender, MouseEventArgs args)
		{
			ExitScreen();

			MainMenuScreen m = new MainMenuScreen(GameScreenManager);
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
			}
			else
			{
				buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
				buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;
			}
		}

		protected override void Cleanup()
		{
			XeGame.GuiManager.RemoveControl(buttonBack);
			buttonBack.Dispose();
		}

		public override bool IsBlockingUpdate
		{
			get { return false; }
		}

		public override bool IsBlockingDraw
		{
			get { return false; }
		}
	}
}

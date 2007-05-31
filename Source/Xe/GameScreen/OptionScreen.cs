using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using XeFramework.GUI;

namespace XeFramework.GameScreen
{
	class OptionScreen : IGameScreen
	{
		XeFramework.GUI.Button buttonBack;

		public OptionScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{
			buttonBack = new Button(GameScreenManager.Game, GameScreenManager.GuiManager);
			buttonBack.Text = "Back";
			buttonBack.Width = 120;
			buttonBack.Height = 30;
			buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
			buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;
			buttonBack.Click += new ClickHandler(buttonBack_Click);
			GameScreenManager.GuiManager.AddControl(buttonBack);
		}

		void buttonBack_Click(object sender, XeFramework.Input.MouseEventArgs args)
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
			GameScreenManager.GuiManager.RemoveControl(buttonBack);
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

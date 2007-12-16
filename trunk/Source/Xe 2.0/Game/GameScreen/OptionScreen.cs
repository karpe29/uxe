using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Xe.Gui;
using Xe.Tools;
using Xe.Input;

namespace Xe.GameScreen
{
	class OptionScreen : IGameScreen
	{

		string[] m_resolutions = new string[] {"640x480","800x600","1024x768","1152x768","1280x720","1280x960","1280x1024","1400x1050","1600x1200","1680x1050","1920x1080","1920x1200","2048x1536","2560x1600"};


		Xe.Gui.Button buttonBack;

		public OptionScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{
			buttonBack = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonBack.Text = "Back";
			buttonBack.Width = 120;
			buttonBack.Height = 30;
			buttonBack.X = XeGame.WitdhPercent(0.20f) - buttonBack.Width / 2;
			buttonBack.Y = XeGame.HeightPercent(0.85f) - buttonBack.Height / 2;
			buttonBack.Click += new ClickHandler(buttonBack_Click);
			XeGame.GuiManager.Controls.Add(buttonBack);
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
			XeGame.GuiManager.Controls.Remove(buttonBack);
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

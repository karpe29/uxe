using System;
using System.Collections.Generic;
using System.Text;
using Xe.Game.GameScreen;
using Microsoft.Xna.Framework;
using Xe.GUI;

namespace Xe.SpaceRace
{
	class SpaceRaceHudScreen : IGameScreen
	{
		Label labelLife;
 
		public SpaceRaceHudScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{
			labelLife = new Label(gameScreenManager.Game, XeGame.GuiManager);
			labelLife.Text = "|||||||||||||||||||||||||||||||||||||||||||||||||||||-";
			labelLife.TextAlign = TextAlignment.Center;
			labelLife.Width = 220;
			labelLife.Height = 30;
			labelLife.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - labelLife.Width / 2;
			labelLife.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - labelLife.Height / 2;
			//GameScreenManager.GuiManager.AddControl(labelLife);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
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

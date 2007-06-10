using System;
using System.Collections.Generic;
using System.Text;
using Xe.GameScreen;
using Microsoft.Xna.Framework;

namespace Xe.SpaceRace
{
	class SpaceRaceHudScreen : IGameScreen
	{
		public SpaceRaceHudScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{

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

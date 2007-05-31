using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace XeFramework.GameScreen
{
	class PauseScreen : IGameScreen
	{
		public PauseScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager,true)
		{
		}

		#region IGameScreen Members

		public override bool IsBlockingUpdate { get { return true; } }

		public override bool IsBlockingDraw { get { return false; } }

		#endregion

		public override void Draw(GameTime gameTime)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Update(GameTime gameTime)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}

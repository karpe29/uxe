using System;
using System.Collections.Generic;
using System.Text;

namespace XeFramework.GameScreen
{
	class TimeTimeScreen : IGameScreen
	{
		private TimeTimeScreen(GameScreenManager gameScreenManager)
			: this(gameScreenManager,50)
		{
		}

		public TimeTimeScreen(GameScreenManager gameScreenManager, float difficultyPercent)
			: base(gameScreenManager, true)
		{

		}

		

		public override bool IsBlockingUpdate
		{
			get { return true; }
		}

		public override bool IsBlockingDraw
		{
			get { return true; }
		}
	}
}

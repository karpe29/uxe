using System;
using System.Collections.Generic;
using System.Text;
using Xe.GameScreen;

namespace Xe.TimeTime
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

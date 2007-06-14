using System;
using System.Collections.Generic;
using System.Text;
using Xe.Physics3D;
using Xe.GameScreen;

namespace Xe.SpaceRace
{
	class AsteroidManager
	{
		GameScreenManager m_gameScreenManager;

		public GameScreenManager GameScreenManager
		{
			get { return m_gameScreenManager; }
		}

		public AsteroidManager(GameScreenManager manager)
		{
			m_gameScreenManager = manager;
		}
	}
}

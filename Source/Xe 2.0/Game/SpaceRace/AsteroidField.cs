using System;
using System.Collections.Generic;
using System.Text;
using Xe.Physics3D;
using Xe.GameScreen;

namespace Xe.SpaceRace
{
	class AsteroidField
	{
		GameScreenManager m_gameScreenManager;

		Planet m_planet;

		List<Asteroid> m_asteroids;

		public AsteroidField(GameScreenManager manager, Planet planet, int bigAsteroidCount, int smallAsteroidCount)
		{
			m_gameScreenManager = manager;

			m_planet = planet;





		}
	}
}

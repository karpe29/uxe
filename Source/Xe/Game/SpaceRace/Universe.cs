using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Xe.GameScreen;

namespace Xe.SpaceRace
{
	class Universe : DrawableGameComponent
	{
		SolarSystem m_solarSystem,m_ss2;

		AsteroidManager m_asteroidManager;

		public Universe(GameScreenManager gameScreenManager)
			: base (gameScreenManager.Game)
		{
			m_solarSystem = new SolarSystem(gameScreenManager,null, 5, 0);

			m_ss2 = new SolarSystem(gameScreenManager, m_solarSystem.m_planets[3], 3, 0);

		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_solarSystem.SetCamera(view, projection);
			m_ss2.SetCamera(view, projection);
		}

		public override void Update(GameTime gameTime)
		{
			m_solarSystem.Update(gameTime);
			m_ss2.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			m_solarSystem.Draw(gameTime);
			m_ss2.Draw(gameTime);
			base.Draw(gameTime);
		}
	}
}

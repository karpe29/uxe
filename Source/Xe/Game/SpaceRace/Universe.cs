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
		Planet m_sun;

		AsteroidManager m_asteroidManager;

		public Universe(GameScreenManager gameScreenManager)
			: base (gameScreenManager.Game)
		{
			PlanetType tmpPlanetType = new PlanetType(PlanetType.Names.Sun, 0, 0, 0, 0);
			m_sun = new Planet(gameScreenManager, null, tmpPlanetType, Vector3.Zero,Vector3.Zero);
			m_solarSystem = new SolarSystem(gameScreenManager,m_sun, 5);
			m_ss2 = new SolarSystem(gameScreenManager, m_solarSystem.m_planets[3], 3);

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

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Xe.GameScreen;

namespace Xe.SpaceRace
{
	class Universe : DrawableGameComponent
	{
		SolarSystem m_solarSystem;

		AsteroidManager m_asteroidManager;

		public Universe(GameScreenManager gameScreenManager)
			: base (gameScreenManager.Game)
		{
			m_solarSystem = new SolarSystem(gameScreenManager, 9);

		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_solarSystem.SetCamera(view, projection);
		}

		public override void Update(GameTime gameTime)
		{
			m_solarSystem.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			m_solarSystem.Draw(gameTime);
			base.Draw(gameTime);
		}
	}
}

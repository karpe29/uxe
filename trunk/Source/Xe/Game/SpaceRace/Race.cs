using System;
using System.Collections.Generic;
using System.Text;
using Xe.GameScreen;
using Microsoft.Xna.Framework;

namespace Xe.SpaceRace
{
	class Race : DrawableGameComponent
	{
		float m_difficultyPercent;

		List<CheckPoint> m_checkPoints;
		List<WormHole> m_wormHoles;

		PlanetManager m_planetManager;
		AsteroidManager m_asteroidManager;


		public Race(GameScreenManager gameScreenManager, float difficultyPercent)
			: base(gameScreenManager.Game)
		{
			m_difficultyPercent = difficultyPercent;

			m_planetManager = new PlanetManager(gameScreenManager);

			m_planetManager.AddPlanet();
		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_planetManager.SetCamera(view, projection);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			m_planetManager.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			//base.Draw(gameTime);

			m_planetManager.Draw(gameTime);
		}

		
	}
}

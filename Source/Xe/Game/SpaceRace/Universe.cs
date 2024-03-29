using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Xe.GameScreen;

namespace Xe.SpaceRace
{
	public class Universe : DrawableGameComponent
	{
		SolarSystem m_solarSystem,m_ss2;

		AsteroidField m_asteroidField;

		public Universe(GameScreenManager gameScreenManager)
			: base (gameScreenManager.Game)
		{
			m_solarSystem = new SolarSystem(gameScreenManager,null, 20, 0, 0);
			m_solarSystem.Sun.Position = new Vector3(0, -2000000, 0);
			//m_solarSystem.Decalage = Matrix.CreateRotationX(MathHelper.PiOver2);


			//m_ss2 = new SolarSystem(gameScreenManager, m_solarSystem.m_planets[3], 3, 0, 0);
		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_solarSystem.SetCamera(view, projection);
			//m_ss2.SetCamera(view, projection);
		}

		public override void Update(GameTime gameTime)
		{
			m_solarSystem.Update(gameTime);
			//m_ss2.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			m_solarSystem.Draw(gameTime);
			//m_ss2.Draw(gameTime);
			base.Draw(gameTime);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Xe.Physics3D;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.SpaceRace
{
	class SolarSystem : DrawableGameComponent
	{
		Sun m_sun;

		List<Planet> m_planets;

		public SolarSystem(GameScreenManager gameScreenManager, int planetCount)
			: base(gameScreenManager.Game)
		{
			m_sun = new Sun(gameScreenManager, new PhysicalType(0,0,0,0,100));
			m_sun.Position = new Vector3(0, 0, -2000);

			m_planets = new List<Planet>(planetCount);

			Random r = new Random();

			for (int i = 0; i < m_planets.Capacity; i++)
			{
				PlanetType tmpPlanetType = new PlanetType((PlanetType.Names)r.Next(0, ((int)PlanetType.Names.LASTUNUSED) - 1), 0, 0, 0, 0);

				m_planets.Add(new Planet(gameScreenManager, tmpPlanetType));

				m_planets[i].distanceToSun = r.Next(100, 1000) * 12;
				m_planets[i].RotationSpeed = (float)r.Next(1000) / 1000.0f;
			}
		}

		public override void Update(GameTime gameTime)
		{
			m_sun.Update(gameTime);

			for (int i = 0; i < m_planets.Count; i++)
			{
				// calculate new position
				Vector3 newPosition = Vector3.Transform(new Vector3(0, 0, -m_planets[i].distanceToSun), 
					Matrix.CreateRotationY(m_planets[i].RotationSpeed * (float)gameTime.TotalGameTime.TotalSeconds));

				m_planets[i].Position = m_sun.Position + newPosition;

				m_planets[i].Update(gameTime);
			}

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			m_sun.Draw(gameTime);

			for (int i = 0; i < m_planets.Count; i++)
			{
				m_planets[i].Draw(gameTime);
			}


			base.Draw(gameTime);
		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_sun.SetCamera(view, projection);

			for (int i = 0; i < m_planets.Count; i++)
			{
				m_planets[i].SetCamera(view, projection);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Xe.Physics3D;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;

namespace Xe.SpaceRace
{
	class SolarSystem : DrawableGameComponent
	{
		Sun m_sun;

		List<Planet> m_planets;

		private Matrix m_orientation = Matrix.Identity;

		public Matrix Orientation
		{
			get { return m_orientation; }
			set
			{
				if (value != m_orientation)
				{
					m_sun.Orientation = value;

					for (int i = 0; i < m_planets.Count; i++)
						m_planets[i].Orientation = value;
				}
			}
		}

		public Sun Sun
		{
			get { return m_sun; }
		}

		public SolarSystem(GameScreenManager gameScreenManager, int planetCount)
			: base(gameScreenManager.Game)
		{
			m_sun = new Sun(gameScreenManager,this, new PhysicalType(0,0,0,0,100));
			m_sun.Position = new Vector3(0, 0, -20000);


			m_planets = new List<Planet>(planetCount);

			Random r = new Random();

			for (int i = 0; i < m_planets.Capacity; i++)
			{
				PlanetType tmpPlanetType = new PlanetType((PlanetType.Names)r.Next(0, ((int)PlanetType.Names.LASTUNUSED) - 1), 0, 0, 0, 0);

				m_planets.Add(new Planet(gameScreenManager, this, tmpPlanetType));

				m_planets[i].distanceToSun = m_planets[i==0?0:i-1].distanceToSun + 1000 + Helper.Random(1000);

				m_planets[i].SelfRotationSpeed = Helper.RandomFloat(0.03f, 0.3f);
				m_planets[i].AroundSunRotationSpeed = Helper.RandomFloat(0.03f, 0.3f);

				m_planets[i].SelfRotationOffset = Helper.RandomFloat(MathHelper.TwoPi);
				m_planets[i].AroundSunRotationOffset = Helper.RandomFloat(MathHelper.TwoPi);
			}
		}

		public override void Update(GameTime gameTime)
		{
			m_sun.Update(gameTime);

			for (int i = 0; i < m_planets.Count; i++)
			{
				
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

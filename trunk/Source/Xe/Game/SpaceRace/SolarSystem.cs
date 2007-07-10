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
	public class SolarSystem : DrawableGameComponent
	{
		Planet m_sun;

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

		public Planet Sun
		{
			get { return m_sun; }
		}

		public SolarSystem(GameScreenManager gameScreenManager,Planet Sun, int planetCount)
			: base(gameScreenManager.Game)
		{
			m_sun = Sun;

			m_planets = new List<Planet>(planetCount);

			Random r = new Random();

			for (int i = 0; i < m_planets.Capacity; i++)
			{
				PlanetType tmpPlanetType = new PlanetType((PlanetType.Names)Helper.Random(0, ((int)PlanetType.Names.LASTUNUSED) - 1), 0, 0, 0, 0);
				float prevDistanceToSun = 0;
				if (i > 0)
				{
					prevDistanceToSun = m_planets[i == 0 ? 0 : i - 1].Position.Z;
				}
				Vector3 startPosition=Vector3.Transform(new Vector3(0,0, prevDistanceToSun+ 1000 + Helper.Random(1000)),Matrix.CreateRotationY(Helper.RandomFloat(MathHelper.TwoPi)));
				Vector3 rotationSpeed=new Vector3(0,Helper.RandomFloat(0.03f, 0.3f),0);
				m_planets.Add(new Planet(gameScreenManager, this, tmpPlanetType,startPosition,rotationSpeed));


				/*m_planets[i].SelfRotationSpeed = Helper.RandomFloat(0.03f, 0.3f);
				m_planets[i].AroundSunRotationSpeed = Helper.RandomFloat(0.03f, 0.3f);

				m_planets[i].SelfRotationOffset = Helper.RandomFloat(MathHelper.TwoPi);
				m_planets[i].AroundSunRotationOffset = Helper.RandomFloat(MathHelper.TwoPi);
				 * */
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

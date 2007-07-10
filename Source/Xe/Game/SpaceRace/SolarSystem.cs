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

		public List<Planet> m_planets;

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

		public SolarSystem(GameScreenManager gameScreenManager,Planet Sun, int planetCount, int subLevelCount, int maxSubLevel)
			: base(gameScreenManager.Game)
		{
			if (Sun == null)
			{
				this.m_sun = new Planet(gameScreenManager, new PlanetType(PlanetType.Names.Sun, 0, 0, 0, 0), Vector3.Zero, Vector3.Zero);
			}
			else
				m_sun = Sun;

			m_planets = new List<Planet>(planetCount);

			Random r = new Random();

			for (int i = 0; i < m_planets.Capacity; i++)
			{
				PlanetType tmpPlanetType = new PlanetType((PlanetType.Names)Helper.Random(1, Enum.GetValues(typeof(PlanetType.Names)).Length), 0, 0, 0, 0);
				float prevDistanceToSun = 0;

				if (i > 0)
					prevDistanceToSun = m_planets[i-1].Position.Z;

				Vector3 startPosition=Vector3.Transform(new Vector3(0,0, prevDistanceToSun + 1000 + Helper.Random(1000)),Matrix.CreateRotationY(Helper.RandomFloat(MathHelper.TwoPi)));
				Vector3 rotationSpeed=new Vector3(0,Helper.RandomFloat(0.03f, 0.3f),0);

				Planet p = new Planet(gameScreenManager, tmpPlanetType, startPosition, rotationSpeed);
				p.SolarSystem = this;

				m_planets.Add(p);
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

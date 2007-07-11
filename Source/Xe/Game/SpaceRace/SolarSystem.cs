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

		private Matrix m_orientation = Matrix.Identity, m_decalage = Matrix.Identity;

		public Matrix Orientation
		{
			get { return m_orientation; }
			set { m_orientation = value; }
		}

		public Matrix Decalage
		{
			get { return m_decalage; }
			set { m_decalage = value; }
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
				this.m_sun = new Planet(gameScreenManager, new PlanetType(PlanetType.Names.Sun, 0, 0, 0, 0),0,0,0,Vector3.Zero,0,Vector3.Zero);
			}
			else
			{
				m_sun = Sun;
			}

			m_planets = new List<Planet>(planetCount);

			Random r = new Random();

			for (int i = 0; i < m_planets.Capacity; i++)
			{
				// determine a random PlanetType.Names
				PlanetType.Names n = (PlanetType.Names)Enum.GetValues(typeof(PlanetType.Names)).GetValue(Helper.Random(0, Enum.GetValues(typeof(PlanetType.Names)).Length-1));

				PlanetType tmpPlanetType = new PlanetType(n, 0, 0, 0, 0);
				float prevDistanceToSun = 0;
				
				if (i > 0)
					prevDistanceToSun = m_planets[i-1].m_distanceToSun + (int)m_planets[i-1].m_planetType.Name;

				float distanceToSun = prevDistanceToSun + 500 + Helper.Random(500);

				float rotationStart = Helper.RandomFloat(0, MathHelper.TwoPi);
				float rotationSpeed = Helper.RandomFloat(0.3f, 0.4f) / (i+1f);
				Vector3 rotationAxe = Vector3.Normalize(new Vector3(Helper.RandomFloat(-0.1f, 0.1f), 1, Helper.RandomFloat(-0.1f, 0.1f)));
				float selfRotationSpeed = rotationSpeed/Helper.RandomFloat(0.2f, 0.4f);
				Vector3 selfRotationAxe = Vector3.Normalize(new Vector3(Helper.RandomFloat(-0.1f, 0.1f), 1, Helper.RandomFloat(-0.1f, 0.1f)));


				Planet p = new Planet(gameScreenManager, tmpPlanetType,distanceToSun,rotationStart, rotationSpeed,rotationAxe,selfRotationSpeed,selfRotationAxe);
				p.SolarSystem = this;

				m_planets.Add(p);
			}
		}

		public override void Update(GameTime gameTime)
		{
			m_sun.Update(gameTime);
			m_orientation = m_sun.Orientation*m_decalage;

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

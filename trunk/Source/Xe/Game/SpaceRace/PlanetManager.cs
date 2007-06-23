using System;
using System.Collections.Generic;
using System.Text;
using Xe.GameScreen;
using Microsoft.Xna.Framework;

namespace Xe.SpaceRace
{
	class PlanetManager : DrawableGameComponent
	{
		GameScreenManager m_gameScreenManager;

		public GameScreenManager GameScreenManager
		{
			get { return m_gameScreenManager; }
		}

		List<Planet> m_planets = new List<Planet>();

		Matrix m_view, m_projection;

		public PlanetManager(GameScreenManager manager)
			: base(manager.Game)
		{
			m_gameScreenManager = manager;
		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			this.m_view = view;
			this.m_projection = projection;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			for (int i = 0; i < m_planets.Count; i++)
			{
				m_planets[i].Model.View = this.m_view;
				m_planets[i].Model.Projection = this.m_projection;
				m_planets[i].Update(gameTime);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			for (int i = 0; i < m_planets.Count; i++)
			{
				m_planets[i].Draw(gameTime);
			}
			
			base.Draw(gameTime);
		}



		public void AddPlanet()
		{
			Planet p = new Planet(this, new PlanetType(@"Content\Models\Asteroid4",0,0,0,0));

			m_planets.Add(p);
		}
	}
}

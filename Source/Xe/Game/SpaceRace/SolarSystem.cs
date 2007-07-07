using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Xe.Physics3D;
using Xe.GameScreen;

namespace Xe.SpaceRace
{
	class SolarSystem : DrawableGameComponent
	{
		Sun m_sun;
		PlanetManager m_planetManager;

		public SolarSystem(GameScreenManager gameScreenManager, int planetCount)
			: base(gameScreenManager.Game)
		{
			m_planetManager = new PlanetManager(gameScreenManager);

			m_sun = new Sun(m_planetManager, new PhysicalType(0,0,0,0,100));
			m_sun.Position = new Vector3(0, 0, 200); ;
		}


		public override void Update(GameTime gameTime)
		{
			m_sun.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			m_sun.Draw(gameTime);
			base.Draw(gameTime);
		}





		public void SetCamera(Matrix view, Matrix projection)
		{
			m_sun.SetCamera(view, projection);
		}
	}
}

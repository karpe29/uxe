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

		//List<CheckPoint> m_checkPoints;
		//List<WormHole> m_wormHoles;

		Universe m_universe;

		public Race(GameScreenManager gameScreenManager, float difficultyPercent)
			: base(gameScreenManager.Game)
		{
			m_difficultyPercent = difficultyPercent;

			m_universe = new Universe(gameScreenManager);
		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_universe.SetCamera(view, projection);
		}

		public override void Update(GameTime gameTime)
		{
			m_universe.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			m_universe.Draw(gameTime);
		}

		
	}
}

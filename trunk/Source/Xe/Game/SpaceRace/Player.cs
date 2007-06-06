using System;
using System.Collections.Generic;
using System.Text;
using XeFramework.XeGame.SpaceRace;
using XeFramework.Graphics3D;
using Microsoft.Xna.Framework;
using XeFramework.GameScreen;

namespace Xe.Game.SpaceRace
{
	class Player : DrawableGameComponent
	{

		private Ship m_ship;

		private ChaseCamera m_camera;

		public Player(GameScreenManager gameScreenManager,Ship Ship):base(gameScreenManager.Game)
		{
			m_ship=Ship;
		}

		public Ship Ship
		{
			get { return m_ship; }
		}

	}
}

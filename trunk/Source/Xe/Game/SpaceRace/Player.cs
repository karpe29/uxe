using System;
using System.Collections.Generic;
using System.Text;
using XeFramework.XeGame.SpaceRace;
using XeFramework.Graphics3D;

namespace Xe.Game.SpaceRace
{
	class Player
	{

		private Ship m_ship;

		private ChaseCamera m_camera;

		public Player(Ship Ship)
		{
			m_ship=Ship;
		}

		public Ship Ship
		{
			get { return m_ship; }
		}

	}
}

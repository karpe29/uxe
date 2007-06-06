using System;
using System.Collections.Generic;
using System.Text;
using XeFramework.XeGame.SpaceRace;

namespace Xe.Game
{
	class SpaceRaceInitDatas
	{

		/// difficulty level in percent
		public float difficultyPercent;

		// total player count (start @ 0)
		public int totalPlayerCount;

		// used for ShipSelectionScreen s
		public int currentPlayerNumber;

		// store selected shipTypes
		public List<ShipType> shipTypes = new List<ShipType>();
	}
}

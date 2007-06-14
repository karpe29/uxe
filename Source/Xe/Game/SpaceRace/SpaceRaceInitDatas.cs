using System;
using System.Collections.Generic;
using System.Text;

namespace Xe.SpaceRace
{
	class SpaceRaceInitDatas
	{
		private float m_difficultyPercent;
		/// <summary>
		/// Difficulty level in percent
		/// </summary>
		public float DifficultyPercent
		{
			get
			{
				return m_difficultyPercent;
			}
			set
			{
				m_difficultyPercent = value;
			}
		}


		private int m_totalPlayerCount;
		/// <summary>
		/// Total player count (start @ 0)
		/// </summary>
		public int TotalPlayerCount
		{
			get
			{
				return m_totalPlayerCount;
			}
			set
			{
				m_totalPlayerCount = value;
			}
		}
		

		private int m_currentPlayerNumber;
		/// <summary>
		/// Only used for ShipSelectionScreens (start @ 0)
		/// </summary>
		public int CurrentPlayerNumber
		{
			get
			{
				return m_currentPlayerNumber;
			}
			set
			{
				m_currentPlayerNumber = value;
			}
		}

		
		private List<ShipType> m_shipTypes = new List<ShipType>();
		/// <summary>
		/// Store selected shipTypes (player one ShipType is ShipTypes[0])
		/// </summary>
		public List<ShipType> ShipTypes
		{
			get { return m_shipTypes; }
		}
	}
}

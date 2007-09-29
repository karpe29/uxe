using System;
using System.Collections.Generic;
using System.Text;
using Xe.Tools;

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

		private List<int> m_gamePadIndexes;
		/// <summary>
		/// Store Gamepad indexes (player one can be controlled by gamepad #2 in some cases)
		/// So i have to determine the lowest free gamepad index in ship selection.
		/// </summary>
		public List<int> GamePadIndexes
		{
			get { return m_gamePadIndexes; }
		}


		public SpaceRaceInitDatas(float difficulty, int playerCount)
		{
			m_difficultyPercent = difficulty;
			m_totalPlayerCount = playerCount;

			m_gamePadIndexes = new List<int>(playerCount);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Xe.GameScreen;
using Microsoft.Xna.Framework;

namespace Xe.SpaceRace
{
	public class Race : DrawableGameComponent
	{
		SpaceRaceInitDatas m_datas;

		public SpaceRaceInitDatas Datas
		{ get { return m_datas; } }

		//List<CheckPoint> m_checkPoints;
		//List<WormHole> m_wormHoles;

		List<Player> m_players;

		Universe m_universe;

		public Universe Universe
		{ get { return m_universe; } }


		public Race(GameScreenManager gameScreenManager, SpaceRaceInitDatas datas)
			: base(gameScreenManager.Game)
		{
			m_datas = datas;



			m_players = new List<Player>(m_datas.TotalPlayerCount);

			for (int i = 0; i < m_datas.TotalPlayerCount; i++)
			{
				m_players.Add(new Player(gameScreenManager, this, m_datas.ShipTypes[i], (PlayerIndex)i, m_datas.GamePadIndexes[i]));
			}

			m_universe = new Universe(gameScreenManager);
		}

		public override void Update(GameTime gameTime)
		{
			for (int i = 0; i < m_datas.TotalPlayerCount; i++)
			{
				m_players[i].Update(gameTime);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			for (int i = 0; i < m_datas.TotalPlayerCount; i++)
			{
				m_players[i].Draw(gameTime);
			}
		}


	}
}

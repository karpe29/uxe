using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Xe.Graphics3D;
using Microsoft.Xna.Framework;
using Xe.GameScreen;


namespace Xe.SpaceRace
{
	class SpaceRaceScreen : IGameScreen
	{
		SpaceRaceInitDatas m_datas;

		List<Player> m_players;
		
		Race m_race;

		public Race Race
		{
			get { return m_race; }
		}

		public SpaceRaceScreen(GameScreenManager gameScreenManager, SpaceRaceInitDatas datas)
			: base(gameScreenManager, true)
		{
			// disable EBI
			//Ebi e = (Ebi)Game.Services.GetService(typeof(IEbiService));
			//e.Enabled = false;

			m_datas = datas;

			m_race = new Race(gameScreenManager, m_datas.DifficultyPercent);

			m_players = new List<Player>(m_datas.TotalPlayerCount+1);

			for(int i = 0; i< m_datas.TotalPlayerCount +1; i++)
			{
				m_players.Add(new Player(gameScreenManager, m_datas.ShipTypes[i]));
			}
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
			}
		}

		public override void Draw(GameTime gameTime)
		{
			m_players[0].Draw(gameTime);
			
			base.Draw(gameTime);
		}

		
		private void RenderSpaceBackground()
		{
			#region Sprite part (unused)
			// TODO : with the ships positions, calculate the offset to apply to the texture.
			//7 4 1
			//8 5 2
			//9 6 3
			/*
			int w = m_backgroundTexture.Width;
			int h = m_backgroundTexture.Height;

			Vector2 v1 = new Vector2((m_ship.Position.X % w) + w, (m_ship.Position.Z % h) - h);
			Vector2 v2 = new Vector2((m_ship.Position.X % w) + w, (m_ship.Position.Z % h));
			Vector2 v3 = new Vector2((m_ship.Position.X % w) + w, (m_ship.Position.Z % h) + h);

			Vector2 v4 = new Vector2((m_ship.Position.X % w), (m_ship.Position.Z % h) - h);
			Vector2 v5 = new Vector2((m_ship.Position.X % w), (m_ship.Position.Z % h));
			Vector2 v6 = new Vector2((m_ship.Position.X % w), (m_ship.Position.Z % h) + h);

			Vector2 v7 = new Vector2((m_ship.Position.X % w) - w, (m_ship.Position.Z % h) - h);
			Vector2 v8 = new Vector2((m_ship.Position.X % w) - w, (m_ship.Position.Z % h));
			Vector2 v9 = new Vector2((m_ship.Position.X % w) - w, (m_ship.Position.Z % h) + h);


			m_spriteBatch.Begin();

			m_spriteBatch.Draw(m_backgroundTexture, -v1, Color.White);
			m_spriteBatch.Draw(m_backgroundTexture, -v2, Color.White);
			m_spriteBatch.Draw(m_backgroundTexture, -v3, Color.White);
			m_spriteBatch.Draw(m_backgroundTexture, -v4, Color.White);
			m_spriteBatch.Draw(m_backgroundTexture, -v5, Color.White);
			m_spriteBatch.Draw(m_backgroundTexture, -v6, Color.White);
			m_spriteBatch.Draw(m_backgroundTexture, -v7, Color.White);
			m_spriteBatch.Draw(m_backgroundTexture, -v8, Color.White);
			m_spriteBatch.Draw(m_backgroundTexture, -v9, Color.White);

			m_spriteBatch.End();
			*/
			#endregion
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			m_players[0].Update(gameTime);
		}

		public override bool IsBlockingUpdate
		{
			get { return false; }
		}

		public override bool IsBlockingDraw
		{
			get { return false; }
		}
	}
}

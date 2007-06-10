using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Xe.Input;
using Xe.Graphics3D;
using Microsoft.Xna.Framework;
using Xe.Objects3D;
using Xe.GameScreen;


namespace Xe.SpaceRace
{
	class SpaceRaceScreen : IGameScreen
	{
		SkyBox s;

		SpaceRaceInitDatas m_datas;

		int m_playerCount;
		float m_difficultyPercent;

		SpriteBatch m_spriteBatch;

		List<Player> m_players;
		Race m_race;

		public SpaceRaceScreen(GameScreenManager gameScreenManager, SpaceRaceInitDatas datas)
			: base(gameScreenManager, true)
		{

			// disable EBI
			//Ebi e = (Ebi)Game.Services.GetService(typeof(IEbiService));
			//e.Enabled = false;

			m_datas = datas;

			m_players = new List<Player>(m_datas.totalPlayerCount+1);

			for(int i = 0; i< m_datas.totalPlayerCount +1; i++)
			{
				m_players.Add(new Player(gameScreenManager, m_datas.shipTypes[i]));
			}

			s = new SkyBox(gameScreenManager.Game, @"Content\Skybox\bryce");
			s.ContentManager = gameScreenManager.ContentManager;
			s.Initialize();
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
			}

			m_spriteBatch = new SpriteBatch(this.GraphicsDevice);
		}

		public override void Draw(GameTime gameTime)
		{
			s.Draw(gameTime);

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


			// Definir les matrices / position du vaisseau ici pour la skybox, et voila :)

			 //pour voir ce que t'as fait
			//m_model.World = Matrix.CreateRotationZ(rotationPosition.Z) * Matrix.CreateRotationX(rotationPosition.X) * Matrix.CreateRotationY(rotationPosition.Y);
			//m_model.View = Matrix.CreateLookAt(new Vector3(0, 4000, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0));
			//m_model.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)(4 / 3), 1, 10000);

			
			s.CameraPosition = m_players[0].m_camera.Position;
			s.CameraDirection = m_players[0].m_camera.LookAtOffset;
			s.ViewMatrix = m_players[0].m_camera.View;
			s.ProjectionMatrix = m_players[0].m_camera.Projection;

			s.Update(gameTime);
			
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

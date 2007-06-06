using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using XeFramework.Input;
using XeFramework.Graphics3D;
using Microsoft.Xna.Framework;
using XeFramework.XeGame.SpaceRace;
using XeFramework.Objects3D;
using Xe.Game.SpaceRace;
using Xe.Game;

namespace XeFramework.GameScreen
{
	class SpaceRaceScreen : IGameScreen
	{
		#region Matrices

		//float AspectRatio = (float)GameScreenManager.Game.Window.ClientBounds.Width / (float)GameScreenManager.Game.Window.ClientBounds.Height;




		public Matrix ProjectionMatrix;
		//Matrix.CreateOrthographic(20000, 20000, 1, 25000);
		//Matrix.CreatePerspective(f, f, 1, 100000);
		//Matrix.CreatePerspectiveOffCenter(-f, f, -f, f, 1, 1000000);
		//Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), AspectRatio, 1.0f, 100000.0f);

		public Matrix ViewMatrix =
			//Matrix.CreateLookAt(new Vector3(0, 20000, 0), Vector3.Zero, Vector3.Forward);
		Matrix.CreateLookAt(new Vector3(5000, 5000, 5000), Vector3.Zero, Vector3.Up);

		SkyBox s;

		#endregion

		int m_playerCount;
		float m_difficultyPercent;

		SpriteBatch m_spriteBatch;

		List<Player> m_players;
		Race m_race;

		private SpaceRaceScreen(GameScreenManager gameScreenManager)
			: this(gameScreenManager, 50, 1)
		{
		}

		public SpaceRaceScreen(GameScreenManager gameScreenManager, float difficultyPercent, int playerCount)
			: base(gameScreenManager, true)
		{
			m_difficultyPercent = difficultyPercent;
			m_playerCount = playerCount;

			m_players = new List<Player>(m_playerCount);

			this.Enabled = false;
			this.Visible = false;

			for (int i = m_playerCount; i > 0; i--)
			{
				m_players.Add(new Player(gameScreenManager,new Ship(gameScreenManager,ShipType.Types[3])));

				ShipSelectionScreen screen = new ShipSelectionScreen(this.GameScreenManager, i, this);

				if (i != 1)
					screen.Visible = false; //should be .Enabled as visible state of control is changed in Update()
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
			base.Draw(gameTime);
			m_players[0].Ship.Draw(gameTime);
			//RenderSpaceBackground();

			s.Draw(gameTime);

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
			m_players[0].Ship.Update(gameTime);

			/*


			s.CameraPosition = m_ship.Position;
			s.CameraDirection = m_ship.Position - m_ship.transformedReference;
			s.ViewMatrix = m_ship.ViewMatrix;
			s.ProjectionMatrix = m_ship.ProjectionMatrix;

			s.Update(gameTime);
			 * */
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

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using XeFramework.Input;
using XeFramework.Graphics3D;
using Microsoft.Xna.Framework;
using XeFramework.XeGame.SpaceRace;
using XeFramework.Objects3D;

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

		SpriteBatch m_spriteBatch;
		Texture2D m_backgroundTexture;

		Ship m_ship;
		List<WormHole> m_wormHoles;

		private SpaceRaceScreen(GameScreenManager gameScreenManager)
			: this(gameScreenManager, 50)
		{
		}

		public SpaceRaceScreen(GameScreenManager gameScreenManager, float difficultyPercent)
			: base(gameScreenManager, true)
		{
			s = new SkyBox(gameScreenManager.Game, @"Content\Skybox\bryce");
			s.ContentManager = gameScreenManager.ContentManager;
			s.Initialize();

			m_ship = new Ship(this.GameScreenManager);
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
				m_backgroundTexture = this.GameScreenManager.ContentManager.Load<Texture2D>(@"Content\Textures\Starfield\ultimate starfield");
			}

			m_spriteBatch = new SpriteBatch(this.GraphicsDevice);



		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//RenderSpaceBackground();


			s.Draw(gameTime);

			m_ship.Draw(gameTime);
		}

		private void RenderSpaceBackground()
		{
			#region Sprite part (unused)
			// TODO : with the ships positions, calculate the offset to apply to the texture.
			//7 4 1
			//8 5 2
			//9 6 3

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
			#endregion


		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			

			m_ship.Update(gameTime);

			s.CameraPosition = m_ship.Position;
			s.CameraDirection = m_ship.Position - m_ship.transformedReference;
			s.ViewMatrix = m_ship.ViewMatrix;
			s.ProjectionMatrix = m_ship.ProjectionMatrix;

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

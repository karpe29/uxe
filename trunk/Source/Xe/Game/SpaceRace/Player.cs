using System;
using System.Collections.Generic;
using System.Text;
using Xe.SpaceRace;
using Xe.Graphics3D;
using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Input;
using Xe.Tools;
using Xe.Physics3D;
using Microsoft.Xna.Framework.Graphics;
using Xe.Input;

namespace Xe.SpaceRace
{


	public class Player : DrawableGameComponent
	{
		int m_gamePadIndex = 0;
		PlayerIndex m_playerIndex = PlayerIndex.One;

		GameScreenManager m_gameScreenManager;

		SpaceRaceScreen m_raceScreen;

		Race m_race;

		private Ship m_ship;

		public ChaseCamera m_camera;

		SkyBox s;

		GamePadState m_gamePadState;

		public Player(GameScreenManager gameScreenManager, ShipType type, PlayerIndex playerIndex, int gamePadIndex)
			: base(gameScreenManager.Game)
		{
			m_gameScreenManager = gameScreenManager;

			m_raceScreen = (SpaceRaceScreen)gameScreenManager.CurrentGameScreen;

			m_race = m_raceScreen.Race;

			m_playerIndex = playerIndex;
			m_gamePadIndex = gamePadIndex;

			m_ship = new Ship(gameScreenManager, type);
			//m_ship.Position = new Vector3(0, 0, 0);
			m_ship.Orientation = Matrix.CreateRotationX(-1.2f);

			m_camera = new ChaseCamera((IShipPhysical)m_ship, new Vector3(0, 40, 0), new Vector3(0, 120, 150));
			//m_camera = new ChaseCamera((IShipPhysical)m_ship, new Vector3(0, 40, 50), new Vector3(0, 200, 100));
			SpaceRaceHudScreen hud = new SpaceRaceHudScreen(gameScreenManager);

			s = new SkyBox(gameScreenManager.Game, @"Content\Skybox\bryce");
			s.ContentManager = XeGame.ContentManager;
			s.Initialize();
		}

		public Ship Ship
		{
			get { return m_ship; }
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);


			GamePadState g = InputHelper.GamePad[(int)this.m_gamePadIndex];

			KeyboardState touche = Keyboard.GetState();

			m_ship.MoveState.Reset();

			if (touche.IsKeyDown(Keys.Up) || g.ThumbSticks.Left.Y < 0)
			{
				m_ship.MoveState.Down = true;
			}
			if (touche.IsKeyDown(Keys.Down) || g.ThumbSticks.Left.Y > 0)
			{
				m_ship.MoveState.Up = true;
			}

			if (touche.IsKeyDown(Keys.Left) || g.ThumbSticks.Left.X < 0)
			{
				m_ship.MoveState.TurnLeft = true;
			}
			if (touche.IsKeyDown(Keys.Right) || g.ThumbSticks.Left.X > 0)
			{
				m_ship.MoveState.TurnRight = true;
			}

			if (touche.IsKeyDown(Keys.Space) || g.Triggers.Right > 0)
			{
				m_ship.MoveState.Forward = true;
			}
			if (touche.IsKeyDown(Keys.RightAlt) || g.Triggers.Left > 0)
			{
				m_ship.MoveState.Brake = true;
			}

			m_ship.Update(gameTime);

			m_camera.Update(gameTime);

			m_ship.Model.View = m_camera.View;
			m_ship.Model.Projection = m_camera.Projection;

			s.CameraPosition = m_ship.Position;
			s.CameraDirection = m_ship.Direction;
			s.ViewMatrix = m_camera.View;
			s.ProjectionMatrix = m_camera.Projection;

			s.Update(gameTime);

			m_race.SetCamera(m_camera.View, m_camera.Projection);

			m_race.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			SetupViewport();



			s.Draw(gameTime);


			XeGame.Device.RenderState.DepthBufferEnable = true;
			XeGame.Device.RenderState.DepthBufferWriteEnable = true;

			m_race.Draw(gameTime);

			XeGame.Device.RenderState.AlphaSourceBlend = Blend.Zero;
			XeGame.Device.RenderState.AlphaBlendEnable = false;
			XeGame.Device.RenderState.DepthBufferEnable = true;
			XeGame.Device.RenderState.DepthBufferWriteEnable = true;

			m_ship.setParticlesView(m_camera.ViewParticles);
			m_ship.Draw(gameTime);



			base.Draw(gameTime);
		}

		private void SetupViewport()
		{
			// 1 player
			if (m_raceScreen.InitDatas.TotalPlayerCount == 0) 
				return; // don't touch viewport, we're obviously the only player :)
			
			Viewport v = new Viewport();
			v.X = 0;
			v.Y = 0;
			v.Width = XeGame.Device.PresentationParameters.BackBufferWidth;
			v.Height = XeGame.Device.PresentationParameters.BackBufferHeight;

			#region 2 players
			if (m_raceScreen.InitDatas.TotalPlayerCount == 1) // 2 player
			{
				if (v.Height > v.Width) // cut height in 2
				{
					v.Height = v.Height / 2;

					if (this.m_playerIndex == PlayerIndex.One)
					{
						v.Y = 0;
					}

					if (this.m_playerIndex == PlayerIndex.Two)
					{
						v.Y = v.Height;
					}
				}
				else
				{
					v.Width = v.Width / 2;

					if (this.m_playerIndex == PlayerIndex.One)
					{
						v.X = 0;
					}
					
					if (this.m_playerIndex == PlayerIndex.Two)
					{
						v.X = v.Width;
					}
				}
			}
			#endregion

			#region 3 players
			if (m_raceScreen.InitDatas.TotalPlayerCount == 2) // 3 player
			{
				if (v.Height > v.Width)
				{
					v.Height = v.Height / 3;

					if (this.m_playerIndex == PlayerIndex.One)
						v.Y = 0;

					if (this.m_playerIndex == PlayerIndex.Two)
						v.Y = v.Height;

					if (this.m_playerIndex == PlayerIndex.Three)
						v.Y = v.Height * 2;
				}
				else // standard case (4/3, 16/9, 16/10)
				{
					v.Width = v.Width / 2;

					if (this.m_playerIndex == PlayerIndex.One)
					{
						v.Y = 0;
						v.X = 0;
					}
					
					if (this.m_playerIndex == PlayerIndex.Two)
					{
						v.Height = v.Height / 2;
						v.X = v.Width;
					}

					if (this.m_playerIndex == PlayerIndex.Three)
					{
						v.Height = v.Height / 2;
						v.X = v.Width;
						v.Y = v.Height;
					}
				}
			}
			#endregion

			#region 4 players
			if (m_raceScreen.InitDatas.TotalPlayerCount == 3) // 4 player
			{
				if (v.Height > v.Width)
				{
					v.Height = v.Height / 4;

					if (this.m_playerIndex == PlayerIndex.One)
						v.Y = 0;

					if (this.m_playerIndex == PlayerIndex.Two)
						v.Y = v.Height;

					if (this.m_playerIndex == PlayerIndex.Three)
						v.Y = v.Height * 2;

					if (this.m_playerIndex == PlayerIndex.Four)
						v.Y = v.Height * 3;
				}
				else // standard case
				{
					v.Width = v.Width / 2;
					v.Height = v.Height / 2;

					if (this.m_playerIndex == PlayerIndex.One)
					{
						v.Y = 0;
						v.X = 0;
					}

					if (this.m_playerIndex == PlayerIndex.Two)
					{
						v.Y = 0;
						v.X = v.Width;
					}

					if (this.m_playerIndex == PlayerIndex.Three)
					{
						v.X = 0;
						v.Y = v.Height;
					}

					if (this.m_playerIndex == PlayerIndex.Four)
					{
						v.X = v.Width;
						v.Y = v.Height;
					}
				}
			}
			#endregion

			XeGame.Device.Viewport = v;
		}

	}
}

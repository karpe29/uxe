using System;
using System.Collections.Generic;
using System.Text;
using Xe.SpaceRace;
using Xe.Graphics3D;
using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Input;
using Xe.Tools;
using Xe.Objects3D;
using Xe.Physics3D;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.SpaceRace
{


	class Player : DrawableGameComponent
	{
		GameScreenManager m_gameScreenManager;

		SpaceRaceScreen m_raceScreen;

		Race m_race;

		private Ship m_ship;

		ChaseCamera m_camera;

		SkyBox s;

		public Player(GameScreenManager gameScreenManager, ShipType type)
			: base(gameScreenManager.Game)
		{
			m_gameScreenManager = gameScreenManager;

			m_raceScreen = (SpaceRaceScreen)gameScreenManager.CurrentGameScreen;

			m_race = m_raceScreen.Race;

			m_ship = new Ship(gameScreenManager, type);

			m_camera = new ChaseCamera((IPhysical3D)m_ship, new Vector3(0, 40, 0), new Vector3(0, 120, 150));

			SpaceRaceHudScreen hud = new SpaceRaceHudScreen(gameScreenManager);

			s = new SkyBox(gameScreenManager.Game, @"Content\Skybox\bryce");
			s.ContentManager = gameScreenManager.ContentManager;
			s.Initialize();
		}

		public Ship Ship
		{
			get { return m_ship; }
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			GamePadState g = GamePad.GetState(PlayerIndex.One);

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
			s.Draw(gameTime);


			GraphicsDevice device = ((IGraphicsDeviceService)Game.Services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

			

			device.RenderState.DepthBufferEnable = true;
			device.RenderState.DepthBufferWriteEnable = true;
			
			m_race.Draw(gameTime);

			device.RenderState.DepthBufferEnable = true;
			device.RenderState.DepthBufferWriteEnable = true;

			m_ship.Draw(gameTime);

			

			base.Draw(gameTime);
		}

	}
}
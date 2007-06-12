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
		private Ship m_ship;

		public ChaseCamera m_camera;

		SkyBox s;


		public Player(GameScreenManager gameScreenManager, ShipType type)
			: base(gameScreenManager.Game)
		{
			m_gameScreenManager = gameScreenManager;

			m_ship = new Ship(gameScreenManager, type);

			m_camera = new ChaseCamera((IPhysical3D)m_ship, new Vector3(0, 40, 0), new Vector3(0, 120, 150));

			/*m_camera.Stiffness = 6000; // default 1800
			m_camera.Mass = 10; // default 50
			m_camera.Damping = 200; // default 600
			m_camera.DesiredPositionOffset = new Vector3(0, 100, 200);
			m_camera.ChasePosition = Vector3.Zero;
			m_camera.ChaseDirection = Vector3.Forward;
			m_camera.Up = Vector3.Up;
			m_camera.Reset();*/

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
			
			s.CameraPosition = m_ship.linearPosition;
			s.CameraDirection = m_ship.direction;
			s.ViewMatrix = m_camera.View;
			s.ProjectionMatrix = m_camera.Projection;
			
			s.Update(gameTime);
		}
		public override void Draw(GameTime gameTime)
		{
			s.Draw(gameTime);


			GraphicsDevice device = ((IGraphicsDeviceService)Game.Services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

			device.RenderState.DepthBufferEnable = true;
			device.RenderState.DepthBufferWriteEnable = true;

			m_ship.Draw(gameTime);

			base.Draw(gameTime);
		}

	}
}

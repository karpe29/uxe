using System;
using System.Collections.Generic;
using System.Text;
using Xe.SpaceRace;
using Xe.Graphics3D;
using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Input;

namespace Xe.SpaceRace
{


	class Player : DrawableGameComponent
	{
		GameScreenManager m_gameScreenManager;
		private Ship m_ship;

		public ChaseCamera m_camera;

		public Player(GameScreenManager gameScreenManager, ShipType type)
			: base(gameScreenManager.Game)
		{
			m_gameScreenManager = gameScreenManager;

			m_ship = new Ship(gameScreenManager, type);

			m_camera = new ChaseCamera();
			m_camera.Stiffness = 20000;
			m_camera.Mass = 10;
			m_camera.Damping = 1200;
			m_camera.DesiredPositionOffset = new Vector3(200, 100, 0);
			m_camera.ChasePosition = m_ship.linearPosition;
			m_camera.ChaseDirection = Vector3.Forward;
			m_camera.Up = Vector3.Up;
			m_camera.Reset();
		}

		public Ship Ship
		{
			get { return m_ship; }
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			KeyboardState touche = Keyboard.GetState();
			m_ship.MoveState.Reset();

			if (touche.IsKeyDown(Keys.Up))
			{
				m_ship.MoveState.Forward = true;
			}
			if (touche.IsKeyDown(Keys.Down))
			{
				m_ship.MoveState.Brake = true;
			}

			if (touche.IsKeyDown(Keys.Left))
			{
				m_ship.MoveState.TurnLeft = true;
			}
			if (touche.IsKeyDown(Keys.Right))
			{
				m_ship.MoveState.TurnRight = true;
			}

			m_ship.Update(gameTime);

			m_camera.ChasePosition = m_ship.linearPosition;
			m_camera.ChaseDirection = Vector3.Transform(Vector3.Forward,m_ship.Model.World);

			//this.m_gameScreenManager.Stats.AddDebugString(m_camera.ChasePosition.ToString());
			//this.m_gameScreenManager.Stats.AddDebugString(m_camera.ChaseDirection.ToString());
			this.m_gameScreenManager.Stats.AddDebugString("");
			this.m_gameScreenManager.Stats.AddDebugString("rot pos : " + m_ship.rotationPosition.ToString());
			this.m_gameScreenManager.Stats.AddDebugString("rot speed : " + m_ship.rotationSpeed.ToString());
			this.m_gameScreenManager.Stats.AddDebugString("rot acc : " + m_ship.rotationAcceleration.ToString());
			this.m_gameScreenManager.Stats.AddDebugString("");
			this.m_gameScreenManager.Stats.AddDebugString("lin pos : " + m_ship.linearPosition.ToString());
			this.m_gameScreenManager.Stats.AddDebugString("lin speed : " + m_ship.linearSpeed.ToString());
			this.m_gameScreenManager.Stats.AddDebugString("lin acc : " + m_ship.linearAcceleration.ToString());
			
			m_camera.Reset();


		}
		public override void Draw(GameTime gameTime)
		{
			m_ship.Model.View = m_camera.View;
			m_ship.Model.Projection = m_camera.Projection;

			m_ship.Draw(gameTime);

			base.Draw(gameTime);
		}

	}
}

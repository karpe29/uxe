using System;
using System.Collections.Generic;
using System.Text;
using Xe.SpaceRace;
using Xe.Graphics3D;
using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Input;
using Xe.Tools;

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
			//m_camera.Stiffness = 1800; // default 1800
			//m_camera.Mass = 50; // default 50
			//m_camera.Damping = 600; // default 600
			m_camera.DesiredPositionOffset = new Vector3(0, 100, 200);
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
				m_ship.MoveState.Down = true;
			}
			if (touche.IsKeyDown(Keys.Down))
			{
				m_ship.MoveState.Up = true;
			}

			if (touche.IsKeyDown(Keys.Left))
			{
				m_ship.MoveState.TurnLeft = true;
			}
			if (touche.IsKeyDown(Keys.Right))
			{
				m_ship.MoveState.TurnRight = true;
			}

			if (touche.IsKeyDown(Keys.Space))
			{
				m_ship.MoveState.Forward = true;
			}
			if (touche.IsKeyDown(Keys.RightAlt))
			{
				m_ship.MoveState.Brake = true;
			}

			m_ship.Update(gameTime);

			m_camera.ChasePosition = m_ship.linearPosition;
			m_camera.ChaseDirection = m_ship.direction;
			m_camera.Up = m_ship.up;

			//this.m_gameScreenManager.Stats.AddDebugString(m_camera.ChasePosition.ToString());
			//this.m_gameScreenManager.Stats.AddDebugString(m_camera.ChaseDirection.ToString());
			this.m_gameScreenManager.Stats.AddDebugString("");
			this.m_gameScreenManager.Stats.AddDebugString("rot pos : " + Helper.Vector3ToString3f(m_ship.rotationPosition));
			this.m_gameScreenManager.Stats.AddDebugString("rot speed : " + Helper.Vector3ToString3f(m_ship.rotationSpeed));
			this.m_gameScreenManager.Stats.AddDebugString("rot acc : " + Helper.Vector3ToString3f(m_ship.rotationAcceleration));
			this.m_gameScreenManager.Stats.AddDebugString("");
			this.m_gameScreenManager.Stats.AddDebugString("lin pos : " + Helper.Vector3ToString3f(m_ship.linearPosition));
			this.m_gameScreenManager.Stats.AddDebugString("lin speed : " + Helper.Vector3ToString3f(m_ship.linearSpeed));
			this.m_gameScreenManager.Stats.AddDebugString("lin acc : " + Helper.Vector3ToString3f(m_ship.linearAcceleration));
			this.m_gameScreenManager.Stats.AddDebugString("");
			this.m_gameScreenManager.Stats.AddDebugString("Direction : " + Helper.Vector3ToString3f(m_ship.direction));
			this.m_gameScreenManager.Stats.AddDebugString("Up : " + Helper.Vector3ToString3f(m_ship.up));
			this.m_gameScreenManager.Stats.AddDebugString("");
			this.m_gameScreenManager.Stats.AddDebugString("Cam pos : " + Helper.Vector3ToString3f(m_camera.Position));
			this.m_gameScreenManager.Stats.AddDebugString("cam chase pos : " + Helper.Vector3ToString3f(m_camera.ChasePosition));
			
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

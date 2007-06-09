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

		private Ship m_ship;

		public ChaseCamera m_camera;

		public Player(GameScreenManager gameScreenManager, ShipType type)
			: base(gameScreenManager.Game)
		{
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
			m_camera.Update(gameTime);


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

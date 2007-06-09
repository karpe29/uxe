using System;
using System.Collections.Generic;
using System.Text;
using XeFramework.XeGame.SpaceRace;
using XeFramework.Graphics3D;
using Microsoft.Xna.Framework;
using XeFramework.GameScreen;
using Microsoft.Xna.Framework.Input;

namespace Xe.Game.SpaceRace
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
			m_camera.DesiredPositionOffset = new Vector3(200, 100, 0);
			m_camera.ChasePosition = m_ship.linearPosition;
			m_camera.ChaseDirection = Vector3.Forward;
			m_camera.Up = Vector3.Up;
		}

		public Ship Ship
		{
			get { return m_ship; }
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			KeyboardState touche = Keyboard.GetState();

			if (touche.IsKeyDown(Keys.Up))
			{
				m_ship.linearSpeed = new Vector3(0, 0, 10);
				
			}
			if (touche.IsKeyDown(Keys.Down))
			{
				m_ship.linearSpeed = new Vector3(0, 0, -10);
			}

			m_ship.Update(gameTime);
			
			m_camera.ChasePosition = m_ship.linearPosition;
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

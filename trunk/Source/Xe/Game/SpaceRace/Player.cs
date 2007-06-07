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

		private ChaseCamera m_camera;

		public Player(GameScreenManager gameScreenManager,Ship Ship):base(gameScreenManager.Game)
		{
			m_ship=Ship;
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
				
			}
			if (touche.IsKeyDown(Keys.Down))
			{
				m_ship.linearAcceleration = new Vector3(0, 0, -100);
			}



			m_ship.Update(gameTime);
		}
		public override void Draw(GameTime gameTime)
		{

			m_ship.Draw(gameTime);
			base.Draw(gameTime);
		}

	}
}

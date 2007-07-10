using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xe.Physics3D
{
	public class IPlanetPhysical : IPhysical3D
	{

		public IPlanetPhysical(Microsoft.Xna.Framework.Game game, PhysicalType type)
			: base(game, type)
		{
		}

		public override void Update(GameTime gameTime)
		{
			// TODO: Add your update code here
			base.Update(gameTime);

			float seconds = ((float)(gameTime.ElapsedGameTime).Milliseconds) / 1000f;
			RotationPosition = m_rotationSpeed * seconds + m_rotationAcceleration * (float)(Math.Pow(seconds, 2) / 2);
			Orientation = Matrix.CreateFromYawPitchRoll(m_rotationPosition.Y, m_rotationPosition.X, m_rotationPosition.Z) * Orientation;

		}

	}
}

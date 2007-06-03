#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XeFramework.GameScreen;
#endregion

namespace Xe._3D.Physics
{
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class IPhysicableObject : DrawableGameComponent
	{

		private Vector3 m_linearAcceleration, m_linearSpeed, m_linearPosition, m_rotationAcceleration, m_rotationSpeed, m_rotationPosition;


		public IPhysicableObject(Microsoft.Xna.Framework.Game game)
			: base(game)
		{
			// TODO: Construct any child components here
		}


		public Vector3 linearAcceleration
		{
			get { return m_linearAcceleration; }
			set { m_linearAcceleration = value; }
		}

		public Vector3 linearSpeed
		{
			get { return m_linearSpeed; }
			set { m_linearSpeed = value; }
		}

		public Vector3 linearPosition
		{
			get { return m_linearPosition; }
			set { m_linearPosition = value; }
		}

		public Vector3 rotationAcceleration
		{
			get { return m_rotationAcceleration; }
			set { m_rotationAcceleration = value; }
		}

		public Vector3 rotationSpeed
		{
			get { return m_rotationSpeed; }
			set { m_rotationSpeed = value; }
		}

		public Vector3 rotationPosition
		{
			get { return m_rotationPosition; }
			set { m_rotationPosition = value; }
		}



		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			// TODO: Add your initialization code here

			base.Initialize();
		}


		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			// TODO: Add your update code here
			int milliseconds = (gameTime.ElapsedGameTime).Milliseconds;
			m_rotationSpeed += m_rotationAcceleration * milliseconds;

			base.Update(gameTime);
		}
	}
}



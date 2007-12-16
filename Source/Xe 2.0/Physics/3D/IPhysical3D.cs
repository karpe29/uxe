#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Xe.Tools;
#endregion

namespace Xe.Physics3D
{

	public class PhysicalType
	{
		float m_handling;
		float m_acceleration;
		float m_maxSpeed;
		float m_resistance;
		float m_gravityFactor;

		#region properties

		public float Handling { get { return m_handling; } }
		public float Acceleration { get { return m_acceleration; } }
		public float MaxSpeed { get { return m_maxSpeed; } }
		public float Resistance { get { return m_resistance; } }
		public float GravityFactor { get { return m_gravityFactor; } }

		#endregion

		public PhysicalType(float handling, float acceleration, float maxSpeed, float resistance, float gFactor)
		{
			m_handling = handling;
			m_acceleration = acceleration;
			m_maxSpeed = maxSpeed;
			m_resistance = resistance;
			m_gravityFactor = gFactor;
		}
	}



	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	/// 
	public class IPhysical3D : DrawableGameComponent
	{
		public static bool SpaceGravity = false;
		protected PhysicalType m_type;
		protected Vector3 m_linearAcceleration, m_linearSpeed, m_linearPosition, m_rotationAcceleration, m_rotationSpeed, m_rotationPosition,m_direction,m_up;
		protected Matrix m_drawOrientation,m_orientation=Matrix.Identity;
		public Microsoft.Xna.Framework.Game m_game;

		public IPhysical3D(Microsoft.Xna.Framework.Game game,PhysicalType type)
			: base(game)
		{
			m_game = game;
			m_type = type;
			// TODO: Construct any child components here
		}

		

		public Vector3 Acceleration
		{
			get { return m_linearAcceleration; }
			set { m_linearAcceleration = value; }
		}

		public Vector3 Speed
		{
			get { return m_linearSpeed; }
			set { m_linearSpeed = value; }
		}

		public Vector3 Direction
		{
			get { return m_direction; }
		}

		public Vector3 Up
		{
			get { return m_up; }
		}

		public Matrix Orientation
		{
			get { return m_orientation; }
			set
			{
				m_orientation = value;
				m_drawOrientation = value;
				m_direction = Vector3.Transform(Vector3.Forward, m_orientation);
				m_up = Vector3.Transform(Vector3.Up, m_orientation);
			}
		}

		public Matrix DrawOrientation
		{
			get { return m_drawOrientation; }
			set { m_drawOrientation=value; }

		}

		public Vector3 Position
		{
			get { return m_linearPosition; }
			set { m_linearPosition = value; }
		}

		public Vector3 RotationAcceleration
		{
			get { return m_rotationAcceleration; }
			set { m_rotationAcceleration = value; }
		}

		public Vector3 RotationSpeed
		{
			get { return m_rotationSpeed; }
			set { m_rotationSpeed = value; }
		}

		public Vector3 RotationPosition
		{
			get { return m_rotationPosition; }
			set {
				m_rotationPosition.X = value.X % MathHelper.TwoPi;
				m_rotationPosition.Y = value.Y % MathHelper.TwoPi;
				m_rotationPosition.Z = value.Z % MathHelper.TwoPi;
			}
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

			base.Update(gameTime);
		}
	}
}



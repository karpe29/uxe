#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Xe.Tools;
#endregion

namespace Xe.Physics
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

	public class MoveState
	{
		bool m_forward, m_brake, m_turnLeft, m_turnRight, m_up, m_down;

		public MoveState()
		{
			Reset();
		}

		public void Reset()
		{
			m_forward = m_brake = m_turnLeft = m_turnRight = m_up = m_down=false;
		}

		public bool Forward
		{
			get { return m_forward; }
			set
			{
				if (value && m_brake)
				{
					m_forward = m_brake = false;
				}
				else
				{
					m_forward = value;
				}
			}
		}

		public bool Brake
		{
			get { return m_brake; }
			set
			{
				if (value && m_forward)
				{
					m_forward = m_brake = false;
				}
				else
				{
					m_brake = value;
				}
			}
		}

		public bool TurnLeft
		{
			get { return m_turnLeft; }
			set
			{
				if (value && m_turnRight)
				{
					m_turnLeft = m_turnRight = false;
				}
				else
				{
					m_turnLeft = value;
				}
			}
		}

		public bool TurnRight
		{
			get { return m_turnRight; }
			set
			{
				if (value && m_turnLeft)
				{
					m_turnLeft = m_turnRight = false;
				}
				else
				{
					m_turnRight = value;
				}
			}
		}



	}


	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	/// 
	public class IPhysical3D : DrawableGameComponent
	{
		public static bool SpaceGravity = false;
		private PhysicalType m_type;
		private MoveState m_move;
		private Vector3 m_linearAcceleration, m_linearSpeed, m_linearPosition, m_rotationAcceleration, m_rotationSpeed, m_rotationPosition;

		public IPhysical3D(Microsoft.Xna.Framework.Game game,PhysicalType type)
			: base(game)
		{
			m_move = new MoveState();
			m_type = type;
			// TODO: Construct any child components here
		}

		public MoveState MoveState
		{
			get { return m_move; }
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
			// TODO: Add your update code here
			float seconds = ((float)(gameTime.ElapsedGameTime).Milliseconds)/1000f;

			if (m_move.TurnRight)
			{
				if (m_rotationSpeed.Y > -5)
				{
					m_rotationAcceleration.Y = -5;
				}
				else
				{
					m_rotationAcceleration.Y = 0;
					m_rotationSpeed.Y = -5;
				}
			}

			if (m_move.TurnLeft)
			{
				if (m_rotationSpeed.Y < 5)
				{
					m_rotationAcceleration.Y = 5;
				}
				else
				{
					m_rotationAcceleration.Y = 0;
					m_rotationSpeed.Y = 5;
				}
			}
			if (!m_move.TurnRight && !m_move.TurnLeft)
			{
				if (m_rotationSpeed.Y != 0)
				{
					if (Math.Abs(m_rotationSpeed.Y) < 0.01)
					{
						m_rotationAcceleration.Y = 0;
						m_rotationSpeed.Y = 0;
					}
					else
					{
						m_rotationAcceleration.Y = -Math.Sign(m_rotationSpeed.Y) * 10;
					}
				}
			}



			rotationSpeed += rotationAcceleration * seconds;
			rotationPosition += rotationSpeed * seconds + rotationAcceleration * (float)(Math.Pow(seconds, 2) / 2);


						if (m_move.Forward)
			{
				if (m_linearSpeed.Length() < 100)
				{
					m_linearAcceleration = Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(m_rotationPosition.X, m_rotationPosition.Y, m_rotationPosition.Z));
				}
			}

			linearSpeed += linearAcceleration * seconds;
			linearPosition += linearSpeed * seconds + linearAcceleration * (float)(Math.Pow(seconds, 2) / 2);
			
			
			
			base.Update(gameTime);
		}
	}
}



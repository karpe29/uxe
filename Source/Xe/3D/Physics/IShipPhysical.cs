using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xe.Physics3D
{

	public class MoveState
	{
		bool m_forward, m_brake, m_turnLeft, m_turnRight, m_up, m_down;

		public MoveState()
		{
			Reset();
		}

		public void Reset()
		{
			m_forward = m_brake = m_turnLeft = m_turnRight = m_up = m_down = false;
		}

		public bool Forward
		{
			get { return m_forward; }
			set
			{
				if (value && m_brake)
				{
					m_forward = false;
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
			set { m_brake = value; }
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


		public bool Up
		{
			get { return m_up; }
			set
			{
				if (value && m_down)
				{
					m_up = m_down = false;
				}
				else
				{
					m_up = value;
				}
			}
		}


		public bool Down
		{
			get { return m_down; }
			set
			{
				if (value && m_up)
				{
					m_up = m_down = false;
				}
				else
				{
					m_down = value;
				}
			}
		}



	}

	public class IShipPhysical : IPhysical3D
	{
		private MoveState m_move;
		public static float m_maxSpeed = 100000;
		public static float m_handling = 3;
		public static float m_acc = 80000;

		public IShipPhysical(Microsoft.Xna.Framework.Game game, PhysicalType type) : base(game,type)
		{
			
			m_move = new MoveState();
		}

		public MoveState MoveState
		{
			get { return m_move; }
		}


		public override void Update(GameTime gameTime)
		{
			// TODO: Add your update code here
			base.Update(gameTime);

			float seconds = ((float)(gameTime.ElapsedGameTime).Ticks) / 10000000f;

			
			if (m_move.TurnRight)
			{
				if (m_rotationSpeed.Y > -2*m_handling)
				{
					if (m_rotationSpeed.Y > 0)
					{
						m_rotationAcceleration.Y = -4 *m_handling;
					}
					else
					{
						m_rotationAcceleration.Y = -2  *m_handling;
					}
				}
				else
				{
					m_rotationAcceleration.Y = 0;
					m_rotationSpeed.Y = -2 * m_handling;
				}
			}

			if (m_move.TurnLeft)
			{
				if (m_rotationSpeed.Y < 2 * m_handling)
				{
					if (m_rotationSpeed.Y < 0)
					{
						m_rotationAcceleration.Y = 4 * m_handling;
					}
					else
					{
						m_rotationAcceleration.Y = 2 * m_handling;
					}
				}
				else
				{
					m_rotationAcceleration.Y = 0;
					m_rotationSpeed.Y = 2 * m_handling;
				}
			}
			if (!m_move.TurnRight && !m_move.TurnLeft)
			{
				if (m_rotationSpeed.Y != 0)
				{
					if (Math.Abs(m_rotationSpeed.Y) < m_handling/100)
					{
						m_rotationAcceleration.Y = 0;
						m_rotationSpeed.Y = 0;
					}
					else
					{
						m_rotationAcceleration.Y = -Math.Sign(m_rotationSpeed.Y) * 2 * m_handling;
					}
				}
			}

			if (m_move.Down)
			{
				if (m_rotationSpeed.X > -2 * m_handling)
				{
					if (m_rotationSpeed.X > 0)
					{
						m_rotationAcceleration.X = -4 * m_handling;
					}
					else
					{
						m_rotationAcceleration.X = -2 * m_handling;
					}
				}
				else
				{
					m_rotationAcceleration.X = 0;
					m_rotationSpeed.X = -2 * m_handling;
				}
			}

			if (m_move.Up)
			{
				if (m_rotationSpeed.X < 2 * m_handling)
				{
					if (m_rotationSpeed.X < 0)
					{
						m_rotationAcceleration.X = 4 * m_handling;
					}
					else
					{
						m_rotationAcceleration.X = 2 * m_handling;
					}
				}
				else
				{
					m_rotationAcceleration.X = 0;
					m_rotationSpeed.X = 2 * m_handling;
				}
			}
			if (!m_move.Up && !m_move.Down)
			{
				if (m_rotationSpeed.X != 0)
				{
					if (Math.Abs(m_rotationSpeed.X) < m_handling/100)
					{
						m_rotationAcceleration.X = 0;
						m_rotationSpeed.X = 0;
					}
					else
					{
						m_rotationAcceleration.X = -Math.Sign(m_rotationSpeed.X) * 2 * m_handling;
					}
				}
			}


			RotationSpeed += m_rotationAcceleration * seconds;
			RotationPosition = m_rotationSpeed * seconds + m_rotationAcceleration * (float)(Math.Pow(seconds, 2) / 2);
			Orientation = Matrix.CreateFromYawPitchRoll(m_rotationPosition.Y, m_rotationPosition.X, m_rotationPosition.Z) * Orientation;

			DrawOrientation = Matrix.CreateFromYawPitchRoll(0, -m_rotationSpeed.X * m_linearSpeed.Z / m_maxSpeed / m_handling / 4, -m_rotationSpeed.Y * m_linearSpeed.Z / m_maxSpeed / m_handling / 3) * Orientation;

			if (m_move.Forward)
			{
				if (m_linearSpeed.Z > -m_maxSpeed )
				{
					m_linearAcceleration.Z = -m_acc;
				}
				else
				{
					m_linearAcceleration.Z = 0;
					m_linearSpeed.Z = -m_maxSpeed;

				}

			}

			if (m_move.Brake)
			{
				if (m_linearSpeed.Z < 0)
				{
					m_linearAcceleration.Z = 2 * m_acc;
				}
				else
				{
					m_linearAcceleration.Z = 0;
					m_linearSpeed.Z = 0;
				}

			}

			if (!m_move.Forward && !m_move.Brake)
			{
				if (m_linearSpeed.Z < 0)
				{
					m_linearAcceleration.Z = m_acc / 2;
				}
				else
				{
					m_linearAcceleration.Z = 0;
					m_linearSpeed.Z = 0;
				}

			}

			Speed += m_linearAcceleration * seconds;
			Position += Vector3.Transform(m_linearSpeed * seconds + m_linearAcceleration * (float)(Math.Pow(seconds, 2) / 2), Orientation);



		}

	}
}

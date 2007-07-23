#region File Description
//-----------------------------------------------------------------------------
// ChaseCamera.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xe.Physics3D;
using Xe.Tools;
#endregion

namespace Xe.Graphics3D
{
	public class ChaseCamera
	{
		public bool FixedCamera = false;

		public float LagRatio = 250f; // lower this to get a bigger amplitude when moving camera target

		private IShipPhysical m_target;
		private Vector3 m_camPositionOffset,
			m_camPosition,
			m_camSpeed,
			m_camAcceleration,
			m_camDesiredPosition,
			m_camTargetOffset,
			m_camTarget,
			m_camDesiredTarget,
			m_camUp;

		private Stats m_stats ;


		public ChaseCamera(IShipPhysical target, Vector3 camTargetOffset, Vector3 camPositionOffset)
		{
			 m_stats = (Stats)target.m_game.Services.GetService(typeof(Stats));
			m_target = target;
			m_camTargetOffset = camTargetOffset;
			m_camPositionOffset=m_camPosition = camPositionOffset;
			this.projection = Matrix.CreatePerspectiveFieldOfView(this.FieldOfView, this.AspectRatio, this.NearPlaneDistance, this.FarPlaneDistance);
		}

		public Vector3 CamPosition
		{
			get { return m_camPosition; }
		}

		public Vector3 CamTargetPosition
		{
			get { return m_camTarget; }
		}

		#region Perspective properties

		/// <summary>
		/// Perspective aspect ratio. Default value should be overriden by application.
		/// </summary>
		public float AspectRatio
		{
			get { return aspectRatio; }
			set
			{
				aspectRatio = value;
				this.projection = Matrix.CreatePerspectiveFieldOfView(this.FieldOfView, this.AspectRatio, this.NearPlaneDistance, this.FarPlaneDistance);
			}
		}
		private float aspectRatio = 4.0f / 3.0f;

		/// <summary>
		/// Perspective field of view.
		/// </summary>
		public float FieldOfView
		{
			get { return fieldOfView; }
			set
			{
				fieldOfView = value;
				this.projection = Matrix.CreatePerspectiveFieldOfView(this.FieldOfView, this.AspectRatio, this.NearPlaneDistance, this.FarPlaneDistance);
			}
		}
		private float fieldOfView = MathHelper.ToRadians(45.0f);

		/// <summary>
		/// Distance to the near clipping plane.
		/// </summary>
		public float NearPlaneDistance
		{
			get { return nearPlaneDistance; }
			set
			{
				nearPlaneDistance = value;
				this.projection = Matrix.CreatePerspectiveFieldOfView(this.FieldOfView, this.AspectRatio, this.NearPlaneDistance, this.FarPlaneDistance);
			}
		}
		private float nearPlaneDistance = 1.0f;

		/// <summary>
		/// Distance to the far clipping plane.
		/// </summary>
		public float FarPlaneDistance
		{
			get { return farPlaneDistance; }
			set
			{
				farPlaneDistance = value;
				this.projection = Matrix.CreatePerspectiveFieldOfView(this.FieldOfView, this.AspectRatio, this.NearPlaneDistance, this.FarPlaneDistance);
			}
		}
		private float farPlaneDistance = 3000000.0f;

		#endregion

		#region Matrix properties

		/// <summary>
		/// View transform matrix.
		/// </summary>
		public Matrix View
		{
			get { return view; }
		}
		private Matrix view;

		/// <summary>
		/// Projecton transform matrix.
		/// </summary>
		public Matrix Projection
		{
			get { return projection; }
		}
		private Matrix projection;

		#endregion

		/// <summary>
		/// Animates the camera from its current position towards the desired offset
		/// behind the chased object. The camera's animation is controlled by a simple
		/// physical spring attached to the camera and anchored to the desired position.
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if (gameTime == null)
				throw new ArgumentNullException("gameTime");

			float seconds = ((float)(gameTime.ElapsedGameTime).Ticks) / 10000000f;

			m_camDesiredTarget = m_target.Position + Vector3.Transform(m_camTargetOffset, m_target.Orientation);
			m_camDesiredPosition = m_target.Position + Vector3.Transform(m_camPositionOffset, m_target.Orientation);

			if (FixedCamera)
			{
				m_camPosition = m_camDesiredPosition;
			}
			else
			{
				float decal=10*(float)Math.Log((double)(-m_target.Speed.Z+IShipPhysical.m_maxSpeed/100),4);
				m_camPosition = m_camDesiredPosition - decal * Vector3.Transform(Vector3.Forward, m_target.Orientation);
				/*if (m_target.MoveState.Forward)
				{
					m_camAcceleration.Z = -IShipPhysical.m_maxSpeed*0.95f;
				}
				else
				{
					m_camAcceleration.Z = 0;
				}

				m_camSpeed += m_camAcceleration * seconds;
				m_camPosition += Vector3.Transform(m_camSpeed * seconds + m_camAcceleration * (float)(Math.Pow(seconds, 2) / 2), m_target.Orientation);
				*/
			}
			
			/*if (m_camTarget != m_camDesiredTarget)
			{
				m_camTarget += (m_camDesiredTarget - m_camTarget) * seconds * 5;
			}*/

			m_camTarget = m_camDesiredTarget;

			//m_camDirection = Vector3.Normalize(m_camTargetPosition - m_camPosition);
			m_camUp = m_target.Up;

			this.view = Matrix.CreateLookAt(m_camPosition, m_camTarget, m_camUp);
		}
	}
}

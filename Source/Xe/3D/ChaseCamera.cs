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
#endregion

namespace Xe.Graphics3D
{
	public class ChaseCamera
	{
		public bool FixedCamera = false;
		public int LagRatio = 25; // lower this to get a bigger amplitude when moving camera target

		private IPhysical3D m_target;
		private Vector3 m_camPositionOffset,
			m_camPosition,
			m_camDesiredPosition,
			m_camTargetOffset,
			m_camTarget,
			m_camDesiredTarget,
			m_camUp;

		public ChaseCamera(IPhysical3D target, Vector3 camTargetOffset, Vector3 camPositionOffset)
		{
			m_target = target;
			m_camTargetOffset = camTargetOffset;
			m_camPositionOffset = camPositionOffset;
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
		private float farPlaneDistance = 1000000.0f;

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

			float seconds = ((float)(gameTime.ElapsedGameTime).Milliseconds) / 1000f;

			m_camDesiredTarget = m_target.Position + Vector3.Transform(m_camTargetOffset, m_target.Orientation);
			m_camDesiredPosition = m_target.Position + Vector3.Transform(m_camPositionOffset, m_target.Orientation);

			if (FixedCamera)
			{
				m_camPosition = m_camDesiredPosition;
			}
			else
				if (m_camPosition != m_camDesiredPosition)
				{
					m_camPosition += (m_camDesiredPosition - m_camPosition) * seconds * LagRatio;
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
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
		Stats m_stats;

		private IPhysical3D m_target;
		private Vector3 m_camPositionOffset,
			m_camPosition,
			m_camDesiredPosition,
			m_camTargetOffset,
			m_camTarget,
			m_camDesiredTarget,
			m_camUp;

		private Xe.Tools.XeFile logFile = new Xe.Tools.XeFile("c:\\log.txt");


		public ChaseCamera(IPhysical3D target, Vector3 camTargetOffset, Vector3 camPositionOffset)
		{
			m_stats = (Stats)target.m_game.Services.GetService(typeof(Stats));

			m_target = target;
			m_camTargetOffset = camTargetOffset;
			m_camPositionOffset=m_camPosition=m_camDesiredPosition = camPositionOffset;
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



			float seconds = ((float)(gameTime.ElapsedGameTime.Milliseconds)) / 1000f;

			m_camDesiredTarget = m_target.Position + Vector3.Transform(m_camTargetOffset, m_target.Orientation);
			m_camDesiredPosition = m_target.Position + Vector3.Transform(m_camPositionOffset, m_target.Orientation);

			if (FixedCamera)
			{
				m_camPosition = m_camDesiredPosition;
			}
			else
			{
				Vector3 diff = m_camDesiredPosition - m_camPosition;
				float step=0, maxStep = diff.Length();

				if (diff!=Vector3.Zero)
				{
					step = m_target.Speed.Length()*seconds;
					if (step < maxStep)
					{
						m_camPosition += Vector3.Normalize(diff) * step;
					}
					else
					{
						m_camPosition = m_camDesiredPosition;
					}

				}
			}
			


					//logFile.WriteLine(seconds.ToString()+"\t"+  m_camPosition.Z.ToString() + "\t" + m_camDesiredPosition.Z.ToString());
				
			m_stats.AddDebugString(Helper.Vector3ToString3f(m_camDesiredPosition));
			m_stats.AddDebugString(Helper.Vector3ToString3f(m_camPosition));
			m_stats.AddDebugString(Helper.Vector3ToString3f(m_camDesiredPosition - m_camPosition));


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

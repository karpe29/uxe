#region License
/*
 *  Xna5D.Graphics3D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 * 
 *  Special Thanks to ElectricBliss for base camera code.
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 07, 2006
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XeFramework.Graphics3D
{
    public partial class Camera : Microsoft.Xna.Framework.GameComponent
    {
        #region Members
        private Matrix m_viewMat;               // View Matrix.
        private Matrix m_projMat;               // Projection Matrix.

        private Vector3 m_pos = Vector3.Zero;   // Camera's Position.

        private float m_fieldOfView;            // Field of View
        private float m_aspectRatio;            // Aspect Ratio
        private float m_nearClip;               // Near clipping plane.
        private float m_farClip;                // Far clipping plane.

        private float m_forwardSpeed;           // Forward moving speed.
        private float m_rotSpeed;               // Rotation speed.
        private float m_zoomSpeed;              // Zoom speed.

        private float m_pitch;                  // Pitch of the camera.
        private float m_yaw;                    // Yaw of the camera.

        private Vector3 m_ref;                  // Camera's reference point.
        private Vector3 m_lookAt;

        private int m_clientHeight;             // Client's Window Bounds
        private int m_clientWidth;

        private bool m_needsUpdate = false;

        private Vector3 m_upDirection;          // Up Direction

        protected IReporterService m_reporter;  // The Reporting Service

        public event MatrixChangedHandler MatrixChanged;
        #endregion

        #region Constructor
		public Camera(Game game)
            : base(game)
        {
            this.MatrixChanged += new MatrixChangedHandler(OnMatrixChanged);
        }
        #endregion

        #region Creating the Camera
        /// <summary>
        /// Creates a 3D Camera using Vector3.Up as the Up Direction.
        /// </summary>
        /// <param name="position">The position of the camera.</param>
        /// <param name="fieldOfView">The field of view.</param>
        /// <param name="aspectRatio">The aspect ratio of the view.</param>
        /// <param name="nearClip">The near clipping plane.</param>
        /// <param name="farClip">The far clipping plane.</param>
        public void CreateCamera(Vector3 position, float fieldOfView, float aspectRatio, float nearClip, float farClip)
        {
            CreateCamera(position, fieldOfView, aspectRatio, nearClip, farClip, Vector3.Up);
        }

        /// <summary>
        /// Creates a 3D Camera.
        /// </summary>
        /// <param name="position">The position of the camera.</param>
        /// <param name="fieldOfView">The field of view.</param>
        /// <param name="aspectRatio">The aspect ratio of the view.</param>
        /// <param name="nearClip">The near clipping plane.</param>
        /// <param name="farClip">The far clipping plane.</param>
        /// <param name="upDirection">The Up direction of the camera.</param>
        public void CreateCamera(Vector3 position, float fieldOfView, float aspectRatio, float nearClip, float farClip, Vector3 upDirection)
        {
            try
            {
                // Set members up
                m_pos = position;               // Set the position.
                m_fieldOfView = fieldOfView;    // Set the field of view.
                m_aspectRatio = aspectRatio;    // Set the aspect ratio
                m_nearClip = nearClip;          // Set the near clipping plane.
                m_farClip = farClip;            // Set the far clipping plane.
                m_upDirection = upDirection;    // Set the up direction.

                m_forwardSpeed = 10f / 60f;     // Set the forward speed.
                m_rotSpeed = 0.3f / 60f;        // Set the rotation speed.
                m_zoomSpeed = 80f / 60f;        // Set the zooming speed.

                // Make sure the camera is pointing down.
                if (m_upDirection == Vector3.Up)
                {
                    m_yaw = 0;
                    m_pitch = 0;
                    m_ref = new Vector3(0, 0, -12);
                }
                else
                {
                    m_yaw = (float)Math.PI;
                    m_pitch = 1.6f;

                    m_ref = new Vector3(0, -12, 0);
                }

                // Create the view matrix.
                this.View = Matrix.CreateLookAt(position, Vector3.Zero, m_upDirection);

                // Create the projection matrix.
                this.Projection = Matrix.CreatePerspectiveFieldOfView(m_fieldOfView, m_aspectRatio, m_nearClip, m_farClip);

                // Update the camera.
                UpdateCamera();
            }
            catch(Exception e)
            {
                if (m_reporter != null)
                {
                    #if DEBUG
                    m_reporter.BroadcastError(this, e.Message + "\n" + e.StackTrace, e);
                    #else
                    m_reporter.BroadcastError(this, e.Message, e);
                    #endif
                }
            }
        }
        #endregion

        #region GameComponent Overrides
        public override void Initialize()
        {
            base.Initialize();

            // Grab the reporting service
            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));

            // Add an event handler.
            this.Game.Window.ClientSizeChanged += new EventHandler(OnClientSizeChanged);

            this.Game.Services.AddService(typeof(Camera), this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the camera.
            if(m_needsUpdate)
                UpdateCamera();
        }
        #endregion

        #region Updating
        /// <summary>
        /// Updates the Camera's View matrix.
        /// </summary>
        public void UpdateCamera()
        {
            try
            {
                Matrix _rotMatrix;

                // Create the rotation matrix for yaw
                if (m_upDirection == Vector3.Up)
                    _rotMatrix = Matrix.CreateRotationY(m_yaw);
                else
                    _rotMatrix = Matrix.CreateRotationZ(m_yaw);

                // Transform our reference point.
                Vector3 _transformedRef = Vector3.Transform(m_ref, _rotMatrix);

                // Get the point at which we are looking at.
                m_lookAt = m_pos + _transformedRef;

                // Create the view matrix.
                this.View = Matrix.CreateLookAt(m_pos, m_lookAt, m_upDirection) * Matrix.CreateRotationX(m_pitch);
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                {
                    #if DEBUG
                    m_reporter.BroadcastError(this, e.Message + "\n" + e.StackTrace, e);
                    #else
                    m_reporter.BroadcastError(this, e.Message, e);
                    #endif
                }
            }
        }

        protected void UpdateProjection()
        {
            // Update the projection matrix.
            this.Projection = Matrix.CreatePerspectiveFieldOfView(m_fieldOfView, m_aspectRatio, m_nearClip, m_farClip);
        }
        #endregion

        #region Event Handlers
        protected virtual void OnMatrixChanged(MatrixType type)
        {
        }

        protected virtual void OnClientSizeChanged(object sender, EventArgs e)
        {
            m_clientHeight = this.Game.Window.ClientBounds.Height;
            m_clientWidth = this.Game.Window.ClientBounds.Width;

            m_aspectRatio = (float)m_clientWidth / (float)m_clientHeight;

            UpdateProjection();
        }
        #endregion

        #region Properties
        #region Matrices
        /// <summary>
        /// Gets or Sets the View Matrix.
        /// </summary>
        public Matrix View
        {
            get
            {
                return m_viewMat;
            }
            set
            {
                m_viewMat = value;

                if (MatrixChanged != null)
                    MatrixChanged.Invoke(MatrixType.View);
            }
        }

        /// <summary>
        /// Gets or Sets the Projection Matrix.
        /// </summary>
        public Matrix Projection
        {
            get
            {
                return m_projMat;
            }
            set
            {
                m_projMat = value;

                if (MatrixChanged != null)
                    MatrixChanged.Invoke(MatrixType.Projection);
            }
        }

        /// <summary>
        /// Gets the View * Projection Matrix.
        /// </summary>
        public Matrix ViewProjection
        {
            get
            {
                return Matrix.Multiply(m_viewMat, m_projMat);
            }
        }
        #endregion

        #region Floats
        /// <summary>
        /// Gets or Sets the Field Of View of the camera in radians.
        /// (Pi / 4 is 45 Degrees).
        /// </summary>
        public float FieldOfView
        {
            get
            {
                return m_fieldOfView;
            }
            set
            {
                m_fieldOfView = value;
            }
        }

        /// <summary>
        /// Gets or Sets the Aspect Ratio
        /// </summary>
        public float AspectRatio
        {
            get
            {
                return m_aspectRatio;
            }
            set
            {
                m_aspectRatio = value;
            }
        }

        /// <summary>
        /// Gets or Sets the near clipping plane.
        /// </summary>
        public float NearClip
        {
            get
            {
                return m_nearClip;
            }
            set
            {
                m_nearClip = value;
            }
        }

        /// <summary>
        /// Gets or Sets the far clipping plane.
        /// </summary>
        public float FarClip
        {
            get
            {
                return m_farClip;
            }
            set
            {
                m_farClip = value;
            }
        }
        #endregion

        #region Speeds
        /// <summary>
        /// Gets or Sets the forward speed in world units.
        /// </summary>
        public float ForwardSpeed
        {
            get
            {
                return m_forwardSpeed;
            }
            set
            {
                m_forwardSpeed = value;
            }
        }

        /// <summary>
        /// Gets or Sets the rotational speed in world units.
        /// </summary>
        public float RotationSpeed
        {
            get
            {
                return m_rotSpeed;
            }
            set
            {
                m_rotSpeed = value;
            }
        }

        /// <summary>
        /// Gets or Sets the zoom speed in world units.
        /// </summary>
        public float ZoomSpeed
        {
            get
            {
                return m_zoomSpeed;
            }
            set
            {
                m_zoomSpeed = value;
            }
        }
        #endregion

        #region Pitch and Yaw
        /// <summary>
        /// Gets or Sets the pitch of the camera.
        /// </summary>
        public float Pitch
        {
            get
            {
                return m_pitch;
            }
            set
            {
                m_pitch = value;

                m_needsUpdate = true;
            }
        }

        /// <summary>
        /// Gets or Sets the yaw of the camera.
        /// </summary>
        public float Yaw
        {
            get
            {
                return m_yaw;
            }
            set
            {
                m_yaw = value;

                m_needsUpdate = true;
            }
        }
        #endregion

        #region General
        public Vector3 LookAt
        {
            get
            {
                return m_lookAt;
            }
        }

        /// <summary>
        /// Gets or Sets the direction the camera points without rotation.
        /// </summary>
        public Vector3 Reference
        {
            get
            {
                return m_ref;
            }
            set
            {
                m_ref = value;

                m_needsUpdate = true;
            }
        }

        /// <summary>
        /// Gets or Sets the Up Direction Vector3.
        /// </summary>
        public Vector3 UpDirection
        {
            get
            {
                return m_upDirection;
            }
            set
            {
                m_upDirection = value;

                m_needsUpdate = true;
            }
        }

        /// <summary>
        /// Gets or Sets the display area of the window.
        /// </summary>
        public int ClientHeight
        {
            get
            {
                return m_clientHeight;
            }
            set
            {
                m_clientHeight = value;

                AspectRatio = (float)m_clientWidth / (float)m_clientHeight;
            }
        }

        /// <summary>
        /// Gets or Sets the display area of the window.
        /// </summary>
        public int ClientWidth
        {
            get
            {
                return m_clientWidth;
            }
            set
            {
                m_clientWidth = value;

                AspectRatio = (float)m_clientWidth / (float)m_clientHeight;
            }
        }

        /// <summary>
        /// Gets or Sets the position of the camera.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;

                m_needsUpdate = true;
            }
        }
        #endregion
        #endregion
    }

    public delegate void MatrixChangedHandler(MatrixType type);

    public enum MatrixType
    {
        View,
        Projection,
        World,
        Other
    }
}



#region License
/*
 *  Xna5D.Objects3D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 * 
 *  Special Thanks to ElectricBliss for the input processing code.
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: November 30, 2006
 */
#endregion License

#region Using Statements
using System;
using System.Collections.Generic;

using Xe;
using Xe.Input;
using Xe.Graphics3D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Xe.Input
{
    public partial class CameraInput : Microsoft.Xna.Framework.GameComponent
    {
        #region Members
        private Camera m_camera;

        private int m_mX = 0, m_mY = 0;
        private int m_mScroll;

        protected IReporterService m_reporter;
        protected IEbiService m_ebi;

        private Dictionary<string, List<Keys>> m_keys = new Dictionary<string, List<Keys>>();
        #endregion

        #region Constructors and Destructor
		public CameraInput(Game game)
            : base(game)
        {
            SetKeys();
        }

		public CameraInput(Game game, Camera camera)
            : base(game)
        {
            m_camera = camera;

            SetKeys();
        }

        ~CameraInput()
        {
            m_keys.Clear();
            m_keys = null;
        }
        #endregion

        #region KeySet Setup
        protected virtual void SetKeys()
        {
            m_keys["Left"] = new List<Keys>();
            m_keys["Left"].Add(Keys.A);
            m_keys["Left"].Add(Keys.Left);

            m_keys["Right"] = new List<Keys>();
            m_keys["Right"].Add(Keys.D);
            m_keys["Right"].Add(Keys.Right);

            m_keys["Up"] = new List<Keys>();
            m_keys["Up"].Add(Keys.W);
            m_keys["Up"].Add(Keys.Up);

            m_keys["Down"] = new List<Keys>();
            m_keys["Down"].Add(Keys.S);
            m_keys["Down"].Add(Keys.Down);

            m_keys["ZoomUp"] = new List<Keys>();
            m_keys["ZoomUp"].Add(Keys.Q);
            m_keys["ZoomUp"].Add(Keys.PageUp);

            m_keys["ZoomDown"] = new List<Keys>();
            m_keys["ZoomDown"].Add(Keys.E);
            m_keys["ZoomDown"].Add(Keys.PageDown);
        }
        #endregion

        #region GameComponent Overrides
        public override void Initialize()
        {
            base.Initialize();

            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));

            m_ebi = (IEbiService)this.Game.Services.GetService(typeof(IEbiService));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            try
            {
                if ((m_ebi != null) && (m_ebi.GetFocus() != null))
                    return;

                if (m_camera.UpDirection == Vector3.Up)
                {
                    ProcessMouseForY();
                    ProcessKBForY();
                }
                else
                {
                    ProcessMouseForZ();
                    ProcessKBForZ();
                }
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, e.Message, e);
            }
        }
        #endregion

        #region Properties
        public Camera Camera
        {
            get
            {
                return m_camera;
            }
            set
            {
                m_camera = value;
            }
        }

        public Dictionary<string, List<Keys>> KeySet
        {
            get
            {
                return m_keys;
            }
            set
            {
                m_keys = value;
            }
        }
        #endregion

        #region Processing For Y
        private void ProcessMouseForY()
        {
            MouseState _state = Mouse.GetState();
            Point _locDiff = new Point((m_mX - _state.X), (_state.Y - m_mY));

            if (_state.RightButton == ButtonState.Pressed)
            {
                if (_state.X > m_mX || _state.X > 720)
                    m_camera.Yaw += _locDiff.X * m_camera.RotationSpeed;
                else if (_state.X < m_mX || m_mX < 80)
                    m_camera.Yaw += _locDiff.X * m_camera.RotationSpeed;

                if (_state.Y > m_mY || _state.Y > 520)
                    m_camera.Pitch += _locDiff.Y * m_camera.RotationSpeed;
                else if (_state.Y < m_mY || _state.Y < 80)
                    m_camera.Pitch += _locDiff.Y * m_camera.RotationSpeed;
            }

            if (_state.ScrollWheelValue > m_mScroll)
            {
                Matrix _forward = Matrix.CreateRotationY(m_camera.Yaw);
                Vector3 _v = new Vector3(0, -m_camera.ZoomSpeed, 0);

                _v = Vector3.Transform(_v, _forward);

                Vector3 _curPos = m_camera.Position;
                _curPos.Y += _v.Y;

                m_camera.Position = _curPos;
            }
            else if (_state.ScrollWheelValue < m_mScroll)
            {
                Matrix _forward = Matrix.CreateRotationY(m_camera.Yaw);
                Vector3 _v = new Vector3(0, m_camera.ZoomSpeed, 0);

                _v = Vector3.Transform(_v, _forward);

                Vector3 _curPos = m_camera.Position;
                _curPos.Y += _v.Y;

                m_camera.Position = _curPos;
            }

            m_mX = _state.X;
            m_mY = _state.Y;
            m_mScroll = _state.ScrollWheelValue;
        }

        private void ProcessKBForY()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Matrix forwardMovement = Matrix.CreateRotationY(m_camera.Yaw);

            foreach (Keys k in keyboardState.GetPressedKeys())
            {
                if (m_keys["Left"].Contains(k))
                {
                    Vector3 v = new Vector3(-m_camera.ForwardSpeed, 0, 0);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Z += v.Z;
                    currentPosition.X += v.X;
                    m_camera.Position = currentPosition;
                }
                if (m_keys["Right"].Contains(k))
                {
                    Vector3 v = new Vector3(m_camera.ForwardSpeed, 0, 0);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Z += v.Z;
                    currentPosition.X += v.X;
                    m_camera.Position = currentPosition;
                }

                if (m_keys["Up"].Contains(k))
                {
                    Vector3 v = new Vector3(0, 0, -m_camera.ForwardSpeed);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Z += v.Z;
                    currentPosition.X += v.X;
                    m_camera.Position = currentPosition;
                }
                if (m_keys["Down"].Contains(k))
                {
                    Vector3 v = new Vector3(0, 0, m_camera.ForwardSpeed);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Z += v.Z;
                    currentPosition.X += v.X;
                    m_camera.Position = currentPosition;
                }

                if (m_keys["ZoomDown"].Contains(k))
                {
                    Vector3 v = new Vector3(0, -m_camera.ForwardSpeed, 0);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Y += v.Y;
                    m_camera.Position = currentPosition;
                }
                if (m_keys["ZoomUp"].Contains(k))
                {
                    Vector3 v = new Vector3(0, m_camera.ForwardSpeed, 0);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Y += v.Y;
                    m_camera.Position = currentPosition;
                }
            }
        }
        #endregion

        #region Processing For Z
        private void ProcessMouseForZ()
        {
            MouseState _state = Mouse.GetState();
            Point _locDiff = new Point((m_mX - _state.X), (_state.Y - m_mY));

            if (_state.RightButton == ButtonState.Pressed)
            {
                if (_state.X > m_mX || _state.X > 720)
                    m_camera.Yaw += _locDiff.X * m_camera.RotationSpeed;
                else if (_state.X < m_mX || m_mX < 80)
                    m_camera.Yaw += _locDiff.X * m_camera.RotationSpeed;

                if (_state.Y > m_mY || _state.Y > 520)
                    m_camera.Pitch += _locDiff.Y * m_camera.RotationSpeed;
                else if (_state.Y < m_mY || _state.Y < 80)
                    m_camera.Pitch += _locDiff.Y * m_camera.RotationSpeed;
            }

            if (_state.ScrollWheelValue > m_mScroll)
            {
                Matrix _forward = Matrix.CreateRotationY(m_camera.Yaw);
                Vector3 _v = new Vector3(0, 0, -m_camera.ZoomSpeed);

                _v = Vector3.Transform(_v, _forward);

                Vector3 _curPos = m_camera.Position;
                _curPos.Z += _v.Z;

                m_camera.Position = _curPos;
            }
            else if (_state.ScrollWheelValue < m_mScroll)
            {
                Matrix _forward = Matrix.CreateRotationY(m_camera.Yaw);
                Vector3 _v = new Vector3(0, 0, m_camera.ZoomSpeed);

                _v = Vector3.Transform(_v, _forward);

                Vector3 _curPos = m_camera.Position;
                _curPos.Z += _v.Z;

                m_camera.Position = _curPos;
            }

            m_mX = _state.X;
            m_mY = _state.Y;
            m_mScroll = _state.ScrollWheelValue;
        }

        private void ProcessKBForZ()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Matrix forwardMovement = Matrix.CreateRotationZ(m_camera.Yaw);

            foreach (Keys k in keyboardState.GetPressedKeys())
            {
                if (m_keys["Left"].Contains(k))
                {
                    Vector3 v = new Vector3(m_camera.ForwardSpeed, 0, 0);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Y += v.Y;
                    currentPosition.X += v.X;
                    m_camera.Position = currentPosition;
                }
                if (m_keys["Right"].Contains(k))
                {
                    Vector3 v = new Vector3(-m_camera.ForwardSpeed, 0, 0);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Y += v.Y;
                    currentPosition.X += v.X;
                    m_camera.Position = currentPosition;
                }

                if (m_keys["Up"].Contains(k))
                {
                    Vector3 v = new Vector3(0, -m_camera.ForwardSpeed, 0);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.X += v.X;
                    currentPosition.Y += v.Y;
                    currentPosition.Z += v.Z;
                    m_camera.Position = currentPosition;
                }
                if (m_keys["Down"].Contains(k))
                {
                    Vector3 v = new Vector3(0, m_camera.ForwardSpeed, 0);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.X += v.X;
                    currentPosition.Y += v.Y;
                    currentPosition.Z += v.Z;
                    m_camera.Position = currentPosition;
                }

                if (m_keys["ZoomDown"].Contains(k))
                {
                    Vector3 v = new Vector3(0, 0, -m_camera.ForwardSpeed);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Z += v.Z;
                    m_camera.Position = currentPosition;
                }
                if (m_keys["ZoomUp"].Contains(k))
                {
                    Vector3 v = new Vector3(0, 0, m_camera.ForwardSpeed);
                    v = Vector3.Transform(v, forwardMovement);

                    Vector3 currentPosition = m_camera.Position;
                    currentPosition.Z += v.Z;
                    m_camera.Position = currentPosition;
                }
            }
        }
        #endregion
    }
}



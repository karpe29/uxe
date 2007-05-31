#region License
/*
 *  Xna5D.Input.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
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
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

using XeFramework.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace XeFramework.Input
{
    #region Delegates
    public delegate void KeyDownHandler(object focus, KeyEventArgs k);
    public delegate void KeyUpHandler(object focus, KeyEventArgs k);

    public delegate void MouseDownHandler(MouseEventArgs args);
    public delegate void MouseUpHandler(MouseEventArgs args);
    #endregion
		
    public partial class Ebi : Microsoft.Xna.Framework.GameComponent, IEbiService
    {
#if !XBOX360
        #region Class: InputKey
        protected class InputKey
        {
            public Keys Key;
            public bool Pressed;
            public int Countdown;

            public InputKey()
            {
            }
        }
        #endregion

        #region Members
        protected List<InputKey> m_keys = new List<InputKey>();

        protected object m_focus;

        protected int m_maxInterval = 250;

        protected MouseState m_curState = new MouseState();

        #region Events
        public event KeyDownHandler KeyDown;
        public event KeyUpHandler KeyUp;

        public event MouseDownHandler MouseDown;
        public event MouseUpHandler MouseUp;

        public event MouseDownHandler RequestingFocus;
        #endregion
        #endregion

        #region Constructors & Initialization
        public Ebi(Game game)
            : base(game)
        {
            KeyDown += new KeyDownHandler(OnKeyDown);
            KeyUp += new KeyUpHandler(OnKeyUp);

            MouseUp += new MouseUpHandler(OnMouseUp);
            MouseDown += new MouseDownHandler(OnMouseDown);

            if (game != null)
                game.Services.AddService(typeof(IEbiService), this);
        }

        public override void Initialize()
        {
            foreach (string k in Enum.GetNames(typeof(Keys)))
            {
                bool found = false;
                foreach (InputKey tK in m_keys)
                {
                    if (tK.Key == (Keys)Enum.Parse(typeof(Keys), k))
                        found = true;
                }

                if (!found)
                {
                    InputKey _key = new InputKey();
                    _key.Key = (Keys)Enum.Parse(typeof(Keys), k);
                    _key.Pressed = false;
                    _key.Countdown = m_maxInterval;

                    m_keys.Add(_key);
                }
            }

            base.Initialize();
        }
        #endregion

        #region Event Handlers
        protected virtual void OnMouseDown(MouseEventArgs args)
        {
        }

        protected virtual void OnMouseUp(MouseEventArgs args)
        {
        }

        protected virtual void OnKeyUp(object focus, KeyEventArgs k)
        {
        }

        protected virtual void OnKeyDown(object focus, KeyEventArgs k)
        {
        }
        #endregion

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (!this.Enabled)
                return;

            #region Keyboard
            KeyEventArgs args = new KeyEventArgs();
            foreach (Keys key in Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys())
            {
                if (key == Keys.LeftAlt || key == Keys.RightAlt)
                    args.Alt = true;
                else if (key == Keys.LeftShift || key == Keys.RightShift)
                    args.Shift = true;
                else if (key == Keys.LeftControl || key == Keys.RightControl)
                    args.Control = true;
            }

            foreach (InputKey key in m_keys)
            {
                if (key.Key == Keys.LeftAlt || key.Key == Keys.RightAlt ||
                    key.Key == Keys.LeftShift || key.Key == Keys.RightShift ||
                    key.Key == Keys.LeftControl || key.Key == Keys.RightControl)
                    continue;

                bool pressed = IsKeyPressed(key.Key);

                if (pressed)
                    key.Countdown -= gameTime.ElapsedRealTime.Milliseconds;

                if ((pressed) && (!key.Pressed))
                {
                    key.Pressed = true;
                    args.Key = key.Key;
                    if (KeyDown != null)
                        KeyDown.Invoke(m_focus, args);
                }
                else if ((!pressed) && (key.Pressed))
                {
                    key.Pressed = false;
                    key.Countdown = m_maxInterval;
                    args.Key = key.Key;
                    if (KeyUp != null)
                        KeyUp.Invoke(m_focus, args);
                }

                if (key.Countdown < 0)
                {
                    key.Pressed = false;
                    key.Countdown = m_maxInterval;
                }
            }
            #endregion

            #region Mouse
            MouseState mstate = Microsoft.Xna.Framework.Input.Mouse.GetState();
            #region Left Button
            if (mstate.LeftButton != m_curState.LeftButton)
            {
                if (mstate.LeftButton == ButtonState.Released)
                {
                    m_curState = mstate;

                    if (MouseUp != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.Left;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        MouseUp.Invoke(e);
                    }
                }
                else
                {
                    m_curState = mstate;

                    if (MouseDown != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.Left;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        if (RequestingFocus != null)
                            RequestingFocus.Invoke(e);

						MouseDown.Invoke(e);
                    }
                }
            }
            #endregion

            #region Right Button
            if (mstate.RightButton != m_curState.RightButton)
            {
                if (mstate.RightButton == ButtonState.Released)
                {
                    m_curState = mstate;

                    if (MouseUp != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.Right;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        MouseUp.Invoke(e);
                    }
                }
                else
                {
                    m_curState = mstate;

                    if (MouseDown != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.Right;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        MouseDown.Invoke(e);
                    }
                }
            }
            #endregion

            #region Middle Button
            if (mstate.MiddleButton != m_curState.MiddleButton)
            {
                if (mstate.MiddleButton == ButtonState.Released)
                {
                    m_curState = mstate;

                    if (MouseUp != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.Middle;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        MouseUp.Invoke(e);
                    }
                }
                else
                {
                    m_curState = mstate;

                    if (MouseDown != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.Middle;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        MouseDown.Invoke(e);
                    }
                }
            }
            #endregion

            #region XButton1
            if (mstate.XButton1 != m_curState.XButton1)
            {
                if (mstate.XButton1 == ButtonState.Released)
                {
                    m_curState = mstate;

                    if (MouseUp != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.XButton1;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        MouseUp.Invoke(e);
                    }
                }
                else
                {
                    m_curState = mstate;

                    if (MouseDown != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.XButton1;
                        e.State = m_curState;

                        MouseDown.Invoke(e);
                    }
                }
            }
            #endregion

            #region XButton2
            if (mstate.XButton2 != m_curState.XButton2)
            {
                if (mstate.XButton2 == ButtonState.Released)
                {
                    m_curState = mstate;

                    if (MouseUp != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.XButton2;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        MouseUp.Invoke(e);
                    }
                }
                else
                {
                    m_curState = mstate;

                    if (MouseDown != null)
                    {
                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButtons.XButton2;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        MouseDown.Invoke(e);
                    }
                }
            }
            #endregion
            #endregion

            base.Update(gameTime);
        }

        protected bool IsKeyPressed(Keys key)
        {
            foreach (Keys kp in Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys())
            {
                if (kp.Equals(key))
                    return true;
            }

            return false;
        }

        #region IEbiService Members
        public void SetFocus(object obj)
        {
            m_focus = obj;
        }

        public object GetFocus()
        {
			if (m_focus != null)
				return m_focus;
			else
				return null;
        }

        public int KeyStrokeInterval
        {
            get
            {
                return m_maxInterval;
            }
            set
            {
                m_maxInterval = value;
            }
        }
        #endregion
#endif
    }

    #region Event Argument Classes
    public class KeyEventArgs
    {
        public Keys Key;
        public bool Control = false;
        public bool Shift = false;
        public bool Alt = false;

        public KeyEventArgs()
        {
        }
    }
#if !XBOX360
    public class MouseEventArgs
    {
        public MouseState State;
        public MouseButtons Button;
        public Point Position;

        public MouseEventArgs()
        {
        }
    }

    public enum MouseButtons
    {
        Left,
        Right,
        Middle,
        XButton1,
        XButton2
    }
#endif
#endregion
    #region Service Interface
    public interface IEbiService
    {
        event KeyDownHandler KeyDown;
        event KeyUpHandler KeyUp;

        event MouseDownHandler MouseDown;
        event MouseUpHandler MouseUp;

        event MouseDownHandler RequestingFocus;

        void SetFocus(object obj);
        object GetFocus();

        int KeyStrokeInterval { get; set; }
    }
    #endregion
}



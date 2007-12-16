#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Xe.Input
{
    public class MKController : ControllerBase
    {
        #region Input State Class
        protected class InputState
        {
            public bool Pressed;
            public float TimePressed;
        }
        #endregion

        #region Members
        protected Dictionary<Keys, InputState> m_keys = new Dictionary<Keys, InputState>();
        protected MouseState m_curState = new MouseState();
        #endregion

        #region Constructors
        public MKController()
            : base()
        {
        }
        #endregion

        #region Updating
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Return if the EbiService is null.
            if (this.EbiService == null)
                return;

            UpdateKeyboard(gameTime);
            UpdateMouse(gameTime);
        }

        protected virtual void UpdateKeyboard(GameTime gameTime)
        {
            KeyEventArgs args = new KeyEventArgs();
            KeyboardState kbstate = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            Keys[] pressedKeys = kbstate.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (key == Keys.LeftAlt || key == Keys.RightAlt)
                    args.Alt = true;
                else if (key == Keys.LeftShift || key == Keys.RightShift)
                    args.Shift = true;
                else if (key == Keys.LeftControl || key == Keys.RightControl)
                    args.Ctrl = true;
                else
                {
                    if (!m_keys.ContainsKey(key))
                        m_keys[key] = new InputState();
                }
            }

            foreach (Keys key in m_keys.Keys)
            {
                if (key == Keys.LeftAlt || key == Keys.RightAlt ||
                    key == Keys.LeftShift || key == Keys.RightShift ||
                    key == Keys.LeftControl || key == Keys.RightControl)
                    continue;

                bool pressed = kbstate.IsKeyDown(key);

                InputState _state = m_keys[key];

                if (pressed)
                    _state.TimePressed += gameTime.ElapsedGameTime.Milliseconds;

                if ((pressed) && (!_state.Pressed))
                {
                    _state.Pressed = true;
                    args.Key = key;

                    this.EbiService.SimulateKey(args, true);

                    if (this.EbiService.TabEnabled)
                    {
                        for (int i = 0; i < this.EbiService.TabKeys.Count; i++)
                        {
                            if (key == this.EbiService.TabKeys[i])
                            {
                                if (args.Shift)
                                    this.EbiService.TabToPrev(new CancelEventArgs());
                                else
                                    this.EbiService.TabToNext(new CancelEventArgs());
                            }
                            
                        }
                    }
                }
                else if ((!pressed) && (_state.Pressed))
                {
                    _state.Pressed = false;
                    _state.TimePressed = 0;
                    args.Key = key;

                    this.EbiService.SimulateKey(args, false);
                }

                if (_state.TimePressed >= this.EbiService.RepeatInterval)
                {
                    _state.Pressed = false;
                    _state.TimePressed = 0;
                }
            }
        }

        protected virtual void UpdateMouse(GameTime gameTime)
        {
            #region Mouse
            MouseState mstate = Microsoft.Xna.Framework.Input.Mouse.GetState();
            #region Left Button
            if (mstate.LeftButton != m_curState.LeftButton)
            {
                if (mstate.LeftButton == ButtonState.Released)
                {
                    m_curState = mstate;

                    MouseEventArgs e = new MouseEventArgs();
                    e.Button = MouseButton.Left;
                    e.State = m_curState;
                    e.Position = new Point(mstate.X, mstate.Y);

                    this.EbiService.SimulateMouse(e, false);
                }
                else
                {
                    // TODO: This was RightButton, correct or incorrect?
                    if (m_curState.LeftButton == ButtonState.Released)
                    {
                        m_curState = mstate;

                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButton.Left;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        this.EbiService.RequestFocus(e);

                        this.EbiService.SimulateMouse(e, true);
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

                    MouseEventArgs e = new MouseEventArgs();
                    e.Button = MouseButton.Right;
                    e.State = m_curState;
                    e.Position = new Point(mstate.X, mstate.Y);

                    this.EbiService.SimulateMouse(e, false);
                }
                else
                {
                    // TODO: This was LeftButton, correct or incorrect?
                    if (m_curState.RightButton == ButtonState.Released)
                    {

                        m_curState = mstate;

                        MouseEventArgs e = new MouseEventArgs();
                        e.Button = MouseButton.Right;
                        e.State = m_curState;
                        e.Position = new Point(mstate.X, mstate.Y);

                        this.EbiService.RequestFocus(e);

                        this.EbiService.SimulateMouse(e, true);
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

                    MouseEventArgs e = new MouseEventArgs();
                    e.Button = MouseButton.Middle;
                    e.State = m_curState;
                    e.Position = new Point(mstate.X, mstate.Y);

                    this.EbiService.SimulateMouse(e, false);
                }
                else
                {
                    m_curState = mstate;

                    MouseEventArgs e = new MouseEventArgs();
                    e.Button = MouseButton.Middle;
                    e.State = m_curState;
                    e.Position = new Point(mstate.X, mstate.Y);

                    this.EbiService.SimulateMouse(e, true);
                }
            }
            #endregion

            #region XButton1
            if (mstate.XButton1 != m_curState.XButton1)
            {
                if (mstate.XButton1 == ButtonState.Released)
                {
                    m_curState = mstate;

                    MouseEventArgs e = new MouseEventArgs();
                    e.Button = MouseButton.XButton1;
                    e.State = m_curState;
                    e.Position = new Point(mstate.X, mstate.Y);

                    this.EbiService.SimulateMouse(e, false);
                }
                else
                {
                    m_curState = mstate;

                    MouseEventArgs e = new MouseEventArgs();
                    e.Button = MouseButton.XButton1;
                    e.State = m_curState;

                    this.EbiService.SimulateMouse(e, true);
                }
            }
            #endregion

            #region XButton2
            if (mstate.XButton2 != m_curState.XButton2)
            {
                if (mstate.XButton2 == ButtonState.Released)
                {
                    m_curState = mstate;

                    MouseEventArgs e = new MouseEventArgs();
                    e.Button = MouseButton.XButton2;
                    e.State = m_curState;
                    e.Position = new Point(mstate.X, mstate.Y);

                    this.EbiService.SimulateMouse(e, false);
                }
                else
                {
                    m_curState = mstate;

                    MouseEventArgs e = new MouseEventArgs();
                    e.Button = MouseButton.XButton2;
                    e.State = m_curState;
                    e.Position = new Point(mstate.X, mstate.Y);

                    this.EbiService.SimulateMouse(e, true);
                }
            }
            #endregion
            #endregion
        }
        #endregion
    }
}
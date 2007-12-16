#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Xe.Input
{
    public class GamePadController : ControllerBase
    {
        #region Members
        // Defines the states for each player.
        protected Dictionary<PlayerIndex, GamePadState> m_gamePads = new Dictionary<PlayerIndex, GamePadState>();

        // Whether or not each player is connected.
        private bool m_p1Connected = false;
        private bool m_p2Connected = false;
        private bool m_p3Connected = false;
        private bool m_p4Connected = false;
        #endregion

        #region Constructors
        public GamePadController()
            : base()
        {
            // Initialize each Player's GamePad State
            m_gamePads[PlayerIndex.One] = new GamePadState();
            m_gamePads[PlayerIndex.Two] = new GamePadState();
            m_gamePads[PlayerIndex.Three] = new GamePadState();
            m_gamePads[PlayerIndex.Four] = new GamePadState();

            // Initialize each Player's Connection State
            m_p1Connected = GamePad.GetState(PlayerIndex.One).IsConnected;
            m_p2Connected = GamePad.GetState(PlayerIndex.Two).IsConnected;
            m_p3Connected = GamePad.GetState(PlayerIndex.Three).IsConnected;
            m_p4Connected = GamePad.GetState(PlayerIndex.Four).IsConnected;
        }
        #endregion

        protected override void OnEbiServiceSet()
        {
            base.OnEbiServiceSet();

            this.EbiService.ButtonDown += new ButtonDownHandler(OnEbiServiceButtonDown);
        }

        void OnEbiServiceButtonDown(object sender, ButtonEventArgs e)
        {
            if (this.EbiService.TabEnabled)
            {
                for (int i = 0; i < this.EbiService.TabNextButtons.Count; i++)
                {
                    if (this.EbiService.TabNextButtons[i] == e.Button)
                        this.EbiService.TabToNext(new CancelEventArgs());
                }

                for (int i = 0; i < this.EbiService.TabPrevButtons.Count; i++)
                {
                    if (this.EbiService.TabPrevButtons[i] == e.Button)
                        this.EbiService.TabToPrev(new CancelEventArgs());
                }
            }
        }

        #region Updating
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Return if the EbiService is null.
            if (this.EbiService == null)
                return;

            CheckConnections();

            // If we can, update for player one.
            if ((this.EbiService.AllowedPlayers & Player.One) == Player.One)
                UpdateGamePad(PlayerIndex.One, gameTime);

            // If we can, update for player two.
            if ((this.EbiService.AllowedPlayers & Player.Two) == Player.Two)
                UpdateGamePad(PlayerIndex.Two, gameTime);

            // If we can, update for player three.
            if ((this.EbiService.AllowedPlayers & Player.Three) == Player.Three)
                UpdateGamePad(PlayerIndex.Three, gameTime);

            // If we can, update for player four.
            if ((this.EbiService.AllowedPlayers & Player.Four) == Player.Four)
                UpdateGamePad(PlayerIndex.Four, gameTime);
        }

        #region Check Player Connects / Disconnects
        protected virtual void CheckConnections()
        {
            bool _p1Check = GamePad.GetState(PlayerIndex.One).IsConnected;
            if (_p1Check != m_p1Connected)
            {
                m_p1Connected = _p1Check;
                //Console.WriteLine("Player One: " + m_p1Connected.ToString());
                if (m_p1Connected)
                {
                    this.EbiService.ConnectPlayer(PlayerIndex.One);
                }
                else
                {
                    this.EbiService.DisconnectPlayer(PlayerIndex.One);
                }
            }

            bool _p2Check = GamePad.GetState(PlayerIndex.Two).IsConnected;
            if (_p2Check != m_p2Connected)
            {
                m_p2Connected = _p2Check;
                //Console.WriteLine("Player Two: " + m_p2Connected.ToString());
                if (m_p2Connected)
                {
                    this.EbiService.ConnectPlayer(PlayerIndex.Two);
                }
                else
                {
                    this.EbiService.DisconnectPlayer(PlayerIndex.Two);
                }
            }

            bool _p3Check = GamePad.GetState(PlayerIndex.Three).IsConnected;
            if (_p3Check != m_p3Connected)
            {
                m_p3Connected = _p3Check;
                if (m_p3Connected)
                {
                    this.EbiService.ConnectPlayer(PlayerIndex.Three);
                }
                else
                {
                    this.EbiService.DisconnectPlayer(PlayerIndex.Three);
                }
            }

            bool _p4Check = GamePad.GetState(PlayerIndex.Four).IsConnected;
            if (_p4Check != m_p4Connected)
            {
                m_p4Connected = _p4Check;
                if (m_p4Connected)
                {
                    this.EbiService.ConnectPlayer(PlayerIndex.Four);
                }
                else
                {
                    this.EbiService.DisconnectPlayer(PlayerIndex.Four);
                }
            }
        }
        #endregion

        protected virtual void UpdateGamePad(PlayerIndex index, GameTime gameTime)
        {
            GamePadState _state = GamePad.GetState(index);
            GamePadState _curState = m_gamePads[index];

            #region A Button
            if (_state.Buttons.A != _curState.Buttons.A)
            {
                if (_state.Buttons.A == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.A;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.A;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region B Button
            if (_state.Buttons.B != _curState.Buttons.B)
            {
                if (_state.Buttons.B == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.B;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.B;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region X Button
            if (_state.Buttons.X != _curState.Buttons.X)
            {
                if (_state.Buttons.X == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.X;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.X;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Y Button
            if (_state.Buttons.Y != _curState.Buttons.Y)
            {
                if (_state.Buttons.Y == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.Y;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.Y;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Back Button
            if (_state.Buttons.Back != _curState.Buttons.Back)
            {
                if (_state.Buttons.Back == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.Back;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.Back;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Left Shoulder
            if (_state.Buttons.LeftShoulder != _curState.Buttons.LeftShoulder)
            {
                if (_state.Buttons.LeftShoulder == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftShoulder;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftShoulder;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Right Shoulder
            if (_state.Buttons.RightShoulder != _curState.Buttons.RightShoulder)
            {
                if (_state.Buttons.RightShoulder == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightShoulder;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightShoulder;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Left Stick
            if (_state.Buttons.LeftStick != _curState.Buttons.LeftStick)
            {
                if (_state.Buttons.LeftStick == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStick;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStick;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Right Stick
            if (_state.Buttons.RightStick != _curState.Buttons.RightStick)
            {
                if (_state.Buttons.RightStick == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStick;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStick;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Start Button
            if (_state.Buttons.Start != _curState.Buttons.Start)
            {
                if (_state.Buttons.Start == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.Start;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.Start;
                    e.PlayerIndex = index;
                    e.State = _state;

                    //Console.WriteLine("Start button of Player " + index.ToString() + " was pressed down.");

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Left Trigger
            if ((_state.Triggers.Left > 0.5f && _curState.Triggers.Left <= 0.5f) ||
                (_state.Triggers.Left <= 0.5f && _curState.Triggers.Left > 0.5f))
            {
                if (_state.Triggers.Left <= 0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftTrigger;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftTrigger;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Right Trigger
            if ((_state.Triggers.Right > 0.5f && _curState.Triggers.Right <= 0.5f) ||
                (_state.Triggers.Right <= 0.5f && _curState.Triggers.Right > 0.5f))
            {
                if (_state.Triggers.Right <= 0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightTrigger;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightTrigger;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region DPad
            #region Down
            if (_state.DPad.Down != _curState.DPad.Down)
            {
                if (_state.DPad.Down == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.DPadDown;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.DPadDown;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Up
            if (_state.DPad.Up != _curState.DPad.Up)
            {
                if (_state.DPad.Up == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.DPadUp;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.DPadUp;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Left
            if (_state.DPad.Left != _curState.DPad.Left)
            {
                if (_state.DPad.Left == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.DPadLeft;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.DPadLeft;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Right
            if (_state.DPad.Right != _curState.DPad.Right)
            {
                if (_state.DPad.Right == ButtonState.Released)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.DPadRight;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.DPadRight;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion
            #endregion

            #region Left Thumbstick
            #region Down
            if ((_state.ThumbSticks.Left.Y < -0.5f && _curState.ThumbSticks.Left.Y >= -0.5f) ||
                (_state.ThumbSticks.Left.Y >= -0.5f && _curState.ThumbSticks.Left.Y < -0.5f))
            {
                if (_state.ThumbSticks.Left.Y >= -0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStickDown;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStickDown;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Up
            if ((_state.ThumbSticks.Left.Y > 0.5f && _curState.ThumbSticks.Left.Y <= 0.5f) ||
                (_state.ThumbSticks.Left.Y <= 0.5f && _curState.ThumbSticks.Left.Y > 0.5f))
            {
                if (_state.ThumbSticks.Left.Y <= 0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStickUp;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStickUp;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Right
            if ((_state.ThumbSticks.Left.X > 0.5f && _curState.ThumbSticks.Left.X <= 0.5f) ||
                (_state.ThumbSticks.Left.X <= 0.5f && _curState.ThumbSticks.Left.X > 0.5f))
            {
                if (_state.ThumbSticks.Left.X <= 0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStickRight;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStickRight;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Left
            if ((_state.ThumbSticks.Left.X < -0.5f && _curState.ThumbSticks.Left.X >= -0.5f) ||
                (_state.ThumbSticks.Left.X >= -0.5f && _curState.ThumbSticks.Left.X < -0.5f))
            {
                if (_state.ThumbSticks.Left.X >= -0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStickLeft;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.LeftStickLeft;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion
            #endregion

            #region Right Thumbstick
            #region Down
            if ((_state.ThumbSticks.Right.Y < -0.5f && _curState.ThumbSticks.Right.Y >= -0.5f) ||
                (_state.ThumbSticks.Right.Y >= -0.5f && _curState.ThumbSticks.Right.Y < -0.5f))
            {
                if (_state.ThumbSticks.Right.Y >= -0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStickDown;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStickDown;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Up
            if ((_state.ThumbSticks.Right.Y > 0.5f && _curState.ThumbSticks.Right.Y <= 0.5f) ||
                (_state.ThumbSticks.Right.Y <= 0.5f && _curState.ThumbSticks.Right.Y > 0.5f))
            {
                if (_state.ThumbSticks.Right.Y <= 0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStickUp;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStickUp;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Right
            if ((_state.ThumbSticks.Right.X > 0.5f && _curState.ThumbSticks.Right.X <= 0.5f) ||
                (_state.ThumbSticks.Right.X <= 0.5f && _curState.ThumbSticks.Right.X > 0.5f))
            {
                if (_state.ThumbSticks.Right.X <= 0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStickRight;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStickRight;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion

            #region Left
            if ((_state.ThumbSticks.Right.X < -0.5f && _curState.ThumbSticks.Right.X >= -0.5f) ||
                (_state.ThumbSticks.Right.X >= -0.5f && _curState.ThumbSticks.Right.X < -0.5f))
            {
                if (_state.ThumbSticks.Right.X >= -0.5f)
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStickLeft;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, false);
                }
                else
                {
                    ButtonEventArgs e = new ButtonEventArgs();
                    e.Button = GamePadButton.RightStickLeft;
                    e.PlayerIndex = index;
                    e.State = _state;

                    this.EbiService.SimulateButton(e, true);
                }
            }
            #endregion
            #endregion

            // Reset the current state
            m_gamePads[index] = _state;
        }
        #endregion
    }
}
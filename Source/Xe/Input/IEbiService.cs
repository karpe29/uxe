#region Using Statements
using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Input
{
    public interface IEbiService
    {
        /// <summary>
        /// Gets or Sets the interval at which a new KeyDown event
        /// will be fired if a key is held down.
        /// </summary>
        float RepeatInterval { get; set; }

        /// <summary>
        /// Gets or Sets the focus.
        /// </summary>
        IFocusable Focus { get; set; }

        /// <summary>
        /// Gets or Sets whether tabbing is enabled.
        /// </summary>
        bool TabEnabled { get; set; }

        /// <summary>
        /// Gets or Sets what Players are allowed to control
        /// input with their GamePad.
        /// </summary>
        Player AllowedPlayers { get; set; }

        List<Keys> TabKeys { get; set; }
        List<GamePadButton> TabNextButtons { get; set; }
        List<GamePadButton> TabPrevButtons { get; set; }

        /// <summary>
        /// KeyDown Event
        /// </summary>
        event KeyDownHandler KeyDown;

        /// <summary>
        /// KeyUp Event
        /// </summary>
        event KeyUpHandler KeyUp;

        /// <summary>
        /// Requesting Focus Event
        /// </summary>
        event MouseDownHandler RequestingFocus;

        /// <summary>
        /// MouseDown Event
        /// </summary>
        event MouseDownHandler MouseDown;

        /// <summary>
        /// MouseUp Event
        /// </summary>
        event MouseUpHandler MouseUp;

        /// <summary>
        /// ButtonDown Event
        /// </summary>
        event ButtonDownHandler ButtonDown;

        /// <summary>
        /// ButtonUp Event
        /// </summary>
        event ButtonUpHandler ButtonUp;

        /// <summary>
        /// Tab Next Event
        /// </summary>
        event TabNextHandler TabNext;

        /// <summary>
        /// Tab Previous Event
        /// </summary>
        event TabPrevHandler TabPrev;

        event PlayerConnectedHandler PlayerConnected;
        event PlayerDisconnectedHandler PlayerDisconnected;

        event SelectHandler SelectPressed;
        event SelectHandler SelectReleased;
        event BackHandler BackPressed;
        event BackHandler BackReleased;

        void Select();
        void Back();

        void TabToNext(CancelEventArgs args);
        void TabToPrev(CancelEventArgs args);

        /// <summary>
        /// Simulates a Key being pressed.
        /// </summary>
        /// <param name="key">The Key to simulate being pressed.</param>
        void SimulateKey(KeyEventArgs args, bool down);

        /// <summary>
        /// Simulates a Player pressing a Button on the GamePad.
        /// </summary>
        /// <param name="index">The Player Index</param>
        /// <param name="button">The Button to simulate being pressed.</param>
        void SimulateButton(ButtonEventArgs args, bool down);

        /// <summary>
        /// Simulates a Mouse Button being pressed.
        /// </summary>
        /// <param name="button">The mouse button to simulate.</param>
        void SimulateMouse(MouseEventArgs args, bool down);

        void ConnectPlayer(PlayerIndex index);
        void DisconnectPlayer(PlayerIndex index);

        void RequestFocus(MouseEventArgs args);
    }

    public delegate void TabNextHandler(CancelEventArgs e);
    public delegate void TabPrevHandler(CancelEventArgs e);
    public delegate void KeyDownHandler(object sender, KeyEventArgs e);
    public delegate void KeyUpHandler(object sender, KeyEventArgs e);
    public delegate void MouseDownHandler(MouseEventArgs e);
    public delegate void MouseUpHandler(MouseEventArgs e);
    public delegate void ClickHandler(object sender, MouseEventArgs e);
    public delegate void ButtonDownHandler(object sender, ButtonEventArgs e);
    public delegate void ButtonUpHandler(object sender, ButtonEventArgs e);

    public delegate void SelectHandler(object sender);
    public delegate void BackHandler(object sender);

    public delegate void PlayerConnectedHandler(PlayerIndex player);
    public delegate void PlayerDisconnectedHandler(PlayerIndex player);

    #region Argument Classes
    public class CancelEventArgs
    {
        private bool m_isCanceled = false;

        public bool Canceled
        {
            get { return m_isCanceled; }
            set { m_isCanceled = value; }
        }
    }

    public class ButtonEventArgs
    {
        private PlayerIndex m_pIndex = PlayerIndex.One;
        private GamePadState m_state;
        private GamePadButton m_button;

        public ButtonEventArgs()
        {
        }

        #region Properties
        public PlayerIndex PlayerIndex
        {
            get { return m_pIndex; }
            set { m_pIndex = value; }
        }

        public GamePadState State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public GamePadButton Button
        {
            get { return m_button; }
            set { m_button = value; }
        }
        #endregion
    }

    [Flags()]
    public enum Player
    {
        One = 1,
        Two = 2,
        Three = 4,
        Four = 8
    }

    public enum GamePadButton
    {
        X,
        Y,
        A,
        B,
        Back,
        LeftShoulder,
        RightShoulder,
        LeftStick,
        RightStick,
        LeftStickUp,
        LeftStickDown,
        LeftStickLeft,
        LeftStickRight,
        RightStickUp,
        RightStickDown,
        RightStickLeft,
        RightStickRight,
        DPadLeft,
        DPadUp,
        DPadDown,
        DPadRight,
        Start,
        RightTrigger,
        LeftTrigger
    }

    public class MouseEventArgs
    {
        private MouseState m_state;
        private MouseButton m_button;
        private Point m_position;

        public MouseEventArgs()
        {
        }

        public MouseState State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
            }
        }

        public MouseButton Button
        {
            get
            {
                return m_button;
            }
            set
            {
                m_button = value;
            }
        }

        public Point Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }
    }

    public enum MouseButton
    {
        Left,
        Right,
        Middle,
        XButton1,
        XButton2
    }

    public class KeyEventArgs
    {
        private Keys m_key;
        private bool m_shift;
        private bool m_alt;
        private bool m_ctrl;

        public KeyEventArgs()
        {
        }

        public Keys Key
        {
            get
            {
                return m_key;
            }
            set
            {
                m_key = value;
            }
        }

        public bool Shift
        {
            get
            {
                return m_shift;
            }
            set
            {
                m_shift = value;
            }
        }

        public bool Alt
        {
            get
            {
                return m_alt;
            }
            set
            {
                m_alt = value;
            }
        }

        public bool Ctrl
        {
            get
            {
                return m_ctrl;
            }
            set
            {
                m_ctrl = value;
            }
        }
    }
    #endregion
}

#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;
#endregion

namespace Xe.Input
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public sealed class Ebi<T> : Microsoft.Xna.Framework.GameComponent, IEbiService where T : ControllerBase, new()
    {
        #region Members
        private ControllerBase m_controller = null;
        private float m_repeatInterval = 250f;
        private bool m_tabEnabled = true;
        private IFocusable m_focus = null;
        private Player m_allowedPlayers = Player.One;

        private List<Keys> m_tabKeys = new List<Keys>();
        private List<GamePadButton> m_tabNextButtons = new List<GamePadButton>();
        private List<GamePadButton> m_tabPrevButtons = new List<GamePadButton>();

        private List<GamePadButton> m_selectButtons = new List<GamePadButton>();
        private List<GamePadButton> m_backButtons = new List<GamePadButton>();
        #endregion

        #region Events
        public event KeyDownHandler KeyDown;
        public event KeyUpHandler KeyUp;

        public event MouseDownHandler RequestingFocus;
        public event MouseDownHandler MouseDown;
        public event MouseUpHandler MouseUp;

        public event ButtonDownHandler ButtonDown;
        public event ButtonUpHandler ButtonUp;

        public event TabNextHandler TabNext;
        public event TabPrevHandler TabPrev;

        public event PlayerConnectedHandler PlayerConnected;
        public event PlayerDisconnectedHandler PlayerDisconnected;

        public event SelectHandler SelectPressed;
        public event SelectHandler SelectReleased;
        public event BackHandler BackPressed;
        public event BackHandler BackReleased;
        #endregion

        #region Constructor
        public Ebi(Game game)
            : base(game)
        {
            m_controller = new T();
            m_controller.EbiService = this;

            m_tabKeys.Add(Keys.Tab);
            m_tabNextButtons.Add(GamePadButton.LeftStickDown);
            m_tabPrevButtons.Add(GamePadButton.LeftStickUp);

            m_selectButtons.Add(GamePadButton.A);
            m_backButtons.Add(GamePadButton.B);
        }
        #endregion

        #region Updating
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Update the controller.
            if (m_controller != null)
                m_controller.Update(gameTime);

            base.Update(gameTime);
        }
        #endregion

        #region Select & Back Methods
        public void Select()
        {
            // Fire the select event
            if (this.SelectPressed != null)
                this.SelectPressed.Invoke(m_focus);
        }

        public void Back()
        {
            // Fire the Back event
            if (this.BackPressed != null)
                this.BackPressed.Invoke(m_focus);
        }
        #endregion

        #region Connect / Disconnect Methods
        public void ConnectPlayer(PlayerIndex index)
        {
            if (this.PlayerConnected != null)
                this.PlayerConnected.Invoke(index);
        }

        public void DisconnectPlayer(PlayerIndex index)
        {
            if (this.PlayerDisconnected != null)
                this.PlayerDisconnected.Invoke(index);
        }
        #endregion

        #region Focusing & Tabbing
        public void RequestFocus(MouseEventArgs args)
        {
            if (this.RequestingFocus != null)
                this.RequestingFocus.Invoke(args);
        }

        public void TabToNext(CancelEventArgs args)
        {
            if (this.TabNext != null)
                this.TabNext.Invoke(args);
        }

        public void TabToPrev(CancelEventArgs args)
        {
            if (this.TabPrev != null)
                this.TabPrev.Invoke(args);
        }
        #endregion

        #region Simulation Methods
        public void SimulateKey(KeyEventArgs args, bool down)
        {
            if (down && this.KeyDown != null)
            {
                this.KeyDown.Invoke(m_focus, args);
            }
            else if (!down && this.KeyUp != null)
                this.KeyUp.Invoke(m_focus, args);
        }

        public void SimulateButton(ButtonEventArgs args, bool down)
        {
            if (down)
            {
                if(this.ButtonDown != null)
                    this.ButtonDown.Invoke(m_focus, args);

                if (m_selectButtons.Contains(args.Button))
                {
                    if (this.SelectPressed != null)
                        this.SelectPressed.Invoke(m_focus);
                }
                else if(m_backButtons.Contains(args.Button))
                {
                    if(this.BackPressed != null)
                        this.BackPressed.Invoke(m_focus);
                }
            }
            else if (!down)
            {
                if(this.ButtonUp != null)
                    this.ButtonUp.Invoke(m_focus, args);

                if (m_selectButtons.Contains(args.Button))
                {
                    if (this.SelectReleased != null)
                        this.SelectReleased.Invoke(m_focus);
                }
                else if(m_backButtons.Contains(args.Button))
                {
                    if(this.BackReleased != null)
                        this.BackReleased.Invoke(m_focus);
                }
            }
        }

        public void SimulateMouse(MouseEventArgs args, bool down)
        {
            //RequestFocus(args);

            if (down && this.MouseDown != null)
            {
                this.MouseDown.Invoke(args);
            }
            else if (!down && this.MouseUp != null)
                this.MouseUp.Invoke(args);
        }
        #endregion

        #region Properties
        public float RepeatInterval
        {
            get { return m_repeatInterval; }
            set { m_repeatInterval = value; }
        }

        public IFocusable Focus
        {
            get { return m_focus; }
            set 
            {
                if ( value == null )
                {
                    if ( m_focus != null )
                        m_focus.UnFocus ();

                    m_focus = value;
                }
                else
                {
                    if ( value.Focus () )
                    {
                        if ( m_focus != null )
                            m_focus.UnFocus ();

                        m_focus = value;
                    }
                }
            }
        }

        public bool TabEnabled
        {
            get { return m_tabEnabled; }
            set { m_tabEnabled = value; }
        }

        public Player AllowedPlayers
        {
            get { return m_allowedPlayers; }
            set { m_allowedPlayers = value; }
        }

        public List<Keys> TabKeys
        {
            get { return m_tabKeys; }
            set { m_tabKeys = value; }
        }

        public List<GamePadButton> TabNextButtons
        {
            get { return m_tabNextButtons; }
            set { m_tabNextButtons = value; }
        }

        public List<GamePadButton> TabPrevButtons
        {
            get { return m_tabPrevButtons; }
            set { m_tabPrevButtons = value; }
        }
        #endregion
    }
}



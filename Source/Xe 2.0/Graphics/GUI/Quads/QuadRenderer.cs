#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Gui
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public partial class QuadRenderer : IDrawable, IUpdateable
    {
        #region Members
        private Game m_game = null;
        private GraphicsDevice m_device = null;
        private IGraphicsDeviceService m_deviceService;

        private int m_drawOrder = 0;
        private int m_updateOrder = 0;
        private bool m_enabled = true;
        private bool m_visible = true;

        private bool m_useBreadthFirst = true;
        #endregion

        #region Events
        public event EventHandler EnabledChanged;
        public event EventHandler UpdateOrderChanged;
        public event EventHandler VisibleChanged;
        public event EventHandler DrawOrderChanged;
        #endregion

        #region Constructor and Initialization
        public QuadRenderer()
        {
            AddHandlers();
        }

        public virtual void Initialize(Game game, GraphicsDevice device)
        {
            m_game = game;
            m_device = device;

            m_deviceService = (IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService));

            m_deviceService.DeviceReset += new EventHandler(OnDeviceReset);
            m_deviceService.DeviceResetting += new EventHandler(OnDeviceResetting);
            m_deviceService.DeviceDisposing += new EventHandler(OnDeviceDisposing);
            m_deviceService.DeviceCreated += new EventHandler(OnDeviceCreated);

            if (device != null)
                LoadGraphicsContent(true);
        }
        #endregion

        #region Event Handlers
        private void AddHandlers()
        {
            this.EnabledChanged += new EventHandler(OnEnabledChanged);
            this.UpdateOrderChanged += new EventHandler(OnUpdateOrderChanged);
            this.VisibleChanged += new EventHandler(OnVisibleChanged);
            this.DrawOrderChanged += new EventHandler(OnDrawOrderChanged);
        }

        #region Device Handlers
        private void OnDeviceCreated(object sender, EventArgs e)
        {
            LoadGraphicsContent(true);
        }

        private void OnDeviceDisposing(object sender, EventArgs e)
        {
            UnloadGraphicsContent(true);
        }

        private void OnDeviceResetting(object sender, EventArgs e)
        {
            UnloadGraphicsContent(false);
        }

        private void OnDeviceReset(object sender, EventArgs e)
        {
            LoadGraphicsContent(false);
        }
        #endregion

        protected virtual void OnDrawOrderChanged(object sender, EventArgs e)
        {
        }

        protected virtual void OnVisibleChanged(object sender, EventArgs e)
        {
        }

        protected virtual void OnUpdateOrderChanged(object sender, EventArgs e)
        {
        }

        protected virtual void OnEnabledChanged(object sender, EventArgs e)
        {
        }
        #endregion

        #region Load / Unload Graphics Content
        protected virtual void LoadGraphicsContent(bool loadAllContent)
        {
        }

        protected virtual void UnloadGraphicsContent(bool unloadAllContent)
        {
        }
        #endregion

        #region Drawing
        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void RenderQuad(QuadBase quad)
        {
        }

        public virtual void Flush()
        {
        }
        #endregion

        #region IUpdateable Members
        public virtual void Update(GameTime gameTime)
        {
        }
        #endregion

        #region Properties
        protected GraphicsDevice GraphicsDevice
        {
            get { return m_device; }
        }

        protected Game Game
        {
            get { return m_game; }
        }

        public int DrawOrder
        {
            get { return m_drawOrder; }
            set
            {
                m_drawOrder = value;

                if (this.DrawOrderChanged != null)
                    this.DrawOrderChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public int UpdateOrder
        {
            get { return m_updateOrder; }
            set
            {
                m_updateOrder = value;

                if (this.UpdateOrderChanged != null)
                    this.UpdateOrderChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public bool Enabled
        {
            get { return m_enabled; }
            set
            {
                m_enabled = value;

                if (this.EnabledChanged != null)
                    this.EnabledChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public bool Visible
        {
            get { return m_visible; }
            set
            {
                m_visible = value;

                if (this.VisibleChanged != null)
                    this.VisibleChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public bool UseBreadthFirst
        {
            get { return m_useBreadthFirst; }
            set { m_useBreadthFirst = value; }
        }
        #endregion
    }
}



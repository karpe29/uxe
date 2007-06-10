#region License
/*
 *  Xna5D.GUI.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 01, 2006
*/
#endregion License

#region Libraries
using System;
using System.Collections.Generic;

using Xe;
using Xe.Input;
using Xe.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;
#endregion

namespace Xe.GUI
{
	public enum TextAlignment
	{
		Left,
		Center,
		Right,
	}

    public partial class UIControl : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        protected SpriteFont m_font;
        protected TextAlignment m_textAlign = TextAlignment.Left;

        protected SpriteBatch m_spriteBatch;

        protected string m_name = "UIControl";
        protected string m_text = "UIControl";

        protected int m_absX = 0, m_absY = 0;
        protected int m_relX = 0, m_relY = 0;
        protected int m_height = 25, m_width = 100;
		protected bool m_lockX = false, m_lockY = false;

        protected IEbiService m_ebi;
        protected IReporterService m_reporter;

        protected List<QuadBase> m_outRects = new List<QuadBase>();
        protected List<QuadBase> m_overRects = new List<QuadBase>();
        protected List<QuadBase> m_downRects = new List<QuadBase>();
        protected List<QuadBase> m_disabledRects = new List<QuadBase>();

        // The current state of the control.
        protected UIState m_state = UIState.Out;

        // Some flags.
        protected bool m_isResizable = false;           // Whether or not the control can be resized.
        protected bool m_isDraggable = false;           // Whether or not the control can be dragged.
        protected bool m_isHoverable = false;           // "" can be hovered over.
        protected bool m_isDragging = false;            // If the control is being dragged.
        protected bool m_isResizing = false;            // If the control is being resized.
        private Vector2 m_offset = new Vector2(0, 0);   // Offset vector used for resizing and dragging.

        // The color of the text
        protected Color m_foreColor = Color.Black;

        // The average width of each character
        protected int m_fontWidth = 7;

        // Rectangles to limit the size of the control
        private Rectangle m_minSize = Rectangle.Empty;
        private Rectangle m_maxSize = Rectangle.Empty;

        // Whether or not the cursor is set.
        //private bool m_isCursorSet = false;

        // Whether or not the control needs to update its quads.
        protected bool m_needsUpdate = true;

        protected UIControl m_parent;
        protected List<UIControl> m_controls = new List<UIControl>();

        protected GUIManager m_guiManager;
        protected Stats m_stats;

        private int m_curTab = 0;
        private int m_tabOrder = 0;

        private Dock m_docking = Dock.None;
        private Anchor m_anchor = Anchor.Left | Anchor.Top;
        #endregion

        #region UIState Enumeration
        protected enum UIState
        {
            Out,
            Over,
            Down,
            Disabled
        }
        #endregion

        #region Events
        public event MouseDownHandler MouseDown;
        public event MouseUpHandler MouseUp;

        public event KeyDownHandler KeyDown;
        public event KeyUpHandler KeyUp;

        public event MoveHandler Move;
        public event ResizeHandler Resize;

        public event ClickHandler Click;

        public event DockChangedHandler DockChanged;
        public event AnchorChangedHandler AnchorChanged;
        #endregion

        #region Constructor
		public UIControl(Game game, GUIManager guiManager)
            : base(game)
        {
            m_guiManager = guiManager;

            AddHandlers();
        }
        #endregion

        #region Load / Unload Graphics
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                m_spriteBatch = new SpriteBatch(this.GraphicsDevice);

				m_font = this.m_guiManager.ContentManager.Load<SpriteFont>(@"Content\Fonts\Comic");
            }
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if (unloadAllContent)
            {
                if (m_spriteBatch != null)
                    m_spriteBatch.Dispose();

                m_spriteBatch.Dispose();
            }
        }
        #endregion

        #region Add / Remove Handlers
        protected virtual void AddHandlers()
        {
            this.MouseDown += new MouseDownHandler(this.OnMouseDown);
            this.MouseUp += new MouseUpHandler(this.OnMouseUp);

            this.KeyDown += new KeyDownHandler(this.OnKeyDown);
            this.KeyUp += new KeyUpHandler(this.OnKeyUp);

            this.Move += new MoveHandler(this.OnMove);
            this.Resize += new ResizeHandler(this.OnResize);

            this.Click += new ClickHandler(this.OnClick);

            this.AnchorChanged += new AnchorChangedHandler(this.OnAnchorChanged);
            this.DockChanged += new DockChangedHandler(this.OnDockChanged);
        }

        protected virtual void RemoveHandlers()
        {
            try
            {
                this.MouseDown -= this.OnMouseDown;
                this.MouseUp -= this.OnMouseUp;

                this.KeyDown -= this.OnKeyDown;
                this.KeyUp -= this.OnKeyUp;

                this.Move -= this.OnMove;
                this.Resize -= this.OnResize;

                this.Click -= this.OnClick;

                this.AnchorChanged -= this.OnAnchorChanged;
                this.DockChanged -= this.OnDockChanged;
            }
            catch(NullReferenceException nre)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, nre.Message, nre);
            }
        }

        private void AddEbiHandlers()
        {
            if (m_ebi != null)
            {
                m_ebi.KeyDown += new KeyDownHandler(Ebi_KeyDown);
                m_ebi.KeyUp += new KeyUpHandler(Ebi_KeyUp);
                m_ebi.MouseDown += new MouseDownHandler(Ebi_MouseDown);
                m_ebi.MouseUp += new MouseUpHandler(Ebi_MouseUp);
            }
        }

        private void RemoveEbiHandlers()
        {
            if (m_ebi != null)
            {
                m_ebi.KeyDown -= this.Ebi_KeyDown;
                m_ebi.KeyUp -= this.Ebi_KeyUp;
                m_ebi.MouseDown -= this.Ebi_MouseDown;
                m_ebi.MouseUp -= this.Ebi_MouseUp;
            }
        }
        #endregion

        #region Event Handlers
        protected virtual void OnDockChanged(object sender)
        {
            m_needsUpdate = true;
        }

        protected virtual void OnAnchorChanged(object sender)
        {
            m_needsUpdate = true;
        }

        protected virtual void OnClick(object sender, MouseEventArgs args)
        {
        }

        protected virtual void OnKeyUp(object focus, KeyEventArgs k)
        {
        }

        protected virtual void OnKeyDown(object focus, KeyEventArgs k)
        {
        }

        protected virtual void OnMouseUp(MouseEventArgs args)
        {
            if (CheckCoords(args.Position.X, args.Position.Y) && m_isHoverable)
                m_state = UIState.Over;
            else
                m_state = UIState.Out;
        }

        protected virtual void OnMouseDown(MouseEventArgs args)
        {
            m_state = UIState.Down;
        }

        protected virtual void OnMove(object sender)
        {
            m_needsUpdate = true;
        }

        protected virtual void OnResize(object sender)
        {
            if (m_minSize != Rectangle.Empty)
            {
                if (m_width < m_minSize.Width)
                    m_width = m_minSize.Width;

                if (m_height < m_minSize.Height)
                    m_height = m_minSize.Height;
            }

            if (m_maxSize != Rectangle.Empty)
            {
                if (m_width > m_maxSize.Width)
                    m_width = m_maxSize.Width;

                if (m_height > m_maxSize.Height)
                    m_height = m_maxSize.Height;
            }
        }

        private void Parent_Move(object sender)
        {
            m_absX = m_parent.AbsolutePosition.X + m_relX;
            m_absY = m_parent.AbsolutePosition.Y + m_relY;

            m_needsUpdate = true;
        }
        #endregion

        #region Ebi Events Handlers
        protected void Ebi_KeyUp(object focus, KeyEventArgs k)
        {
            if (focus == this && this.Enabled)
                this.KeyUp.Invoke(this, k);
        }

        protected void Ebi_KeyDown(object focus, KeyEventArgs k)
        {
            if (focus == this && this.Enabled)
            {
                if (k.Key == Keys.Tab)
                {
                    if (k.Shift)
                    {
                        if (Parent != null)
                            Parent.TabPrevControl();
                        else
                            m_guiManager.TabPrevControl();
                    }
                    else
                    {
                        if (Parent != null)
                            Parent.TabNextControl();
                        else
                            m_guiManager.TabNextControl();
                    }
                }
                else
                {
                    this.KeyDown.Invoke(this, k);
                }
            }
        }

        protected void Ebi_MouseUp(MouseEventArgs args)
        {
            if (m_ebi.GetFocus() == this && this.Enabled)
            {
                this.MouseUp.Invoke(args);

                if (m_isResizing)
                    this.Resize.Invoke(this);

                m_isDragging = false;
                m_isResizing = false;
            }
        }

        protected void Ebi_MouseDown(MouseEventArgs args)
        {
            if (m_ebi.GetFocus() == this && this.Enabled)
            {
                if (this.MouseDown != null)
                    this.MouseDown.Invoke(args);

                if (this.Click != null)
                    this.Click.Invoke(this, args);

                if (args.Position.X > (m_absX + this.Width - (2 * m_guiManager.CornerSize)) && args.Position.X < (m_absX + this.Width) &&
                    args.Position.Y > (m_absY + this.Height - (2 * m_guiManager.CornerSize)) && args.Position.Y < (m_absY + this.Height) && this.IsResizable)
                {
                    m_isResizing = true;
                    m_isDragging = false;
                    if (!m_lockX)
						m_offset.X = m_absX + this.Width;
					if (!m_lockY)
						m_offset.Y = m_absY + this.Height;
                }
                else
                {
                    m_isDragging = true;
                    m_isResizing = false;
					if (!m_lockX)
						m_offset.X = args.Position.X - m_absX;
					if (!m_lockY)
						m_offset.Y = args.Position.Y - m_absY;
                }
            }
        }
        #endregion

        #region GameComponent Overrides
        public override void Initialize()
        {
            base.Initialize();

            m_ebi = (IEbiService)this.Game.Services.GetService(typeof(IEbiService));
            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));
            m_stats = (Stats)this.Game.Services.GetService(typeof(Stats));

            AddEbiHandlers();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState _state = Mouse.GetState();
            if (m_state != UIState.Down)
            {
                if (CheckCoords(_state.X, _state.Y) && m_isHoverable)
                    m_state = UIState.Over;
                else
                    m_state = UIState.Out;
            }

            if (!this.Enabled)
                m_state = UIState.Disabled;

            if (m_isDragging && m_isDraggable)
            {
                if (!this.m_lockX)
					this.X = _state.X - (int)m_offset.X;
				if (!this.m_lockY)
					this.Y = _state.Y - (int)m_offset.Y;
            }
            else if (m_isResizing && m_isResizable)
            {
                if (_state.X < m_absX + 5 || _state.Y < m_absY + 5)
                    return;

				if (!this.m_lockX)
					this.Width = _state.X - m_absX;
				if (!this.m_lockY)
					this.Height = _state.Y - m_absY;

				if (m_lockX || m_lockY)
					m_needsUpdate = true;
            }

            //if (_state.X > (m_absX + this.Width - m_guiManager.CornerSize * 2) && _state.X < (m_absX + this.Width) &&
            //        _state.Y > (m_absY + this.Height - m_guiManager.CornerSize * 2) && _state.Y < (m_absY + this.Height))
            //{
            //    Cursor _cursor = (Cursor)this.Game.Services.GetService(typeof(Cursor));
            //    if (_cursor != null)
            //    {
            //        if (!m_isCursorSet)
            //        {
            //            _cursor.SetCursor("CURSOR_RESIZE");
            //            m_isCursorSet = true;
            //        }
            //    }
            //}
            //else
            //{
            //    if (m_isCursorSet)
            //    {
            //        Cursor _cursor = (Cursor)this.Game.Services.GetService(typeof(Cursor));
            //        if (_cursor != null)
            //        {
            //            _cursor.SetCursor("CURSOR_START");
            //            m_isCursorSet = false;
            //        }
            //    }
            //}
        }

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.m_outRects != null)
				{
					for (int i = 0; i < this.m_outRects.Count; i++)
					{
						this.m_outRects[i].Dispose();
					}
					this.m_outRects.Clear();
				}

				if (this.m_overRects != null)
				{
					for (int j = 0; j < this.m_overRects.Count; j++)
					{
						this.m_overRects[j].Dispose();
					}
					this.m_overRects.Clear();
				}

				if (this.m_downRects != null)
				{
					for (int k = 0; k < this.m_downRects.Count; k++)
					{
						this.m_downRects[k].Dispose();
					}
					this.m_downRects.Clear();
				}

				if (this.m_disabledRects != null)
				{
					for (int m = 0; m < this.m_disabledRects.Count; m++)
					{
						this.m_disabledRects[m].Dispose();
					}
					this.m_disabledRects.Clear();
				}

				for (int n = 0; n < this.m_controls.Count; n++)
				{
					this.m_controls[n].Dispose(true);
				}
				this.m_controls.Clear();
			}

			base.Dispose(disposing);
		}
        #endregion

        #region Drawing
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!this.Enabled)
                Update(gameTime);

            m_spriteBatch.Begin(SpriteBlendMode.AlphaBlend,SpriteSortMode.Immediate,SaveStateMode.SaveState);
            if (m_state == UIState.Out)
            {
                foreach (QuadBase _rect in m_outRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White); 
                
                //m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 32000);

                if (m_stats != null && m_stats.Visible)
                    m_stats.PolygonCount += m_outRects.Count * 2;
            }
            else if (m_state == UIState.Disabled)
            {
                foreach (QuadBase _rect in m_disabledRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);

                if (m_stats != null && m_stats.Visible)
                    m_stats.PolygonCount += m_disabledRects.Count * 2;
            }
            else if (m_state == UIState.Over)
            {
                foreach (QuadBase _rect in m_overRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);

                if (m_stats != null && m_stats.Visible)
                    m_stats.PolygonCount += m_overRects.Count * 2;
            }
            else if (m_state == UIState.Down)
            {
                foreach (QuadBase _rect in m_downRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);

                if (m_stats != null && m_stats.Visible)
                    m_stats.PolygonCount += m_downRects.Count * 2;
            }

            if (m_font != null && !String.IsNullOrEmpty(m_text))
            {
                #region Text
                //Rectangle _rect = new Rectangle(m_absX + m_guiManager.CornerSize, m_absY + (int)(this.Height / 2) - (int)(m_font.LineHeight / 2), m_width - 5, m_height);
				Rectangle _rect = new Rectangle(m_absX + m_guiManager.CornerSize, m_absY + m_guiManager.CornerSize, m_width - m_guiManager.CornerSize * 2, m_height - m_guiManager.CornerSize * 2);

                int from = _rect.Width / m_fontWidth;
                string toDraw = m_text;
                if (m_text.Length > from)
                    toDraw = m_text.Substring(m_text.Length - from);

				DrawTextInABox(_rect, m_foreColor, toDraw);
                #endregion
            }

			m_spriteBatch.End();

            ResetDeviceStates();
        }

		/// <summary>
		/// Draw text formatted to fit in the specified rectangle
		/// Won't throw exception if called outside a begin/end in release mode
		/// </summary>
		/// <param name="r">The rectangle to fit the text</param>
		/// <param name="cText">Text color</param>
		/// <param name="strFormat">String format</param>
		/// <param name="args">String format args</param>
		public void DrawTextInABox(Rectangle r, Color cText, string strFormat, params object[] args)
		{
			string str = string.Format(strFormat, args);

			int nChars;
			int pxWidth;
			Vector2 vAt = new Vector2(r.Left, r.Top);

			while (str.Length != 0)
			{
				// stop drawing if there isn't room for this line
				if (vAt.Y + m_font.LineSpacing > r.Bottom)
					return;

				CountCharWidth(r.Width, str, out nChars, out pxWidth);

				switch (m_textAlign)
				{
					case TextAlignment.Left:
						vAt.X = r.Left;
						break;
					case TextAlignment.Right:
						vAt.X = r.Left + (r.Width - pxWidth);
						break;
					case TextAlignment.Center:
						vAt.X = r.Left + ((r.Width - pxWidth) / 2);
						break;
				}
#if DEBUG
				m_spriteBatch.DrawString(m_font, str.Substring(0, nChars), vAt, cText);
#else
				try
				{
					m_spriteBatch.DrawString(m_font, str.Substring(0, nChars), vAt, cText);
				}
				catch (InvalidOperationException) { }
#endif
				str = str.Substring(nChars);
				vAt.Y += m_font.LineSpacing;
			}
		}

		/// <summary>
		/// Calculate the number of characters that fit in the given width.
		/// </summary>
		/// <param name="pxMaxWidth">Maximum string width</param>
		/// <param name="str">String</param>
		/// <param name="nChars">Number of characters that fit</param>
		/// <param name="pxWidth">Width of substring</param>
		private void CountCharWidth(int pxMaxWidth, string str, out int nChars, out int pxWidth)
		{
			nChars = 0;
			pxWidth = 0;

			for (int i = 0; i <= str.Length; i++)
			{
				int l = (int)m_font.MeasureString(str.Substring(0, i)).X;

				if (pxWidth < pxMaxWidth)
				{
					nChars = i;
					pxWidth = l;
				}
				else
					break;
			}
		}


        protected virtual void ResetDeviceStates()
        {
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.AlphaTestEnable = false;
        }
        #endregion

        #region DrawOrder, UpdateOrder, Enabled and Visible Event Handlers
        protected override void OnDrawOrderChanged(object sender, EventArgs args)
        {
            base.OnDrawOrderChanged(sender, args);

            foreach (UIControl _control in m_controls)
                _control.DrawOrder = this.DrawOrder + 1;
        }

        protected override void OnUpdateOrderChanged(object sender, EventArgs args)
        {
            base.OnUpdateOrderChanged(sender, args);

            foreach (UIControl _control in m_controls)
                _control.UpdateOrder = this.UpdateOrder + 1;
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            base.OnEnabledChanged(sender, args);

            foreach (UIControl _control in m_controls)
                _control.Enabled = this.Enabled;
        }

        protected override void OnVisibleChanged(object sender, EventArgs args)
        {
            base.OnVisibleChanged(sender, args);

            foreach (UIControl _control in m_controls)
                _control.Visible = this.Visible;
        }
        #endregion

        #region Getting Focus
        public object GetFocus(int x, int y)
        {
            if (!this.Visible)
                return null;

            if (CheckCoords(x, y))
            {
                object _obj = null;
                foreach (UIControl _con in m_controls)
                {
                    object _o = _con.GetFocus(x, y);
                    if (_o != null)
                        _obj = _o;
                }

                if (_obj == null)
                {
                    //System.Console.WriteLine("OBJECT_FOCUS_RETURNED: " + this.Name);

                    return this;
                }

                //System.Console.WriteLine("OBJECT_FOCUS_RETURNED: " + ((UIControl)_obj).Name);

                return _obj;
            }
            else
            {
                object _obj = null;
                foreach (UIControl _con in m_controls)
                {
                    object _o = _con.GetFocus(x, y);
                    if (_o != null)
                        _obj = _o;
                }

                if (_obj != null)
                    return _obj;
            }

            return null;
        }

        private bool CheckCoords(int x, int y)
        {
            if (x >= m_absX && (x <= m_absX + m_width))
            {
                if (y >= m_absY && (y <= m_absY + m_height))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Properties
        public Dock Dock
        {
            get
            {
                return m_docking;
            }
            set
            {
                if (value == m_docking)
                    return;

                m_docking = value;

                if (DockChanged != null)
                    DockChanged.Invoke(this);
            }
        }

        public Anchor Anchor
        {
            get
            {
                return m_anchor;
            }
            set
            {
                if (value == m_anchor)
                    return;

                m_anchor = value;

                if (AnchorChanged != null)
                    AnchorChanged.Invoke(this);
            }
        }

        public Point AbsolutePosition
        {
            get
            {
                return new Point(m_absX, m_absY);
            }
        }

        public Rectangle MaximumSize
        {
            get
            {
                return m_maxSize;
            }
            set
            {
                m_maxSize = value;
            }
        }

        public Rectangle MinimumSize
        {
            get
            {
                return m_minSize;
            }
            set
            {
                m_minSize = value;
            }
        }

        public List<UIControl> Controls
        {
            get
            {
                return m_controls;
            }
        }

        public UIControl Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                if (m_parent != null)
                {
                    try
                    {
                        m_parent.Move -= this.Parent_Move;
                    }
                    catch(Exception e)
                    {
                        if (m_reporter != null)
                            m_reporter.BroadcastError(this, e.Message, e);
                    }
                }

                m_parent = value;

                m_absX = m_parent.AbsolutePosition.X + m_relX;
                m_absY = m_parent.AbsolutePosition.Y + m_relY;

                m_parent.Move += new MoveHandler(Parent_Move);

                this.TabOrder = Parent.GetTabOrder();
            }
        }

        public int FontWidth
        {
            get
            {
                return m_fontWidth;
            }
            set
            {
                m_fontWidth = value;
            }
        }

        public Color ForeColor
        {
            get
            {
                return m_foreColor;
            }
            set
            {
                m_foreColor = value;
            }
        }

        public TextAlignment TextAlign
        {
            get
            {
                return m_textAlign;
            }
            set
            {
                m_textAlign = value;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }

        public bool IsHoverable
        {
            get
            {
                return m_isHoverable;
            }
            set
            {
                m_isHoverable = value;
            }
        }

        public bool IsDraggable
        {
            get
            {
                return m_isDraggable;
            }
            set
            {
                m_isDraggable = value;
            }
        }

        public bool IsResizable
        {
            get
            {
                return m_isResizable;
            }
            set
            {
                m_isResizable = value;
            }
        }

        public int X
        {
            get
            {
                return m_relX;
            }
            set
            {
                m_relX = value;

                if (m_parent != null)
                    m_absX = m_parent.AbsolutePosition.X + m_relX;
                else
                    m_absX = m_relX;

                this.Move.Invoke(this);
            }
        }

        public int Y
        {
            get
            {
                return m_relY;
            }
            set
            {
                m_relY = value;

                if (m_parent != null)
                    m_absY = m_parent.AbsolutePosition.Y + m_relY;
                else
                    m_absY = m_relY;

                this.Move.Invoke(this);
            }
        }

        public int Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }

        public int Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }

        public int TabOrder
        {
            get
            {
                return m_tabOrder;
            }
            set
            {
                m_tabOrder = value;
            }
        }

		public bool LockX
		{
			get
			{
				return m_lockX;
			}
			set
			{
				m_lockX = value;
			}
		}

		public bool LockY
		{
			get
			{
				return m_lockY;
			}
			set
			{
				m_lockY = value;
			}
		}
        #endregion

        #region Tabbing
        public void TabPrevControl()
        {
            m_controls.Sort(new CompareTab());

            int _index = m_controls.Count - 1;
            for (; _index >= 0; _index--)
            {
                //System.Console.WriteLine("\tTAB_ORDER: " + m_controls[_index].TabOrder.ToString());
                if (m_controls[_index].TabOrder > m_curTab)
                {
                    break;
                }
            }

            if (_index < 0)
                _index = m_controls.Count - 1;

            for (; _index >= 0; _index--)
            {
                if (m_controls[_index].Enabled && m_controls[_index].Visible)
                    break;
                if (_index <= 0)
                    _index = m_controls.Count - 1;
            }

            m_ebi.SetFocus(m_controls[_index]);
            m_curTab = m_controls[_index].TabOrder;
        }

        public void TabNextControl()
        {
            m_controls.Sort(new CompareTab());

            int _index;
            for (_index = 0; _index < m_controls.Count; _index++)
            {
                //System.Console.WriteLine("\tTAB_ORDER: " + m_controls[_index].TabOrder.ToString());
                if (m_controls[_index].TabOrder > m_curTab)
                {
                    break;
                }
            }

            if (_index >= m_controls.Count)
                _index = 0;

            for (; _index < m_controls.Count; _index++)
            {
                if (m_controls[_index].Enabled && m_controls[_index].Visible)
                    break;
                if (_index == m_controls.Count - 1)
                    _index = 0;
            }

            m_ebi.SetFocus(m_controls[_index]);
            m_curTab = m_controls[_index].TabOrder;
        }

        public int GetTabOrder()
        {
            int _max = 0;
            foreach (UIControl _control in m_controls)
            {
                if (_control.TabOrder > _max)
                    _max = _control.TabOrder;
            }

            return _max;
        }
        #endregion
    }

    #region Delegates
    public delegate void MoveHandler(object sender);
    public delegate void ResizeHandler(object sender);

    public delegate void ClickHandler(object sender, MouseEventArgs args);

    public delegate void DockChangedHandler(object sender);
    public delegate void AnchorChangedHandler(object sender);
    #endregion
}



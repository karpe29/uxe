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
using Xe.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;
using Xe.Input;
#endregion

namespace Xe.GUI
{
	public class UIControl : Microsoft.Xna.Framework.DrawableGameComponent, IFocusable, IMoveable
	{
		#region Members
		private string m_name = "UIControl";    // The name of the control
		private string m_tag = "uicontrol";     // The tag that defines what part of the texture to use.

		private int m_treeLevel = 0;            // The height in the tree structure.
		private int m_tabOrder = 0;             // The order at which the control is tabbed to.
		private int m_stockDrawOrder = 0;       // The original draw order.

		private ControlCollection m_controls;   // Collection of child controls.

		private QuadRenderer m_renderer;        // The rendering core used to draw.
		private Font2D m_font2d;                // The font core used to render fonts.
		private IGUIManager m_guiManager;       // The base GUI manager.
		private IEbiService m_ebiService;       // The Event Based Input Service

		// State Possibilites
		private bool m_isTabable = true;        // Whether or not the control can be tabbed to.
		private bool m_isHoverable = true;      // Whether or not the control can be hovered over.
		private bool m_isDraggable = false;      // Whether or not the control can be dragged.
		private bool m_isDragging = false;      // Whether or not the control is being dragged.
		private bool m_isSizeable = false;       // Whether or not the control can be resized.
		private bool m_isSnapable = false;      // Whether or not the control snaps to the grid.
		private bool m_isTextVisible = true;    // Whether or not the text is drawn
		private bool m_isFocusable = true;      // Whether or not the control can be given focus.
		private bool m_isOneQuad = false;       // Whether or not the control uses one quad to draw the background.
		private bool m_useTextScroll = false;   // Whether or not the text is scrolled horizontally.

		private bool m_lockX = false;
		private bool m_lockY = false;
		private int m_snappingStep = GUIManager<QuadRenderer>.SnappingStep;

		private bool m_useEnter = false;
		private bool m_useAButton = false;

		private UIControl m_parent = null;
		private Rectangle m_minSize = new Rectangle();
		private Rectangle m_maxSize = new Rectangle();

		// State Variables
		//private bool m_isDragging = false;      // Whether or not the control is being dragged.
		//private bool m_isResizing = false;      // Whether or not the control is being resized.

		// Background quads
		private List<GUIQuad> m_outQuads = new List<GUIQuad>();       // Background: Out : Base
		private List<GUIQuad> m_overQuads = new List<GUIQuad>();      // When being hovered over by the mouse.
		private List<GUIQuad> m_disabledQuads = new List<GUIQuad>();  // When the control is disabled.
		private List<GUIQuad> m_downQuads = new List<GUIQuad>();      // When the control is being pressed.
		private List<GUIQuad> m_focusQuads = new List<GUIQuad>();     // When the control is being focused.

		// Paths for Movement
		private Path m_overPath;

		// State management
		private UIState m_lastState = UIState.Out;  // The last state used.
		private UIState m_curState = UIState.Out;   // The control's current state.
		private UIState m_nextState = UIState.Out;  // The next state to apply.

		// Position fields
		protected float m_x = 10;         // Relative X Position
		protected float m_y = 10;         // Relative Y Position
		protected float m_absX = 10;      // Absolute X Position
		protected float m_absY = 10;      // Absolute Y Position
		protected float m_width = 32;   // Width
		protected float m_height = 32;  // Height
		private float m_lastX = 10;     // Previous X Position
		private float m_lastY = 10;     // Previous Y Position
		private float m_lastW = 32;
		private float m_lastH = 32;
		private Vector2 m_absPosition = new Vector2();
		private Vector2 m_initialPos = new Vector2();
		private Vector2 m_initialSize = new Vector2();

		// Vector Position & Size
		private Vector2 m_vecPosition = new Vector2(10, 10);
		private Vector2 m_vecSize = new Vector2(32, 32);

		// Text to be drawn.
		private string m_text = "Hello, world.";
		private string m_internalText = "Hello, world.";
		private string m_hideString = String.Empty;
		private bool m_useHideString = false;

		// Text alignment
		private TextAlign m_textAlign = TextAlign.Left;
		private TextAlignVertical m_textAlignVertical = TextAlignVertical.Top;
		private BreakStyle m_breakStyle = BreakStyle.Word;


		// Color of the text
		private Color m_foreColor = Color.White;        // Base
		private Color m_foreColorHover = Color.Yellow;  // When hovered.

		// Whether or not the control needs to be updated.
		protected bool m_needsUpdate = false;

		// The current tab index.
		private int m_curTabIndex = -1;

		// Whether or not the control has initialized and
		// gone through a hard reset.
		protected bool m_initialized = false;

		protected IReporterService m_reporter;

		// Offsets
		private Vector2 m_offset = new Vector2(0, 0);       // Offset vector used for resizing and dragging.
		protected Vector2 m_textOffset = new Vector2(0, 0);   // Offset vector used for the text rendering
		protected Rectangle m_viewportOffset = new Rectangle(0, 0, 0, 0); // Offset vector used for resizing the viewport

		// Clipping Viewport
		private Viewport m_viewport = new Viewport();
		private bool m_updateViewport = true;
		#endregion

		#region Events
		/// <summary>
		/// Got Focus Event: Fires when the control receives focus.
		/// </summary>
		public event GotFocusHandler GotFocus;

		/// <summary>
		/// Lost Focus Event: Fires when the control loses focus.
		/// </summary>
		public event LostFocusHandler LostFocus;

		public event EventHandler TabOrderChanged;

		public event SelectHandler Selected;

		public event MouseDownHandler MouseDown;
		//public event ClickHandler Clicked;
		public event MouseUpHandler MouseUp;

		public event KeyDownHandler KeyDown;
		public event KeyUpHandler KeyUp;

		public event ButtonDownHandler ButtonDown;
		public event ButtonUpHandler ButtonUp;

		/// <summary>
		/// Move Event: Fires when the control is moved.
		/// </summary>
		public event MoveHandler Move;

		/// <summary>
		/// Resize Event: Fires when the control is resized.
		/// </summary>
		public event ResizeHandler Resize;

		/// <summary>
		/// Click Event: Fires after MouseDown and right before the MouseUp event.
		/// </summary>
		/// <seealso cref="MouseUp"/>
		public event ClickHandler Click;

		/// <summary>
		/// Dock Changed Event: Fires when the Dock property is changed.
		/// </summary>
		public event DockChangedHandler DockChanged;

		/// <summary>
		/// Anchor Changed Event: Fires when the Anchor property is changed.
		/// </summary>
		public event AnchorChangedHandler AnchorChanged;

		/// <summary>
		/// Viewport Changed Event: Fires when the control's viewport has been updated.
		/// </summary>
		public event EventHandler ViewportChanged;
		#endregion

		#region Constructor
		public UIControl(Game game, IGUIManager guiManager)
			: base(game)
		{
			m_guiManager = guiManager;

			m_font2d = m_guiManager.Font2D;
			m_renderer = m_guiManager.QuadRenderer;

			m_overPath = new Path(this);

			m_ebiService = m_guiManager.Ebi;

			AddEbiHandlers();
			AddHandlers();
		}
		#endregion

		#region Ebi Event Handlers
		protected virtual void AddEbiHandlers()
		{
			m_ebiService.SelectPressed += new SelectHandler(OnEbiSelectPressed);
			m_ebiService.SelectReleased += new SelectHandler(OnEbiSelectReleased);
			m_ebiService.BackPressed += new BackHandler(OnBackPressed);

			m_ebiService.ButtonDown += new ButtonDownHandler(OnEbiButtonDown);
			m_ebiService.ButtonUp += new ButtonUpHandler(OnEbiButtonUp);

			m_ebiService.MouseDown += new MouseDownHandler(OnEbiMouseDown);
			m_ebiService.MouseUp += new MouseUpHandler(OnEbiMouseUp);

			m_ebiService.KeyDown += new KeyDownHandler(OnEbiKeyDown);
			m_ebiService.KeyUp += new KeyUpHandler(OnEbiKeyUp);
		}

		protected virtual void OnEbiKeyUp(object sender, KeyEventArgs e)
		{
			if (m_ebiService.Focus != this)
				return;

			if (this.KeyUp != null)
				this.KeyUp.Invoke(this, e);
		}

		protected virtual void OnEbiKeyDown(object sender, KeyEventArgs e)
		{
			if (m_ebiService.Focus != this)
				return;

			if (this.KeyDown != null)
				this.KeyDown.Invoke(this, e);
		}

		protected virtual void OnEbiMouseUp(MouseEventArgs e)
		{
			if (m_ebiService.Focus != this)
				return;

			m_isDragging = false;

			//Console.WriteLine("Clicking!");

			if (this.Click != null)
			{
				//Console.WriteLine("Invoking Click Event!");
				this.Click.Invoke(this, e);
			}

			if (this.MouseUp != null)
				this.MouseUp.Invoke(e);
		}

		protected virtual void OnEbiMouseDown(MouseEventArgs e)
		{
			if (m_ebiService.Focus != this)
				return;

			if (m_isDraggable)
			{
				m_isDragging = true;

				if (!m_lockX)
					m_offset.X = e.Position.X - m_x;// -m_absX;

				if (!m_lockY)
					m_offset.Y = e.Position.Y - m_y;// -m_absY;
			}

			if (this.MouseDown != null)
			{
				this.MouseDown.Invoke(e);
			}
		}

		protected virtual void OnEbiButtonUp(object sender, ButtonEventArgs e)
		{
			if (m_ebiService.Focus != this)
				return;

			if (this.ButtonUp != null)
				this.ButtonUp.Invoke(this, e);
		}

		protected virtual void OnEbiButtonDown(object sender, ButtonEventArgs e)
		{
			if (m_ebiService.Focus != this)
				return;

			if (this.ButtonDown != null)
				this.ButtonDown.Invoke(this, e);
		}

		protected virtual void OnBackPressed(object sender)
		{
			if (m_ebiService.Focus != this)
				return;
		}

		protected virtual void OnEbiSelectReleased(object sender)
		{
			if (m_ebiService.Focus == this)
			{
				m_nextState = m_lastState;
				m_lastState = m_curState;
				m_curState = UIState.Transitioning;
				System.Console.WriteLine("Going up.");
			}
		}

		protected virtual void OnEbiSelectPressed(object sender)
		{
			if (m_ebiService.Focus == this)
			{
				m_lastState = m_curState;
				m_curState = UIState.Transitioning;
				m_nextState = UIState.Down;
				System.Console.WriteLine("Going down.");

				if (this.Selected != null)
					this.Selected.Invoke(this);
			}
		}
		#endregion

		#region Event Handlers
		protected virtual void AddHandlers()
		{
			this.Game.Window.ClientSizeChanged += new EventHandler(OnClientSizeChanged);
			this.MouseDown += new MouseDownHandler(this.OnMouseDown);
			this.MouseUp += new MouseUpHandler(this.OnMouseUp);

			this.KeyDown += new KeyDownHandler(this.OnKeyDown);
			this.KeyUp += new KeyUpHandler(this.OnKeyUp);

			this.Move += new MoveHandler(this.OnMove);
			this.Resize += new ResizeHandler(this.OnResize);

			this.ButtonDown += new ButtonDownHandler(this.OnButtonDown);
			this.ButtonUp += new ButtonUpHandler(this.OnButtonUp);

			this.Click += new ClickHandler(this.OnClick);

			this.AnchorChanged += new AnchorChangedHandler(this.OnAnchorChanged);
			this.DockChanged += new DockChangedHandler(this.OnDockChanged);

			this.LostFocus += new LostFocusHandler(this.OnLostFocus);
			this.GotFocus += new GotFocusHandler(this.OnGotFocus);
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

				this.ButtonDown -= this.OnButtonDown;
				this.ButtonUp -= this.OnButtonUp;

				this.AnchorChanged -= this.OnAnchorChanged;
				this.DockChanged -= this.OnDockChanged;

				this.LostFocus -= this.OnLostFocus;
				this.GotFocus -= this.OnGotFocus;
			}
			catch (NullReferenceException nre)
			{
				if (m_reporter != null)
					m_reporter.BroadcastError(this, nre.Message, nre);
			}
		}

		protected virtual void OnControlRemoved(UIControl control)
		{
		}

		protected virtual void OnControlAdded(UIControl control)
		{
			control.Initialize();
		}

		protected virtual void OnGotFocus(object sender)
		{
			ChangeState(UIState.Focused);
		}

		protected virtual void OnLostFocus(object sender)
		{
			ChangeState(UIState.Out);
		}

		protected virtual void OnClientSizeChanged(object sender, EventArgs e)
		{
			m_needsUpdate = true;
		}

		protected override void OnDrawOrderChanged(object sender, EventArgs args)
		{
			base.OnDrawOrderChanged(sender, args);
		}

		/// <summary>
		/// Called when the Docking property has changed.
		/// </summary>
		/// <param name="sender">The object that was changed.</param>
		protected virtual void OnDockChanged(object sender)
		{
			m_needsUpdate = true;
		}

		/// <summary>
		/// Called when the Anchor property has changed.
		/// </summary>
		/// <param name="sender">The object that was changed.</param>
		protected virtual void OnAnchorChanged(object sender)
		{
			m_needsUpdate = true;
		}

		/// <summary>
		/// Called when the user has Clicked the object.
		/// </summary>
		/// <param name="sender">The object that was clicked.</param>
		/// <param name="args">Mouse Event Arguments that describe the current state.</param>
		protected virtual void OnClick(object sender, MouseEventArgs args)
		{
		}

		/// <summary>
		/// Called when a Key is let go.
		/// </summary>
		/// <param name="focus">The object that is currently in focus.</param>
		/// <param name="k">Key Event Args object that describes what key was pressed.</param>
		protected virtual void OnKeyUp(object focus, KeyEventArgs k)
		{
		}

		/// <summary>
		/// Called when a Key is pressed down.
		/// </summary>
		/// <param name="focus">The object that is currently in focus.</param>
		/// <param name="k">Key Event Args object that describes what key was pressed.</param>
		protected virtual void OnKeyDown(object focus, KeyEventArgs k)
		{
		}

		/// <summary>
		/// Called when a Mouse button is let go.
		/// </summary>
		/// <param name="args">Mouse Event Arguments that describe the current state.</param>
		protected virtual void OnMouseUp(MouseEventArgs args)
		{
			if (!this.Enabled)
				return;

			if (CheckCoords(args.Position.X, args.Position.Y) && this.Visible)
				ChangeState(UIState.Focused);
			else
				ChangeState(UIState.Out);
		}

		/// <summary>
		/// Called when a Mouse button is pressed.
		/// </summary>
		/// <param name="args">Mouse Event Arguments that describe the current state.</param>
		protected virtual void OnMouseDown(MouseEventArgs args)
		{
			ChangeState(UIState.Down);
		}

		/// <summary>
		/// Called when the control is moved.
		/// </summary>
		/// <param name="sender">The object that was moved.</param>
		protected virtual void OnMove(object sender)
		{
			m_needsUpdate = true;
		}

		/// <summary>
		/// Called when the control is resized.
		/// </summary>
		/// <param name="sender">The object that was resized.</param>
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

			//UpdateGeometry();
			m_needsUpdate = true;
		}

		/// <summary>
		/// Called when the Parent is moved.
		/// </summary>
		/// <param name="sender"></param>
		private void Parent_Move(object sender)
		{
			m_absX = m_parent.AbsolutePosition.X + m_x;
			m_absY = m_parent.AbsolutePosition.Y + m_y;

			/*if (this.Move != null)
				this.Move.Invoke(this);*/

			m_needsUpdate = true;

			/*if (m_effect != null)
			{
				if (m_effect.Parameters["Position"] != null)
				{
					int _width = (this.Parent != null) ? this.Parent.Viewport.Width : this.GraphicsDevice.Viewport.Width;
					int _height = (this.Parent != null) ? this.Parent.Viewport.Height : this.GraphicsDevice.Viewport.Height;

					Vector2 _pos = new Vector2(((float)m_relX * 2) / _width, -((float)m_relY * 2) / _height);
					m_effect.Parameters["Position"].SetValue(_pos);
				}
			}

			m_viewport.X = m_absX + (int)m_viewportOffset.X;
			m_viewport.Y = m_absY + (int)m_viewportOffset.Y;
			m_viewport.Width = m_width - (int)m_viewportOffset.Z;
			m_viewport.Height = m_height - (int)m_viewportOffset.W;*/
		}

		private void Parent_ViewportChanged(object sender, EventArgs e)
		{
			//UpdateGeometry();
		}

		/// <summary>
		/// Called when a Button on the XBOX 360 GamePad is let go.
		/// </summary>
		/// <param name="sender">The object that is receiving the event.</param>
		/// <param name="e">Button event arguments.</param>
		protected virtual void OnButtonUp(object sender, ButtonEventArgs e)
		{
		}

		/// <summary>
		/// Called when a Button on the XBOX 360 GamePad is pressed.
		/// </summary>
		/// <param name="sender">The object that is receiving the event.</param>
		/// <param name="e">Button event arguments.</param>
		protected virtual void OnButtonDown(object sender, ButtonEventArgs e)
		{
			if (!this.Enabled)
				return;

			if (m_useAButton && e.Button == GamePadButton.A)
				ChangeState(UIState.Down);
		}
		#endregion

		#region Updating
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

#if !XBOX && !XBOX360
			MouseState _state = Mouse.GetState();
			if (m_isHoverable && this.Enabled && m_curState != UIState.Down && m_nextState != UIState.Down)
			{
				if (CheckCoords(_state.X, _state.Y) && m_isHoverable && (m_curState != UIState.Transitioning || m_curState != UIState.Over))
				{
					m_lastState = m_curState;
					m_curState = UIState.Transitioning;
					m_nextState = UIState.Over;
				}
				else
				{
					if (m_curState != UIState.Out)
					{
						if (m_curState == UIState.Over || m_nextState == UIState.Over)
						{
							m_lastState = m_curState;
							m_curState = UIState.Transitioning;
							m_nextState = UIState.Out;
						}
					}
				}
			}
#endif

			if (m_curState == UIState.Transitioning)
			{
				/*if (m_nextState == UIState.Focused)
				{
					m_overPath.GoingForward = true;
					if (!m_overPath.Update(gameTime))
					{
						m_curState = m_nextState;
					}
				}
				else if (m_nextState == UIState.Out)
				{
					m_overPath.GoingForward = false;
					if (!m_overPath.Update(gameTime))
					{
						m_curState = m_nextState;
						this.DrawOrder = m_stockDrawOrder;
					}
				}
				else
				{*/
				m_curState = m_nextState;
				//}
			}

#if !XBOX && !XBOX360
			if (m_isDragging && m_isDraggable)
			{
				if (!m_lockX)
				{
					if (GUIManager<QuadRenderer>.UseSnapping || m_isSnapable)
					{
						int _sign = ((_state.X - (int)m_offset.X) - this.X < 0) ? -1 : 1;
						int _dist = (int)Math.Abs((_state.X - (int)m_offset.X) - this.X);

						if (_dist >= (m_snappingStep - 1) * (_dist / m_snappingStep) && _dist <= (m_snappingStep + 1) * (_dist / m_snappingStep))
							this.X += m_snappingStep * _sign * (_dist / m_snappingStep);
					}
					else
					{
						this.X += (_state.X - (int)m_offset.X) - this.X;
					}
					//this.X += (_state.X - (int)m_offset.X) - this.X;
				}

				// If Y is not locked, update the position
				if (!m_lockY)
				{
					if (GUIManager<QuadRenderer>.UseSnapping || m_isSnapable)
					{
						int _sign = ((_state.Y - (int)m_offset.Y) - this.Y < 0) ? -1 : 1;
						int _dist = (int)Math.Abs((_state.Y - (int)m_offset.Y) - this.Y);
						if (_dist >= (m_snappingStep - 1) * (_dist / m_snappingStep) && _dist <= (m_snappingStep + 1) * (_dist / m_snappingStep))
							this.Y += m_snappingStep * _sign * (_dist / m_snappingStep);
					}
					else
					{
						this.Y += (_state.Y - (int)m_offset.Y) - this.Y;
					}
					//this.Y += (_state.Y - (int)m_offset.Y) - this.Y;
				}
			}
#endif
		}

		protected virtual void UpdateViewport()
		{
			m_viewport.X = (int)(m_absX + m_viewportOffset.X);
			m_viewport.Y = (int)(m_absY + m_viewportOffset.Y);
			m_viewport.Width = (int)(m_width - m_viewportOffset.Width);
			m_viewport.Height = (int)(m_height - m_viewportOffset.Height);

			if (m_parent != null)
			{
				if (m_viewport.Width > m_parent.Width)
					m_viewport.Width = (int)(m_parent.Width - m_x - m_parent.ClippingOffset.Width);

				if (m_viewport.Height > m_parent.Height)
					m_viewport.Height = (int)(m_parent.Height - m_y - m_parent.ClippingOffset.Height);
			}

			//UpdateGeometry();
		}

		protected virtual void UpdateGraphics()
		{
			ResetQuads(false);
		}
		#endregion

		#region Drawing
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (m_needsUpdate)
			{
				UpdateGraphics();

				m_needsUpdate = false;
			}

			RenderByState(m_curState, gameTime);

			if (m_isTextVisible)
			{
				//Vector2 tempVector = new Vector2(m_vecPosition.X + this.GUIManager.CornerSize/2, m_vecPosition.Y + this.GUIManager.CornerSize/2);
				//Vector2 tempVectorSize = new Vector2(m_vecSize.X - this.GUIManager.CornerSize / 2, m_vecSize.Y - this.GUIManager.CornerSize / 2);
				//Rectangle tempRect = new Rectangle((int)tempVector.X, (int)tempVector.Y, (int)tempVectorSize.X, (int)tempVectorSize.Y);

				Rectangle tempRect = new Rectangle((int)m_vecPosition.X, (int)m_vecPosition.Y, (int)m_vecSize.X, (int)m_vecSize.Y);

				if (m_vecSize.X >= 2 * GUIManager.CornerSize)
				{
					tempRect.X = (int)m_vecPosition.X + this.GUIManager.CornerSize / 2;
					tempRect.Width = (int)m_vecSize.X - this.GUIManager.CornerSize ;
				}

				if (m_vecSize.Y >= 2 * GUIManager.CornerSize)
				{
					tempRect.Y = (int)m_vecPosition.Y + this.GUIManager.CornerSize / 2;
					tempRect.Height = (int)m_vecSize.Y - this.GUIManager.CornerSize ;
				}
#if DEBUG
				XeGame.s_vectorRenderer.SetColor(Color.Red);

				XeGame.s_vectorRenderer.DrawLine2D(new Vector3(tempRect.X, tempRect.Y, 0), new Vector3(tempRect.X + tempRect.Width, tempRect.Y, 0));
				XeGame.s_vectorRenderer.DrawLine2D(new Vector3(tempRect.X, tempRect.Y, 0), new Vector3(tempRect.X, tempRect.Y + tempRect.Height, 0));
				XeGame.s_vectorRenderer.DrawLine2D(new Vector3(tempRect.X, tempRect.Y + tempRect.Height, 0), new Vector3(tempRect.X + tempRect.Width, tempRect.Y + tempRect.Height, 0));
				XeGame.s_vectorRenderer.DrawLine2D(new Vector3(tempRect.X + tempRect.Width, tempRect.Y, 0), new Vector3(tempRect.X + tempRect.Width, tempRect.Y + tempRect.Height, 0));
#endif
				if (m_curState == UIState.Over)
					m_font2d.DrawTextBox(m_guiManager.FontName, m_text, m_textAlign, m_textAlignVertical, m_breakStyle, tempRect, GUIManager.CornerSize, m_foreColorHover);
				else
					m_font2d.DrawTextBox(m_guiManager.FontName, m_text, m_textAlign, m_textAlignVertical, m_breakStyle, tempRect, GUIManager.CornerSize, m_foreColor);
			}
		}

		protected virtual void RenderByState(UIState state, GameTime gameTime)
		{
			switch (state)
			{
				case UIState.Out:
					for (int i = 0; i < m_outQuads.Count; i++)
					{
						m_renderer.RenderQuad(m_outQuads[i]);
						m_outQuads[i].Update(gameTime);
					}
					break;

				case UIState.Over:
					for (int i = 0; i < m_overQuads.Count; i++)
					{
						m_renderer.RenderQuad(m_overQuads[i]);
						m_overQuads[i].Update(gameTime);
					}
					break;

				case UIState.Down:
					for (int i = 0; i < m_downQuads.Count; i++)
					{
						m_renderer.RenderQuad(m_downQuads[i]);
						m_downQuads[i].Update(gameTime);
					}
					break;

				case UIState.Focused:
					for (int i = 0; i < m_focusQuads.Count; i++)
					{
						m_renderer.RenderQuad(m_focusQuads[i]);
						m_focusQuads[i].Update(gameTime);
					}
					break;

				case UIState.Transitioning:
					if (m_nextState != UIState.Transitioning)
						RenderByState(m_nextState, gameTime);
					//for (int i = 0; i < m_outQuads.Count; i++)
					//{
					//    m_renderer.RenderQuad(m_outQuads[i]);
					//}
					break;
			}
		}
		#endregion

		#region Load / Unload Graphics
		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
				ResetQuads(true);
			}
		}

		protected override void UnloadGraphicsContent(bool unloadAllContent)
		{
			base.UnloadGraphicsContent(unloadAllContent);
		}

		protected virtual void ResetQuads(bool fullReset)
		{
			m_vecPosition.X = m_x;
			m_vecPosition.Y = m_y;

			m_vecSize.X = m_width;
			m_vecSize.Y = m_height;

			if (fullReset)
			{
				m_lastX = m_x;
				m_lastY = m_y;

				m_outQuads = m_guiManager.CreateControl(m_tag + ".out", new Vector2(m_x, m_y), new Vector2(m_width, m_height));
				m_overQuads = m_guiManager.CreateControl(m_tag + ".over", new Vector2(m_x, m_y), new Vector2(m_width, m_height));
				m_disabledQuads = m_guiManager.CreateControl(m_tag + ".disabled", new Vector2(m_x, m_y), new Vector2(m_width, m_height));
				m_focusQuads = m_guiManager.CreateControl(m_tag + ".focused", new Vector2(m_x, m_y), new Vector2(m_width, m_height));
				m_downQuads = m_guiManager.CreateControl(m_tag + ".down", new Vector2(m_x, m_y), new Vector2(m_width, m_height));

				m_initialPos.X = m_x;
				m_initialPos.Y = m_y;

				m_initialSize.X = m_width;
				m_initialSize.Y = m_height;

				m_initialized = true;
			}
			else
			{
				m_guiManager.SoftReset(m_outQuads, m_vecPosition, m_vecSize);
				m_guiManager.SoftReset(m_overQuads, m_vecPosition, m_vecSize);
				m_guiManager.SoftReset(m_disabledQuads, m_vecPosition, m_vecSize);
				m_guiManager.SoftReset(m_focusQuads, m_vecPosition, m_vecSize);
				m_guiManager.SoftReset(m_downQuads, m_vecPosition, m_vecSize);
			}
		}
		#endregion

		#region Focusing
		/// <summary>
		/// Checks to see if the control can get focus.
		/// </summary>
		/// <param name="x">X Position of the Mouse.</param>
		/// <param name="y">Y Position of the Mouse</param>
		/// <returns>An object that should be focused.</returns>
		public virtual object GetFocus(int x, int y)
		{
			// If we are invisible or disabled, return nothing.
			if (!this.Visible || !this.Enabled)
				return null;

			// If the mouse is within our bounds...
			if (CheckCoords(x, y))
			{
				// Setup a temporary object.
				object _obj = null;

				// Check each child control.
				foreach (UIControl _con in m_controls.MasterList)
				{
					// If the control can get focus...
					object _o = _con.GetFocus(x, y);

					// Set the temporary control to be focused
					// if not null.
					if (_o != null)
						_obj = _o;
				}

				// If there is no child control to be focused,
				// then return ourselves.
				if (_obj == null)
				{
					return this;
				}

				// Otherwise return the child control.
				return _obj;
			}
			else
			{
				object _obj = null;

				// Check each child control.
				foreach (UIControl _con in m_controls.MasterList)
				{
					// Try to get focus and set it if successful
					object _o = _con.GetFocus(x, y);
					if (_o != null)
						_obj = _o;
				}

				// If there is a child control,
				// return it.
				if (_obj != null)
					return _obj;
			}

			// Return nothing if unsuccessful.
			return null;
		}

		/// <summary>
		/// Attempts to give the control focus.
		/// </summary>
		/// <returns>True if the control can focus.</returns>
		public bool Focus()
		{
			// If we are disabled or just can't be focused,
			// return false.
			if (!this.Enabled)
				return false;

			// Set the next state
			//ChangeState(UIState.Focused);

			ChangeDrawOrder(int.MaxValue);

			// Invoke the got focus event
			if (this.GotFocus != null)
				this.GotFocus.Invoke(this);

			// Return true.
			return true;
		}

		/// <summary>
		/// Performs any tasks necessary when the control
		/// loses focus.
		/// </summary>
		public virtual void UnFocus()
		{
			// Set the next state.
			//ChangeState(UIState.Out);

			// Invoke the lost focus event.
			if (this.LostFocus != null)
				this.LostFocus.Invoke(this);
		}

		/// <summary>
		/// Tabs to the next control in the child collection.
		/// </summary>
		/// <returns></returns>
		public bool TabNext()
		{
			// If the child list doesn't exist yet,
			// return false.
			if (m_controls.TabList.Count <= 0)
				return false;

			// If the index is at a max, reset it to 0
			if (m_curTabIndex == m_controls.TabList.Count - 1)
				m_curTabIndex = 0;
			// Otherwise, increment it.
			else
				m_curTabIndex++;

			// Set the focus and return true.
			m_guiManager.Ebi.Focus = m_controls.TabList[m_curTabIndex];

			return true;
		}

		/// <summary>
		/// Tabs to a previous control in the child collection.
		/// </summary>
		/// <returns>True if tabbing was successful.</returns>
		public bool TabPrev()
		{
			// If the current tab index is at the bottom,
			// try to set it to the top of the list.
			if (m_curTabIndex <= 0)
				m_curTabIndex = (m_controls.TabList.Count > 0) ? m_controls.TabList.Count - 1 : -1;
			// Otherwise, decrement it.
			else
				m_curTabIndex--;

			// If the current index is still -1 (No Child Controls),
			// return.
			if (m_curTabIndex == -1)
				return false;

			// Set the focus and return true.
			m_guiManager.Ebi.Focus = m_controls.TabList[m_curTabIndex];

			return true;
		}
		#endregion

		#region Methods
		protected virtual void ChangeState(UIState newState)
		{
			if (newState == UIState.Transitioning)
				return;

			m_lastState = m_curState;
			m_curState = UIState.Transitioning;
			m_nextState = newState;
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

		protected void ChangeDrawOrder(int drawOrder)
		{
			m_stockDrawOrder = this.DrawOrder;

			this.DrawOrder = drawOrder;
		}
		#endregion

		#region Properties
		public bool LockX
		{
			get { return m_lockX; }
			set { m_lockX = value; }
		}

		public bool LockY
		{
			get { return m_lockY; }
			set { m_lockY = value; }
		}

		public int SnapStep
		{
			get { return m_snappingStep; }
			set { m_snappingStep = value; }
		}

		protected IGUIManager GUIManager
		{
			get { return m_guiManager; }
		}

		protected IEbiService Ebi
		{
			get { return m_ebiService; }
		}

		public string ControlTag
		{
			get { return m_tag; }
			set { m_tag = value; }
		}

		public Color ForeColor
		{
			get { return m_foreColor; }
			set { m_foreColor = value; }
		}

		public Color ForeColorHover
		{
			get { return m_foreColorHover; }
			set { m_foreColorHover = value; }
		}

		/// <summary>
		/// Gets or Sets the parent control.
		/// </summary>
		//[Browsable(false)]
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
						m_parent.ViewportChanged -= this.Parent_ViewportChanged;
					}
					catch (Exception e)
					{
						if (m_reporter != null)
							m_reporter.BroadcastError(this, e.Message, e);
					}
				}

				m_parent = value;

				if (m_parent != null)
				{
					//Console.WriteLine("PARENT ADDED TO " + this.Name + ": " + m_parent.Name);

					m_absX = m_parent.AbsolutePosition.X + m_x;
					m_absY = m_parent.AbsolutePosition.Y + m_y;

					m_parent.Move += new MoveHandler(Parent_Move);
					m_parent.ViewportChanged += new EventHandler(Parent_ViewportChanged);

					//this.TabOrder = Parent.GetTabOrder();
					//m_needsUpdate = true;
					m_updateViewport = true;
					m_needsUpdate = true;
				}
			}
		}

		/// <summary>
		/// Gets or Sets whether or not
		/// pressing the Enter Key simulates
		/// a Click Event.
		/// </summary>
		public bool AcceptsEnter
		{
			get { return m_useEnter; }
			set { m_useEnter = value; }
		}

		/// <summary>
		/// Gets or Sets whether or not
		/// pressing the A Button on the GamePad
		/// simulates a Click Event.
		/// </summary>
		public bool AcceptsA
		{
			get { return m_useAButton; }
			set { m_useAButton = value; }
		}

		//[Browsable(false)]
		public Vector2 AbsolutePosition
		{
			get
			{
				m_absPosition.X = m_absX;
				m_absPosition.Y = m_absY;

				return m_absPosition;
			}
		}

		/// <summary>
		/// The maximum size the control can be.
		/// </summary>
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

		/// <summary>
		/// The minimum size the control can be. By
		/// default this value relates directly to
		/// 2 * GUIManager.CornerSize.
		/// </summary>
		/// <seealso cref="GUIManager.CornerSize"/>
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

		public Vector2 InitialSize
		{
			get { return m_initialSize; }
		}

		public Vector2 InitialPos
		{
			get { return m_initialPos; }
		}

		public Path FocusPath
		{
			get { return m_overPath; }
			set { m_overPath = value; }
		}

		public UIState State
		{
			get { return m_curState; }
		}

		/// <summary>
		/// Gets or Sets the tree level. Note that setting
		/// the tree level recreates the control collection.
		/// </summary>
		public int TreeLevel
		{
			get { return m_treeLevel; }
			set
			{
				m_treeLevel = value;

				m_controls = new ControlCollection(m_renderer, m_font2d, m_treeLevel);
				m_controls.ControlAdded += new ControlCollectionHandler(OnControlAdded);
				m_controls.ControlRemoved += new ControlCollectionHandler(OnControlRemoved);
			}
		}

		/// <summary>
		/// Gets or Sets the collection of child controls.
		/// </summary>
		public ControlCollection Controls
		{
			get { return m_controls; }
			set { m_controls = value; }
		}

		#region State Controls
		/// <summary>
		/// Gets or Sets whether or not the control can be tabbed to.
		/// </summary>
		public bool IsTabable
		{
			get { return m_isTabable; }
			set { m_isTabable = value; }
		}

		/// <summary>
		/// Gets or Sets whether or not the control can be dragged.
		/// </summary>
		public bool IsDraggable
		{
			get { return m_isDraggable; }
			set { m_isDraggable = value; }
		}

		/// <summary>
		/// Gets or Sets whether or not the control can be hovered
		/// over.
		/// </summary>
		public bool IsHoverable
		{
			get { return m_isHoverable; }
			set { m_isHoverable = value; }
		}

		/// <summary>
		/// Gets or Sets whether or not the control can be given focus.
		/// </summary>
		public bool IsFocusable
		{
			get { return m_isFocusable; }
			set { m_isFocusable = value; }
		}

		/// <summary>
		/// Gets or Sets whether or not the text is visible.
		/// </summary>
		public bool IsTextVisible
		{
			get { return m_isTextVisible; }
			set { m_isTextVisible = value; }
		}

		/// <summary>
		/// Gets or Sets whether or not the control can be resized.
		/// </summary>
		public bool IsResizable
		{
			get { return m_isSizeable; }
			set { m_isSizeable = value; }
		}

		/// <summary>
		/// Gets whether or not the control uses one or nine quads.
		/// </summary>
		public bool IsOneQuad
		{
			get { return m_isOneQuad; }
			protected set { m_isOneQuad = value; }
		}
		#endregion

		/// <summary>
		/// Gets or Sets whether or not to snap to a grid when moving.
		/// </summary>
		public bool UseSnapping
		{
			get { return m_isSnapable; }
			set { m_isSnapable = false; }
		}

		/// <summary>
		/// Gets or Sets whether or not to scroll text horizontally.
		/// </summary>
		public bool UseTextScroll
		{
			get { return m_useTextScroll; }
			set { m_useTextScroll = value; }
		}

		/// <summary>
		/// Gets or Sets the order in which the control
		/// is tabbed to.
		/// </summary>
		/// <seealso cref="DrawOrder"/>
		/// <seealso cref="UpdateOrder"/>
		public int TabOrder
		{
			get { return m_tabOrder; }
			set
			{
				if (value != m_tabOrder)
				{
					m_tabOrder = value;

					if (this.TabOrderChanged != null)
						this.TabOrderChanged.Invoke(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or Sets the name of the control.
		/// </summary>
		/// <seealso cref="Tag"/>
		public string Name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		/// <summary>
		/// Gets or Sets the text that the control draws.
		/// </summary>
		public string Text
		{
			get { return m_internalText; }
			set
			{
				m_internalText = value;

				if (m_useHideString)
				{
					m_text = String.Empty;
					for (int i = 0; i < m_internalText.Length; i++)
						m_text += m_hideString;
				}
				else
					m_text = value;
			}
		}

		public string HideString
		{
			get { return m_hideString; }
			set
			{
				m_hideString = value;

				if (!String.IsNullOrEmpty(m_hideString))
				{
					m_useHideString = true;

					m_text = String.Empty;
					for (int i = 0; i < m_internalText.Length; i++)
						m_text += m_hideString;
				}
				else
				{
					m_useHideString = false;

					m_text = m_internalText;
				}
			}
		}

		/// <summary>
		/// Gets or Sets the alignment for the control's text.
		/// </summary>
		/// <seealso cref="Text"/>
		public TextAlign TextAlign
		{
			get { return m_textAlign; }
			set { m_textAlign = value; }
		}

		public TextAlignVertical TextAlignVertical
		{
			get { return m_textAlignVertical; }
			set { m_textAlignVertical = value; }
		}

		public BreakStyle BreakStyle
		{
			get { return m_breakStyle; }
			set { m_breakStyle = value; }
		}

		/// <summary>
		/// Gets or Sets the associated source rectangle
		/// tag.
		/// </summary>
		public string Tag
		{
			get { return m_tag; }
			set { m_tag = value; }
		}

		#region Positioning
		/// <summary>
		/// Gets or Sets the X position of the control.
		/// </summary>
		public float X
		{
			get { return m_x; }
			set
			{
				m_lastX = m_x;
				m_lastY = m_y;

				m_x = value;

				if (m_initialized)
					m_needsUpdate = true;

				m_absX = m_x;

				if (this.Move != null)
					this.Move.Invoke(this);
			}
		}

		/// <summary>
		/// Gets or Sets the Y position of the control.
		/// </summary>
		public float Y
		{
			get { return m_y; }
			set
			{
				m_lastX = m_x;
				m_lastY = m_y;

				m_y = value;

				if (m_initialized)
					m_needsUpdate = true;

				m_absY = m_y;

				if (this.Move != null)
					this.Move.Invoke(this);
			}
		}

		/// <summary>
		/// Gets or Sets the Width of the control.
		/// </summary>
		public float Width
		{
			get { return m_width; }
			set
			{
				m_lastW = m_width;
				m_lastH = m_height;

				m_width = value;

				if (m_initialized)
					m_needsUpdate = true;
			}
		}

		/// <summary>
		/// Gets or Sets the Height of the control.
		/// </summary>
		public float Height
		{
			get { return m_height; }
			set
			{
				m_lastW = m_width;
				m_lastH = m_height;

				m_height = value;

				if (m_initialized)
					m_needsUpdate = true;
			}
		}
		#endregion

		/// <summary>
		/// Gets the Clipping Viewport of the
		/// UIControl.
		/// </summary>
		public Viewport Viewport
		{
			get { return m_viewport; }
		}

		#region Offsets
		/// <summary>
		/// Gets or Sets the offset for where the text should
		/// start drawing from. This is not an absolute position
		/// but rather adding/subtracting a relative amount.
		/// </summary>
		public Vector2 TextOffset
		{
			get { return m_textOffset; }
			set { m_textOffset = value; }
		}

		/// <summary>
		/// Gets or sets the offset for the clipping of child
		/// controls. For instance the value (10,10,10,10) will
		/// bring the clipping viewport in by 10 pixels on each side.
		/// </summary>
		public Rectangle ClippingOffset
		{
			get { return m_viewportOffset; }
			set
			{
				m_viewportOffset = value;
				m_updateViewport = true;
			}
		}
		#endregion
		#endregion
	}

	#region UI State
	public enum UIState
	{
		Out,
		Over,
		Down,
		Focused,
		Disabled,
		Transitioning
	}
	#endregion

	#region Delegates
	public delegate void MoveHandler(object sender);
	public delegate void ResizeHandler(object sender);

	public delegate void DockChangedHandler(object sender);
	public delegate void AnchorChangedHandler(object sender);
	#endregion
}



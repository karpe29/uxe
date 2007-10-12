#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Graphics;

using Xe.Tools;
using Xe.Input;
using Xe.Graphics;
#endregion

namespace Xe.GUI
{
	public partial class GUIManager<T> : Microsoft.Xna.Framework.DrawableGameComponent, IGUIManager where T : QuadRenderer, new()
	{
		#region Static Members
		
		private int m_cornerSize = 5;

		private static float m_alphaFadeSpeed = 0.01f;
		private static int m_globalSnapValue = 5;
		private static bool m_globalSnap = false;

		/// <summary>
		/// Gets or Sets the speed at which controls
		/// become visible or invisible.
		/// </summary>
		public static float AlphaFadeSpeed
		{
			get { return m_alphaFadeSpeed; }
			set { m_alphaFadeSpeed = value; }
		}

		/// <summary>
		/// Gets or Sets whether or not controls should snap
		/// to a grid.
		/// </summary>
		public static bool UseSnapping
		{
			get { return m_globalSnap; }
			set { m_globalSnap = value; }
		}

		/// <summary>
		/// Gets or Sets how much each step
		/// on the grid movement makes.
		/// </summary>
		public static int SnappingStep
		{
			get { return m_globalSnapValue; }
			set { m_globalSnapValue = value; }
		}

		/// <summary>
		/// Gets or Sets the GUI's corner size.
		/// </summary>
		public int CornerSize
		{
			get { return m_cornerSize; }
			set { m_cornerSize = value; }
		}
		#endregion

		#region Control Definition Class
		internal class ControlDef
		{
			//public Rectangle Source = Rectangle.Empty;
			public Dictionary<string, List<Rectangle>> Sources = new Dictionary<string, List<Rectangle>>();
			public string ElementName = string.Empty;
			public ControlStyle Style;
			public int TextureIndex = 0;
		}
		#endregion

		#region Members
		// Provides rendering support.
		private T m_renderer = null;

		// Provides input support.
		private IEbiService m_ebi;

		// Provides font rendering.
		private Font2D m_font2d;

		// Provides a list of controls.
		private ControlCollection m_controls;

		// Provides definitions for the controls to be created from.
		private List<ControlDef> m_definitions = new List<ControlDef>();

		// The global gui texture.
		private Dictionary<int, Texture2D> m_guiTextures = new Dictionary<int, Texture2D>();
		private Dictionary<int, string> m_texSources = new Dictionary<int, string>();

		// The content manager to load the texture with
		private ContentManager m_conManager;

		private string m_fontSource = @"Content\Fonts\" + XeGame.FONT_GUI;
		private string m_fontName = XeGame.FONT_GUI;

		private bool m_initialized = false;

		private int m_curTabIndex = -1;

		private Cursor m_cursor;
		#endregion

		#region Constructor
		public GUIManager(Game game, IEbiService ebi)
			: base(game)
		{
			// Create the content manager
			m_conManager = new ContentManager(this.Game.Services);

			// Reference the rendering core
			m_renderer = new T();

			// Store the Ebi reference.
			m_ebi = ebi;
			AddHandlers();

			// Instance a new font core.
			m_font2d = new Font2D(game);

			// Add the font core to the component list.
			game.Components.Add(m_font2d);

			// Setup the Control Collection
			m_controls = new ControlCollection(m_renderer, m_font2d, 0);
			m_controls.ControlAdded += new ControlCollectionHandler(OnControlAdded);

			// Instantiate the cursor.
			//m_cursor = new Cursor(game);
			//m_cursor.DrawOrder = 36000;

			// Add the cursor to the component list
			//Game.Components.Add(m_cursor);
		}

		protected virtual void AddHandlers()
		{
			m_ebi.TabNext += new TabNextHandler(OnEbiTabNext);
			m_ebi.TabPrev += new TabPrevHandler(OnEbiTabPrev);

			m_ebi.RequestingFocus += new MouseDownHandler(OnEbiRequestingFocus);
		}

		protected void OnEbiRequestingFocus(MouseEventArgs e)
		{
			object _obj = null;

			// Loop through the sorted draw list backwards.
			for (int i = m_controls.DrawList.Count - 1; i >= 0; i--)
			{
				_obj = m_controls.DrawList[i].GetFocus(e.Position.X, e.Position.Y);

				if (_obj != null)
					break;
			}

			System.Console.WriteLine("New Focus: {0}", (_obj != null) ? _obj : "Null");
			if (_obj != null && _obj is IFocusable)
				m_ebi.Focus = (IFocusable)_obj;
			else
				m_ebi.Focus = null;
		}

		protected virtual void OnEbiTabPrev(CancelEventArgs e)
		{
			if (e.Canceled == true)
				return;

			if (m_ebi.Focus == null || !m_ebi.Focus.TabPrev())
			{
				if (m_curTabIndex <= 0)
					m_curTabIndex = (m_controls.TabList.Count > 0) ? m_controls.TabList.Count - 1 : -1;
				else
					m_curTabIndex--;

				if (m_curTabIndex == -1)
					return;

				m_ebi.Focus = m_controls.TabList[m_curTabIndex];

				e.Canceled = true;
			}
		}

		protected virtual void OnEbiTabNext(CancelEventArgs e)
		{
			if (e.Canceled == true)
				return;

			if (m_ebi.Focus == null || !m_ebi.Focus.TabNext())
			{
				if (m_controls.TabList.Count <= 0)
					return;

				if (m_curTabIndex == m_controls.TabList.Count - 1)
					m_curTabIndex = 0;
				else
					m_curTabIndex++;

				m_ebi.Focus = m_controls.TabList[m_curTabIndex];
			}
		}

		protected virtual void OnControlAdded(UIControl control)
		{
			if (m_initialized)
				control.Initialize();
		}

		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			// Initialize the rendering core
			m_renderer.Initialize(this.Game, this.GraphicsDevice);

			// Initialize the font core
			m_font2d.Initialize();

			m_initialized = true;

			for (int i = 0; i < m_controls.MasterList.Count; i++)
				m_controls.MasterList[i].Initialize();
		}
		#endregion

		#region Update and Draw
		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			// Update all the controls.
			m_controls.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			// Draw all the controls.
			m_controls.Draw(gameTime);
		}
		#endregion

		#region Load / Unload Graphics
		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			// Load the Font2D content.
			m_font2d.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
				// Load the content manager
				if (m_conManager == null)
					m_conManager = new ContentManager(this.Game.Services);

				// Load the textures
				foreach (int srcIndex in m_texSources.Keys)
					m_guiTextures[srcIndex] = m_conManager.Load<Texture2D>(m_texSources[srcIndex]);

				//m_guiTexture = m_conManager.Load<Texture2D>(m_texSource);

				// Set the cursor texture
				//m_cursor.BaseTexture = m_guiTextures[0];

				// Load the main GUI font.
				m_font2d.LoadFont(m_fontSource, m_fontName);
			}
		}

		protected override void UnloadGraphicsContent(bool unloadAllContent)
		{
			base.UnloadGraphicsContent(unloadAllContent);

			// Unload the Font2D content.
			m_font2d.UnloadGraphicsContent(unloadAllContent);

			if (unloadAllContent)
			{
				// Unload the main GUI font.
				m_font2d.UnloadFont(m_fontName);

				// Unload the content manager
				if (m_conManager != null)
				{
					m_conManager.Unload();
					m_conManager.Dispose();
					m_conManager = null;
				}

				// Unload the texture
				if (m_guiTextures != null)
				{
					foreach (int _tIndex in m_guiTextures.Keys)
					{
						m_guiTextures[_tIndex].Dispose();
						m_guiTextures[_tIndex] = null;
						m_guiTextures.Remove(_tIndex);
					}
				}
			}
		}
		#endregion

		#region Load / Unload Font Methods
		/// <summary>
		/// Loads a font into the font registry.
		/// </summary>
		/// <param name="assetName">The name of the font asset.</param>
		/// <param name="fontName">The name of the font itself.</param>
		public void LoadFont(string assetName, string fontName)
		{
			m_font2d.LoadFont(assetName, fontName);
		}

		/// <summary>
		/// Unloads a font from the registry.
		/// </summary>
		/// <param name="fontName">The name of the font to unload.</param>
		public void UnloadFont(string fontName)
		{
			m_font2d.UnloadFont(fontName);
		}
		#endregion

		#region Creating Controls
		#region Soft Resets
		/// <summary>
		/// Does a soft reset on the quad lists, only resizing them
		/// and not generating any garbage.
		/// </summary>
		/// <param name="guiQuads">The list of quads to affect.</param>
		/// <param name="topLeft">The top left of the control.</param>
		/// <param name="size">The size of the control.</param>
		public void SoftReset(List<GUIQuad> guiQuads, Vector2 topLeft, Vector2 size)
		{
			Viewport vp = GraphicsDevice.Viewport;

			SoftReset(guiQuads, topLeft, size, vp);
		}

		/// <summary>
		/// Does a soft reset on the quad lists, only resizing them
		/// and not generating any garbage.
		/// </summary>
		/// <param name="guiQuads">The list of quads to affect.</param>
		/// <param name="topLeft">The top left of the control.</param>
		/// <param name="size">The size of the control.</param>
		/// <param name="vp">The viewport the control lies within.</param>
		public void SoftReset(List<GUIQuad> guiQuads, Vector2 topLeft, Vector2 size, Viewport vp)
		{
			// If it is a one quad control, simply reset
			// the main quad
			if (guiQuads.Count == 1)
			{
				guiQuads[0].X = topLeft.X;
				guiQuads[0].Y = topLeft.Y;
				guiQuads[0].Width = size.X;
				guiQuads[0].Height = size.Y;
			}
			else
			{
				// Loop through each quad in the control
				for (int i = 0; i < guiQuads.Count; i++)
				{
					switch (guiQuads[i].QuadType)
					{
						case GUIQuadType.TopLeft:
							guiQuads[i].X = topLeft.X;
							guiQuads[i].Y = topLeft.Y;
							guiQuads[i].Width = m_cornerSize;
							guiQuads[i].Height = m_cornerSize;
							break;
						case GUIQuadType.TopRight:
							guiQuads[i].X = topLeft.X + size.X - m_cornerSize;
							guiQuads[i].Y = topLeft.Y;
							guiQuads[i].Width = m_cornerSize;
							guiQuads[i].Height = m_cornerSize;
							break;
						case GUIQuadType.BottomRight:
							guiQuads[i].X = topLeft.X + size.X - m_cornerSize;
							guiQuads[i].Y = topLeft.Y + size.Y - m_cornerSize;
							guiQuads[i].Width = m_cornerSize;
							guiQuads[i].Height = m_cornerSize;
							break;
						case GUIQuadType.BottomLeft:
							guiQuads[i].X = topLeft.X;
							guiQuads[i].Y = topLeft.Y + size.Y - m_cornerSize;
							guiQuads[i].Width = m_cornerSize;
							guiQuads[i].Height = m_cornerSize;
							break;
						case GUIQuadType.TopCenter:
							guiQuads[i].X = topLeft.X + m_cornerSize;
							guiQuads[i].Y = topLeft.Y;
							guiQuads[i].Width = size.X - (2 * m_cornerSize);
							guiQuads[i].Height = m_cornerSize;
							break;
						case GUIQuadType.MiddleRight:
							guiQuads[i].X = topLeft.X + size.X - m_cornerSize;
							guiQuads[i].Y = topLeft.Y + m_cornerSize;
							guiQuads[i].Width = m_cornerSize;
							guiQuads[i].Height = size.Y - (2 * m_cornerSize);
							break;
						case GUIQuadType.BottomCenter:
							guiQuads[i].X = topLeft.X + m_cornerSize;
							guiQuads[i].Y = topLeft.Y + size.Y - m_cornerSize;
							guiQuads[i].Width = size.X - (2 * m_cornerSize);
							guiQuads[i].Height = m_cornerSize;
							break;
						case GUIQuadType.MiddleLeft:
							guiQuads[i].X = topLeft.X;
							guiQuads[i].Y = topLeft.Y + m_cornerSize;
							guiQuads[i].Width = m_cornerSize;
							guiQuads[i].Height = size.Y - (2 * m_cornerSize);
							break;
						case GUIQuadType.MiddleCenter:
							guiQuads[i].X = topLeft.X + m_cornerSize;
							guiQuads[i].Y = topLeft.Y + m_cornerSize;
							guiQuads[i].Width = size.X - (2 * m_cornerSize);
							guiQuads[i].Height = size.Y - (2 * m_cornerSize);
							break;
					}
				}
			}
		}
		#endregion

		#region Public Creation Methods
		public List<Rectangle> GetSourceRects(string tag)
		{
			string _controlTag = tag.Split('.')[0];
			string _sourceTag = tag.Split('.')[1];

			for (int i = 0; i < m_definitions.Count; i++)
			{
				if (m_definitions[i].ElementName == _controlTag)
					return m_definitions[i].Sources[_sourceTag];
			}

			return null;
		}

		public List<GUIQuad> CreateControl(string tag, Vector2 topLeft, Vector2 size)
		{
			Viewport vp = GraphicsDevice.Viewport;

			return CreateControl(tag, topLeft, size, vp);
		}

		public List<GUIQuad> CreateControl(string tag, Vector2 topLeft, Vector2 size, Viewport vp)
		{
			string _controlTag = tag.Split('.')[0];
			string _sourceTag = tag.Split('.')[1];

			for (int i = 0; i < m_definitions.Count; i++)
			{
				if (m_definitions[i].ElementName == _controlTag)
				{
					return (m_definitions[i].Style == ControlStyle.One) ? CreateQuad(m_definitions[i], _sourceTag, topLeft, size, vp, GUIQuadType.None) :
																		  CreateNineQuad(m_definitions[i], _sourceTag, topLeft, size, vp);
				}
			}

			//Console.WriteLine("Cannot find element name in definition list. The tag, {0}, does not exist.", _controlTag);
			return null;
		}
		#endregion

		#region Private Creation Methods
		private List<GUIQuad> CreateQuad(ControlDef definition, string sourceTag, Vector2 topLeft, Vector2 size, Viewport vp, GUIQuadType type)
		{
			try
			{

				float vp_width = vp.Width;
				float vp_height = vp.Height;

				GUIQuad _screenQuad = new GUIQuad(this.Game, topLeft.X, topLeft.Y, size.X, size.Y);
				_screenQuad.QuadType = type;
				_screenQuad.AddSourceSet(definition.Sources[sourceTag]);

				_screenQuad.Texture = m_guiTextures[definition.TextureIndex];

				_screenQuad.Initialize();

				List<GUIQuad> _tempList = new List<GUIQuad>();

				_tempList.Add(_screenQuad);

				return _tempList;
			}
			catch (Exception e)
			{
				System.Console.WriteLine(definition.ElementName + "." + sourceTag + " could not be found.");
				System.Console.WriteLine(e.Message);
				return null;
			}
		}

		private GUIQuad CreateQuad(ControlDef definition, string sourceTag, Vector2 topLeft, Vector2 size, List<Rectangle> sources, Viewport vp, GUIQuadType type)
		{
			try
			{
				float vp_width = vp.Width;
				float vp_height = vp.Height;

				GUIQuad _screenQuad = new GUIQuad(this.Game, topLeft.X, topLeft.Y, size.X, size.Y);
				_screenQuad.QuadType = type;
				_screenQuad.AddSourceSet(sources);

				_screenQuad.Texture = m_guiTextures[definition.TextureIndex];

				_screenQuad.Initialize();

				return _screenQuad;
			}
			catch
			{
				//Console.WriteLine(definition.ElementName + "." + sourceTag + " could not be found.");
				return null;
			}
		}

		private List<GUIQuad> CreateNineQuad(ControlDef definition, string sourceTag, Vector2 topLeft, Vector2 size, Viewport vp)
		{
			//Console.WriteLine("Creating Nine Quad");
			List<GUIQuad> _tempList = new List<GUIQuad>();
			List<Rectangle> _sources = new List<Rectangle>();

			#region Top Left Corner
			Vector2 _c1TL = new Vector2(topLeft.X, topLeft.Y);
			Vector2 _c1BR = new Vector2(m_cornerSize, m_cornerSize);

			foreach (Rectangle _rect in definition.Sources[sourceTag])
				_sources.Add(new Rectangle(_rect.X, _rect.Y, m_cornerSize, m_cornerSize));
			_tempList.Add(CreateQuad(definition, sourceTag, _c1TL, _c1BR, _sources, vp, GUIQuadType.TopLeft));
			#endregion

			#region Top Right Corner
			Vector2 _c2TL = new Vector2(topLeft.X + size.X - m_cornerSize, topLeft.Y);
			Vector2 _c2BR = new Vector2(m_cornerSize, m_cornerSize);

			_sources.Clear();
			foreach (Rectangle _rect in definition.Sources[sourceTag])
				_sources.Add(new Rectangle(_rect.X + _rect.Width - m_cornerSize, _rect.Y, m_cornerSize, m_cornerSize));
			_tempList.Add(CreateQuad(definition, sourceTag, _c2TL, _c2BR, _sources, vp, GUIQuadType.TopRight));
			#endregion

			#region Bottom Right Corner
			Vector2 _c3TL = new Vector2(topLeft.X + size.X - m_cornerSize, topLeft.Y + size.Y - m_cornerSize);
			Vector2 _c3BR = new Vector2(m_cornerSize, m_cornerSize);

			_sources.Clear();
			foreach (Rectangle _rect in definition.Sources[sourceTag])
				_sources.Add(new Rectangle(_rect.X + _rect.Width - m_cornerSize, _rect.Y + _rect.Height - m_cornerSize, m_cornerSize, m_cornerSize));
			_tempList.Add(CreateQuad(definition, sourceTag, _c3TL, _c3BR, _sources, vp, GUIQuadType.BottomRight));
			#endregion

			#region Bottom Left Corner
			Vector2 _c4TL = new Vector2(topLeft.X, topLeft.Y + size.Y - m_cornerSize);
			Vector2 _c4BR = new Vector2(m_cornerSize, m_cornerSize);

			_sources.Clear();
			foreach (Rectangle _rect in definition.Sources[sourceTag])
				_sources.Add(new Rectangle(_rect.X, _rect.Y + _rect.Height - m_cornerSize, m_cornerSize, m_cornerSize));
			_tempList.Add(CreateQuad(definition, sourceTag, _c4TL, _c4BR, _sources, vp, GUIQuadType.BottomLeft));
			#endregion

			#region Top Side
			Vector2 _s1TL = new Vector2(topLeft.X + m_cornerSize, topLeft.Y);
			Vector2 _s1BR = new Vector2(size.X - (2 * m_cornerSize), m_cornerSize);

			_sources.Clear();
			foreach (Rectangle _rect in definition.Sources[sourceTag])
				_sources.Add(new Rectangle(_rect.X + m_cornerSize, _rect.Y, _rect.Width - (2 * m_cornerSize), m_cornerSize));
			_tempList.Add(CreateQuad(definition, sourceTag, _s1TL, _s1BR, _sources, vp, GUIQuadType.TopCenter));
			#endregion

			#region Right Side
			Vector2 _s2TL = new Vector2(topLeft.X + size.X - m_cornerSize, topLeft.Y + m_cornerSize);
			Vector2 _s2BR = new Vector2(m_cornerSize, size.Y - (2 * m_cornerSize));

			_sources.Clear();
			foreach (Rectangle _rect in definition.Sources[sourceTag])
				_sources.Add(new Rectangle(_rect.X + _rect.Width - m_cornerSize, _rect.Y + m_cornerSize, m_cornerSize, _rect.Height - (2 * m_cornerSize)));
			_tempList.Add(CreateQuad(definition, sourceTag, _s2TL, _s2BR, _sources, vp, GUIQuadType.MiddleRight));
			#endregion

			#region Bottom Side
			Vector2 _s3TL = new Vector2(topLeft.X + m_cornerSize, topLeft.Y + size.Y - m_cornerSize);
			Vector2 _s3BR = new Vector2(size.X - (2 * m_cornerSize), m_cornerSize);

			_sources.Clear();
			foreach (Rectangle _rect in definition.Sources[sourceTag])
				_sources.Add(new Rectangle(_rect.X + m_cornerSize, _rect.Y + _rect.Height - m_cornerSize, _rect.Width - (2 * m_cornerSize), m_cornerSize));
			_tempList.Add(CreateQuad(definition, sourceTag, _s3TL, _s3BR, _sources, vp, GUIQuadType.BottomCenter));
			#endregion

			#region Left Side
			Vector2 _s4TL = new Vector2(topLeft.X, topLeft.Y + m_cornerSize);
			Vector2 _s4BR = new Vector2(m_cornerSize, size.Y - (2 * m_cornerSize));

			_sources.Clear();
			foreach (Rectangle _rect in definition.Sources[sourceTag])
				_sources.Add(new Rectangle(_rect.X, _rect.Y + m_cornerSize, m_cornerSize, _rect.Height - (2 * m_cornerSize)));
			_tempList.Add(CreateQuad(definition, sourceTag, _s4TL, _s4BR, _sources, vp, GUIQuadType.MiddleLeft));
			#endregion

			#region Background
			Vector2 _bgTL = new Vector2(topLeft.X + m_cornerSize, topLeft.Y + m_cornerSize);
			Vector2 _bgBR = new Vector2(size.X - (2 * m_cornerSize), size.Y - (2 * m_cornerSize));

			_sources.Clear();
			foreach (Rectangle _rect in definition.Sources[sourceTag])
				_sources.Add(new Rectangle(_rect.X + m_cornerSize, _rect.Y + m_cornerSize, _rect.Width - (2 * m_cornerSize), _rect.Height - (2 * m_cornerSize)));
			_tempList.Add(CreateQuad(definition, sourceTag, _bgTL, _bgBR, _sources, vp, GUIQuadType.MiddleCenter));
			#endregion

			return _tempList;
		}
		#endregion
		#endregion

		#region Loading Settings
		/// <summary>
		/// Loads settings from an XML node.
		/// </summary>
		/// <param name="node">A valid Thrust_GUI XML Node.</param>
		public void LoadSettings(XmlNode node)
		{
			if (node.Name.Equals("Xe_GUI"))
			{
				foreach (XmlNode _child in node.ChildNodes)
				{
					if (_child.Name.Equals("Texture"))
					{
						int _index = 0;
						Parser.ParseInt(XmlLoader.GetAttribute(_child, "ID"), out _index);
						m_texSources[_index] = _child.ChildNodes[0].Value;

						//LoadGraphicsContent(true);
					}
					else if (_child.Name.Equals("CornerSize"))
						m_cornerSize = int.Parse(_child.ChildNodes[0].Value);
					else if (_child.Name.Equals("GUI_Elements"))
						LoadSettings(_child);
				}
			}
			else if (node.Name.Equals("GUI_Elements"))
			{
				foreach (XmlNode _child in node.ChildNodes)
				{
					if (_child.Name.Equals("Element"))
					{
						ControlDef _def = new ControlDef();
						_def.ElementName = (_child.Attributes["name"] != null) ? _child.Attributes["name"].Value : "EMPTY";
						Parser.ParseInt(XmlLoader.GetAttribute(_child, "TextureID"), out _def.TextureIndex);

						foreach (XmlNode _source in _child.ChildNodes)
						{

							if (_source.Name.Equals("Source"))
							{
								string _tag = String.Empty;

								if (_source.Attributes["name"] != null)
								{
									_tag = _source.Attributes["name"].Value;

									//Rectangle _temp = new Rectangle();
									//_temp.X = int.Parse(_source.ChildNodes[0].ChildNodes[0].Value);
									//_temp.Y = int.Parse(_source.ChildNodes[1].ChildNodes[0].Value);
									//_temp.Width = int.Parse(_source.ChildNodes[2].ChildNodes[0].Value);
									//_temp.Height = int.Parse(_source.ChildNodes[3].ChildNodes[0].Value);

									_def.Sources[_tag] = new List<Rectangle>();

									foreach (XmlNode _rectangle in _source.ChildNodes)
										_def.Sources[_tag].Add(XmlLoader.BuildRectangle(_rectangle));
								}

								if (_source.Attributes["style"] != null && !String.IsNullOrEmpty(_tag))
								{
									_def.Style = (_source.Attributes["style"].Value.ToLower() == "9quad") ? ControlStyle.Nine : ControlStyle.One;
								}
							}
						}

						m_definitions.Add(_def);
					}
				}
			}
		}

		/// <summary>
		/// Load settings from a valid XML File.
		/// </summary>
		/// <param name="xmlFile">The XML file to load from.</param>
		public void LoadSettings(string xmlFile)
		{
			XmlDocument _doc = new XmlDocument();
			_doc.Load(xmlFile);

			XmlNode _node = _doc.FirstChild;
			if (_node.Name.Equals("xml"))
				_node = _node.NextSibling;

			LoadSettings(_node);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or Sets the GUI's main font source.
		/// </summary>
		public string FontSource
		{
			get { return m_fontSource; }
			set { m_fontSource = value; }
		}

		/// <summary>
		/// Gets or Sets the GUI's main font name.
		/// </summary>
		public string FontName
		{
			get { return m_fontName; }
			set { m_fontName = value; }
		}

		/// <summary>
		/// Gets or Sets the collection of UIControls.
		/// </summary>
		public ControlCollection Controls
		{
			get { return m_controls; }
			set { m_controls = value; }
		}

		/// <summary>
		/// Gets the reference to the event
		/// based input service.
		/// </summary>
		public IEbiService Ebi
		{
			get { return m_ebi; }
			set { m_ebi = value; }
		}

		/// <summary>
		/// Gets the reference to the rendering
		/// service.
		/// </summary>
		public T Renderer
		{
			get { return m_renderer; }
		}

		/// <summary>
		/// Gets the reference to the font
		/// rendering class.
		/// </summary>
		public Font2D Font2D
		{
			get { return m_font2d; }
		}

		/// <summary>
		/// Gets or Sets the GUI Texture.
		/// </summary>
		public Dictionary<int, Texture2D> GUITextures
		{
			get { return m_guiTextures; }
			set { m_guiTextures = value; }
		}

		public QuadRenderer QuadRenderer
		{
			get { return m_renderer; }
			set { m_renderer = (T)value; }
		}
		#endregion
	}

	public interface IGUIManager
	{
		Font2D Font2D { get; }
		QuadRenderer QuadRenderer { get; set; }
		IEbiService Ebi { get; set; }

		string FontName { get; set; }

		int CornerSize { get; set; }

		ControlCollection Controls { get; set; }

		Dictionary<int, Texture2D> GUITextures { get; set; }

		List<GUIQuad> CreateControl(string tag, Vector2 topLeft, Vector2 size);
		List<GUIQuad> CreateControl(string tag, Vector2 topLeft, Vector2 size, Viewport vp);
		void SoftReset(List<GUIQuad> quads, Vector2 topLeft, Vector2 size);
		void SoftReset(List<GUIQuad> quads, Vector2 topLeft, Vector2 size, Viewport vp);
	}

	public enum ControlStyle
	{
		One,
		Nine
	}
}



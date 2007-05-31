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
using System.Xml;
using System.Collections.Generic;

using XeFramework;
using XeFramework.Input;
using XeFramework.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XeFramework.GUI
{
    public partial class GUIManager : Microsoft.Xna.Framework.DrawableGameComponent, IService, IGUIManagerService
    {
        internal class ControlDef
        {
            //public Rectangle Source = Rectangle.Empty;
            public Dictionary<string, Rectangle> Sources = new Dictionary<string,Rectangle>();
            public string ElementName = string.Empty;
        }

        #region Members
        private ContentManager m_conManager;
        private Texture2D m_texture;
        private string m_texSource = String.Empty;

        private int m_cornerSize = 5;

        private List<ControlDef> m_definitions = new List<ControlDef>();
        private List<UIControl> m_controls = new List<UIControl>();

        protected IReporterService m_reporter;
        protected IEbiService m_ebi;

        private int m_curTab = 0;
        #endregion

        #region Constructor and Initialization
		public GUIManager(Game game)
            : base(game)
        {
            if (game != null)
                game.Services.AddService(typeof(IGUIManagerService), this);
        }

        ~GUIManager()
        {
            RemoveHandlers();
        }

        public override void Initialize()
        {
            base.Initialize();

            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));

            m_ebi = (IEbiService)this.Game.Services.GetService(typeof(IEbiService));

            AddHandlers();
        }
        #endregion

        #region Adding / Removing Event Handlers
        protected virtual void AddHandlers()
        {
            if (m_ebi != null)
            {
                m_ebi.RequestingFocus += new MouseDownHandler(Ebi_RequestingFocus);
                m_ebi.KeyDown += new KeyDownHandler(Ebi_KeyDown);
            }

            if (m_reporter != null)
            {
                m_reporter.ErrorReported += new ErrorReportedHandler(OnErrorReported);
                m_reporter.WarningReported += new WarningReportedHandler(OnWarningReported);
                m_reporter.MessageReported += new MessageReportedHandler(OnMessageReported);
            }
        }

        protected virtual void RemoveHandlers()
        {
            if (m_ebi != null)
            {
                m_ebi.RequestingFocus -= this.Ebi_RequestingFocus;
                m_ebi.KeyDown -= this.Ebi_KeyDown;
            }

            if (m_reporter != null)
            {
                m_reporter.ErrorReported -= this.OnErrorReported;
                m_reporter.WarningReported -= this.OnWarningReported;
                m_reporter.MessageReported -= this.OnMessageReported;
            }
        }
        #endregion

        #region Event Handlers
        protected virtual void OnMessageReported(Message msg)
        {
        }

        protected virtual void OnWarningReported(Message msg)
        {
        }

        protected virtual void OnErrorReported(Message msg, Exception except)
        {
        }

        private void Ebi_RequestingFocus(MouseEventArgs args)
        {
            object _obj = null;

            // Loop through the controls
            foreach (UIControl _control in m_controls)
            {
                // Let the controls do a recursive tree search
                _obj = _control.GetFocus(args.Position.X, args.Position.Y);

                // If we got an object, exit out of the loop
                if (_obj != null)
                    break;
            }

            // Set focus!
            m_ebi.SetFocus(_obj);

            /*if (_obj != null)
                System.Console.WriteLine("CONTROL_FOCUS: " + ((UIControl)_obj).Name);*/
        }

        private void Ebi_KeyDown(object focus, KeyEventArgs k)
        {
            if (m_ebi.GetFocus() == null)
                if (k.Key == Microsoft.Xna.Framework.Input.Keys.Tab)
                    TabNextControl();
        }
        #endregion

        #region Loading Settings
        public void LoadSettings(XmlNode node)
        {
            if (node.Name.Equals("Xna5D_GUI"))
            {
                foreach (XmlNode _child in node.ChildNodes)
                {
                    if (_child.Name.Equals("Texture"))
                    {
                        m_texSource = _child.ChildNodes[0].Value;

                        LoadGraphicsContent(true);
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

                        foreach (XmlNode _source in _child.ChildNodes)
                        {

                            if (_source.Name.Equals("Source"))
                            {
                                string _tag = String.Empty;

                                if (_source.Attributes["name"] != null)
                                {
                                    _tag = _source.Attributes["name"].Value;

                                    Rectangle _temp = new Rectangle();
                                    _temp.X = int.Parse(_source.ChildNodes[0].ChildNodes[0].Value);
                                    _temp.Y = int.Parse(_source.ChildNodes[1].ChildNodes[0].Value);
                                    _temp.Width = int.Parse(_source.ChildNodes[2].ChildNodes[0].Value);
                                    _temp.Height = int.Parse(_source.ChildNodes[3].ChildNodes[0].Value);

                                    _def.Sources[_tag] = _temp;
                                }
                            }
                        }

                        m_definitions.Add(_def);
                    }
                }
            }
        }

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

        #region Load / Unload Graphics
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                try
                {
                    if (String.IsNullOrEmpty(m_texSource))
                        return;

                    m_conManager = new ContentManager(this.Game.Services);

                    m_texture = m_conManager.Load<Texture2D>(m_texSource);
                }
                catch(Exception e)
                {
                    if (m_reporter != null)
                        m_reporter.BroadcastError(this, e.Message, e);
                }
            }
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if (unloadAllContent)
            {
                if (m_conManager != null)
                {
                    m_conManager.Unload();
                    m_conManager.Dispose();
                }
                m_conManager = null;

                if (m_texture != null)
                    m_texture.Dispose();
                m_texture = null;
            }
        }
        #endregion

		#region Draw / Update
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			foreach (UIControl c in m_controls)
			{
				c.Update(gameTime);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			foreach (UIControl c in m_controls)
			{
				c.Draw(gameTime);
			}

			base.Draw(gameTime);
		}
		#endregion

		#region IGUIManagerService Members
		public List<QuadBase> CreateControl(string tag, Rectangle destination)
        {
            string _controlTag = tag.Split('.')[0];
            string _sourceTag = tag.Split('.')[1];

            foreach (ControlDef _def in m_definitions)
                if (_def.ElementName.Equals(_controlTag))
                    return CreateControl(_def.Sources[_sourceTag], destination);

            return null;
        }

        public List<QuadBase> CreateControl(Rectangle source, Rectangle size)
        {
            List<QuadBase> m_sprites = new List<QuadBase>();
            QuadBase[] m_bg = new QuadBase[9];

            #region Corners
            // Top Left
            m_bg[0] = new QuadBase();
            m_bg[0].Source = new Rectangle(source.X, source.Y, m_cornerSize, m_cornerSize);
            m_bg[0].Destination = new Rectangle(size.X, size.Y, m_cornerSize, m_cornerSize);
            m_sprites.Add(m_bg[0]);

            // Bottom Left
            m_bg[1] = new QuadBase();
            m_bg[1].Source = new Rectangle(source.X, source.Bottom - m_cornerSize, m_cornerSize, m_cornerSize);
            m_bg[1].Destination = new Rectangle(size.X, size.Bottom - m_cornerSize, m_cornerSize, m_cornerSize);
            m_sprites.Add(m_bg[1]);

            // Bottom Right
            m_bg[2] = new QuadBase();
            m_bg[2].Source = new Rectangle(source.Right - m_cornerSize, source.Bottom - m_cornerSize, m_cornerSize, m_cornerSize);
            m_bg[2].Destination = new Rectangle(size.Right - m_cornerSize, size.Bottom - m_cornerSize, m_cornerSize, m_cornerSize);
            m_sprites.Add(m_bg[2]);

            // Top Right
            m_bg[3] = new QuadBase();
            m_bg[3].Source = new Rectangle(source.Right - m_cornerSize, source.Y, m_cornerSize, m_cornerSize);
            m_bg[3].Destination = new Rectangle(size.Right - m_cornerSize, size.Y, m_cornerSize, m_cornerSize);
            m_sprites.Add(m_bg[3]);
            #endregion

            #region Sides
            // Top
            m_bg[4] = new QuadBase();
            m_bg[4].Source = new Rectangle(source.X + m_cornerSize, source.Y, source.Width - (2 * m_cornerSize), m_cornerSize);
            m_bg[4].Destination = new Rectangle(size.X + m_cornerSize, size.Y, size.Width - (2 * m_cornerSize), m_cornerSize);
            m_sprites.Add(m_bg[4]);

            // Bottom
            m_bg[5] = new QuadBase();
            m_bg[5].Source = new Rectangle(source.X + m_cornerSize, source.Bottom - m_cornerSize, source.Width - (2 * m_cornerSize), m_cornerSize);
            m_bg[5].Destination = new Rectangle(size.X + m_cornerSize, size.Bottom - m_cornerSize, size.Width - (2 * m_cornerSize), m_cornerSize);
            m_sprites.Add(m_bg[5]);

            // Left
            m_bg[6] = new QuadBase();
            m_bg[6].Source = new Rectangle(source.X, source.Y + m_cornerSize, m_cornerSize, source.Height - (3 * m_cornerSize));
            m_bg[6].Destination = new Rectangle(size.X, size.Y + m_cornerSize, m_cornerSize, size.Height - (2 * m_cornerSize));
            m_sprites.Add(m_bg[6]);

            // Right
            m_bg[7] = new QuadBase();
            m_bg[7].Source = new Rectangle(source.Right - m_cornerSize, source.Y + m_cornerSize, m_cornerSize, source.Height - (3 * m_cornerSize));
            m_bg[7].Destination = new Rectangle(size.Right - m_cornerSize, size.Y + m_cornerSize, m_cornerSize, size.Height - (2 * m_cornerSize));
            m_sprites.Add(m_bg[7]);
            #endregion

            // BG
            m_bg[8] = new QuadBase();
            m_bg[8].Source = new Rectangle(source.X + m_cornerSize, source.Y + m_cornerSize, source.Width - (2 * m_cornerSize), source.Height - (2 * m_cornerSize)); ;
            m_bg[8].Destination = new Rectangle(size.X + m_cornerSize, size.Y + m_cornerSize, size.Width - (2 * m_cornerSize), size.Height - (2 * m_cornerSize));
            m_sprites.Add(m_bg[8]);

            return m_sprites;
        }

        public QuadBase CreateBox(string tag, Rectangle destination)
        {
            string _controlTag = tag.Split('.')[0];
            string _sourceTag = tag.Split('.')[1];

            foreach (ControlDef _def in m_definitions)
                if (_def.ElementName.Equals(_controlTag))
                    return CreateBox(_def.Sources[_sourceTag], destination);

            return null;
        }

        public QuadBase CreateBox(Rectangle source, Rectangle size)
        {
            QuadBase _base = new QuadBase();
            _base.Source = source;
            _base.Destination = size;
            _base.Texture = m_texture;
            _base.TextureSource = m_texSource;
            _base.Color = Color.White;

            return _base;
        }

        public void AddControl(UIControl control)
        {
			// don't add it twice
			if (m_controls.IndexOf(control) == -1)
			{
				control.Initialize();
				control.DrawOrder = this.DrawOrder;

				m_controls.Add(control);
				control.TabOrder = GetTabOrder();
			}
        }

        public void RemoveControl(UIControl control)
        {
            m_controls.Remove(control);
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
            int _max = -1;
            foreach (UIControl _control in m_controls)
            {
                if (_control.TabOrder > _max)
                    _max = _control.TabOrder;
            }

            return _max + 1;
        }
        #endregion

        #region Properties
        public string ID
        {
            get
            {
                return "Xna5D_GUIManager";
            }
        }

        public int CornerSize
        {
            get
            {
                return m_cornerSize;
            }
            set
            {
                m_cornerSize = value;
            }
        }

        public Texture2D GUITexture
        {
            get
            {
                return m_texture;
            }
            set
            {
                m_texture = value;
            }
        }

		public ContentManager ContentManager
		{
			get
			{
				return m_conManager;
			}
			set
			{
				m_conManager = value;
			}
		}
        #endregion
    }

    public interface IGUIManagerService
    {
        List<QuadBase> CreateControl(string tag, Rectangle destination);
        List<QuadBase> CreateControl(Rectangle source, Rectangle size);

        QuadBase CreateBox(string tag, Rectangle destination);
        QuadBase CreateBox(Rectangle source, Rectangle size);

        void AddControl(UIControl control);
        void RemoveControl(UIControl control);

        int CornerSize { get; }
        Texture2D GUITexture { get; set; }

        void LoadSettings(string xmlFile);
    }

    internal class CompareTab : IComparer<UIControl>
    {
        #region IComparer<UIControl> Members

        public int Compare(UIControl x, UIControl y)
        {
            return (x.TabOrder < y.TabOrder) ? -1 : 1;
        }

        #endregion
    }
}



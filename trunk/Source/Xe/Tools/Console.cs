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

using Xe.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Xe.Input;
#endregion

namespace Xe.Tools
{
    public partial class Console : Microsoft.Xna.Framework.DrawableGameComponent, IService, IConsoleService, IFocusable
    {
        #region History Class
        protected class HistoryString
        {
            public string String;
            public bool IsMsg = false;

            public HistoryString()
            {
            }
        }
        #endregion

        #region Members
        private SpriteBatch m_spriteBatch;
        private Texture2D m_texture;
        private Rectangle m_drawRect = new Rectangle();
        private int m_y = -20;

        protected IEbiService m_ebis;
        protected IReporterService m_reporter;

        protected bool m_isActive;

        protected SpriteFont m_font;
        protected GraphicsDevice m_device;
        protected ContentManager m_conManager;

        protected string m_prefix = "> ";
        protected string m_suffix = "_";
        protected string m_input = "";

        protected List<HistoryString> m_history = new List<HistoryString>();
        protected int m_curIndex = 0;

        public event NewMessageHandler NewMessage;
        #endregion

        #region Constructor & Initialization
		public Console(Game game, ContentManager contentManager)
            : base(game)
        {
            this.NewMessage += new NewMessageHandler(OnNewMessage);

			m_conManager = contentManager;

            this.Visible = true;
            this.Enabled = true;

            if (game != null)
                game.Services.AddService(typeof(IConsoleService), this);
        }

        public override void Initialize()
        {
            base.Initialize();

            m_ebis = (IEbiService)this.Game.Services.GetService(typeof(IEbiService));
            if (m_ebis != null)
                m_ebis.KeyDown += new KeyDownHandler(Ebi_KeyDown);

            m_device = ((IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));
        }
        #endregion

        public void SetActive(bool active)
        {
            if (active)
            {
                m_ebis.Focus = this;

                m_isActive = true;
            }
            else
            {
                m_ebis.Focus = null;

                m_isActive = false;
            }
        }

        #region Event Handlers
        protected virtual void OnNewMessage(Message msg)
        {
            if (m_reporter != null)
            {
                m_reporter.BroadcastMessage(msg);
            }

            if (msg.Destination.Equals(this.ID))
            {
                if (msg.Msg.StartsWith("LoadScript"))
                {
                    string _file = msg.Msg.Substring(11);
                    LoadScript(_file);
                }
            }
        }

        private void Ebi_KeyDown(object focus, KeyEventArgs k)
        {
            if (m_isActive && focus == null)
            {
                m_ebis.Focus = this;
                focus = this;
            }

            if (focus == null)
            {
                if (k.Key == Microsoft.Xna.Framework.Input.Keys.OemTilde)
                {
                    m_ebis.Focus = this;

                    m_isActive = true;

                    return;
                }
            }
            else if (focus.Equals(this))
            {
                if (k.Key == Microsoft.Xna.Framework.Input.Keys.OemTilde)
                {
                    m_ebis.Focus = null;

                    m_isActive = false;
                }
                else
                {
                    switch (k.Key)
                    {
                        case Keys.Enter:
                            if (NewMessage != null)
                            {
                                string[] commands = m_input.Split('|');
                                for(int i = 0; i < commands.Length; i++)
                                {
                                    commands[i] = commands[i].TrimEnd().TrimStart();
                                    ReadMessage(commands[i]);
                                }

                                HistoryString hstr = new HistoryString();
                                hstr.String = m_input;
                                hstr.IsMsg = false;

                                m_history.Add(hstr);
                                m_curIndex = 0;

                                m_input = "";
                            }
                            break;
                        case Keys.Back:
                            if (m_input.Length > 0)
                                m_input = m_input.Substring(0, m_input.Length - 1);
                            break;
                        case Keys.Up:
                            if (m_curIndex == 0)
                            {
                                if (m_history.Count > 0)
                                    m_curIndex = m_history.Count - 1;
                            }
                            else
                            {
                                m_curIndex--;
                            }

                            if(m_history.Count > 0)
                                m_input = m_history[m_curIndex].String;

                            break;
                        case Keys.Down:
                            if (m_curIndex == m_history.Count - 1)
                                m_curIndex = 0;
                            else
                                m_curIndex++;

                            if (m_history.Count > 0)
                                m_input = m_history[m_curIndex].String;

                            break;
                        default:
                            m_input += Helper.KeyToString(k.Key, k.Shift);
                            break;
                    }
                }
            }
        }
        #endregion

        #region IService Members
        public string ID
        {
            get
            {
                return "Console";
            }
        }

        public void LoadSettings(XmlNode node)
        {
            //System.Console.WriteLine("Loading settings!");
            if (node.Name.Equals(this.ID))
            {
                foreach (XmlNode _node in node.ChildNodes)
                {
                    //System.Console.WriteLine(_node.Name + ": " + _node.ChildNodes[0].Value);
                    if (_node.Name.Equals("Prefix"))
                    {
                        m_prefix = _node.ChildNodes[0].Value;
                    }
                    else if (_node.Name.Equals("Suffix"))
                    {
                        m_suffix = _node.ChildNodes[0].Value;
                    }
                }
            }
        }
        #endregion

        #region IConsoleService Members
        public void Write(string msg)
        {
            HistoryString hstr = new HistoryString();
            hstr.IsMsg = true;
            hstr.String = msg;

            m_history.Add(hstr);

            if (m_reporter != null)
            {
                Message _msg = new Message(this);
                _msg.Destination = this.ID;
                _msg.Source = "";
                _msg.Msg = msg;

                m_reporter.BroadcastMessage(_msg);
            }
        }

        public string Prefix
        {
            get
            {
                return m_prefix;
            }
            set
            {
                m_prefix = value;
            }
        }

        public void LoadScript(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("Script file could not be found!", file);

            StreamReader sr = new StreamReader(file);
            string _line = "";

            while (!sr.EndOfStream)
            {
                _line = sr.ReadLine();

                string[] commands = _line.Split('|');
                for (int i = 0; i < commands.Length; i++)
                {
                    commands[i] = commands[i].TrimEnd().TrimStart();

                    this.Write("Processing command [" + commands[i] + "]");
                    ReadMessage(commands[i]);
                }
            }

            sr.Close();
        }
        #endregion

        protected void ReadMessage(string message)
        {
            Message _msg = new Message(this);
            if (message.StartsWith("sm "))
            {
                string str = message.Substring(3);
                _msg.Destination = str.Substring(0, str.IndexOf(" "));
                _msg.Msg = str.Substring(str.IndexOf(" ") + 1);
                _msg.Source = this.ID;
            }
            else
            {
                _msg.Msg = message;
                _msg.Source = this.ID;
            }

            NewMessage.Invoke(_msg);
        }

        #region Load / Unload Graphics
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
				m_font = m_conManager.Load<SpriteFont>(@"Content\Fonts\" + XeGame.FONT_DBG);

                m_spriteBatch = new SpriteBatch(this.GraphicsDevice);

                Assembly _assembly = Assembly.GetExecutingAssembly();

                m_texture = m_conManager.Load<Texture2D>(@"Content\Textures\Console");
                m_y = -m_texture.Height;

                m_drawRect = new Rectangle(0, 0, m_texture.Width, m_texture.Height);
            }
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if (unloadAllContent)
            {
                if(m_spriteBatch != null)
                    m_spriteBatch.Dispose();

                if(m_texture != null)
                    m_texture.Dispose();
            }
        }
        #endregion

        #region Overrides (Update & Draw)
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            m_drawRect.Y = m_y;
        }

        public override void Draw(GameTime gameTime)
        {
            if (m_isActive)
            {
                if (m_y < 0)
                    m_y++;

                m_drawRect.Width = this.Game.Window.ClientBounds.Width;

                m_spriteBatch.Begin();
                m_spriteBatch.Draw(m_texture, m_drawRect, Color.White);
                //TODO : Variable Font Scale
                m_spriteBatch.DrawString(m_font,m_prefix + m_input + m_suffix, new Vector2(5,m_y + m_texture.Height -16),Color.White);

                int index = m_history.Count - 1;
                for (int y = m_y + m_texture.Height - 32; y > 0; y -= 16)
                {
                    if (index < 0)
                        break;

					m_spriteBatch.DrawString(m_font, m_prefix + m_history[index].String, new Vector2(5, y), Color.White);

                    index--;
                }

				m_spriteBatch.End();
            }
            else
            {
                if (m_y > -m_texture.Height)
                {
                    m_y--;

                    m_drawRect.Width = this.Game.Window.ClientBounds.Width;

                    m_spriteBatch.Begin();
                    m_spriteBatch.Draw(m_texture, m_drawRect, Color.White);

					m_spriteBatch.DrawString(m_font,m_prefix + m_input + m_suffix, new Vector2(5,m_y + m_texture.Height -16),Color.White);
					
                    int index = m_history.Count - 1;
                    for (int y = m_y + m_texture.Height - 32; y > 0; y -= 16)
                    {
                        if (index < 0)
                            break;

                        m_spriteBatch.DrawString(m_font, m_prefix + m_history[index].String, new Vector2(5, y), Color.White);

                        index--;
                    }

					m_spriteBatch.End();
                }
            }
        }
        #endregion

		#region IFocusable Members

		public bool Focus()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void UnFocus()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool TabNext()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool TabPrev()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public event GotFocusHandler GotFocus;

		public event LostFocusHandler LostFocus;

		public bool IsTabable
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public int TabOrder
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		#endregion
	}

    public delegate void NewMessageHandler(Message msg);

    public interface IConsoleService
    {
        event NewMessageHandler NewMessage;

        void Write(string msg);

        string Prefix { get; set; }

        void LoadScript(string file);
    }
}



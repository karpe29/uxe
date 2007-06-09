#region License
/*
 *  Xna5D
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 */
#endregion

#region Using Statements
using System;
using System.Reflection;
using System.Collections.Generic;

using Xe;
using Xe.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Xe.Tools
{
    public partial class Stats : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        #region General
		protected ContentManager m_contentManager;
		protected SpriteBatch m_spriteBatch;
        protected SpriteFont m_font;
        protected IReporterService m_reporter;

        protected int m_polygonCount = 0;
        #endregion

        #region FPS
        protected int m_fps = 0;
        protected int m_frameCount = 0;
        protected int m_elapsedTime = 0;

        protected int[] m_fpsTicks = new int[30];
        protected int m_currentTick = 0;
        protected int m_avgFPS = 0;
        #endregion

        #region Frame Timing
        protected int m_lastFrame;
        #endregion

		#region Debug Strings
		List<string> m_debugStrings = new List<string>();
		#endregion

		protected Color m_foreColor = Color.Black;

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
        #endregion

        #region Constructors & Initialization
		public Stats(Game game, ContentManager contentManager)
            : base(game)
        {
			m_contentManager = contentManager;

			GraphicsDeviceManager g = (GraphicsDeviceManager)this.Game.Services.GetService(typeof(IGraphicsDeviceManager));

			m_spriteBatch = new SpriteBatch(g.GraphicsDevice);
            
			this.Visible = false;

            if (game != null)
                game.Services.AddService(typeof(Stats), this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));
            if(m_reporter != null)
                m_reporter.MessageReported += new MessageReportedHandler(Reporter_NewMessage);
        }
        #endregion

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);
            if (loadAllContent)
            {
				m_font = m_contentManager.Load<SpriteFont>(@"Content\Fonts\Comic");
            }
        }

		public int AddModelPolygonsCount(Model thisModel)
		{
			for (int i = 0; i < thisModel.Meshes.Count; i++)
			{
				ModelMesh modelMesh = thisModel.Meshes[i];
				for (int j = 0; j < modelMesh.MeshParts.Count; j++)
				{
					this.m_polygonCount += modelMesh.MeshParts[j].PrimitiveCount;
				}
			}

			return this.m_polygonCount;
		}

		public void AddDebugString(string s)
		{
			m_debugStrings.Add(s);
		}

        #region Event Handlers
        void Reporter_NewMessage(Message msg)
        {
            if (msg.Destination.Equals("Xna5D_Stats"))
            {
                if (msg.Msg.Equals("on"))
                    this.Visible = true;
                else if (msg.Msg.Equals("off"))
                    this.Visible = false;
                else
                {
                    if (msg.Msg.StartsWith("Color."))
                    {
                        string _color = msg.Msg.Substring(6);
                        m_foreColor = Globals.StringToColor(_color);
                    }
                }
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            #region FPS
            m_elapsedTime += gameTime.ElapsedRealTime.Milliseconds;
            if (m_elapsedTime > 1000)
            {
                m_fps = m_frameCount;
                m_elapsedTime = 0;
                m_frameCount = 0;
            }
            m_frameCount++;

            if (m_currentTick == 30)
            {
                int _total = 0;
                for (int i = 0; i < 30; i++)
                {
                    _total += m_fpsTicks[i];
                }
                _total /= 30;

                if (m_avgFPS == 0)
                    m_avgFPS = _total;
                else
                    m_avgFPS = (m_avgFPS + _total) / 2;

                m_currentTick = 0;
            }
            if (m_fps != 0)
            {
                m_fpsTicks[m_currentTick] = m_fps;
                m_currentTick++;
            }
            #endregion

            m_lastFrame = gameTime.ElapsedRealTime.Milliseconds;

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            if (m_font != null)
            {
                string _fps = String.Format("FPS: {0}", m_fps.ToString());
                string _frameTime = String.Format("Frame Time: {0}ms", m_lastFrame.ToString());
                string _mouseCoords = String.Format("Mouse Coords (x,y): ({0},{1})", Mouse.GetState().X, Mouse.GetState().Y);
                string _polyCount = String.Format("Reported Drawn Polygons: {0}", m_polygonCount.ToString());
				string _components = String.Format("Game.Components.Count: {0}", this.Game.Components.Count);

                m_polygonCount = 0;

                #region Screen Size
                int _top, _left, _width, _height;
                _top = Game.Window.ClientBounds.Top;
                _left = Game.Window.ClientBounds.Left;
                _width = Game.Window.ClientBounds.Width;
                _height = Game.Window.ClientBounds.Height;

                string _size = String.Format("Window Size: {0}x{1} from {2},{3}", _width.ToString(), _height.ToString(), _left.ToString(), _top.ToString());
                #endregion

				m_spriteBatch.Begin();

				m_spriteBatch.DrawString(m_font, _fps, new Vector2(5, 5), m_foreColor);
				m_spriteBatch.DrawString(m_font, _frameTime, new Vector2(5, m_font.LineSpacing + 5), m_foreColor);
				m_spriteBatch.DrawString(m_font, _size, new Vector2(5, m_font.LineSpacing * 2 + 5), m_foreColor);
				m_spriteBatch.DrawString(m_font, _mouseCoords, new Vector2(5, m_font.LineSpacing * 3 + 5), m_foreColor);
				m_spriteBatch.DrawString(m_font, _polyCount, new Vector2(5, m_font.LineSpacing * 4 + 5), m_foreColor);
				m_spriteBatch.DrawString(m_font, _components, new Vector2(5, m_font.LineSpacing * 5 + 5), m_foreColor);

				#region Debug Strings
				if (m_debugStrings.Count > 0)
				{
					int i = 6;
					foreach (string s in m_debugStrings)
					{
						m_spriteBatch.DrawString(m_font, s, new Vector2(5, m_font.LineSpacing * i + 5), m_foreColor);
						i++;
					}
					m_debugStrings.Clear();
				}
				#endregion

				m_spriteBatch.End();
            }

            base.Draw(gameTime);
        }
        #endregion

        #region Properties
        public int PolygonCount
        {
            get
            {
                return m_polygonCount;
            }
            set
            {
                m_polygonCount = value;
            }
        }

        public int FPS
        {
            get
            {
                return m_fps;
            }
        }

        public int AvgFPS
        {
            get
            {
                return m_avgFPS;
            }
        }
        #endregion
    }
}



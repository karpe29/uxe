#region License
/*
 *  Xna5D.Graphics2D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 04, 2006
 */
#endregion License

#region Using Statements
using System;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Graphics2D
{
    public partial class Animation2D : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        protected List<Frame2D> m_frames = new List<Frame2D>();

        protected SpriteBatch m_sBatch = null;
        protected ContentManager m_conManager = null;
        protected Texture2D m_texture = null;

        protected string m_texSource = String.Empty;

        protected int m_curFrame = 0;
        protected double m_curCount = 0;

        protected bool m_isUsingDest = true;
        protected Rectangle m_dest = Rectangle.Empty;
        #endregion

        #region Construction & Destruction
		public Animation2D(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        ~Animation2D()
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                UnloadGraphicsContent(true);
        }
        #endregion

        #region Load/Unload Graphics Content
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                m_sBatch = new SpriteBatch(this.GraphicsDevice);

                m_conManager = new ContentManager(this.Game.Services);

                m_texture = m_conManager.Load<Texture2D>(m_texSource);
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

                    m_conManager = null;
                }

                if (m_texture != null)
                    m_texture.Dispose();
                m_texture = null;

                if (m_sBatch != null)
                    m_sBatch.Dispose();
                m_sBatch = null;
            }
        }
        #endregion

        #region Updating & Drawing
        /// <summary>
        /// Updates the Animation.
        /// </summary>
        /// <param name="gameTime">A valid GameTime object.</param>
        public override void Update(GameTime gameTime)
        {
            if (m_frames.Count > 0)
            {
                //System.Console.WriteLine(m_frames[m_curFrame].FrameTime.ToString());
                if (m_curCount >= m_frames[m_curFrame].FrameTime)
                {
                    if (!String.IsNullOrEmpty(m_frames[m_curFrame].JumpFrame))
                    {
                        for (int i = 0; i < m_frames.Count; i++)
                        {
                            if (m_frames[i].ID.Equals(m_frames[m_curFrame].JumpFrame))
                            {
                                m_curFrame = i;
                                m_curCount = 0;
                                break;
                            }
                        }

                    }
                    else
                    {
                        m_curCount = 0;
                        m_curFrame++;

                        if (m_curFrame > m_frames.Count - 1)
                        {
                            m_curFrame = 0;
                        }
                    }
                }

                m_curCount += gameTime.ElapsedRealTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the Animation.
        /// </summary>
        /// <param name="gameTime">A valid GameTime object.</param>
        public override void Draw(GameTime gameTime)
        {
            if (m_texture != null && m_sBatch != null)
            {
                m_sBatch.Begin(SpriteBlendMode.AlphaBlend);

                if (m_isUsingDest)
                    m_sBatch.Draw(m_texture, m_dest, m_frames[m_curFrame].Source, m_frames[m_curFrame].Color);
                else
                    m_sBatch.Draw(m_texture, m_frames[m_curFrame].Destination, m_frames[m_curFrame].Source, m_frames[m_curFrame].Color);

                m_sBatch.End();
            }

            base.Draw(gameTime);
        }
        #endregion

        #region General
        public void SetFrame(string frameID)
        {
            for (int i = 0; i < m_frames.Count; i++)
            {
                if (m_frames[i].ID.Equals(frameID))
                {
                    m_curFrame = i;
                    m_curCount = 0;

                    return;
                }
            }
        }
        #endregion

        #region Adding / Removing
        /// <summary>
        /// Add a Frame to the FrameSet.
        /// </summary>
        /// <param name="frame">The Frame to be added.</param>
        public void Add(Frame2D frame)
        {
            foreach (Frame2D _frame in m_frames)
                if (_frame.ID.Equals(frame.ID))
                    throw new ObjectFoundException(String.Format("{0} was already found in the current frame set!", frame.ID));

            m_frames.Add(frame);
        }

        /// <summary>
        /// Add an entire FrameSet to the current collection of frames.
        /// </summary>
        /// <param name="frameSet">The FrameSet to add.</param>
        public void Add(IEnumerable<Frame2D> frameSet)
        {
            foreach (Frame2D _frame in frameSet)
                Add(_frame);
        }

        /// <summary>
        /// Remove a Frame from the FrameSet.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool Remove(Frame2D frame)
        {
            return m_frames.Remove(frame);
        }

        public bool Remove(string frameID)
        {
            foreach (Frame2D _frame in m_frames)
                if (_frame.ID.Equals(frameID))
                    return Remove(_frame);

            return false;
        }

        public void ClearSet()
        {
            m_frames.Clear();
        }
        #endregion

        #region Properties
        public string TextureSource
        {
            get
            {
                return m_texSource;
            }
            set
            {
                m_texSource = value;
            }
        }

        public bool IsUsingDest
        {
            get
            {
                return m_isUsingDest;
            }
            set
            {
                m_isUsingDest = value;
            }
        }

        public Rectangle Destination
        {
            get
            {
                return m_dest;
            }
            set
            {
                m_dest = value;
            }
        }
        #endregion
    }

    public class Frame2D
    {
        #region Members
        private string m_id = "Frame2D";
        private string m_jumpFrame = string.Empty;

        private Rectangle m_dest = Rectangle.Empty;
        private Rectangle m_src = Rectangle.Empty;

        private double m_frameTime = 0.1;

        private Color m_tintColor = Color.White;
        #endregion

        #region Constructor & Destructor
        public Frame2D()
        {
        }

        ~Frame2D()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Unique ID for the individual frame.
        /// </summary>
        public string ID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        /// <summary>
        /// The frame to jump to in a sequence, String.Empty means the next frame.
        /// </summary>
        public string JumpFrame
        {
            get
            {
                return m_jumpFrame;
            }
            set
            {
                m_jumpFrame = value;
            }
        }

        /// <summary>
        /// The Destination Rectangle for the texture.
        /// </summary>
        public Rectangle Destination
        {
            get
            {
                return m_dest;
            }
            set
            {
                m_dest = value;
            }
        }

        /// <summary>
        /// The Source Rectangle for the texture.
        /// </summary>
        public Rectangle Source
        {
            get
            {
                return m_src;
            }
            set
            {
                m_src = value;
            }
        }

        /// <summary>
        /// How long the frame should last.
        /// </summary>
        public double FrameTime
        {
            get
            {
                return m_frameTime;
            }
            set
            {
                m_frameTime = value;
            }
        }

        /// <summary>
        /// What color to tint the texture.
        /// </summary>
        public Color Color
        {
            get
            {
                return m_tintColor;
            }
            set
            {
                m_tintColor = value;
            }
        }
        #endregion
    }
}



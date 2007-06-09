#region License
/*
 *  Xna5D.Graphics2D.dll
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Graphics2D
{
    public partial class Sprite : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        // Let's us draw to the screen.
        private SpriteBatch m_spriteBatch;

        // Let's us load content files.
        private ContentManager m_conManager;

        // Our texture and the source file
        private Texture2D m_texture;
        private string m_texSource = String.Empty;

        // Where we want to draw our texture in screen coords
        private Rectangle m_destRect = new Rectangle();

        // What part of the texture to draw
        private Rectangle m_srcRect = new Rectangle();

        // The tinting color.
        private Color m_color = Color.White;

        // The sprite's blend mode for blending transparencies
        private SpriteBlendMode m_blendMode = SpriteBlendMode.AlphaBlend;

        // Used to report errors
        protected IReporterService m_reporter;
        #endregion

        #region Constructor & Destructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="game">A valid Game object.</param>
		public Sprite(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Secondary Constructor. Sets the texture's source.
        /// </summary>
        /// <param name="game">A valid Game object.</param>
        /// <param name="assetName">The name of the content to load into the texture.</param>
		public Sprite(Game game, string assetName)
            : base(game)
        {
            SetTextureSource(assetName);
        }

        ~Sprite()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion

        #region Graphics Content
        /// <summary>
        /// Set the source for the texture.
        /// </summary>
        /// <param name="assetName">Asset Name, relative to the root loader
        /// directory, and not including the .xnb extension.</param>
        public void SetTextureSource(string assetName)
        {
            m_texSource = assetName;
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                try
                {
                    // Check to make sure the source is not dummy
                    if (String.IsNullOrEmpty(m_texSource))
                        throw new Exception("Texture source was invalid!");

                    // Create our sprite batch
                    m_spriteBatch = new SpriteBatch(this.GraphicsDevice);

                    // Create our content manager
                    m_conManager = new ContentManager(this.Game.Services);

                    // Load the texture
                    m_texture = m_conManager.Load<Texture2D>(m_texSource);

                    // Setup the rectangles.
                    m_srcRect = Rectangle.Empty;
                    m_destRect = new Rectangle(0, 0, m_texture.Width, m_texture.Height);
                }
                catch (Exception e)
                {
                    // Report any errors
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
                // Unload and dispose of the content manager
                if (m_conManager != null)
                {
                    m_conManager.Unload();
                    m_conManager.Dispose();
                }
                m_conManager = null;

                // Dispose of the sprite batch
                if (m_spriteBatch != null)
                    m_spriteBatch.Dispose();
                m_spriteBatch = null;

                // Dispose of the texture
                if (m_texture != null)
                    m_texture.Dispose();
                m_texture = null;
            }
        }
        #endregion

        #region GameComponent Overrides
        public override void Initialize()
        {
            base.Initialize();

            // Grab a reference to the reporting service
            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            try
            {
                // Begin our sprite batch
                m_spriteBatch.Begin(m_blendMode);

                // Draw the texture
                m_spriteBatch.Draw(m_texture, m_destRect, m_srcRect, m_color);

                // End drawing
                m_spriteBatch.End();

                ResetDeviceStates();
            }
            catch (NullReferenceException nre)
            {
                // Report the null reference
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, "Null Reference occured in Sprite.Draw!", nre);
            }
            catch (Exception e)
            {
                // Report any other exceptions
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, "Exception occured in Sprite.Draw!", e);
            }
        }

        protected virtual void ResetDeviceStates()
        {
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.AlphaTestEnable = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The position of the sprite (top, left) along the X axis.
        /// </summary>
        public int X
        {
            get
            {
                return m_destRect.X;
            }
            set
            {
                m_destRect.X = value;
            }
        }

        /// <summary>
        /// The position of the sprite (top, left) along the Y axis.
        /// </summary>
        public int Y
        {
            get
            {
                return m_destRect.Y;
            }
            set
            {
                m_destRect.Y = value;
            }
        }

        /// <summary>
        /// The width of the sprite.
        /// </summary>
        public int Width
        {
            get
            {
                return m_destRect.Width;
            }
            set
            {
                m_destRect.Width = value;
            }
        }

        /// <summary>
        /// The height of the sprite.
        /// </summary>
        public int Height
        {
            get
            {
                return m_destRect.Height;
            }
            set
            {
                m_destRect.Height = value;
            }
        }

        /// <summary>
        /// The Destination Rectangle.
        /// </summary>
        public Rectangle Destination
        {
            get
            {
                return m_destRect;
            }
            set
            {
                m_destRect = value;
            }
        }

        /// <summary>
        /// The Source Rectangle.
        /// </summary>
        public Rectangle Source
        {
            get
            {
                return m_srcRect;
            }
            set
            {
                m_srcRect = value;
            }
        }

        /// <summary>
        /// The Tinting Color of the Sprite.
        /// </summary>
        public Color Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }

        /// <summary>
        /// How to blend transparencies and translucencies.
        /// </summary>
        public SpriteBlendMode BlendMode
        {
            get
            {
                return m_blendMode;
            }
            set
            {
                m_blendMode = value;
            }
        }
        #endregion
    }

    public class SpriteParameters
    {
        #region Members
        private SpriteEffects m_sEffects = SpriteEffects.None;

        private float m_rotation = 0.0f;

        private Vector2 m_scale = new Vector2(1.0f, 1.0f);
        private Vector2 m_origin = new Vector2(0.0f, 0.0f);
        #endregion

        #region Constructor
        public SpriteParameters()
        {
        }
        #endregion

        #region Properties
        public SpriteEffects SpriteEffects
        {
            get
            {
                return m_sEffects;
            }
            set
            {
                m_sEffects = value;
            }
        }

        public float Rotation
        {
            get
            {
                return m_rotation;
            }
            set
            {
                m_rotation = value;
            }
        }

        public Vector2 Scale
        {
            get
            {
                return m_scale;
            }
            set
            {
                m_scale = value;
            }
        }

        public Vector2 Origin
        {
            get
            {
                return m_origin;
            }
            set
            {
                m_origin = value;
            }
        }
        #endregion
    }
}



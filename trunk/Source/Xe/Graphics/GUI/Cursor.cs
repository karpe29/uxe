
#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Gui
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public partial class Cursor : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteBatch m_sBatch;
        private ContentManager m_conManager;

        private string m_texSource = String.Empty;
        private Texture2D m_baseTexture;

        private QuadBase m_cursor;

        public Cursor(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            m_cursor = new QuadBase(Game);
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();

            m_cursor.Initialize();
        }

        public Texture2D BaseTexture
        {
            get { return m_baseTexture; }
            set
            {
                m_baseTexture = value;

                m_cursor.Texture = m_baseTexture;
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            m_sBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            //Console.WriteLine(m_cursor.SrcRect + " " + m_cursor.U);
            m_sBatch.Draw(m_cursor.Texture, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), m_cursor.SrcRect, Color.White);

            m_sBatch.End();
        }

        #region Load / Unload Graphics
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                m_sBatch = new SpriteBatch(this.GraphicsDevice);
                m_conManager = new ContentManager(Game.Services);

                if (!String.IsNullOrEmpty(m_texSource))
                {
                    m_baseTexture = m_conManager.Load<Texture2D>(m_texSource);
                    m_cursor.Texture = m_baseTexture;
                }

                m_cursor.U = 256;
                m_cursor.V = 320;
                m_cursor.UVWidth = 32;
                m_cursor.UVHeight = 32;
            }
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if (unloadAllContent)
            {
                if (m_sBatch != null)
                {
                    m_sBatch.Dispose();
                    m_sBatch = null;
                }

                if (m_conManager != null)
                {
                    m_conManager.Unload();
                    m_conManager.Dispose();
                    m_conManager = null;
                }
            }
        }
        #endregion
    }
}



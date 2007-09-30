#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.GUI
{
    public class SpriteRenderer : QuadRenderer
    {
        #region Members
        private SpriteBatch m_sBatch = null;
        private SpriteBlendMode m_blendMode = SpriteBlendMode.AlphaBlend;
        private SpriteSortMode m_sortMode = SpriteSortMode.BackToFront;
        private SaveStateMode m_stateMode = SaveStateMode.SaveState;

        private Queue<QuadBase> m_quads = new Queue<QuadBase>();

        private Vector2 m_vectorOne = new Vector2(1, 1);
        #endregion

        #region Constructors
        public SpriteRenderer()
            : base()
        {
        }
        #endregion

        #region Initialization
        public override void Initialize(Game game, GraphicsDevice device)
        {
            base.Initialize(game, device);
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                m_sBatch = new SpriteBatch(this.GraphicsDevice);
            }
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if (unloadAllContent)
            {
                if (m_sBatch != null)
                    m_sBatch.Dispose();
                m_sBatch = null;
            }
        }
        #endregion

        public override void RenderQuad(QuadBase quad)
        {
            // If the quad isn't worthy, don't add it.
            if (quad == null)
                return;

            base.RenderQuad(quad);

            // Add the quad to the queue
            m_quads.Enqueue(quad);

            if (!this.UseBreadthFirst)
                Flush();
        }

        public override void Flush()
        {
            base.Flush();

            // Return if there are no quads to draw
            if (m_quads.Count <= 0)
                return;

            // Flush out the quads
            // Begin the sprite batch
            m_sBatch.Begin(m_blendMode, m_sortMode, m_stateMode);

            while (m_quads.Count > 0)
            {
                // Get the quad.
                QuadBase _quad = m_quads.Dequeue();

                // If the texture is null, don't bother.
                if (_quad.Texture == null)
                    continue;

                // Render the quad.
                m_sBatch.Draw(_quad.Texture, _quad.DestRect, _quad.SrcRect, _quad.Color, _quad.RotationZ, Vector2.Zero, SpriteEffects.None, _quad.Z);
            }

            // End the sprite batch
            m_sBatch.End();
        }

        #region Properties
        protected SpriteBatch SpriteBatch
        {
            get { return m_sBatch; }
            set { m_sBatch = value; }
        }

        protected Queue<QuadBase> Quads
        {
            get { return m_quads; }
            set { m_quads = value; }
        }

        public SpriteBlendMode BlendMode
        {
            get { return m_blendMode; }
            set { m_blendMode = value; }
        }

        public SaveStateMode SaveStateMode
        {
            get { return m_stateMode; }
            set { m_stateMode = value; }
        }

        public SpriteSortMode SortMode
        {
            get { return m_sortMode; }
            set { m_sortMode = value; }
        }
        #endregion
    }
}

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

#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xe.GUI;
#endregion

namespace Xe.Graphics2D
{
    public partial class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        private List<QuadBase> m_quads = new List<QuadBase>();

        private SpriteBatch m_sBatch;
        private SpriteBlendMode m_blendMode = SpriteBlendMode.AlphaBlend;
        #endregion

		public SpriteManager(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            m_sBatch.Begin(m_blendMode);

            foreach (QuadBase _quad in m_quads)
                m_sBatch.Draw(_quad.Texture, _quad.Destination, _quad.Source, _quad.Color);

            m_sBatch.End();

            base.Draw(gameTime);

            ResetDeviceStates();
        }

        protected virtual void ResetDeviceStates()
        {
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.AlphaTestEnable = false;
        }

        #region Load / Unload Graphics
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
                if(m_sBatch != null)
                    m_sBatch.Dispose();
                m_sBatch = null;
            }
        }
        #endregion

        #region Properties
        public List<QuadBase> Quads
        {
            get
            {
                return m_quads;
            }
            set
            {
                m_quads = value;
            }
        }

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
}



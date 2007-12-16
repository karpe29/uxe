#region License
/*
 *  Xna5D.Graphics3D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 * 
 *  Big thanks to Ziggy of ziggyware.com for Quad Rendering base.
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: January 28, 2006
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Graphics3D
{
    public partial class RenderGroup : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        // The Render Target
        private RenderTarget2D m_renderTarget;

        // Items to Render in the Scene
        private List<IDrawable> m_sceneItems = new List<IDrawable>();

        // The Color to clear the Render Target to and the Clear options.
        private Color m_clearColor = new Color(0, 0, 0, 0);
        private ClearOptions m_clearOptions = ClearOptions.Target | ClearOptions.DepthBuffer;

        // Content Manager for loading an Effect
        private ContentManager m_conManager;

        // The Effect to Post Process with
        private Effect m_effect;
        private string m_effectSrc;

        // The Camera object
        private Camera m_camera;

        // Blending mode for each pass of the post process
        private BlendMode m_blendMode = BlendMode.AlphaBlend;

        #region Quad Members
        private VertexDeclaration m_vDec = null;        // Lets the GPU know what's comin
        private VertexPositionTexture[] m_verts = null; // Vertex List
        private short[] m_ib = null;                    // Index Buffer List
        private IndexBuffer m_iBuffer;                  // Index Buffer
        private VertexBuffer m_vBuffer;                 // Vertex Buffer
        #endregion

        // Drawing Points for the Quad (Allows for "viewports").
        private Vector2 m_topLeft = new Vector2(-1, 1);
        private Vector2 m_botRight = new Vector2(1, -1);
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="game">Valid Game object.</param>
		public RenderGroup(Game game)
            : base(game)
        {
        }
        #endregion

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < m_sceneItems.Count; i++)
            {
                IGameComponent _igc = m_sceneItems[i] as IGameComponent;
                if (_igc != null)
                    _igc.Initialize();
            }
        }

        #region Load / Unload Graphics
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                m_vDec = new VertexDeclaration(this.GraphicsDevice, VertexPositionTexture.VertexElements);

                Tesselate.CreateQuad(10, m_topLeft, m_botRight, out m_verts, out m_ib);

                m_conManager = new ContentManager(this.Game.Services);

                if (!String.IsNullOrEmpty(m_effectSrc))
                    m_effect = m_conManager.Load<Effect>(m_effectSrc);

				m_iBuffer = new IndexBuffer(this.GraphicsDevice, typeof(short), m_ib.Length, BufferUsage.None);
                m_iBuffer.SetData<short>(m_ib);

				m_vBuffer = new VertexBuffer(this.GraphicsDevice, m_verts.Length * VertexPositionTexture.SizeInBytes, BufferUsage.None);
                m_vBuffer.SetData<VertexPositionTexture>(m_verts);
            }

            m_renderTarget = new RenderTarget2D(this.GraphicsDevice, this.GraphicsDevice.Viewport.Width,
                                                    this.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
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


                if (m_effect != null)
                    m_effect.Dispose();
            }
        }
        #endregion

        #region Rendering
        public override void Draw(GameTime gameTime)
        {
            // Store whether or not blending was enabled.
            bool _wasBlending = this.GraphicsDevice.RenderState.AlphaBlendEnable;

            // Get the old RenderTarget and hold it.
            RenderTarget _tempTarget = this.GraphicsDevice.GetRenderTarget(0);
            RenderTarget2D _oldTarget = _tempTarget as RenderTarget2D;

            // Set the new RenderTarget.
            this.GraphicsDevice.SetRenderTarget(0, m_renderTarget);

            // Clear the RenderTarget.
            this.GraphicsDevice.Clear(m_clearOptions, m_clearColor, 1, 1);

            // Render Scene Items
            for (int i = 0; i < m_sceneItems.Count; i++)
                if (m_sceneItems[i].Visible)
                    m_sceneItems[i].Draw(gameTime);

            // Resolve the RenderTarget
            this.GraphicsDevice.GetRenderTarget(0);

            // Get the Texture from the RenderTarget
            Texture2D _texRTT = m_renderTarget.GetTexture();

            // Set the old RenderTarget
            this.GraphicsDevice.SetRenderTarget(0, _oldTarget);
            //this.GraphicsDevice.Clear(m_clearOptions, m_clearColor, 1, 1);

            // Set initial RenderStates
            OnPreRender(gameTime);

            // Set Effect Parameters
            if (m_effect.Parameters["UserTexture"] != null)
                m_effect.Parameters["UserTexture"].SetValue(_texRTT);
            if (m_effect.Parameters["WorldViewProj"] != null)
                m_effect.Parameters["WorldViewProj"].SetValue(m_camera.ViewProjection);

            // Set the Current Technique and Begin the Effect
            m_effect.CurrentTechnique = m_effect.Techniques[0];
            m_effect.Begin();

            Texture2D _texBB = _texRTT;
            for (int i = 0; i < m_effect.CurrentTechnique.Passes.Count; i++)
            {
                // Begin the pass.
                m_effect.CurrentTechnique.Passes[i].Begin();

                // Render the Quad.
                RenderQuad(new Vector2(-1, -1), new Vector2(1, 1));

                // End the pass.
                m_effect.CurrentTechnique.Passes[i].End();
            }

            // End the effect.
            m_effect.End();

            // Unset RenderStates
            OnPostRender(gameTime, _wasBlending);
        }

        protected virtual void RenderQuad(Vector2 v1, Vector2 v2)
        {
            // Set the Vertex Declaration
            this.GraphicsDevice.VertexDeclaration = m_vDec;

            /*this.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
                (PrimitiveType.TriangleList, m_verts, 0, m_verts.Length, m_ib, 0, m_ib.Length / 3);*/

            this.GraphicsDevice.Vertices[0].SetSource(m_vBuffer, 0, m_vDec.GetVertexStrideSize(0));

            this.GraphicsDevice.Indices = m_iBuffer;

            this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_verts.Length, 0, m_ib.Length / 3);
        }

        protected virtual void OnPreRender(GameTime gameTime)
        {
            RenderState _state = this.GraphicsDevice.RenderState;
            switch(m_blendMode)
            {
                case BlendMode.AlphaBlend:
                    _state.AlphaBlendEnable = true;
                    _state.DepthBufferWriteEnable = false;
                    _state.AlphaFunction = CompareFunction.Greater;
                    _state.AlphaBlendOperation = BlendFunction.Add;
                    _state.SourceBlend = Blend.SourceAlpha;
                    _state.DestinationBlend = Blend.InverseSourceAlpha;
                    break;
                case BlendMode.Additive:
                    _state.AlphaBlendEnable = true;
                    _state.DepthBufferWriteEnable = false;
                    _state.AlphaFunction = CompareFunction.Always;
                    _state.AlphaBlendOperation = BlendFunction.Add;
                    _state.SourceBlend = Blend.SourceAlpha;
                    _state.DestinationBlend = Blend.DestinationAlpha;
                    break;
                case BlendMode.None:
                    _state.AlphaBlendEnable = false;
                    break;
            }
        }

        protected virtual void OnPostRender(GameTime gameTime, bool enable)
        {
            this.GraphicsDevice.RenderState.AlphaBlendEnable = enable;
            this.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

            //this.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Top Left Position of the RenderGroup's RenderTarget.
        /// </summary>
        public Vector2 TopLeft
        {
            get
            {
                return m_topLeft;
            }
            set
            {
                m_topLeft = value;
            }
        }

        /// <summary>
        /// Bottom Right Position of the RenderGroup's RenderTarget.
        /// </summary>
        public Vector2 BotRight
        {
            get
            {
                return m_botRight;
            }
            set
            {
                m_botRight = value;
            }
        }

        public Camera Camera
        {
            get
            {
                return m_camera;
            }
            set
            {
                m_camera = value;
            }
        }

        public RenderTarget2D RenderTarget
        {
            get
            {
                return m_renderTarget;
            }
            set
            {
                m_renderTarget = value;
            }
        }

        public List<IDrawable> SceneItems
        {
            get
            {
                return m_sceneItems;
            }
        }

        public ClearOptions ClearOptions
        {
            get
            {
                return m_clearOptions;
            }
            set
            {
                m_clearOptions = value;
            }
        }

        public Color ClearColor
        {
            get
            {
                return m_clearColor;
            }
            set
            {
                m_clearColor = value;
            }
        }

        public Effect Effect
        {
            get
            {
                return m_effect;
            }
            set
            {
                m_effect = value;
            }
        }

        public string EffectSrc
        {
            get
            {
                return m_effectSrc;
            }
            set
            {
                m_effectSrc = value;
            }
        }

        public BlendMode BlendMode
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

    public enum BlendMode
    {
        None,
        AlphaBlend,
        Additive
    }
}




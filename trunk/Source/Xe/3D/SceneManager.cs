#region License
/*
 *  Xna5D.Graphics3D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: January 30, 2006
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
    public partial class SceneManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        // The list of Render Groups
        private SortedDictionary<int, RenderGroup> m_rGroups = new SortedDictionary<int, RenderGroup>();

        // The Message/Error Reporting Service
        private IReporterService m_reporter;

        // Whether or not the class has been Initialized.
        private bool m_isInitialized = false;

        // The Camera object
        private Camera m_camera;

        // The Content Manager
        private ContentManager m_conManager;

        // The post processing Effect, and its source
        private Effect m_effect;
        private string m_effectSrc;

        // The Texture to post process on
        private Texture2D m_texture;
        private RenderTarget2D m_renderTarget;

        // Whether or not to post process.
        private bool m_postProcess = false;

        #region Quad Members
        private VertexDeclaration m_vDec = null;            // Let's the Device know what's coming.
        private VertexPositionTexture[] m_verts = null;     // The Vertex list.
        private short[] m_ib = null;                        // The Index Buffer list.
        private IndexBuffer m_iBuffer;                      // The Index Buffer.
        private VertexBuffer m_vBuffer;                     // The Vertex Buffer

        private Vector2 m_topLeft = new Vector2(-1, 1);     // Top left position of the Quad.
        private Vector2 m_botRight = new Vector2(1, -1);    // Bottom right position of the Quad.
        #endregion

        private bool _save = true;
        #endregion

        #region Constructor
		public SceneManager(Game game)
            : base(game)
        {
        }
        #endregion

        #region Initialize
        public override void Initialize()
        {
            base.Initialize();

            try
            {
                // Grab a reference to the Reporter Service
                m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));

                // Try to grab a reference to the Camera if it exists
                m_camera = (Camera)this.Game.Services.GetService(typeof(Camera));

                // Initialize each Render Group
                foreach (RenderGroup _rGroup in m_rGroups.Values)
                    _rGroup.Initialize();

                // Set the Initialized Var
                m_isInitialized = true;
            }
            catch (NullReferenceException nre)
            {
                // Broadcast the NRE
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, nre.Message, nre);
            }
            catch (Exception e)
            {
                // Broadcast any other Exception
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, e.Message, e);
            }
        }
        #endregion

        #region Updating
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update Enabled RenderGroups
            foreach (RenderGroup _rGroup in m_rGroups.Values)
                if (_rGroup.Enabled)
                    _rGroup.Update(gameTime);
        }
        #endregion

        #region Drawing / Rendering
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (m_postProcess)
                this.GraphicsDevice.SetRenderTarget(0, m_renderTarget);

            // Render Visible RenderGroups
            foreach (RenderGroup _rGroup in m_rGroups.Values)
            {
                if (_rGroup.Visible)
                {
                    _rGroup.Draw(gameTime);
                }
                if(m_postProcess)
                    this.GraphicsDevice.ResolveRenderTarget(0);
            }

            if (m_postProcess && m_texture != null)
            {
                //this.GraphicsDevice.ResolveBackBuffer(m_texture);
                this.GraphicsDevice.ResolveRenderTarget(0);
                m_texture = m_renderTarget.GetTexture();

                if (_save)
                {
                    m_texture.Save("renderTarget.jpg", ImageFileFormat.Jpg);
                    _save = false;
                }

                this.GraphicsDevice.SetRenderTarget(0, null);

                // Set the Texture in the Effect file.
                if (m_effect.Parameters["UserTexture"] != null)
                    m_effect.Parameters["UserTexture"].SetValue(m_texture);

                m_effect.CurrentTechnique = m_effect.Techniques[0];
                m_effect.Begin();

                for (int i = 0; i < m_effect.CurrentTechnique.Passes.Count; i++)
                {
                    // Begin the pass.
                    m_effect.CurrentTechnique.Passes[i].Begin();

                    // Set the Vertex Declaration
                    this.GraphicsDevice.VertexDeclaration = m_vDec;

                    // Set the Vertex Buffer
                    this.GraphicsDevice.Vertices[0].SetSource(m_vBuffer, 0, m_vDec.GetVertexStrideSize(0));

                    // Then set the Index Buffer
                    this.GraphicsDevice.Indices = m_iBuffer;

                    // Draw the Primitives
                    this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_verts.Length, 0, m_ib.Length / 3);

                    // End the pass.
                    m_effect.CurrentTechnique.Passes[i].End();
                }

                m_effect.End();
            }
        }
        #endregion

        #region Load / Unload Graphics Content
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                // Create the Vertex Declaration
                m_vDec = new VertexDeclaration(this.GraphicsDevice, VertexPositionTexture.VertexElements);

                // Tesselate a Quad
                Tesselate.CreateQuad(10, m_topLeft, m_botRight, out m_verts, out m_ib);

                // Create the Content Manager
                m_conManager = new ContentManager(this.Game.Services);

                // Load the Effect file if possible
                if (!String.IsNullOrEmpty(m_effectSrc))
                {
                    m_effect = m_conManager.Load<Effect>(m_effectSrc);
                    m_postProcess = true;
                }

                // Load and Set the Index Buffer
                m_iBuffer = new IndexBuffer(this.GraphicsDevice, typeof(short), m_ib.Length, ResourceUsage.None, ResourceManagementMode.Automatic);
                m_iBuffer.SetData<short>(m_ib);

                // Load and Set the Vertex Buffer
                m_vBuffer = new VertexBuffer(this.GraphicsDevice, m_verts.Length * VertexPositionTexture.SizeInBytes, ResourceUsage.None);
                m_vBuffer.SetData<VertexPositionTexture>(m_verts);
            }

            // Create the Post Processing Texture Params
            m_texture = new Texture2D(this.GraphicsDevice, this.GraphicsDevice.Viewport.Width,
                                      this.GraphicsDevice.Viewport.Height, 1,
                                      ResourceUsage.ResolveTarget, SurfaceFormat.Color, ResourceManagementMode.Manual);

            m_renderTarget = new RenderTarget2D(this.GraphicsDevice, this.GraphicsDevice.Viewport.Width,
                                                this.GraphicsDevice.Viewport.Height, 1, SurfaceFormat.Color);
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if(m_texture != null)
                m_texture.Dispose();

            if (unloadAllContent)
            {
                if (m_conManager != null)
                {
                    m_conManager.Unload();
                    m_conManager.Dispose();
                }
                m_conManager = null;

                if (m_effect != null)
                    m_effect.Dispose();

                if (m_iBuffer != null)
                    m_iBuffer.Dispose();

                if (m_vBuffer != null)
                    m_vBuffer.Dispose();
            }
        }
        #endregion

        #region Properties
        public RenderGroup this[int index]
        {
            get
            {
                if (m_rGroups.ContainsKey(index))
                    return m_rGroups[index];

                m_rGroups[index] = new RenderGroup(this.Game);

                if (m_isInitialized)
                    m_rGroups[index].Initialize();

                return m_rGroups[index];
            }
            set
            {
                m_rGroups[index] = value;
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

        public Effect Effect
        {
            get
            {
                return m_effect;
            }
            set
            {
                m_effect = value;
                if (m_effect != null)
                    m_postProcess = true;
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
        #endregion
    }
}



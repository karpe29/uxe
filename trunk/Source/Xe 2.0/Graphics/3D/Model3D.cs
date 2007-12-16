#region License
/*
 *  Xna5D.Graphics3D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 * 
 *  Special Thanks to ElectricBliss for the input processing code.
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
using System.Collections.Generic;

using Xe;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Xe.Tools;
using Xe.Gui;
#endregion

namespace Xe.Graphics3D
{
    public class Model3D : DrawableGameComponent
    {
        #region Members
        private Model m_model;
        private ContentManager m_conManager;

        private Matrix m_view;
        private Matrix m_projection;

        private Camera m_camera;

        private float m_rX = 0f, m_rY = 0f, m_rZ = 0f;
        private float m_scale = 1.0f;

        private Vector3 m_pos = new Vector3(0, 0, 0);

        private string m_assetName;
        private bool m_useAsset = false;

        private Effect m_effect;
        private string m_effectSrc;

        protected IReporterService m_reporter;
        protected Stats m_stats;
        #endregion

        #region Constructors
		public Model3D(Game game)
            : base(game)
        {
        }

		public Model3D(Game game, string assetName)
            : base(game)
        {
            this.AssetName = assetName;
        }
        #endregion

        #region Load / Unload Graphics Content
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                if(m_useAsset)
                {
                    m_conManager = new ContentManager(this.Game.Services);
                    if (!String.IsNullOrEmpty(m_assetName))
                    {
                        m_model = m_conManager.Load<Model>(m_assetName);
                    }

                    if(!String.IsNullOrEmpty(m_effectSrc))
                    {
                        m_effect = m_conManager.Load<Effect>(m_effectSrc);
                    }
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

                if (m_model != null)
                    m_model = null;
            }
        }
        #endregion

        #region Event Handlers
        protected virtual void OnCameraMatrixChanged(object sender, MatrixType type)
        {
            if (m_camera != null)
            {
                if (type == MatrixType.View)
                    m_view = m_camera.View;
                else if(type == MatrixType.Projection)
                    m_projection = m_camera.Projection;
            }
        }
        #endregion

        #region Game Component Overrides
        public override void Initialize()
        {
            base.Initialize();

            m_stats = (Stats)this.Game.Services.GetService(typeof(Stats));
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            try
            {
                if (m_model != null)
                    DrawModel(gameTime);
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, e.Message, e);
            }
        }

        protected virtual void DrawModel(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[m_model.Bones.Count];
            m_model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m_model.Meshes)
            {
                GraphicsDevice.Indices = mesh.IndexBuffer;

                SetParameters();

                m_effect.Begin();

                foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        GraphicsDevice.VertexDeclaration = part.VertexDeclaration;
                        GraphicsDevice.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);

                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                            part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);

                        if (m_stats != null)
                            m_stats.PolygonCount += part.PrimitiveCount;
                    }
                    pass.End();
                }

                m_effect.End();
            }
        }

        protected virtual void SetParameters()
        {
            if (m_effect.Parameters["World"] != null)
                m_effect.Parameters["World"].SetValue(this.World);

            if (m_effect.Parameters["View"] != null)
                m_effect.Parameters["View"].SetValue(this.View);

            if (m_effect.Parameters["Projection"] != null)
                m_effect.Parameters["Projection"].SetValue(this.Projection);

            if (m_effect.Parameters["WorldViewProj"] != null)
                m_effect.Parameters["WorldViewProj"].SetValue(this.World * this.View * this.Projection);
        }
        #endregion

        #region Properties
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

        public Camera Camera
        {
            get
            {
                return m_camera;
            }
            set
            {
                if (m_camera != null)
                    m_camera.MatrixChanged -= this.OnCameraMatrixChanged;

                m_camera = value;

                m_view = m_camera.View;
                m_projection = m_camera.Projection;

                m_camera.MatrixChanged += new MatrixChangedHandler(OnCameraMatrixChanged);
            }
        }

        public Matrix View
        {
            get
            {
                return m_view;
            }
            set
            {
                m_view = value;
            }
        }

        public Matrix Projection
        {
            get
            {
                return m_projection;
            }
            set
            {
                m_projection = value;
            }
        }

        public Matrix World
        {
            get
            {
                return Matrix.CreateRotationY(m_rY) * Matrix.CreateRotationX(m_rX) *
                       Matrix.CreateRotationZ(m_rZ) * Matrix.CreateScale(m_scale) *
                       Matrix.CreateTranslation(m_pos);
            }
        }

        public Model Model
        {
            get
            {
                return m_model;
            }
            set
            {
                m_model = value;

                m_useAsset = false;
            }
        }

        public string AssetName
        {
            get
            {
                return m_assetName;
            }
            set
            {
                m_assetName = value;

                m_useAsset = true;
            }
        }

        public float RotationX
        {
            get
            {
                return m_rX;
            }
            set
            {
                m_rX = value;
            }
        }

        public float RotationY
        {
            get
            {
                return m_rY;
            }
            set
            {
                m_rY = value;
            }
        }

        public float RotationZ
        {
            get
            {
                return m_rZ;
            }
            set
            {
                m_rZ = value;
            }
        }

        public float Scale
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

        public Vector3 Position
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;
            }
        }
        #endregion
    }
}

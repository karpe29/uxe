//#region License
///*
// *  Xna5D.Graphics3D.dll
// *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
// * 
// *  Special Thanks to ElectricBliss for the input processing code.
// *  
// *  This software is distributed in the hope that it will be useful,
// *  but WITHOUT ANY WARRANTY; without even the implied warranty of
// *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// * 
// *  Date Created: December 17, 2006
// */
//#endregion License

//#region Using Statements
//using System;
//using System.Collections.Generic;


//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Content;
//#endregion


//namespace Xe.Graphics3D
//{
//    public partial class TexQuad : Microsoft.Xna.Framework.DrawableGameComponent
//    {
//        #region Members
//        private Camera m_camera;

//        private Matrix m_view;
//        private Matrix m_proj;

//        private Effect m_effect;

//        private Texture2D m_texture;

//        private VertexPositionTexture[] m_verts;
//        private VertexBuffer m_vBuffer;
//        private VertexDeclaration m_vDec;

//        protected IReporterService m_reporter;
//        #endregion

//        public TexQuad(Game game)
//            : base(game)
//        {
//        }

//        #region Camera / Matrix Methods
//        public void SetCamera(Camera camera)
//        {
//            if (m_camera != null)
//                m_camera.MatrixChanged -= this.OnCameraMatrixChanged;

//            m_camera = camera;

//            m_camera.MatrixChanged += new MatrixChangedHandler(this.OnCameraMatrixChanged);
//        }

//        protected virtual void OnCameraMatrixChanged(MatrixType type)
//        {
//            switch (type)
//            {
//                case MatrixType.View:
//                    m_view = m_camera.View;
//                    break;
//                case MatrixType.Projection:
//                    m_proj = m_camera.Projection;
//                    break;
//            }
//        }
//        #endregion

//        public override void Initialize()
//        {
//            base.Initialize();

//            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));
//        }

//        #region Load / Unload Graphics
//        protected override void LoadGraphicsContent(bool loadAllContent)
//        {
//            base.LoadGraphicsContent(loadAllContent);
//        }

//        protected override void UnloadGraphicsContent(bool unloadAllContent)
//        {
//            base.UnloadGraphicsContent(unloadAllContent);
//        }
//        #endregion

//        public override void Update(GameTime gameTime)
//        {
//            base.Update(gameTime);
//        }

//        public override void Draw(GameTime gameTime)
//        {
//            base.Draw(gameTime);

//            try
//            {
//            }
//            catch (NullReferenceException nre)
//            {
//            }
//            catch (Exception e)
//            {
//            }
//        }

//        #region Properties
//        public Texture2D Texture
//        {
//            get
//            {
//                return m_texture;
//            }
//            set
//            {
//                m_texture = value;
//            }
//        }

//        public Effect Effect
//        {
//            get
//            {
//                return m_effect;
//            }
//            set
//            {
//                m_effect = value;
//            }
//        }
        
//        public Matrix View
//        {
//            get
//            {
//                return m_view;
//            }
//            set
//            {
//                m_view = value;
//            }
//        }

//        public Matrix Projection
//        {
//            get
//            {
//                return m_proj;
//            }
//            set
//            {
//                m_proj = value;
//            }
//        }
//        #endregion
//    }
//}



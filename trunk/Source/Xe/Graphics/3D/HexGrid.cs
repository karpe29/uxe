
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;
#endregion

namespace Xe.Graphics3D
{
    public partial class HexGrid : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        private Color m_color = Color.Red;

        private VertexDeclaration m_vDec;
        private VertexPositionColor[] m_verts;
        private VertexBuffer m_vBuffer;

        private ContentManager m_conManager;

        private Effect m_effect;
        private string m_effectSrc;

        private int m_totalVerts = 0;

        private int m_gridSize = 20;
        private float m_tileSize = 6;

        private Vector2 m_center = new Vector2(0, 0);

        private IReporterService m_reporter;
        #endregion

		public HexGrid(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));
        }

        #region Drawing
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            bool _effectStarted = false;

            try
            {
                this.GraphicsDevice.VertexDeclaration = m_vDec;

                m_effect.Begin();
                _effectStarted = true;

                foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    GraphicsDevice.Vertices[0].SetSource(m_vBuffer, 0, VertexPositionColor.SizeInBytes);
                    GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, m_totalVerts / 2);

                    pass.End();
                }
                m_effect.End();
            }
            catch (OutOfVideoMemoryException oovm)
            {
                if (_effectStarted)
                    m_effect.End();

                if (m_reporter != null)
                    m_reporter.BroadcastError(this, "Your video card does not support a HexGrid this big!", oovm, true);
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, e.Message, e, true);
            }
        }
        #endregion

        public void SetMatrices(Matrix view, Matrix projection, Matrix world)
        {
            if (m_effect.Parameters["WorldViewProj"] != null)
                m_effect.Parameters["WorldViewProj"].SetValue(view * projection);
        }

        #region Load / Unload Content
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                FillBuffer(m_center, m_gridSize, m_tileSize);

                //m_vBuffer.SetData<VertexPositionColor>(m_verts);

                m_conManager = new ContentManager(this.Game.Services);

                LoadEffect();
            }
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if (unloadAllContent)
            {
                if (m_conManager != null)
                    m_conManager.Unload();

                if (m_vBuffer != null)
                    m_vBuffer.Dispose();

                if (m_effect != null)
                    m_effect.Dispose();

                m_verts = null;
            }
        }

        protected virtual void LoadEffect()
        {
            if (!String.IsNullOrEmpty(m_effectSrc))
                m_effect = m_conManager.Load<Effect>(m_effectSrc);
        }

        public virtual void ResetBuffer()
        {
            if (m_verts != null && m_verts.Length > 0)
            {
                m_vBuffer = new VertexBuffer(this.GraphicsDevice, VertexPositionColor.SizeInBytes * m_verts.Length,
                                            ResourceUsage.None, ResourceManagementMode.Automatic);

                m_vBuffer.SetData<VertexPositionColor>(m_verts);
            }
        }

        protected virtual void FillBuffer(Vector2 center, int gridSize, float tileSize)
        {
            int _size = gridSize * gridSize * 12;
            m_verts = new VertexPositionColor[_size];

            int index = 0;
            for (float i = 0; i < gridSize; i++)
            {
                float y = i * tileSize - tileSize / 3 + (center.Y - (gridSize * tileSize) / 2);
                for (float k = 0; k < gridSize; k++)
                {
                    float x = k * tileSize + (center.X - (gridSize * tileSize) / 2);

                    if (i % 2 != 0)
                    {
                        x += tileSize / 2;
                        //y -= 0.5f;
                    }

                    #region Draw the Hex
                    // Line One
                    m_verts[index] = new VertexPositionColor();
                    m_verts[index].Position = new Vector3(x - tileSize / 2, y, 0);
                    m_verts[index].Color = m_color;

                    m_verts[index + 1] = new VertexPositionColor();
                    m_verts[index + 1].Position = new Vector3(x, y - tileSize / 3, 0);
                    m_verts[index + 1].Color = m_color;

                    // Line Two
                    m_verts[index + 2] = new VertexPositionColor();
                    m_verts[index + 2].Position = new Vector3(x, y - tileSize / 3, 0);
                    m_verts[index + 2].Color = m_color;

                    m_verts[index + 3] = new VertexPositionColor();
                    m_verts[index + 3].Position = new Vector3(x + tileSize / 2, y, 0);
                    m_verts[index + 3].Color = m_color;

                    // Line Three
                    m_verts[index + 4] = new VertexPositionColor();
                    m_verts[index + 4].Position = new Vector3(x + tileSize / 2, y, 0);
                    m_verts[index + 4].Color = m_color;

                    m_verts[index + 5] = new VertexPositionColor();
                    m_verts[index + 5].Position = new Vector3(x + tileSize / 2, y + (2 * tileSize) / 3, 0);
                    m_verts[index + 5].Color = m_color;

                    // Line 4
                    m_verts[index + 6] = new VertexPositionColor();
                    m_verts[index + 6].Position = new Vector3(x + tileSize / 2, y + (2 * tileSize) / 3, 0);
                    m_verts[index + 6].Color = m_color;

                    m_verts[index + 7] = new VertexPositionColor();
                    m_verts[index + 7].Position = new Vector3(x, y + tileSize, 0);
                    m_verts[index + 7].Color = m_color;

                    // Line 5
                    m_verts[index + 8] = new VertexPositionColor();
                    m_verts[index + 8].Position = new Vector3(x, y + tileSize, 0);
                    m_verts[index + 8].Color = m_color;

                    m_verts[index + 9] = new VertexPositionColor();
                    m_verts[index + 9].Position = new Vector3(x - tileSize / 2, y + (2 * tileSize) / 3, 0);
                    m_verts[index + 9].Color = m_color;

                    // Line 6
                    m_verts[index + 10] = new VertexPositionColor();
                    m_verts[index + 10].Position = new Vector3(x - tileSize / 2, y + (2 * tileSize) / 3, 0);
                    m_verts[index + 10].Color = m_color;

                    m_verts[index + 11] = new VertexPositionColor();
                    m_verts[index + 11].Position = new Vector3(x - tileSize / 2, y, 0);
                    m_verts[index + 11].Color = m_color;
                    #endregion

                    index += 12;
                }
            }

            m_totalVerts = index;

            int blah = m_totalVerts * VertexPositionColor.SizeInBytes;

            //Console.WriteLine("Size In Bytes (VPC): " + VertexPositionColor.SizeInBytes.ToString());
            //Console.WriteLine("Total Bytes: " + blah.ToString());

            //Console.WriteLine("Grid's Total Vert Count: " + m_totalVerts.ToString());

            m_vBuffer = new VertexBuffer(this.GraphicsDevice, VertexPositionColor.SizeInBytes * m_verts.Length,
                                        ResourceUsage.None, ResourceManagementMode.Automatic);

            m_vBuffer.SetData<VertexPositionColor>(m_verts);

            m_vDec = new VertexDeclaration(this.GraphicsDevice, VertexPositionColor.VertexElements);
        }
        #endregion

        #region Properties
        public int GridSize
        {
            get
            {
                return m_gridSize;
            }
            set
            {
                m_gridSize = value;

                //if(this.GraphicsDevice != null)
                //    FillBuffer(m_center, m_gridSize, m_tileSize);
            }
        }

        public Vector2 Center
        {
            get
            {
                return m_center;
            }
            set
            {
                m_center = value;

                //if (this.GraphicsDevice != null)
                //    FillBuffer(m_center, m_gridSize, m_tileSize);
            }
        }

        public float TileSize
        {
            get
            {
                return m_tileSize;
            }
            set
            {
                m_tileSize = value;

                //if (this.GraphicsDevice != null)
                //    FillBuffer(m_center, m_gridSize, m_tileSize);
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

        public VertexPositionColor[] Vertices
        {
            get
            {
                return m_verts;
            }
            set
            {
                m_verts = value;
            }
        }

        public Color Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;

                if (m_verts != null && m_verts.Length > 0)
                    for (int i = 0; i < m_verts.Length; i++)
                        m_verts[i].Color = m_color;
            }
        }
        #endregion
    }
}
#region License
/*
 *  Xna5D.Graphics3D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 07, 2006
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Graphics3D
{
    public partial class Grid3D : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        private Grid3DAxis m_axis = Grid3DAxis.XY;
        private Color m_color = Color.White;

        private VertexDeclaration m_vDec;
        private VertexPositionColor[] m_verts;
        private VertexBuffer m_vBuffer;

        private BasicEffect m_effect;
        #endregion

		public Grid3D(Game game)
            : base(game)
        {
        }

        #region Load / Unload Graphics Content
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                m_verts = new VertexPositionColor[132];

                switch (m_axis)
                {
                    case Grid3DAxis.XY:
                        FillBufferForXYPlane(m_verts, 32);
                        break;
                    case Grid3DAxis.XZ:
                        FillBufferForXZPlane(m_verts, 32);
                        break;
                    case Grid3DAxis.YZ:
                        FillBufferForYZPlane(m_verts, 32);
                        break;
                }

                m_vDec = new VertexDeclaration(this.GraphicsDevice, VertexPositionColor.VertexElements);

                m_vBuffer = new VertexBuffer(this.GraphicsDevice, VertexPositionColor.SizeInBytes * m_verts.Length,
                                             ResourceUsage.None, ResourceManagementMode.Automatic);

                m_vBuffer.SetData<VertexPositionColor>(m_verts);

                LoadEffect();
            }
        }

        #region Effect Loading
        private void LoadEffect()
        {
            m_effect = new BasicEffect(GraphicsDevice, null);
            m_effect.Alpha = 1.0f;
            m_effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            m_effect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            m_effect.SpecularPower = 5.0f;
            m_effect.AmbientLightColor = new Vector3(0.75f, 0.75f, 0.75f);

            m_effect.DirectionalLight0.Enabled = true;
            m_effect.DirectionalLight0.DiffuseColor = Vector3.One;
            m_effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));
            m_effect.DirectionalLight0.SpecularColor = Vector3.One;

            m_effect.DirectionalLight1.Enabled = true;
            m_effect.DirectionalLight1.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            m_effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1.0f, -1.0f, 1.0f));
            m_effect.DirectionalLight1.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);

            m_effect.LightingEnabled = true;
        }
        #endregion

        #region Fill Buffers
        private void FillBufferForXYPlane(VertexPositionColor[] data, int gridSize)
        {
            int index = 0;
            float yPos = 1;
            float xPos = 1;
            int gridSizeHalfed = gridSize / 2;

            // Draw y zero line
            data[index++] = new VertexPositionColor(new Vector3(gridSizeHalfed, 0, 0), m_color);
            data[index++] = new VertexPositionColor(new Vector3(-gridSizeHalfed, 0, 0), m_color);

            // Draw rows in positive y
            for (int row = 0; row < gridSizeHalfed; row++)
            {
                data[index++] = new VertexPositionColor(new Vector3(gridSizeHalfed, yPos, 0), m_color);
                data[index++] = new VertexPositionColor(new Vector3(-gridSizeHalfed, yPos, 0), m_color);
                yPos++;
            }

            yPos = -1;

            // Draw rows in negative y
            for (int row = 0; row < gridSizeHalfed; row++)
            {
                data[index++] = new VertexPositionColor(new Vector3(gridSizeHalfed, yPos, 0), m_color);
                data[index++] = new VertexPositionColor(new Vector3(-gridSizeHalfed, yPos, 0), m_color);
                yPos--;
            }

            // Draw x zero line
            data[index++] = new VertexPositionColor(new Vector3(0, gridSizeHalfed, 0), m_color);
            data[index++] = new VertexPositionColor(new Vector3(0, -gridSizeHalfed, 0), m_color);

            // Draw columns in positive x
            for (int col = 0; col < gridSizeHalfed; col++)
            {
                data[index++] = new VertexPositionColor(new Vector3(xPos, gridSizeHalfed, 0), m_color);
                data[index++] = new VertexPositionColor(new Vector3(xPos, -gridSizeHalfed, 0), m_color);
                xPos++;
            }

            xPos = -1;

            // Draw columns in negative x
            for (int col = 0; col < gridSizeHalfed; col++)
            {
                data[index++] = new VertexPositionColor(new Vector3(xPos, gridSizeHalfed, 0), m_color);
                data[index++] = new VertexPositionColor(new Vector3(xPos, -gridSizeHalfed, 0), m_color);
                xPos--;
            }
        }

        private void FillBufferForXZPlane(VertexPositionColor[] data, int gridSize)
        {
            int index = 0;
            float zPos = 1;
            float xPos = 1;
            int gridSizeHalfed = gridSize / 2;

            // Draw x zero line
            data[index++] = new VertexPositionColor(new Vector3(gridSizeHalfed, 0, 0), m_color);
            data[index++] = new VertexPositionColor(new Vector3(-gridSizeHalfed, 0, 0), m_color);

            // Draw rows in positive Z
            for (int row = 0; row < gridSizeHalfed; row++)
            {
                data[index++] = new VertexPositionColor(new Vector3(gridSizeHalfed, 0, zPos), m_color);
                data[index++] = new VertexPositionColor(new Vector3(-gridSizeHalfed, 0, zPos), m_color);
                zPos++;
            }

            zPos = -1;

            // Draw rows in negative z
            for (int row = 0; row < gridSizeHalfed; row++)
            {
                data[index++] = new VertexPositionColor(new Vector3(gridSizeHalfed, 0, zPos), m_color);
                data[index++] = new VertexPositionColor(new Vector3(-gridSizeHalfed, 0, zPos), m_color);
                zPos--;
            }

            // Draw z zero line
            data[index++] = new VertexPositionColor(new Vector3(0, 0, gridSizeHalfed), m_color);
            data[index++] = new VertexPositionColor(new Vector3(0, 0, -gridSizeHalfed), m_color);

            // Draw columns in positive x
            for (int col = 0; col < gridSizeHalfed; col++)
            {
                data[index++] = new VertexPositionColor(new Vector3(xPos, 0, gridSizeHalfed), m_color);
                data[index++] = new VertexPositionColor(new Vector3(xPos, 0, -gridSizeHalfed), m_color);
                xPos++;
            }

            xPos = -1;

            // Draw columns in negative x
            for (int col = 0; col < gridSizeHalfed; col++)
            {
                data[index++] = new VertexPositionColor(new Vector3(xPos, 0, gridSizeHalfed), m_color);
                data[index++] = new VertexPositionColor(new Vector3(xPos, 0, -gridSizeHalfed), m_color);
                xPos--;
            }
        }

        private void FillBufferForYZPlane(VertexPositionColor[] data, int gridSize)
        {
            int index = 0;
            float zPos = 1;
            float yPos = 1;
            int gridSizeHalfed = gridSize / 2;

            // Draw y zero line
            data[index++] = new VertexPositionColor(new Vector3(0, gridSizeHalfed, 0), m_color);
            data[index++] = new VertexPositionColor(new Vector3(0, -gridSizeHalfed, 0), m_color);

            // Draw cols in positive Z
            for (int row = 0; row < gridSizeHalfed; row++)
            {
                data[index++] = new VertexPositionColor(new Vector3(0, gridSizeHalfed, zPos), m_color);
                data[index++] = new VertexPositionColor(new Vector3(0, -gridSizeHalfed, zPos), m_color);
                zPos++;
            }

            zPos = -1;

            // Draw cols in negative z
            for (int row = 0; row < gridSizeHalfed; row++)
            {
                data[index++] = new VertexPositionColor(new Vector3(0, gridSizeHalfed, zPos), m_color);
                data[index++] = new VertexPositionColor(new Vector3(0, -gridSizeHalfed, zPos), m_color);
                zPos--;
            }

            // Draw z zero line
            data[index++] = new VertexPositionColor(new Vector3(0, 0, gridSizeHalfed), m_color);
            data[index++] = new VertexPositionColor(new Vector3(0, 0, -gridSizeHalfed), m_color);

            // Draw rows in positive y
            for (int col = 0; col < gridSizeHalfed; col++)
            {
                data[index++] = new VertexPositionColor(new Vector3(0, yPos, gridSizeHalfed), m_color);
                data[index++] = new VertexPositionColor(new Vector3(0, yPos, -gridSizeHalfed), m_color);
                yPos++;
            }

            yPos = -1;

            // Draw columns in negative x
            for (int col = 0; col < gridSizeHalfed; col++)
            {
                data[index++] = new VertexPositionColor(new Vector3(0, yPos, gridSizeHalfed), m_color);
                data[index++] = new VertexPositionColor(new Vector3(0, yPos, -gridSizeHalfed), m_color);
                yPos--;
            }
        }
        #endregion

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if (unloadAllContent)
            {
                if(m_effect != null)
                    m_effect.Dispose();

                if (m_vDec != null)
                    m_vDec.Dispose();

                if (m_vBuffer != null)
                    m_vBuffer.Dispose();
            }
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.VertexDeclaration = m_vDec;

            m_effect.Begin();
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                GraphicsDevice.Vertices[0].SetSource(m_vBuffer, 0, VertexPositionColor.SizeInBytes);
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 66);

                pass.End();
            }
            m_effect.End();
        }
        #endregion

        #region Properties
        public void SetMatrices(Matrix view, Matrix projection)
        {
            m_effect.World = Matrix.CreateTranslation(0, 0, 0);
            m_effect.View = view;
            m_effect.Projection = projection;
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

                m_effect.DiffuseColor = new Vector3((float)Color.R / 255,
                                                    (float)Color.G / 255,
                                                    (float)Color.B / 255);
            }
        }

        public Grid3DAxis Axis
        {
            get
            {
                return m_axis;
            }
            set
            {
                m_axis = value;
            }
        }
        #endregion
    }

    public enum Grid3DAxis
    {
        XY,
        XZ,
        YZ,
    }
}



#region License
/*
 *  Xna5D.Graphics3D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: January 29, 2006
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XeFramework.Graphics3D
{
    public static class Tesselate
    {
        public static void CreateQuad(int numTimes, Vector2 topLeft, Vector2 botRight, out VertexPositionTexture[] verts, out short[] ib)
        {
            CreateQuad(numTimes, topLeft, botRight, new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f), out verts, out ib);
        }

        public static void CreateQuad(int numTimes, Vector2 topLeft, Vector2 botRight, Vector2 topLeftUV, Vector2 botRightUV, out VertexPositionTexture[] verts, out short[] ib)
        {
            // Common Equations, (N+2)^2
            int N = (numTimes + 2);
            int N2 = N * N;

            // Create the Vertex List
            verts = new VertexPositionTexture[N2];

            // The temporary 2D array to help build the IndexBuffer
            short[,] _tempVerts = new short[N, N];

            // And the Index Buffer (2N^2+4N+2)
            ib = new short[((2 * numTimes * numTimes) + 4 * numTimes + 2) * 3];

            // Max - Mins
            Vector2 _maxMin = botRightUV - topLeftUV;

            int row = 0, col = 0;
            int index = 0;
            // For each Row of Verts
            for (row = 0; row < N; row++)
            {
                // And each Column
                for (col = 0; col < N; col++)
                {
                    // Build the position
                    Vector3 _pos = new Vector3(topLeft.X + ((botRight.X - topLeft.X) / (N - 1) * col),
                                               topLeft.Y + ((botRight.Y - topLeft.Y) / (N - 1) * row),
                                               0);

                    //Console.WriteLine("Vertex: " + _pos.X.ToString() + ", " + _pos.Y.ToString());
                    float _colPercent = (float)col / (N - 1);
                    float _rowPercent = (float)row / (N - 1);

                    //Vector2 _texCoord = new Vector2((float)col / (N - 1) + topLeft.X, (float)row / (N - 1) + topLeft.Y);
                    Vector2 _texCoord = new Vector2(topLeftUV.X + _colPercent * _maxMin.X, topLeftUV.Y + _rowPercent * _maxMin.Y);

                    // Create the vertex
                    verts[index] = new VertexPositionTexture(_pos, _texCoord);

                    // Set the index.
                    _tempVerts[col, row] = (short)index;

                    // Increase the index
                    index++;
                }
            }

            index = 0;
            // Starting in the bottom right corner of the IndexBuffer helper array
            // Rows
            for (row = N - 1; row > 0; row--)
            {
                // Then Columns
                for (col = N - 1; col > 0; col--)
                {
                    // Bottom Right
                    ib[index] = _tempVerts[col, row];

                    // Bottom Left
                    ib[index + 1] = _tempVerts[col - 1, row];

                    // Top Left
                    ib[index + 2] = _tempVerts[col - 1, row - 1];

                    // Top Left
                    ib[index + 3] = _tempVerts[col - 1, row - 1];

                    // Top Right
                    ib[index + 4] = _tempVerts[col, row - 1];

                    // Bottom Right
                    ib[index + 5] = _tempVerts[col, row];

                    // Increase index.
                    index += 6;
                }
            }
        }

        public static void CreateQuad(int numTimes, Vector2 topLeft, Vector2 botRight, out VertexPositionColor[] verts, out short[] ib, Color color)
        {
            // Common Equations, (N+2)^2
            int N = (numTimes + 2);
            int N2 = N * N;

            // Create the Vertex List
            verts = new VertexPositionColor[N2];

            // The temporary 2D array to help build the IndexBuffer
            short[,] _tempVerts = new short[N, N];

            // And the Index Buffer
            ib = new short[((2 * numTimes * numTimes) + 4 * numTimes + 2) * 3];

            int row = 0, col = 0;
            int index = 0;
            // For each Row of Verts
            for (row = 0; row < N; row++)
            {
                // And each Column
                for (col = 0; col < N; col++)
                {
                    // Build the position
                    Vector3 _pos = new Vector3(topLeft.X + ((botRight.X - topLeft.X) / (N - 1) * col),
                                               topLeft.Y + ((botRight.Y - topLeft.Y) / (N - 1) * col),
                                               0);

                    // Create the vertex
                    verts[index] = new VertexPositionColor(_pos, color);

                    // Set the index.
                    _tempVerts[col, row] = (short)index;

                    // Increase the index
                    index++;
                }
            }

            index = 0;
            // Starting in the bottom right corner of the IndexBuffer helper array
            // Rows
            for (row = N - 1; row > 0; row--)
            {
                // Then Columns
                for (col = N - 1; col > 0; col--)
                {
                    // Bottom Right
                    ib[index] = _tempVerts[col, row];

                    // Bottom Left
                    ib[index + 1] = _tempVerts[col - 1, row];

                    // Top Left
                    ib[index + 2] = _tempVerts[col - 1, row - 1];

                    // Top Left
                    ib[index + 3] = _tempVerts[col - 1, row - 1];

                    // Top Right
                    ib[index + 4] = _tempVerts[col, row - 1];

                    // Bottom Right
                    ib[index + 5] = _tempVerts[col, row];

                    // Increase index.
                    index+=6;
                }
            }
        }
    }
}

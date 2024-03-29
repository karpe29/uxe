#region License
/*
 *  Xna5D.Graphics3D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 * 
 *  Big thanks to Tomas Moller and Ben Trumbore for this algorithm.
 *  Original Document: http://www.graphics.cornell.edu/pubs/1997/MT97.pdf
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: February 02, 2007
 */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Graphics3D
{
    public static class Helper3D
    {
        // Range of error for float errors.
        public static float Epsilon = 0.000001f;

        #region Ray Triangle Intersection Test
        /// <summary>
        /// RayTriangleIntersect: Calculates whether a ray intersects a triangle
        /// as well as where if it does.
        /// </summary>
        /// <param name="origin">The origin of the ray.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="triangle">The three vertices that make up a triangle</param>
        /// <param name="t">Distance to the intersecting point.</param>
        /// <param name="u">X coordinate of intersection.</param>
        /// <param name="v">Y coordinate of intersection.</param>
        /// <param name="testCull">Whether or not to test against culling.</param>
        /// <returns>A Boolean: True if intersection occurs, False if it does not.</returns>
        public static bool RayTriangleInterset(Vector3 origin, Vector3 direction, Vector3[] triangle, out float t, out float u, out float v, bool testCull)
        {
            return RayTriangleIntersect(origin, direction, triangle[0], triangle[1], triangle[2], out t, out u, out v, testCull);
        }

        /// <summary>
        /// RayTriangleIntersect: Calculates whether a ray intersects a triangle
        /// as well as where if it does.
        /// </summary>
        /// <param name="origin">The origin of the ray.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="triangle">The three vertices that make up a triangle</param>
        /// <param name="uvt">Vector3, X=u, Y=v, Z=t</param>
        /// <param name="testCull">Whether or not to test against culling.</param>
        /// <returns>A Boolean: True if intersection occurs, False if it does not.</returns>
        public static bool RayTriangleIntersect(Vector3 origin, Vector3 direction, Vector3[] triangle, out Vector3 uvt, bool testCull)
        {
            return RayTriangleIntersect(origin, direction, triangle[0], triangle[1], triangle[2], out uvt.Z, out uvt.X, out uvt.Y, testCull);
        }
        
        /// <summary>
        /// RayTriangleIntersect: Calculates whether a ray intersects a triangle
        /// as well as where if it does.
        /// </summary>
        /// <param name="origin">The origin of the ray.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="vert0">First vertex of the triangle.</param>
        /// <param name="vert1">Second vertex of the triangle.</param>
        /// <param name="vert2">Third vertex of the triangle.</param>
        /// <param name="t">Distance to the intersecting point.</param>
        /// <param name="u">X coordinate of intersection.</param>
        /// <param name="v">Y coordinate of intersection.</param>
        /// <param name="testCull">Whether or not to test against culling.</param>
        /// <returns>A Boolean: True if intersection occurs, False if it does not.</returns>
        public static bool RayTriangleIntersect(Vector3 origin, Vector3 direction, Vector3 vert0, Vector3 vert1, Vector3 vert2, out float t, out float u, out float v, bool testCull)
        {
            // Make sure the "out" params are set.
            t = 0; u = 0; v = 0;

            // Get vectors for the two edges that share vert0
            Vector3 edge1 = vert1 - vert0;
            Vector3 edge2 = vert2 - vert0;

            Vector3 tvec, pvec, qvec;
            float det, inv_det;

            // Begin calculating determinant
            pvec = Vector3.Cross(direction, edge2);

            // If the determinant is near zero, ray lies in plane of triangle
            det = Vector3.Dot(edge1, pvec);

            if (testCull)
            {
                if (det < Helper3D.Epsilon)
                    return false;

                tvec = origin - vert0;

                u = Vector3.Dot(tvec, pvec);
                if (u < 0.0 || u > det)
                    return false;

                qvec = Vector3.Cross(tvec, edge1);

                v = Vector3.Dot(direction, qvec);
                if (v < 0.0f || u + v > det)
                    return false;

                t = Vector3.Dot(edge2, qvec);
                inv_det = 1.0f / det;
                t *= inv_det;
                u *= inv_det;
                v *= inv_det;
            }
            else
            {
                // Account for Float rounding errors / inaccuracies.
                if (det > -Helper3D.Epsilon && det < Helper3D.Epsilon)
                    return false;

                // Get the inverse determinant
                inv_det = 1.0f / det;

                // Calculate distance from vert0 to ray origin
                tvec = origin - vert0;

                // Calculate U parameter and test bounds
                u = Vector3.Dot(tvec, pvec) * inv_det;
                if (u < 0.0f || u > 1.0f)
                    return false;

                // Prepare for v
                qvec = Vector3.Cross(tvec, edge1);

                // Calculate V parameter and test bounds
                v = Vector3.Dot(direction, qvec) * inv_det;
                if (v < 0.0f || u + v > 1.0f)
                    return false;

                // Calculate t, ray intersects triangle.
                t = Vector3.Dot(edge2, qvec) * inv_det;
            }

            return true;
        }
        #endregion
    }
}

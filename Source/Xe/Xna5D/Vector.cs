#region License
/*
 *  Xna5D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 03, 2006
*/
#endregion

#region Libraries
using System;

using Microsoft.Xna.Framework;
#endregion

namespace XeFramework.Geometry
{
    public class Vector
    {
        #region Members
        /// <summary>
        /// X Component
        /// </summary>
        protected double m_x = 0.0;

        /// <summary>
        /// Y Component
        /// </summary>
        protected double m_y = 0.0;

        /// <summary>
        /// Z Component
        /// </summary>
        protected double m_z = 0.0;

        /// <summary>
        /// The Vector's ID
        /// </summary>
        protected string m_id = string.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor: Default
        /// </summary>
        public Vector()
        {
        }

        /// <summary>
        /// Constructor: 2D
        /// </summary>
        /// <param name="x">X Component</param>
        /// <param name="y">Y Component</param>
        public Vector(double x, double y)
        {
            m_x = x;
            m_y = y;
        }

        /// <summary>
        /// Constructor: 3D
        /// </summary>
        /// <param name="x">X Component</param>
        /// <param name="y">Y Component</param>
        /// <param name="z">Z Component</param>
        public Vector(double x, double y, double z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Converts the Vector to a String.
        /// </summary>
        /// <returns>A string containing the 3 components of the Vector.</returns>
        public override string ToString()
        {
            return "Vector:" + m_id + "(" + ((float)m_x).ToString() + "," + ((float)m_y).ToString() + ")";
        }

        /// <summary>
        /// Converts the Vector to an Xna.Framework.Vector3 object.
        /// </summary>
        /// <returns>A Vector3 object.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3((float)m_x, (float)m_y, (float)m_z);
        }

        /// <summary>
        /// Converts the Vector to an Xna.Framework.Vector2 object.
        /// </summary>
        /// <returns>A Vector2 object.</returns>
        public Vector2 ToVector2()
        {
            return new Vector2((float)m_x, (float)m_y);
        }

        /// <summary>
        /// Converts the Vector to an Xna.Framework.Point object.
        /// </summary>
        /// <returns>A Point object.</returns>
        public Point ToPoint()
        {
            return new Point((int)m_x, (int)m_y);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <returns></returns>
        public double Normalize()
        {
            double _len = Length;

            if (_len == 0.0f)
                return 0.0f;

            double _val = (1.0 / _len);
            m_x *= _val;
            m_y *= _val;
            m_z *= _val;

            return _len;
        }

        /// <summary>
        /// Gets the normal of the vector in 2D space.
        /// </summary>
        /// <returns>A vector representing a Vector perpindicular to this Vector.</returns>
        public Vector Normal2D()
        {
            return new Vector(-m_y, m_x);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The ID
        /// </summary>
        public string ID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        /// <summary>
        /// The X Component
        /// </summary>
        public double X
        {
            get
            {
                return m_x;
            }
            set
            {
                m_x = value;
            }
        }

        /// <summary>
        /// The Y Component
        /// </summary>
        public double Y
        {
            get
            {
                return m_y;
            }
            set
            {
                m_y = value;
            }
        }

        /// <summary>
        /// The Z Component
        /// </summary>
        public double Z
        {
            get
            {
                return m_y;
            }
            set
            {
                m_y = value;
            }
        }

        /// <summary>
        /// The magnitude of the vector.
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(m_x * m_x + m_y * m_y + m_z * m_z);
            }
        }
        #endregion

        #region Static Functions
        public static Vector Zero
        {
            get
            {
                return new Vector(0, 0, 0);
            }
        }

        /// <summary>
        /// Gets the Dot Product of two vectors.
        /// </summary>
        /// <param name="vecA">Vector A</param>
        /// <param name="vecB">Vector B</param>
        /// <returns>The Dot Product of two Vectors.</returns>
        public static double DotProduct(Vector vecA, Vector vecB)
        {
            return (vecA.X * vecB.X) + (vecA.Y * vecB.Y) + (vecA.Z * vecB.Z);
        }

        /// <summary>
        /// Gets the Cross Product of two Vectors in 2D.
        /// </summary>
        /// <param name="vecA">Vector A</param>
        /// <param name="vecB">Vector B</param>
        /// <returns>The Cross Product of two vectors.</returns>
        public static double CrossProduct2D(Vector vecA, Vector vecB)
        {
            return (vecA.X * vecB.Y) - (vecA.Y * vecB.X);
        }

        /// <summary>
        /// Multiply Operator
        /// </summary>
        /// <param name="vecA">Vector A</param>
        /// <param name="dblB">The Double</param>
        /// <returns>Vector A * B</returns>
        public static Vector operator *(Vector vecA, double dblB)
        {
            return new Vector(vecA.X * dblB, vecA.Y * dblB, vecA.Z * dblB);
        }

        /// <summary>
        /// Multiply Operator
        /// </summary>
        /// <param name="dblB">The Double</param>
        /// <param name="vecA">Vector A</param>
        /// <returns>Vector A * B</returns>
        public static Vector operator *(double dblB, Vector vecA)
        {
            return new Vector(vecA.X * dblB, vecA.Y * dblB, vecA.Z * dblB);
        }

        /// <summary>
        /// Multiply Operator
        /// </summary>
        /// <param name="vecA">Vector A</param>
        /// <param name="vecB">Vector B</param>
        /// <returns>Product of two vectors.</returns>
        public static Vector operator *(Vector vecA, Vector vecB)
        {
            return new Vector(vecA.X * vecB.X, vecA.Y * vecB.Y, vecA.Z * vecB.Z);
        }

        /// <summary>
        /// Subtraction Operator
        /// </summary>
        /// <param name="vecA">Vector A</param>
        /// <param name="vecB">Vector B</param>
        /// <returns>The result of the subtraction of two vectors.</returns>
        public static Vector operator -(Vector vecA, Vector vecB)
        {
            return new Vector(vecA.X - vecB.X, vecA.Y - vecB.Y, vecA.Z - vecB.Z);
        }

        /// <summary>
        /// Addition Operator
        /// </summary>
        /// <param name="vecA">Vector A</param>
        /// <param name="vecB">Vector B</param>
        /// <returns>The sum of two vectors.</returns>
        public static Vector operator +(Vector vecA, Vector vecB)
        {
            return new Vector(vecA.X + vecB.X, vecA.Y + vecB.Y, vecA.Z + vecB.Z);
        }

        /// <summary>
        /// Negation Operator
        /// </summary>
        /// <param name="vecA">Vector to be negated.</param>
        /// <returns>A negated vector.</returns>
        public static Vector operator -(Vector vecA)
        {
            return new Vector(-vecA.X, -vecA.Y);
        }

        /// <summary>
        /// Less Than Operator
        /// </summary>
        /// <param name="vecA">Vector A</param>
        /// <param name="dblB">Double B</param>
        /// <returns>True if the Vector is less than the double.</returns>
        public static bool operator <(Vector vecA, float dblB)
        {
            if (vecA.Length < dblB)
                return true;

            return false;
        }

        /// <summary>
        /// Greater Than Operator
        /// </summary>
        /// <param name="vecA">Vector A</param>
        /// <param name="dblB">Double B</param>
        /// <returns>True  if the Vector is greater than the double.</returns>
        public static bool operator >(Vector vecA, float dblB)
        {
            if (vecA.Length > dblB)
                return true;

            return false;
        }
        #endregion
    }
}

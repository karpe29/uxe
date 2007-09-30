#region Using Statements
using System;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.GUI
{
    public class Rect
    {
        private float m_x;
        private float m_y;
        private float m_width;
        private float m_height;

        public Rect()
            : this(0, 0, 0, 0)
        {
        }

        public Rect(float x, float y, float width, float height)
        {
            m_x = x;
            m_y = y;
            m_width = width;
            m_height = height;
        }

        public Vector4 ToVector4()
        {
            return new Vector4(m_x, m_y, m_width, m_height);
        }

        public float X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public float Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public float Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        public float Height
        {
            get { return m_height; }
            set { m_height = value; }
        }
    }
}

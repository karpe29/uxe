#region Using Statements
using System;
using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Xe.GUI
{
    public class Frame
    {
        private Rectangle m_srcRect = Rectangle.Empty;
        private float m_time = 0.15f;

        public Rectangle SourceRect
        {
            get { return m_srcRect; }
            set { m_srcRect = value; }
        }

        public float TimeSpan
        {
            get { return m_time; }
            set { m_time = value; }
        }
    }
}

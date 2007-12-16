#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Xe;
#endregion

namespace Xe.Gui
{
    public class QuadBase : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        private float m_x = 0.0f;
        private float m_y = 0.0f;
        private float m_z = 0.0f;
        private float m_width = 0.0f;
        private float m_height = 0.0f;

        private float m_u = 0.0f;
        private float m_v = 0.0f;
        private float m_uvWidth = 0.0f;
        private float m_uvHeight = 0.0f;

        private float m_rotX = 0.0f;
        private float m_rotY = 0.0f;
        private float m_rotZ = 0.0f;

        private Color m_color = Color.White;

        private Rectangle m_destRect = new Rectangle();
        private Rectangle m_srcRect = new Rectangle();

        private Vector2 m_center = new Vector2(0,0);
        private Vector2 m_localCenter = new Vector2(0, 0);
        private Texture2D m_texture;
        #endregion

        #region Constructors
        public QuadBase(Game game)
            : base(game)
        {
        }

        public QuadBase(Game game, float x, float y, float width, float height)
            : this(game)
        {
            m_x = x;
            m_y = y;
            m_width = width;
            m_height = height;

            SetCenter();
        }

        public QuadBase(Game game, float x, float y, float z, float width, float height)
            : this(game, x, y, width, height)
        {
            m_z = z;
        }
        #endregion

        #region Methods
        protected virtual void SetCenter()
        {
            m_center.X = m_x + m_width / 2;
            m_center.Y = m_y + m_height / 2;

            SetLocalCenter();
        }

        protected virtual void SetLocalCenter()
        {
            m_localCenter.X = m_width/ 2;
            m_localCenter.Y = m_height / 2;
        }
        #endregion

        #region Properties
        #region Positioning
        /// <summary>
        /// Gets the center of the quad in the XY plane.
        /// </summary>
        public Vector2 Center
        {
            get { return m_center; }
        }

        public Vector2 LocalCenter
        {
            get { return m_localCenter; }
        }

        /// <summary>
        /// Gets or Sets the X position of the Quad.
        /// </summary>
        public float X
        {
            get { return m_x; }
            set
            {
                m_x = value;

                SetCenter();
            }
        }

        /// <summary>
        /// Gets or Sets the Y position of the Quad.
        /// </summary>
        public float Y
        {
            get { return m_y; }
            set
            {
                m_y = value;

                SetCenter();
            }
        }

        /// <summary>
        /// Gets or Sets the Z position of the Quad.
        /// </summary>
        public float Z
        {
            get { return m_z; }
            set { m_z = value; }
        }

        /// <summary>
        /// Gets or Sets the Width of the Quad.
        /// </summary>
        public float Width
        {
            get { return m_width; }
            set
            {
                m_width = value;

                SetCenter();
            }
        }

        /// <summary>
        /// Gets or Sets the Height of the Quad.
        /// </summary>
        public float Height
        {
            get { return m_height; }
            set
            {
                m_height = value;

                SetCenter();
            }
        }
        #endregion

        #region Rotation
        /// <summary>
        /// Gets or Sets the rotation in radians
        /// on the X axis.
        /// </summary>
        public float RotationX
        {
            get { return m_rotX; }
            set { m_rotX = value; }
        }

        /// <summary>
        /// Gets or Sets the rotation in radians
        /// on the Y axis.
        /// </summary>
        public float RotationY
        {
            get { return m_rotY; }
            set { m_rotY = value; }
        }

        /// <summary>
        /// Gets or Sets the rotation in radians on the
        /// Z axis.
        /// </summary>
        public float RotationZ
        {
            get { return m_rotZ; }
            set { m_rotZ = value; }
        }
        #endregion

        #region Texture
        /// <summary>
        /// Gets or Sets the color of the quad.
        /// </summary>
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        /// <summary>
        /// Gets or Sets the texture to draw from. Resets the UV
        /// coordinates as well if the width or height is zero.
        /// </summary>
        public Texture2D Texture
        {
            get { return m_texture; }
            set
            {
                m_texture = value;

                if(m_uvWidth == 0)
                    m_uvWidth = m_texture.Width;

                if(m_uvHeight == 0)
                    m_uvHeight = m_texture.Height;
            }
        }

        /// <summary>
        /// Gets or Sets the X coordinate of the
        /// source texture.
        /// </summary>
        public float U
        {
            get { return m_u; }
            set { m_u = value; }
        }

        /// <summary>
        /// Gets or Sets the Y coordinate of the
        /// source texture.
        /// </summary>
        public float V
        {
            get { return m_v; }
            set { m_v = value; }
        }

        /// <summary>
        /// Gets or Sets the width of the source texture.
        /// </summary>
        public float UVWidth
        {
            get { return m_uvWidth; }
            set { m_uvWidth = value; }
        }

        /// <summary>
        /// Gets or Sets the height of the source texture.
        /// </summary>
        public float UVHeight
        {
            get { return m_uvHeight; }
            set { m_uvHeight = value; }
        }
        #endregion

        #region Rectangles
        /// <summary>
        /// Gets the Destination Rectangle.
        /// </summary>
        public Rectangle DestRect
        {
            get
            {
                m_destRect.X = (int)(m_x);
                m_destRect.Y = (int)(m_y);
                m_destRect.Width = (int)(m_width);
                m_destRect.Height = (int)(m_height);

                return m_destRect;
            }
        }

        /// <summary>
        /// Gets the Source Rectangle.
        /// </summary>
        public Rectangle SrcRect
        {
            get
            {
                m_srcRect.X = (int)m_u;
                m_srcRect.Y = (int)m_v;
                m_srcRect.Width = (int)m_uvWidth;
                m_srcRect.Height = (int)m_uvHeight;

                return m_srcRect;
            }
        }
        #endregion
        #endregion
    }
}
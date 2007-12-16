#region License
/*
 *  Xna5D.Graphics2D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 01, 2006
*/
#endregion License

#region Libraries
using System;
using System.Collections.Generic;

using Xe;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Graphics2D
{
    public class QuadBase : IDisposable
    {
        #region Members
        private Texture2D m_texture;
        private string m_texSource;

        private Rectangle m_destRect = Rectangle.Empty;
        private Rectangle m_srcRect = Rectangle.Empty;

        private Color m_color = Color.White;
        #endregion

        public QuadBase()
        {
        }

		public void Dispose()
		{
			if (Texture != null && !Texture.IsDisposed)
				Texture.Dispose();
		}

        #region Properties
        public Texture2D Texture
        {
            get
            {
                return m_texture;
            }
            set
            {
                m_texture = value;
            }
        }

        public string TextureSource
        {
            get
            {
                return m_texSource;
            }
            set
            {
                m_texSource = value;
            }
        }

        public Rectangle Destination
        {
            get
            {
                return m_destRect;
            }
            set
            {
                m_destRect = value;
            }
        }

        public Rectangle Source
        {
            get
            {
                return m_srcRect;
            }
            set
            {
                m_srcRect = value;
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
            }
        }
        #endregion
	}
}

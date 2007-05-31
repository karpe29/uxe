#region License
/*
 *  Xna5D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: November 30, 2006
*/
#endregion License

#region Libraries
using System;
using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XeFramework.Graphics2D
{
    public static class Font2D
    {
        #region Members
        private static BitmapFont m_font;
        private static GraphicsDevice m_device;
        #endregion

        /// <summary>
        /// Loads a Bitmap Font into a BitmapFont object.
        /// </summary>
        /// <param name="device">A valid Graphics Device</param>
        /// <param name="xmlFile">The Font describing XML File</param>
        /// <returns>A boolean: True if the font was loaded, false if not.</returns>
        public static bool LoadFont(GraphicsDevice device, string xmlFile)
        {
            try
            {
                if (m_font == null)
                {
					m_font = new BitmapFont(xmlFile);

                    if (m_device == null)
                    {
                        m_device = device;
                        m_device.DeviceReset += new EventHandler(m_device_DeviceReset);
                    }

                    LoadFont();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

		private static void LoadFont()
		{
			m_font.Reset(m_device);
		}
        
        private static void m_device_DeviceReset(object sender, EventArgs e)
        {
            LoadFont();
        }

        /// <summary>
        /// Retrieve a Bitmap Font from the collection.
        /// </summary>
        /// <param name="fontName">The name of the font to retrieve.</param>
        /// <returns>A BitmapFont object.</returns>
       	public static BitmapFont GetNamedFont(string fontName)
        {
            try
            {
                return BitmapFont.GetNamedFont(fontName);
            }
            catch
            {
                return null;
            }
        }
    }
}

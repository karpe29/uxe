#region License
/*
 *  Xna5D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 04, 2006
 */
#endregion

#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Tools
{
    public static class XmlLoader
    {
        /// <summary>
        /// Builds a Rectangle object from a valid XmlNode.
        /// </summary>
        /// <param name="node">The Xml Node to build the rectangle from.</param>
        /// <returns>A valid Rectangle object, Rectangle.Empty if loading fails.</returns>
        public static Rectangle BuildRectangle(XmlNode node)
        {
            Rectangle _rect = Rectangle.Empty;

            if (node.Name.ToLower().Equals("rectangle") ||
               node.Name.ToLower().Equals("source") ||
               node.Name.ToLower().Equals("destination"))
            {
                foreach (XmlNode _child in node.ChildNodes)
                {
                    if (_child.Name.ToLower().Equals("x"))
                        int.TryParse(_child.FirstChild.Value, out _rect.X);
                    else if (_child.Name.ToLower().Equals("y"))
                        int.TryParse(_child.FirstChild.Value, out _rect.Y);
                    else if (_child.Name.ToLower().Equals("width"))
                        int.TryParse(_child.FirstChild.Value, out _rect.Width);
                    else if (_child.Name.ToLower().Equals("height"))
                        int.TryParse(_child.FirstChild.Value, out _rect.Height);
                }
            }

            return _rect;
        }
    }
}

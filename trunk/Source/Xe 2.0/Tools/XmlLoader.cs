#region Using Statements
using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Tools.Xml
{
    /// <summary>
    /// XmlLoader provides basic, default functions for loading and parsing
    /// XML filesl
    /// </summary>
    public static class XmlLoader
    {
        #region Building a Rectangle
        /// <summary>
        /// Builds a Rectangle object from a valid XmlNode.
        /// </summary>
        /// <param name="node">The Xml Node to build the rectangle from.</param>
        /// <returns>A valid Rectangle object, Rectangle.Empty if loading fails.</returns>
        public static Rectangle BuildRectangle(XmlNode node)
        {
            try
            {
                Rectangle _rect = Rectangle.Empty;

                // Search for the child nodes and load the data.
                foreach (XmlNode _child in node.ChildNodes)
                {
                    if (_child.Name.ToLower().Equals("x"))
                        Parser.ParseInt(_child.FirstChild.Value, out _rect.X);
                    else if (_child.Name.ToLower().Equals("y"))
                        Parser.ParseInt(_child.FirstChild.Value, out _rect.Y);
                    else if (_child.Name.ToLower().Equals("width"))
                        Parser.ParseInt(_child.FirstChild.Value, out _rect.Width);
                    else if (_child.Name.ToLower().Equals("height"))
                        Parser.ParseInt(_child.FirstChild.Value, out _rect.Height);
                }

                return _rect;
            }
            catch
            {
                return Rectangle.Empty;
            }
        }
        #endregion

        public static string GetAttribute(XmlNode node, string attr)
        {
            string _value = (node.Attributes[attr] != null) ? node.Attributes[attr].Value : string.Empty;

            return _value;
        }

        //public static T GetAttribute<T>(XmlNode node, string attr)
        //{
        //    return (T)(object)GetAttribute(node, attr);
        //}
    }
}

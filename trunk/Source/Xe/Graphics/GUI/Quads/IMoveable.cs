#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Gui
{
    public interface IMoveable
    {
        float X { get; set; }
        float Y { get; set; }
        float Width { get; set; }
        float Height { get; set; }

        Vector2 InitialPos { get; }
        Vector2 InitialSize { get; }

        /*float Alpha { get; set; }
        Color Color { get; set; }*/
    }
}

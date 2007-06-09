using System;
using System.Collections.Generic;
using System.Text;

namespace Xe.GUI
{
    public enum Dock
    {
        Left,
        Right,
        Top,
        Bottom,
        Fill,
        None
    }

    [FlagsAttribute]
    public enum Anchor
    {
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }
}

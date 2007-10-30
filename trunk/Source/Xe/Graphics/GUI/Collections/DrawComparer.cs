using System;
using System.Collections.Generic;

namespace Xe.Gui
{
    internal class DrawComparer : IComparer<UIControl>
    {
        #region IComparer<UIControl> Members
        public int Compare(UIControl x, UIControl y)
        {
            if (x.DrawOrder < y.DrawOrder)
                return -1;
            else if (x.DrawOrder > y.DrawOrder)
                return 1;

            return 0;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;

namespace Xe.Gui
{
    internal class TabComparer : IComparer<UIControl>
    {
        #region IComparer<UIControl> Members
        public int Compare(UIControl x, UIControl y)
        {
            if (x.TabOrder < y.TabOrder)
                return -1;
            else if (x.TabOrder > y.TabOrder)
                return 1;

            return 0;
        }
        #endregion
    }
}

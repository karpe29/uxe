using System;
using System.Collections.Generic;
using System.Text;

namespace Xe.GUI
{
    internal class UpdateComparer : IComparer<UIControl>
    {
        #region IComparer<UIControl> Members
        public int Compare(UIControl x, UIControl y)
        {
            if (x.UpdateOrder < y.UpdateOrder)
                return -1;
            else if (x.UpdateOrder > y.UpdateOrder)
                return 1;

            return 0;
        }
        #endregion
    }
}

#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

#endregion

namespace Xe.Tools
{
    public static class Parser
    {
        public static CultureInfo Culture = CultureInfo.InvariantCulture;

        public static bool ParseFloat(string text, out float value)
        {
            value = 0.0f;

            return float.TryParse(text, NumberStyles.Float, Culture.NumberFormat, out value);
        }

        public static bool ParseInt(string text, out int value)
        {
            value = 0;

            return int.TryParse(text, NumberStyles.Integer, Culture.NumberFormat, out value);
        }

        public static bool ParseDouble(string text, out double value)
        {
            value = 0.0;

            return double.TryParse(text, NumberStyles.Float, Culture.NumberFormat, out value);
        }

        public static bool ParseBool(string text, out bool value)
        {
            value = false;

            return bool.TryParse(text, out value);
        }
    }
}

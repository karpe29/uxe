using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xe.Tools
{
	static class Helper
	{
		/// <summary>
		/// Return interpolated value based on two TimeSpan
		/// </summary>
		/// <param name="f">The max value</param>
		/// <param name="duration">The current duration</param>
		/// <param name="totalDuration">The total desired effect duration</param>
		/// <returns>A value between 0 and f</returns>
		public static float GetInterpolatedValue(float f, TimeSpan duration, TimeSpan totalDuration)
		{
			if (duration <= TimeSpan.Zero)
				return 0;

			if (duration >= totalDuration)
				return f;

			return MathHelper.SmoothStep(0, f, duration.Ticks / totalDuration.Ticks);
		}

		public static string Vector3ToString3f(Vector3 vector)
		{
			return "( "+Math.Round(vector.X,3)+" , "+ Math.Round(vector.Y,3)+" , "+Math.Round(vector.Z,3)+" )";		
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xe.Tools
{
	public static class Helper
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

		/// <summary>
		/// Return a string containing all values of a Vector3
		/// </summary>
		/// <param name="vector">The vector containing values</param>
		/// <returns>a string formatted : "( x , y , z )"</returns>
		public static string Vector3ToString3f(Vector3 vector)
		{
			return "( "+Math.Round(vector.X,3)+" , "+ Math.Round(vector.Y,3)+" , "+Math.Round(vector.Z,3)+" )";		
		}

		private static Random random = new Random((int)DateTime.Now.Ticks);

		/// <summary>
		/// Start a new random seed
		/// </summary>
		public static void NewSeed()
		{
			random = new Random((int)DateTime.Now.Ticks);
		}

		/// <summary>
		/// Return a random int between 0 and MaxValue
		/// </summary>
		/// <param name="MaxValue">The maximum</param>
		/// <returns>An int</returns>
		public static int Random(int MaxValue)
		{
			return Random(0, MaxValue);
		}

		/// <summary>
		/// Return a random int between MinValue and MaxValue
		/// </summary>
		/// <param name="MinValue">The minimum</param>
		/// <param name="MaxValue">The maximum</param>
		/// <returns>An int</returns>
		public static int Random(int MinValue, int MaxValue)
		{
			return random.Next(MinValue, MaxValue);
		}

		/// <summary>
		/// Return a random float between 0 and MaxValue
		/// </summary>
		/// <param name="MaxValue">The maximum</param>
		/// <returns>A float</returns>
		public static float RandomFloat(float MaxValue)
		{
			return RandomFloat(0, MaxValue);
		}

		/// <summary>
		/// Return a random float between MinValue and MaxValue
		/// </summary>
		/// <param name="MinValue">The minimum</param>
		/// <param name="MaxValue">The maximum</param>
		/// <returns>A float</returns>
		public static float RandomFloat(float MinValue, float MaxValue)
		{
			return (float)random.NextDouble() * (MaxValue - MinValue) + MinValue;
		}
	}
}

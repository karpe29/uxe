using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Tools
{
	public static class Helper
	{
		public static string KeyToString(Keys key, bool shift)
		{
			switch (key)
			{
				#region Alphabet
				case Keys.A:
					return (shift) ? "A" : "a";
				case Keys.B:
					return (shift) ? "B" : "b";
				case Keys.C:
					return (shift) ? "C" : "c";
				case Keys.D:
					return (shift) ? "D" : "d";
				case Keys.E:
					return (shift) ? "E" : "e";
				case Keys.F:
					return (shift) ? "F" : "f";
				case Keys.G:
					return (shift) ? "G" : "g";
				case Keys.H:
					return (shift) ? "H" : "h";
				case Keys.I:
					return (shift) ? "I" : "i";
				case Keys.J:
					return (shift) ? "J" : "j";
				case Keys.K:
					return (shift) ? "K" : "k";
				case Keys.L:
					return (shift) ? "L" : "l";
				case Keys.M:
					return (shift) ? "M" : "m";
				case Keys.N:
					return (shift) ? "N" : "n";
				case Keys.O:
					return (shift) ? "O" : "o";
				case Keys.P:
					return (shift) ? "P" : "p";
				case Keys.Q:
					return (shift) ? "Q" : "q";
				case Keys.R:
					return (shift) ? "R" : "r";
				case Keys.S:
					return (shift) ? "S" : "s";
				case Keys.T:
					return (shift) ? "T" : "t";
				case Keys.U:
					return (shift) ? "U" : "u";
				case Keys.V:
					return (shift) ? "V" : "v";
				case Keys.W:
					return (shift) ? "W" : "w";
				case Keys.X:
					return (shift) ? "X" : "x";
				case Keys.Y:
					return (shift) ? "Y" : "y";
				case Keys.Z:
					return (shift) ? "Z" : "z";
				#endregion

				#region Numbers
				case Keys.D0:
					return (shift) ? ")" : "0";
				case Keys.D1:
					return (shift) ? "!" : "1";
				case Keys.D2:
					return (shift) ? "@" : "2";
				case Keys.D3:
					return (shift) ? "#" : "3";
				case Keys.D4:
					return (shift) ? "$" : "4";
				case Keys.D5:
					return (shift) ? "%" : "5";
				case Keys.D6:
					return (shift) ? "^" : "6";
				case Keys.D7:
					return (shift) ? "&" : "7";
				case Keys.D8:
					return (shift) ? "*" : "8";
				case Keys.D9:
					return (shift) ? "(" : "9";
				#endregion

				#region Extra
				case Keys.OemPlus:
					return (shift) ? "+" : "=";
				case Keys.OemMinus:
					return (shift) ? "_" : "-";
				case Keys.OemOpenBrackets:
					return (shift) ? "{" : "[";
				case Keys.OemCloseBrackets:
					return (shift) ? "}" : "]";
				case Keys.OemQuestion:
					return (shift) ? "?" : "/";
				case Keys.OemPeriod:
					return (shift) ? ">" : ".";
				case Keys.OemComma:
					return (shift) ? "<" : ",";
				case Keys.OemPipe:
					return (shift) ? "|" : "\\";
				case Keys.Space:
					return " ";
				case Keys.OemSemicolon:
					return (shift) ? ":" : ";";
				case Keys.OemQuotes:
					return (shift) ? "\"" : "'";
				case Keys.OemTilde:
					return (shift) ? "~" : "`";
				#endregion

				default:
					return "";
			}
		}

		public static Color StringToColor(string color)
		{
			#region Colors
			switch (color)
			{
				case "TransparentBlack":
					return Color.TransparentBlack;
				case "TransparentWhite":
					return Color.TransparentWhite;
				case "AliceBlue":
					return Color.AliceBlue;
				case "AntiqueWhite":
					return Color.AntiqueWhite;
				case "Aqua":
					return Color.Aqua;
				case "Aquamarine":
					return Color.Aquamarine;
				case "Azure":
					return Color.Azure;
				case "Beige":
					return Color.Beige;
				case "Bisque":
					return Color.Bisque;
				case "Black":
					return Color.Black;
				case "BlanchedAlmond":
					return Color.BlanchedAlmond;
				case "Blue":
					return Color.Blue;
				case "BlueViolet":
					return Color.BlueViolet;
				case "Brown":
					return Color.Brown;
				case "BurlyWood":
					return Color.BurlyWood;
				case "CadetBlue":
					return Color.CadetBlue;
				case "Chartreuse":
					return Color.Chartreuse;
				case "Chocolate":
					return Color.Chocolate;
				case "Coral":
					return Color.Coral;
				case "CornflowerBlue":
					return Color.CornflowerBlue;
				case "Cornsilk":
					return Color.Cornsilk;
				case "Crimson":
					return Color.Crimson;
				case "Cyan":
					return Color.Cyan;
				case "DarkBlue":
					return Color.DarkBlue;
				case "DarkCyan":
					return Color.DarkCyan;
				case "DarkGoldenrod":
					return Color.DarkGoldenrod;
				case "DarkGray":
					return Color.DarkGray;
				case "DarkGreen":
					return Color.DarkGreen;
				case "DarkKhaki":
					return Color.DarkKhaki;
				case "DarkMagenta":
					return Color.DarkMagenta;
				case "DarkOliveGreen":
					return Color.DarkOliveGreen;
				case "DarkOrange":
					return Color.DarkOrange;
				case "DarkOrchid":
					return Color.DarkOrchid;
				case "DarkRed":
					return Color.DarkRed;
				case "DarkSalmon":
					return Color.DarkSalmon;
				case "DarkSeaGreen":
					return Color.DarkSeaGreen;
				case "DarkSlateBlue":
					return Color.DarkSlateBlue;
				case "DarkSlateGray":
					return Color.DarkSlateGray;
				case "DarkTurquoise":
					return Color.DarkTurquoise;
				case "DarkViolet":
					return Color.DarkViolet;
				case "DeepPink":
					return Color.DeepPink;
				case "DeepSkyBlue":
					return Color.DeepSkyBlue;
				case "DimGray":
					return Color.DimGray;
				case "DodgerBlue":
					return Color.DodgerBlue;
				case "Firebrick":
					return Color.Firebrick;
				case "FloralWhite":
					return Color.FloralWhite;
				case "ForestGreen":
					return Color.ForestGreen;
				case "Fuchsia":
					return Color.Fuchsia;
				case "Gainsboro":
					return Color.Gainsboro;
				case "GhostWhite":
					return Color.GhostWhite;
				case "Gold":
					return Color.Gold;
				case "Goldenrod":
					return Color.Goldenrod;
				case "Gray":
					return Color.Gray;
				case "Green":
					return Color.Green;
				case "GreenYellow":
					return Color.GreenYellow;
				case "Honeydew":
					return Color.Honeydew;
				case "HotPink":
					return Color.HotPink;
				case "IndianRed":
					return Color.IndianRed;
				case "Indigo":
					return Color.Indigo;
				case "Ivory":
					return Color.Ivory;
				case "Khaki":
					return Color.Khaki;
				case "Lavender":
					return Color.Lavender;
				case "LavenderBlush":
					return Color.LavenderBlush;
				case "LawnGreen":
					return Color.LawnGreen;
				case "LemonChiffon":
					return Color.LemonChiffon;
				case "LightBlue":
					return Color.LightBlue;
				case "LightCoral":
					return Color.LightCoral;
				case "LightCyan":
					return Color.LightCyan;
				case "LightGoldenrodYellow":
					return Color.LightGoldenrodYellow;
				case "LightGreen":
					return Color.LightGreen;
				case "LightGray":
					return Color.LightGray;
				case "LightPink":
					return Color.LightPink;
				case "LightSalmon":
					return Color.LightSalmon;
				case "LightSeaGreen":
					return Color.LightSeaGreen;
				case "LightSkyBlue":
					return Color.LightSkyBlue;
				case "LightSlateGray":
					return Color.LightSlateGray;
				case "LightSteelBlue":
					return Color.LightSteelBlue;
				case "LightYellow":
					return Color.LightYellow;
				case "Lime":
					return Color.Lime;
				case "LimeGreen":
					return Color.LimeGreen;
				case "Linen":
					return Color.Linen;
				case "Magenta":
					return Color.Magenta;
				case "Maroon":
					return Color.Maroon;
				case "MediumAquamarine":
					return Color.MediumAquamarine;
				case "MediumBlue":
					return Color.MediumBlue;
				case "MediumOrchid":
					return Color.MediumOrchid;
				case "MediumPurple":
					return Color.MediumPurple;
				case "MediumSeaGreen":
					return Color.MediumSeaGreen;
				case "MediumSlateBlue":
					return Color.MediumSlateBlue;
				case "MediumSpringGreen":
					return Color.MediumSpringGreen;
				case "MediumTurquoise":
					return Color.MediumTurquoise;
				case "MediumVioletRed":
					return Color.MediumVioletRed;
				case "MidnightBlue":
					return Color.MidnightBlue;
				case "MintCream":
					return Color.MintCream;
				case "MistyRose":
					return Color.MistyRose;
				case "Moccasin":
					return Color.Moccasin;
				case "NavajoWhite":
					return Color.NavajoWhite;
				case "Navy":
					return Color.Navy;
				case "OldLace":
					return Color.OldLace;
				case "Olive":
					return Color.Olive;
				case "OliveDrab":
					return Color.OliveDrab;
				case "Orange":
					return Color.Orange;
				case "OrangeRed":
					return Color.OrangeRed;
				case "Orchid":
					return Color.Orchid;
				case "PaleGoldenrod":
					return Color.PaleGoldenrod;
				case "PaleGreen":
					return Color.PaleGreen;
				case "PaleTurquoise":
					return Color.PaleTurquoise;
				case "PaleVioletRed":
					return Color.PaleVioletRed;
				case "PapayaWhip":
					return Color.PapayaWhip;
				case "PeachPuff":
					return Color.PeachPuff;
				case "Peru":
					return Color.Peru;
				case "Pink":
					return Color.Pink;
				case "Plum":
					return Color.Plum;
				case "PowderBlue":
					return Color.PowderBlue;
				case "Purple":
					return Color.Purple;
				case "Red":
					return Color.Red;
				case "RosyBrown":
					return Color.RosyBrown;
				case "RoyalBlue":
					return Color.RoyalBlue;
				case "SaddleBrown":
					return Color.SaddleBrown;
				case "Salmon":
					return Color.Salmon;
				case "SandyBrown":
					return Color.SandyBrown;
				case "SeaGreen":
					return Color.SeaGreen;
				case "SeaShell":
					return Color.SeaShell;
				case "Sienna":
					return Color.Sienna;
				case "Silver":
					return Color.Silver;
				case "SkyBlue":
					return Color.SkyBlue;
				case "SlateBlue":
					return Color.SlateBlue;
				case "SlateGray":
					return Color.SlateGray;
				case "Snow":
					return Color.Snow;
				case "SpringGreen":
					return Color.SpringGreen;
				case "SteelBlue":
					return Color.SteelBlue;
				case "Tan":
					return Color.Tan;
				case "Teal":
					return Color.Teal;
				case "Thistle":
					return Color.Thistle;
				case "Tomato":
					return Color.Tomato;
				case "Turquoise":
					return Color.Turquoise;
				case "Violet":
					return Color.Violet;
				case "Wheat":
					return Color.Wheat;
				case "White":
					return Color.White;
				case "WhiteSmoke":
					return Color.WhiteSmoke;
				case "Yellow":
					return Color.Yellow;
				case "YellowGreen":
					return Color.YellowGreen;
				default:
					return Color.White;
			}
			#endregion
		}

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
		public static string Vector3ToString(Vector3 vector)
		{
			return "( " + Math.Round(vector.X, 3) + " , " + Math.Round(vector.Y, 3) + " , " + Math.Round(vector.Z, 3) + " )";
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

		/// <summary>
		/// Return a random Vector3 on a specified sphere perimeter.
		/// </summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Vector3 RandomVector3OnSphere(Vector3 center, float radius)
		{
			float t = RandomFloat(MathHelper.TwoPi);
			float z = RandomFloat(-1.0f, 1.0f);
			float r = (float)Math.Sqrt(1.0 - z * z) * radius;

			return new Vector3(center.X + r * (float)Math.Cos(t), center.Y + r * (float)Math.Sin(t), center.Z + z * radius);
		}

		/// <summary>
		/// Return a random Vector3 in a specified sphere.
		/// </summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Vector3 RandomVector3InSphere(Vector3 center, float radius)
		{
			float t = RandomFloat(MathHelper.TwoPi);
			float z = RandomFloat(-1.0f, 1.0f);
			float r = (float)Math.Sqrt(1.0 - z * z) * RandomFloat(radius);

			return new Vector3(center.X + r * (float)Math.Cos(t), center.Y + r * (float)Math.Sin(t), center.Z + z * RandomFloat(radius));
		}

		/*private static void ShuffleList(ref List<object> l)
		{
			Random rand = new Random();
			int idx = listCard.Count;
			for (int i = 0; i < idx; i++)
			{
				int randInt = rand.Next(0, listCard.Count - 1);
				tempList.Add(listCard[randInt]);
				listCard.Remove(listCard[randInt]);
			}
			return tempList;
		}*/

		private static void ShuffleList(ref IList<object> list)
		{
			Random random = new Random();

			for (int i = 0; i < list.Count; i++)
			{
				int j = random.Next(i, list.Count);
				Object tmp;

				tmp = list[i];
				list[i] = list[j];
				list[j] = tmp;
			}
		}

		public static Color RampGreenRed(float ratio)
		{
			ratio = Math.Min(Math.Max(ratio, 0), 1);
			
			int r = 0;
			int g = 0;
			int b = 0;

			if (ratio > 0.5)
			{
				g = 255 - (int)(Math.Max(ratio - 0.5, 0) * 1.6 * 255);
				r = 255;
			}
			else
			{
				g = 255;
				r = (int)(ratio * 2 * 255);
			}

			r = Math.Min(Math.Max(r, 0), 255);
			g = Math.Min(Math.Max(g, 0), 255);
			b = Math.Min(Math.Max(b, 0), 255);

			return new Color(new Vector4(r,g,b,255));
		}
	}
}

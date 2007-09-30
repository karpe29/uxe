using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xe.GUI
{
	public class GUIQuad : AnimatedQuad
	{
		private GUIQuadType m_quadType = GUIQuadType.None;

		#region Constructors
		public GUIQuad(Game game)
			: base(game)
		{
		}

		public GUIQuad(Game game, float x, float y, float width, float height)
			: base(game, x, y, width, height)
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or Sets the GUIQuad's type. This determines how
		/// a soft reset is applied.
		/// <seealso cref="GUIQuadType"/>
		/// </summary>
		public GUIQuadType QuadType
		{
			get { return m_quadType; }
			set { m_quadType = value; }
		}
		#endregion
	}

	/// <summary>
	/// Defines the position in a control that a quad plays.
	/// </summary>
	public enum GUIQuadType
	{
		TopLeft,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
		None
	}
}

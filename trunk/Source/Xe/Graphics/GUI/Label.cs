#region Using Statements
using System;
using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Xe.Tools;
#endregion

namespace Xe.GUI
{
	public class Label : UIControl
	{
		public Label(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "label";

			this.IsFocusable = false;
			this.IsHoverable = false;
		}
	}
}

#region Using Statements
using System;
using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Xe;
using Xe.Tools;
#endregion

namespace Xe.GUI
{
	public class Button : UIControl
	{
		public Button(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "button";
			this.TextAlign = TextAlign.Center;
		}
	}
}

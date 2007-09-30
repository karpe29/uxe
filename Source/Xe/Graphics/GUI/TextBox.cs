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
using Xe.Graphics;
using Xe.Input;
#endregion

namespace Xe.GUI
{
	public class TextBox : UIControl
	{
		protected string m_cursor = "_";
		protected double m_seconds = 0;

		public TextBox(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.ControlTag = "textbox";

			this.IsHoverable = false;
		}

		public override void Update(GameTime gameTime)
		{
			if (this.Ebi.Focus != this)
			{
				m_cursor = "_";
			}
			else
			{
				m_seconds += gameTime.ElapsedGameTime.TotalSeconds;
				if (m_seconds > 0.5)
				{
					m_cursor = " ";
				}
				else
					m_cursor = "_";

				if (m_seconds > 1)
					m_seconds = 0;
			}

			base.Update(gameTime);
		}

		protected override void OnKeyDown(object focus, KeyEventArgs k)
		{
			if (focus == this)
			{
				if (k.Key == Keys.Back)
				{
					if (this.Text.Length > 0)
						this.Text = this.Text.Substring(0, this.Text.Length - 1);
				}
				else if (!k.Alt && !k.Ctrl)
				{
					this.Text += Xe.Tools.Helper.KeyToString(k.Key, k.Shift);
				}
			}

			base.OnKeyDown(focus, k);
		}
	}
}

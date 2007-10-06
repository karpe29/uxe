#region Using System
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Xe.Input;

#endregion

namespace Xe.GUI
{
	public class StringSlider : UIControl
	{
		private int m_curIndex;
		private List<string> m_strings = new List<string>();

		private string m_leftButton = "<";
		private string m_rightButton = ">";

		private string m_text = "";

		private bool m_isLoopable = true;

		public StringSlider(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			this.TreeLevel = 0;



			this.Index = 0;
		}

		public override void Initialize()
		{
			this.ClippingOffset.Width = -100;

			base.Initialize();
		}

		protected override void OnKeyDown(object sender, KeyEventArgs e)
		{
			base.OnKeyDown(sender, e);

			if (e.Key == Keys.Right)
			{
				if (m_isLoopable && this.Index == m_strings.Count - 1)
					this.Index = 0;
				else
					this.Index++;
			}
			else if (e.Key == Keys.Left)
			{
				if (m_isLoopable && this.Index == 0)
					this.Index = m_strings.Count - 1;
				else
					this.Index--;
			}
		}

		protected override void OnButtonDown(object sender, ButtonEventArgs e)
		{
			base.OnButtonDown(sender, e);

			if (e.Button == GamePadButton.LeftStickRight || e.Button == GamePadButton.DPadRight)
			{
				if (m_isLoopable && this.Index == m_strings.Count - 1)
					this.Index = 0;
				else
					this.Index++;
			}
			else if (e.Button == GamePadButton.LeftStickLeft || e.Button == GamePadButton.DPadLeft)
			{
				if (m_isLoopable && this.Index == 0)
					this.Index = m_strings.Count - 1;
				else
					this.Index--;
			}
		}

		protected override void OnButtonUp(object sender, ButtonEventArgs e)
		{
			base.OnButtonUp(sender, e);
		}

		private void ResetText()
		{
			string _text;
			if (m_curIndex < 0 || m_strings.Count == 0)
				_text = "";
			else
				_text = m_strings[m_curIndex];

			base.Text = String.Format("{0}{1} {2} {3}", m_text, m_leftButton, _text, m_rightButton);
		}

		public new string Text
		{
			get { return base.Text; }
			set
			{
				m_text = value;

				ResetText();
			}
		}

		public List<string> Strings
		{
			get { return m_strings; }
			set { m_strings = value; }
		}

		public int Index
		{
			get { return m_curIndex; }
			set
			{
				if (m_strings.Count > 0)
					m_curIndex = (int)MathHelper.Clamp(value, 0, m_strings.Count - 1);
				else
					m_curIndex = -1;

				ResetText();
			}
		}

		public bool Loopable
		{
			get { return m_isLoopable; }
			set { m_isLoopable = value; }
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//b.Draw(gameTime);
		}
	}
}

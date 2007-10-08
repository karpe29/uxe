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
	public enum SliderType
	{
		Vertical,
		Horizontal
	}

	public class SliderString : UIControl
	{
		private int m_curIndex;
		private List<string> m_strings = new List<string>();

		SliderType m_type = SliderType.Horizontal;

		private UIControl m_buttonMinus, m_buttonPlus;

		private string m_text = "";

		private bool m_isLoopable = true;

		public SliderString(Game game, IGUIManager guiManager)
			: this(game, guiManager, SliderType.Horizontal)
		{
		}

		public SliderString(Game game, IGUIManager guiManager, SliderType type)
			: base(game, guiManager)
		{
			this.TreeLevel = 0;
			this.Index = 0;
			this.TextAlign = TextAlign.Center;

			this.Width = 52;

			m_type = type;

			if (m_type == SliderType.Vertical)
			{
				m_buttonPlus = new SliderUpButton(game, guiManager);
				m_buttonMinus = new SliderDownButton(game, guiManager);
			}

			if (m_type == SliderType.Horizontal)
			{
				m_buttonPlus = new SliderRightButton(game, guiManager);
				m_buttonMinus = new SliderLeftButton(game, guiManager);
			}

			this.m_buttonPlus.Click += new ClickHandler(buttonPlus_Click);
			this.m_buttonMinus.Click += new ClickHandler(buttonMinus_Click);

			
		}

		void buttonMinus_Click(object sender, MouseEventArgs e)
		{
			//throw new Exception("The method or operation is not implemented.");
		}

		void buttonPlus_Click(object sender, MouseEventArgs e)
		{
			//throw new Exception("The method or operation is not implemented.");
		}

		public override void Initialize()
		{
			//this.ClippingOffset = new Rect(100,100,100,100);

			this.Controls.Add(m_buttonPlus);
			this.Controls.Add(m_buttonMinus);

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

			base.Text = String.Format("{0}{1}", m_text, _text);
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
		}

		public override void Update(GameTime gameTime)
		{
			bool _needUpdate = m_needsUpdate;

			if (m_needsUpdate)
			{
				m_buttonMinus.X = this.X - 37; m_buttonMinus.Y = this.Y;
				m_buttonPlus.X = this.X + 5; m_buttonPlus.Y = this.Y;

				//ClippingOffset = new Vector4(0, 0, -m_label.Width - 5, 0);
				//ClippingOffset = Vector4.UnitZ * (-m_label.Width - 5);
				//this.ClippingOffset.Width = -m_label.Width - 5;

				//m_label.Alpha = this.Alpha;
			}

			base.Update(gameTime);

			if (_needUpdate)
			{
				m_buttonMinus.X = this.X - 37; m_buttonMinus.Y = this.Y;
				m_buttonPlus.X = this.X + 5; m_buttonPlus.Y = this.Y;

			}

		}
	}
}

#region Using System
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Xe.Input;

#endregion

namespace Xe.Gui
{
	public enum SliderType
	{
		Vertical, // disabled atm
		Horizontal
	}

	public class SliderString : UIControl
	{
		private int m_index;
		private List<string> m_strings = new List<string>();

		SliderType m_type;

		private UIControl m_buttonMinus, m_buttonPlus;
		private Label m_label;

		private string m_text = "";

		private bool m_isLoopable = true;

		public SliderString(Game game, IGuiManager guiManager)
			: this(game, guiManager, SliderType.Horizontal)
		{
		}

		public SliderString(Game game, IGuiManager guiManager, SliderType type)
			: base(game, guiManager)
		{
			this.TreeLevel = 0;
			this.m_index = 0;	
			this.TextAlign = TextAlign.Center;
			this.IsTextVisible = false;
			this.m_text = "dummy";
			this.ControlTag = "";

			this.m_label = new Label(game, guiManager);
			this.m_label.ControlTag = "label";
			this.m_label.Name = "SS_LABEL";
			//Sthis.m_label.Enabled = false;
			this.m_label.TextAlign = TextAlign.Center;
			
			//this.Height = 30;

			m_type = type;

			if (m_type == SliderType.Vertical)
			{
				m_buttonPlus = new SliderUpButton(game, guiManager);
				m_buttonPlus.Name = "SS_BUTTONPLUS";
				m_buttonMinus = new SliderDownButton(game, guiManager);
				m_buttonMinus.Name = "SS_BUTTONMINUS";
			}

			if (m_type == SliderType.Horizontal)
			{
				m_buttonPlus = new SliderRightButton(game, guiManager);
				m_buttonPlus.Name = "SS_BUTTONPLUS";
				m_buttonMinus = new SliderLeftButton(game, guiManager);
				m_buttonMinus.Name = "SS_BUTTONMINUS";
			}

			this.m_buttonPlus.Click += new ClickHandler(buttonPlus_Click);
			this.m_buttonMinus.Click += new ClickHandler(buttonMinus_Click);


		}

		public override void Initialize()
		{
			//this.ClippingOffset = new Rect(100,100,100,100);

			this.Controls.Add(m_label);
			this.Controls.Add(m_buttonPlus);
			this.Controls.Add(m_buttonMinus);

			base.Initialize();
		}

		void buttonMinus_Click(object sender, MouseEventArgs e)
		{
			if (m_isLoopable && this.Index == 0)
				this.Index = m_strings.Count - 1;
			else
				this.Index--;
		}

		void buttonPlus_Click(object sender, MouseEventArgs e)
		{
			if (m_isLoopable && this.Index == m_strings.Count - 1)
				this.Index = 0;
			else
				this.Index++;
		}

		protected override void OnKeyDown(object sender, KeyEventArgs e)
		{
			base.OnKeyDown(sender, e);

			if (e.Key == Keys.Right || e.Key == Keys.Up)
			{
				if (m_isLoopable && this.Index == m_strings.Count - 1)
					this.Index = 0;
				else
					this.Index++;
			}
			else if (e.Key == Keys.Left || e.Key == Keys.Down)
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

		private void ResetText()
		{
			string _text;
			if (m_index < 0 || m_strings.Count == 0)
				_text = "No Values Set";
			else
				_text = m_strings[m_index];

			m_label.Text = _text;//String.Format("{0}{1}", m_text, _text);
		}

		public new string Text
		{
			get { return m_label.Text; }
		}

		public List<string> Strings
		{
			get { return m_strings; }
			set { m_strings = value; }
		}

		public int Index
		{
			get { return m_index; }
			set
			{
				if (m_strings.Count > 0)
					m_index = (int)MathHelper.Clamp(value, 0, m_strings.Count - 1);
				else
					m_index = -1;

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
			//base.Draw(gameTime);
		}

		public override void Update(GameTime gameTime)
		{
			bool _needUpdate = m_needsUpdate;

			if (m_needsUpdate)
			{
				ResetPositions();

				//ClippingOffset = new Vector4(0, 0, -m_label.Width - 5, 0);
				//ClippingOffset = Vector4.UnitZ * (-m_label.Width - 5);
				//this.ClippingOffset.Width = -m_label.Width - 5;

				//m_label.Alpha = this.Alpha;
			}

			base.Update(gameTime);

			if (_needUpdate)
			{
				ResetPositions();
			}
		}

		private void ResetPositions()
		{
			if (m_buttonMinus != null && m_buttonPlus != null)
			{
				m_buttonMinus.X = this.X;
				m_label.X = this.X + m_buttonMinus.Width;
				m_buttonPlus.X = this.X + m_buttonMinus.Width + m_label.Width;


				m_buttonMinus.Y = this.Y;
				m_label.Y = this.Y;
				m_buttonPlus.Y = this.Y;
			}
		}

		protected override void OnMove(object sender)
		{
			base.OnMove(sender);

			ResetPositions();
		}

		public new float Width
		{
			get { return base.Width; }
			set
			{
				base.Width = value;

				m_label.Width = base.Width - m_buttonMinus.Width - m_buttonPlus.Width;
			}
		}
	}
}

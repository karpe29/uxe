#region Using Statements
using System;
using System.Xml;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Xe.Tools;
using Xe.Input;
#endregion

namespace Xe.GUI
{
	public class CheckBox : UIControl
	{
		#region Members
		protected Label m_label;
		protected string m_labelText = "";

		protected bool m_isChecked = false;
		#endregion

		public CheckBox(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			m_label = new Label(game, guiManager);
			m_label.Name = "CB_LABEL";
			m_label.TextAlign = TextAlign.Center;
			m_label.MouseDown += new MouseDownHandler(Label_MouseDown);

			this.IsTextVisible = false;
			this.IsFocusable = true;
			this.Width = 32;

			this.Text = "";
			this.ControlTag = "checkbox";
		}

		public override void Initialize()
		{
			this.ClippingOffset.Width = -m_label.Width - 5;
			this.Controls.Add(m_label);

			base.Initialize();
		}

		protected void Label_MouseDown(MouseEventArgs e)
		{
			this.Checked = !this.Checked;
		}

		protected override void OnMouseUp(MouseEventArgs args)
		{
			//base.OnMouseUp(args);
		}

		protected override void OnMouseDown(MouseEventArgs args)
		{
			//base.OnMouseDown(args);
		}

		protected override void OnLostFocus(object sender)
		{
			//base.OnLostFocus(sender);
		}

		protected override void OnGotFocus(object sender)
		{
			//base.OnGotFocus(sender);
		}

		protected override void OnMove(object sender)
		{
			base.OnMove(sender);

			if (m_label != null)
			{
				m_label.X = this.X + 37;
				m_label.Y = this.Y;
			}
		}

		protected override void OnClick(object sender, MouseEventArgs args)
		{
			System.Console.WriteLine("Processing click!");

			base.OnClick(sender, args);

			if (this.Ebi.Focus == this)
				this.Checked = !this.Checked;

			System.Console.WriteLine("Checked: " + this.Checked.ToString());
		}

		public override void Update(GameTime gameTime)
		{
			bool _needUpdate = m_needsUpdate;

			if (m_needsUpdate)
			{
				m_label.X = this.X + 37;
				m_label.Y = this.Y;
				m_label.Text = m_labelText;

				//ClippingOffset = new Vector4(0, 0, -m_label.Width - 5, 0);
				//ClippingOffset = Vector4.UnitZ * (-m_label.Width - 5);
				this.ClippingOffset.Width = -m_label.Width - 5;

				//m_label.Alpha = this.Alpha;
			}

			base.Update(gameTime);

			if (_needUpdate)
			{
				m_label.X = this.X + 37;
				m_label.Y = this.Y;
			}
		}

		public Label Label
		{
			get { return m_label; }
			set { m_label = value; }
		}

		public bool Checked
		{
			get
			{
				return m_isChecked;
			}
			set
			{
				m_isChecked = value;

				if (m_isChecked)
					ChangeState(UIState.Down);
				else
					ChangeState(UIState.Out);
			}
		}
				
		public new string Text
		{
			get
			{
				return m_labelText;
			}
			set
			{
				m_labelText = value;

				if (m_label != null)
					m_label.Text = m_labelText;
			}
		}
	}
}

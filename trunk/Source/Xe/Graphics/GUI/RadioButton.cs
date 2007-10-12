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
	public class RadioButton : UIControl
	{
		#region Members
		protected Label m_label;
		protected string m_labelText = "";

		protected bool m_isChecked = false;

		public event ClickHandler CheckChanged;
		#endregion

		public RadioButton(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			m_label = new Label(game, guiManager);
			m_label.Name = "RB_LABEL";
			m_label.MouseDown += new MouseDownHandler(Label_MouseDown);

			this.IsFocusable = false;
			this.Width = 32;

			this.Text = "";
			this.ControlTag = "radiobutton";
		}

		public override void Initialize()
		{
			this.Controls.Add(m_label);

			base.Initialize();
		}

		protected void Label_MouseDown(MouseEventArgs e)
		{
			this.Checked = !this.Checked;

			if (CheckChanged != null)
				CheckChanged.Invoke(this, e);
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

		protected override void OnClick(object sender, MouseEventArgs args)
		{
			base.OnClick(sender, args);

			if (this.Ebi.Focus == this)
			{
				this.Checked = !this.Checked;

				if (CheckChanged != null)
					CheckChanged.Invoke(this, args);
			}
		}

		public override void Update(GameTime gameTime)
		{
			bool _needUpdate = m_needsUpdate;
			if (m_needsUpdate)
			{
				m_label.X = this.X + 37;
				m_label.Y = this.Y;

				m_label.Text = m_labelText;

				//ClippingOffset = new Vector4(0, 0, -105, 0);
				Rectangle tempRectangle = this.ClippingOffset;
				tempRectangle.Width = (int)-m_label.Width - 5;
				this.ClippingOffset = tempRectangle;

				//m_label.Alpha = this.Alpha;
			}

			base.Update(gameTime);

			if (_needUpdate)
			{
				m_label.X = this.X + 37;
				m_label.Y = this.Y;
			}
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
				{
					//m_state = UIState.Down;
					ChangeState(UIState.Down);

					UpdateGroupedControls();
				}
				else
				{
					if (!CheckForEmpty())
						ChangeState(UIState.Out);
					//m_state = UIState.Out;
				}
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

		protected virtual bool CheckForEmpty()
		{
			if (this.Parent != null)
			{
				for (int i = 0; i < this.Parent.Controls.MasterList.Count; i++)
				{
					RadioButton _button = this.Parent.Controls.MasterList[i] as RadioButton;
					if (_button != null && _button != this && _button.Visible && _button.Enabled)
						if (_button.Checked)
							return false;
				}
			}
			else
			{
				if (this.GUIManager != null)
				{
					for (int i = 0; i < this.GUIManager.Controls.MasterList.Count; i++)
					{
						RadioButton _button = this.GUIManager.Controls.MasterList[i] as RadioButton;
						if (_button != null && _button != this && _button.Visible && _button.Enabled)
							if (_button.Checked)
								return false;
					}
				}
			}

			return true;
		}

		protected virtual void UpdateGroupedControls()
		{
			if (m_isChecked)
			{
				if (this.Parent != null)
				{
					for (int i = 0; i < this.Parent.Controls.MasterList.Count; i++)
					{
						RadioButton _button = this.Parent.Controls.MasterList[i] as RadioButton;
						if (_button != null && _button != this && _button.Visible && _button.Enabled)
							_button.Checked = false;
					}
				}
				else
				{
					if (this.GUIManager != null)
					{
						for (int i = 0; i < this.GUIManager.Controls.MasterList.Count; i++)
						{
							RadioButton _button = this.GUIManager.Controls.MasterList[i] as RadioButton;
							if (_button != null && _button != this && _button.Visible && _button.Enabled)
								_button.Checked = false;
						}
					}
				}
			}
		}
	}
}

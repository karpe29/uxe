#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using XeFramework;
using XeFramework.Data;
using XeFramework.Graphics2D;
using XeFramework.GUI;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace XeFramework.GUI
{
	public delegate void ValueChangedHandler(object sender, float value);

	public class Slider : UIControl
	{
		SliderLine m_sliderLine;
		SliderArrow m_sliderArrow;

		public SliderLine SliderLine { get { return m_sliderLine; } }
		public SliderArrow SliderArrow { get { return m_sliderArrow; } }

		public event ValueChangedHandler ValueChanged;

		protected float m_value = 50.0f;
		public float Value
		{
			get
			{
				return m_value;
			}

			set
			{
				float oldValue = m_value;

				// Determine nearest value from Step
				if (value >= MaxValue)
					m_value = MaxValue;
				else
					for (float f = MinValue; f <= MaxValue; f += Step)
					{
						if (value < f || value > f + Step)
							continue;

						if (value < f + Step / 2)
						{
							m_value = f;
							break;
						}

						if (value >= f + Step / 2)
						{
							m_value = f + Step;
							break;
						}
					}


				// clamp between min & max values.
				m_value = MathHelper.Clamp(m_value, MinValue, MaxValue);

				if (oldValue != m_value)
				{
					if (this.ValueChanged != null)
					{
						this.ValueChanged(this, this.m_value);
					}
				}

				UpdateArrowWithoutChangingValue();

				//this.Game.Window.Title = "value:" + value.ToString() + " - m_value : " + m_value.ToString() + " - ValuePercent: " + ValuePercent.ToString() + "%"; 
			}
		}

		public float ValuePercent
		{
			get
			{
				return m_value / (MaxValue - MinValue) * 100.0f;
			}
			set
			{
				Value = Math.Max(0, Math.Min(100, value)) * (MaxValue - MinValue);
			}
		}

		protected float m_maxValue = 100.0f;
		public float MaxValue
		{
			get
			{
				return m_maxValue;
			}

			set
			{
				m_maxValue = value;
				Value = Value;
			}
		}

		protected float m_minValue = 0.0f;
		public float MinValue
		{
			get
			{
				return m_minValue;
			}

			set
			{
				m_minValue = value;
				Value = Value;
			}
		}

		public float m_step = 1.0f;
		public float Step
		{
			get
			{
				return m_step;
			}

			set
			{
				m_step = value;
				Value = Value;
			}
		}

		public new string Text
		{
			get { return base.Text; }
		}

		public Slider(Game game, GUIManager guiManager)
			: base(game, guiManager)
		{
			base.Text = "";

			m_sliderLine = new SliderLine(game, guiManager, this);
			m_sliderLine.UpdateOrder = guiManager.DrawOrder;
			m_sliderLine.DrawOrder = guiManager.DrawOrder;

			//guiManager.AddControl(m_sliderLine);
			//game.Components.Add(m_sliderLine);
			this.m_controls.Add(m_sliderLine);

			m_sliderArrow = new SliderArrow(game, guiManager, this);
			m_sliderArrow.LockY = true;
			m_sliderArrow.UpdateOrder = guiManager.DrawOrder + 1;
			m_sliderArrow.DrawOrder = guiManager.DrawOrder + 1;

			m_sliderArrow.Move += new MoveHandler(OnSliderArrowMove);

			//guiManager.AddControl(m_sliderArrow);
			//game.Components.Add(m_sliderArrow);
			this.m_controls.Add(m_sliderArrow);

			this.Width = 200;
			this.Height = 30;

			this.X = 200;
			this.Y = 200;
		}

		void OnSliderArrowMove(object sender)
		{
			this.UpdateValue();
		}

		public override void Initialize()
		{
			base.Initialize();

			foreach (UIControl c in m_controls)
				c.Initialize();
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			//foreach (UIControl c in m_controls)
			//	c.LoadGraphicsContent(loadAllContent);
		}

		protected override void UnloadGraphicsContent(bool unloadAllContent)
		{
			base.UnloadGraphicsContent(unloadAllContent);

			//foreach (UIControl c in m_controls)
			//	c.UnloadGraphicsContent(unloadAllContent);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			foreach (UIControl c in m_controls)
				c.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			foreach (UIControl c in m_controls)
				c.Draw(gameTime);
		}

		/// <summary>
		/// Used to update slider value when the arrow has moved.
		/// </summary>
		/// <returns>Slider Value</returns>
		public float UpdateValue()
		{
			return Value = ((float)(this.m_sliderArrow.AbsolutePosition.X - this.m_sliderLine.AbsolutePosition.X) / (float)(this.m_sliderLine.Width - this.m_sliderArrow.Width) * (MaxValue - MinValue));
		}

		/// <summary>
		/// Used to update the arrow position when the value or position or size have changed
		/// </summary>
		public int UpdateArrowWithoutChangingValue()
		{
			return m_sliderArrow.AbsX = (int)(m_value * (this.m_sliderLine.Width - this.m_sliderArrow.Width) / (MaxValue - MinValue)) + this.m_sliderLine.AbsolutePosition.X;
		}

		public new int X
		{
			get
			{
				return m_sliderLine.X;
			}
			set
			{
				this.m_sliderLine.AbsX = value;
				UpdateArrowWithoutChangingValue();
			}
		}

		public new int Y
		{
			get
			{
				return m_sliderLine.Y;
			}
			set
			{
				this.m_sliderArrow.Y = value;
				this.m_sliderLine.Y = this.m_sliderArrow.Y + this.m_sliderArrow.Height / 2 - this.m_sliderLine.Height / 2;
			}
		}


		public new int Width
		{
			get
			{
				return m_sliderLine.Width;
			}
			set
			{
				m_sliderLine.Width = value;
				UpdateArrowWithoutChangingValue();
			}
		}

		public new int Height
		{
			get
			{
				return m_sliderArrow.Height;
			}
			set
			{
				if (value < 10)
					throw new ArgumentException("Too low (<10)", "Slider.Height");

				this.m_sliderArrow.Height = value;
				this.m_sliderLine.Height = (int)(value / 2);
			}
		}
	}
}

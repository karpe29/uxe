#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using Xe;
using Xe.Tools;
using Xe.Graphics2D;
using Xe.GUI;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Xe.GUI
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

				// little bug here if value <= min value
				// value will be minvalue + step at the end (minor)
				// and if minvalue != 0
				
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

		public Slider(Game game, IGUIManager guiManager)
			: base(game, guiManager)
		{
			base.Text = "";

			m_sliderLine = new SliderLine(game, guiManager, this);
			//m_sliderLine.UpdateOrder = guiManager.DrawOrder;
			//m_sliderLine.DrawOrder = guiManager.DrawOrder;

			//guiManager.AddControl(m_sliderLine);
			//game.Components.Add(m_sliderLine);
			this.Controls.Add(m_sliderLine);

			m_sliderArrow = new SliderArrow(game, guiManager, this);
			m_sliderArrow.LockY = true;
			//m_sliderArrow.UpdateOrder = guiManager.DrawOrder + 1;
			//m_sliderArrow.DrawOrder = guiManager.DrawOrder + 1;

			m_sliderArrow.Move += new MoveHandler(OnSliderArrowMove);

			//guiManager.AddControl(m_sliderArrow);
			//game.Components.Add(m_sliderArrow);
			this.Controls.Add(m_sliderArrow);

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

			//foreach (UIControl c in Controls)
			//	c.Initialize();
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

			//foreach (UIControl c in Controls)
			//	c.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//foreach (UIControl c in Controls)
			//	c.Draw(gameTime);
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
			return m_sliderArrow.AbsX = (int) ((m_value * (this.m_sliderLine.Width - this.m_sliderArrow.Width) / (MaxValue - MinValue)) + this.m_sliderLine.AbsolutePosition.X);
		}

		public new int X
		{
			get
			{
				return (int) m_sliderLine.X;
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
				return (int) m_sliderLine.Y;
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
				return (int)m_sliderLine.Width;
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
				return (int) m_sliderArrow.Height;
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

	public class SliderLine : UIControl
	{

		Slider m_slider;

		public SliderLine(Game game, IGUIManager guiManager, Slider slider)
			: base(game, guiManager)
		{
			m_slider = slider;

			this.IsHoverable = true;
			this.IsDraggable = false;
			this.IsResizable = true;
			this.Enabled = true;

			this.Text = "";

			this.Width = 200;
			this.Height = 10;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			/*
			 * base.LoadGraphicsContent(loadAllContent);

			//m_outRects = m_guiManager.CreateControl(new Rectangle(5, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_overRects = m_guiManager.CreateControl(new Rectangle(35, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_downRects = m_guiManager.CreateControl(new Rectangle(65, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_disabledRects = m_guiManager.CreateControl(new Rectangle(35, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			m_outRects = m_guiManager.CreateControl("sliderline.out", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_overRects = m_guiManager.CreateControl("sliderline.over", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_downRects = m_guiManager.CreateControl("sliderline.down", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_disabledRects = m_guiManager.CreateControl("sliderline.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));
			 * */
		}

		public override void Update(GameTime gameTime)
		{
			/*base.Update(gameTime);

			if (m_needsUpdate)
			{
				m_outRects = m_guiManager.CreateControl("sliderline.out", new Rectangle(m_absX, m_absY, m_width, m_height));
				m_overRects = m_guiManager.CreateControl("sliderline.over", new Rectangle(m_absX, m_absY, m_width, m_height));
				m_downRects = m_guiManager.CreateControl("sliderline.down", new Rectangle(m_absX, m_absY, m_width, m_height));
				m_disabledRects = m_guiManager.CreateControl("sliderline.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));

				m_needsUpdate = false;
			}*/
		}

		public int AbsX
		{
			set
			{
			/*	this.m_absX = value;
				m_needsUpdate = true;*/
			}
		}

	}

	public class SliderArrow : UIControl
	{
		Slider m_slider;

		public SliderArrow(Game game, IGUIManager guiManager, Slider slider)
			: base(game, guiManager)
		{
			m_slider = slider;

			this.IsHoverable = true;
			this.IsDraggable = true;
			this.IsResizable = false;
			this.Enabled = true;

			this.Text = "";

			this.Width = 10;
			this.Height = 20;
		}


		public override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			/*
			 * base.LoadGraphicsContent(loadAllContent);

			//m_outRects = m_guiManager.CreateControl(new Rectangle(5, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_overRects = m_guiManager.CreateControl(new Rectangle(35, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_downRects = m_guiManager.CreateControl(new Rectangle(65, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_disabledRects = m_guiManager.CreateControl(new Rectangle(35, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			m_outRects = m_guiManager.CreateControl("sliderarrow.out", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_overRects = m_guiManager.CreateControl("sliderarrow.over", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_downRects = m_guiManager.CreateControl("sliderarrow.down", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_disabledRects = m_guiManager.CreateControl("sliderarrow.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));
			 * */
		}

		public override void Update(GameTime gameTime)
		{
/*
			base.Update(gameTime);


			if (this.AbsolutePosition.X < this.m_slider.SliderLine.AbsolutePosition.X)
				this.AbsX = this.m_slider.SliderLine.AbsolutePosition.X;

			if (this.AbsolutePosition.X > this.m_slider.SliderLine.AbsolutePosition.X + this.m_slider.SliderLine.Width - this.Width)
				this.AbsX = this.m_slider.SliderLine.AbsolutePosition.X + this.m_slider.SliderLine.Width - this.Width;

			if (m_needsUpdate)
			{
				m_outRects = m_guiManager.CreateControl("sliderarrow.out", new Rectangle(m_absX, m_absY, m_width, m_height));
				m_overRects = m_guiManager.CreateControl("sliderarrow.over", new Rectangle(m_absX, m_absY, m_width, m_height));
				m_downRects = m_guiManager.CreateControl("sliderarrow.down", new Rectangle(m_absX, m_absY, m_width, m_height));
				m_disabledRects = m_guiManager.CreateControl("sliderarrow.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));

				m_needsUpdate = false;
			}
 * */
		}

		public int AbsX
		{
			set
			{/*
				this.m_absX = value;
				m_needsUpdate = true;*/
			}
		}


	}
}

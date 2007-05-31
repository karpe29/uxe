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
using XeFramework.Input;
#endregion

namespace XeFramework.GUI
{
	public class SliderArrow : UIControl
	{
		Slider m_slider;

		public SliderArrow(Game game, GUIManager guiManager, Slider slider)
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
			base.LoadGraphicsContent(loadAllContent);

			//m_outRects = m_guiManager.CreateControl(new Rectangle(5, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_overRects = m_guiManager.CreateControl(new Rectangle(35, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_downRects = m_guiManager.CreateControl(new Rectangle(65, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_disabledRects = m_guiManager.CreateControl(new Rectangle(35, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			m_outRects = m_guiManager.CreateControl("sliderarrow.out", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_overRects = m_guiManager.CreateControl("sliderarrow.over", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_downRects = m_guiManager.CreateControl("sliderarrow.down", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_disabledRects = m_guiManager.CreateControl("sliderarrow.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));
		}

		public override void Update(GameTime gameTime)
		{
			
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
		}

		public int AbsX
		{
			set
			{
				this.m_absX = value;
				m_needsUpdate = true;
			}
		}


	}
}

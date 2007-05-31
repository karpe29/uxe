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
	public class SliderLine : UIControl
	{

		Slider m_slider;

		public SliderLine(Game game, GUIManager guiManager, Slider slider)
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
			base.LoadGraphicsContent(loadAllContent);

			//m_outRects = m_guiManager.CreateControl(new Rectangle(5, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_overRects = m_guiManager.CreateControl(new Rectangle(35, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_downRects = m_guiManager.CreateControl(new Rectangle(65, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			//m_disabledRects = m_guiManager.CreateControl(new Rectangle(35, 125, 25, 25), new Rectangle(m_absX, m_absY, m_width, m_height));
			m_outRects = m_guiManager.CreateControl("sliderline.out", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_overRects = m_guiManager.CreateControl("sliderline.over", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_downRects = m_guiManager.CreateControl("sliderline.down", new Rectangle(m_absX, m_absY, m_width, m_height));
			m_disabledRects = m_guiManager.CreateControl("sliderline.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (m_needsUpdate)
			{
				m_outRects = m_guiManager.CreateControl("sliderline.out", new Rectangle(m_absX, m_absY, m_width, m_height));
				m_overRects = m_guiManager.CreateControl("sliderline.over", new Rectangle(m_absX, m_absY, m_width, m_height));
				m_downRects = m_guiManager.CreateControl("sliderline.down", new Rectangle(m_absX, m_absY, m_width, m_height));
				m_disabledRects = m_guiManager.CreateControl("sliderline.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));

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

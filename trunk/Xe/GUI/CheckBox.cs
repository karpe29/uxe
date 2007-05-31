#region License
/*
 *  Xna5D.GUI.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/
#endregion License

#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using XeFramework;
using XeFramework.Input;
using XeFramework.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace XeFramework.GUI
{
    public class CheckBox : UIControl
    {
        #region Members
        protected Label m_label;
        protected string m_labelText = "";

        protected bool m_isChecked = false;
        #endregion

		public CheckBox(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            m_label = new Label(game, guiManager);
            m_label.MouseDown += new MouseDownHandler(Label_MouseDown);

            game.Components.Add(m_label);
            this.Width = 25;

            m_label.Width -= 30;

            m_text = "";
            this.Text = "CheckBox";

            //guiManager.AddControl(m_label);
            this.Controls.Add(m_label);
        }

        protected void Label_MouseDown(MouseEventArgs args)
        {
            this.Checked = !this.Checked;
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);
            
            // out: new Rectangle(5, 35, 25, 25)
            // over: new Rectangle(5, 35, 25, 25)
            // down: new Rectangle(35, 35, 25, 25)
            // diabled: new Rectangle(5, 35, 25, 25)
            m_outRects = m_guiManager.CreateControl("checkbox.out", new Rectangle(m_absX, m_absY, m_width, m_height));
            m_overRects = m_guiManager.CreateControl("checkbox.over", new Rectangle(m_absX, m_absY, m_width, m_height));
            m_downRects = m_guiManager.CreateControl("checkbox.down", new Rectangle(m_absX, m_absY, m_width, m_height));
            m_disabledRects = m_guiManager.CreateControl("checkbox.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //if (m_isChecked)
            //  m_state = UIState.Down;

            if (m_needsUpdate)
            {
                m_outRects = m_guiManager.CreateControl("checkbox.out", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_overRects = m_guiManager.CreateControl("checkbox.over", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_downRects = m_guiManager.CreateControl("checkbox.down", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_disabledRects = m_guiManager.CreateControl("checkbox.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));

                m_label.X = m_absX + 30;
                m_label.Y = m_absY;
                m_label.Text = m_labelText;

                m_needsUpdate = false;
            }
        }

        protected override void OnVisibleChanged(object sender, EventArgs args)
        {
            base.OnVisibleChanged(sender, args);

            m_label.Visible = this.Visible;
        }

        protected override void OnDrawOrderChanged(object sender, EventArgs args)
        {
            base.OnDrawOrderChanged(sender, args);

            m_label.DrawOrder = this.DrawOrder;
        }

        protected override void OnMouseUp(MouseEventArgs args)
        {
            //base.OnMouseUp(args);
        }

        protected override void OnMouseDown(MouseEventArgs args)
        {
            //base.OnMouseDown(args);

            if (m_ebi.GetFocus() == this)
            {
                this.Checked = !this.Checked;
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
                    m_state = UIState.Down;
                else
                    m_state = UIState.Out;
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
            }
        }
    }
}

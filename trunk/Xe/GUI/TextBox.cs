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
    public partial class TextBox : UIControl
    {
        protected string m_cursor = "_";
        protected double m_seconds = 0;

        protected bool m_isFocused = false;

		public TextBox(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            // Out: new Rectangle(5, 5, 25, 25)
            // Over: new Rectangle(5, 5, 25, 25)
            // Down: new Rectangle(5, 5, 25, 25)
            // Diabled: new Rectangle(35, 5, 25, 25)
            m_outRects = m_guiManager.CreateControl("textbox.out", new Rectangle(m_absX, m_absY, m_width, m_height));
            m_overRects = m_guiManager.CreateControl("textbox.over", new Rectangle(m_absX, m_absY, m_width, m_height));
            m_downRects = m_guiManager.CreateControl("textbox.down", new Rectangle(m_absX, m_absY, m_width, m_height));
            m_disabledRects = m_guiManager.CreateControl("textbox.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_ebi.GetFocus() != this)
            {
                m_cursor = "_";
            }
            else
            {
                m_seconds += gameTime.ElapsedRealTime.TotalSeconds;
                if (m_seconds > 0.5)
                {
                    m_cursor = " ";
                }
                else
                    m_cursor = "_";

                if (m_seconds > 1)
                    m_seconds = 0;
            }

            if (m_needsUpdate)
            {
                m_outRects = m_guiManager.CreateControl("textbox.out", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_overRects = m_guiManager.CreateControl("textbox.over", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_downRects = m_guiManager.CreateControl("textbox.down", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_disabledRects = m_guiManager.CreateControl("textbox.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));

                m_needsUpdate = false;
            }
        }

        protected override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);

            m_isFocused = true;
        }

        protected override void OnKeyDown(object focus, KeyEventArgs k)
        {
            if (focus == this)
            {
                if (k.Key == Keys.Back)
                {
                    if (this.Text.Length > 0)
                        this.Text = this.Text.Substring(0, this.Text.Length - 1);
                }
                else if (!k.Alt && !k.Control)
                    this.Text += Globals.KeyToString(k.Key, k.Shift);
            }

            base.OnKeyDown(focus, k);
        }

        #region Drawing
        public override void Draw(GameTime gameTime)
        {
            //base.Draw(gameTime);

            if (!this.Enabled)
                Update(gameTime);

            m_spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            if (m_state == UIState.Out)
            {
                foreach (QuadBase _rect in m_outRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);
            }
            else if (m_state == UIState.Disabled)
            {
                foreach (QuadBase _rect in m_disabledRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);
            }
            else if (m_state == UIState.Over)
            {
                foreach (QuadBase _rect in m_overRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);
            }
            else if (m_state == UIState.Down)
            {
                foreach (QuadBase _rect in m_downRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);
            }
            m_spriteBatch.End();

            if (m_font != null)
            {
                #region Text
                Rectangle _rect = new Rectangle(m_absX + 5, m_absY + (int)(this.Height / 2) - (int)(m_font.LineSpacing / 2), m_width - 5, m_height);

                int from = _rect.Width / m_fontWidth;
                string toDraw = m_text + m_cursor;
                if (m_text.Length + m_cursor.Length > from)
                    toDraw = m_text.Substring(m_text.Length - from + 1) + m_cursor;

				DrawTextInABox(_rect, m_foreColor, toDraw);
                #endregion
            }
        }
        #endregion
    }
}

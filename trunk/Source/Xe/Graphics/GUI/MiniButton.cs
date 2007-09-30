#region License
/*
 *  Xna5D.GUI.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 04, 2006
 */
#endregion

#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using Xe;

using Xe.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Xe.Tools;
using Xe.Input;
#endregion

namespace Xe.GUI
{
    public partial class MiniButton : UIControl
    {
        protected string m_guiElementID = "minibutton";

        protected SpriteParameters m_params = new SpriteParameters();

		public MiniButton(Game game, IGUIManager guiManager)
            : base(game, guiManager)
        {
            this.IsHoverable = true;
            this.TextAlign = TextAlignment.Center;

            this.Text = "";

            //m_params.Rotation = MathHelper.ToRadians(0.0f);
            //m_params.Origin = new Vector2(0, 0);
            //m_params.SpriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                if (m_outRects != null)
                    m_outRects.Clear();

                if (m_overRects != null)
                    m_overRects.Clear();

                if (m_downRects != null)
                    m_downRects.Clear();

                if (m_disabledRects != null)
                    m_disabledRects.Clear();

                m_outRects.Add(m_guiManager.CreateBox(m_guiElementID + ".out", new Rectangle(m_absX, m_absY, m_width, m_height)));
                m_overRects.Add(m_guiManager.CreateBox(m_guiElementID + ".over", new Rectangle(m_absX, m_absY, m_width, m_height)));
                m_downRects.Add(m_guiManager.CreateBox(m_guiElementID + ".out", new Rectangle(m_absX, m_absY, m_width, m_height)));
                m_disabledRects.Add(m_guiManager.CreateBox(m_guiElementID + ".out", new Rectangle(m_absX, m_absY, m_width, m_height)));
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_needsUpdate)
            {
                try
                {
                    if (m_outRects != null)
                        m_outRects.Clear();

                    if (m_overRects != null)
                        m_overRects.Clear();

                    if (m_downRects != null)
                        m_downRects.Clear();

                    if (m_disabledRects != null)
                        m_disabledRects.Clear();

                    m_outRects.Add(m_guiManager.CreateBox(m_guiElementID + ".out", new Rectangle(m_absX, m_absY, m_width, m_height)));
                    m_overRects.Add(m_guiManager.CreateBox(m_guiElementID + ".over", new Rectangle(m_absX, m_absY, m_width, m_height)));
                    m_downRects.Add(m_guiManager.CreateBox(m_guiElementID + ".out", new Rectangle(m_absX, m_absY, m_width, m_height)));
                    m_disabledRects.Add(m_guiManager.CreateBox(m_guiElementID + ".out", new Rectangle(m_absX, m_absY, m_width, m_height)));

                    m_needsUpdate = false;
                }
                catch (NullReferenceException nre)
                {
                    if (m_reporter != null)
                        m_reporter.BroadcastError(this, nre.Message, nre);
                }
                catch (Exception e)
                {
                    if (m_reporter != null)
                        m_reporter.BroadcastError(this, e.Message, e);
                }
            }

            if (!String.IsNullOrEmpty(this.Text))
                this.Text = String.Empty;
        }

        public override void Draw(GameTime gameTime)
        {
            //base.Draw(gameTime);

            if (!this.Enabled)
                Update(gameTime);

            m_spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            if (m_state == UIState.Out)
            {
                foreach (QuadBase _rect in m_outRects)
                {
                    m_spriteBatch.Draw(m_guiManager.GUITexture, new Vector2(m_absX, m_absY), _rect.Source, Color.White, m_params.Rotation, m_params.Origin, m_params.Scale, m_params.SpriteEffects, 0);
                }
                //m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);

                /*if (m_stats != null && m_stats.Visible)
                    m_stats.PolygonCount += m_outRects.Count * 2;*/
            }
            else if (m_state == UIState.Disabled)
            {
                foreach (QuadBase _rect in m_disabledRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, new Vector2(m_absX, m_absY), _rect.Source, Color.White, m_params.Rotation, m_params.Origin, m_params.Scale, m_params.SpriteEffects, 0);
                
                //m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);

                /*if (m_stats != null && m_stats.Visible)
                    m_stats.PolygonCount += m_disabledRects.Count * 2;*/
            }
            else if (m_state == UIState.Over)
            {
                foreach (QuadBase _rect in m_overRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, new Vector2(m_absX, m_absY), _rect.Source, Color.White, m_params.Rotation, m_params.Origin, m_params.Scale, m_params.SpriteEffects, 0);

                //m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);

                /*if (m_stats != null && m_stats.Visible)
                    m_stats.PolygonCount += m_overRects.Count * 2;*/
            }
            else if (m_state == UIState.Down)
            {
                foreach (QuadBase _rect in m_downRects)
                    m_spriteBatch.Draw(m_guiManager.GUITexture, new Vector2(m_absX, m_absY), _rect.Source, Color.White, m_params.Rotation, m_params.Origin, m_params.Scale, m_params.SpriteEffects, 0);

                //m_spriteBatch.Draw(m_guiManager.GUITexture, _rect.Destination, _rect.Source, Color.White);

                /*if (m_stats != null && m_stats.Visible)
                    m_stats.PolygonCount += m_downRects.Count * 2;*/
            }
            m_spriteBatch.End();

            if (m_font != null && !String.IsNullOrEmpty(m_text))
            {
                #region Text
                //Rectangle _rect = new Rectangle(m_absX + m_guiManager.CornerSize, m_absY + (int)(this.Height / 2) - (int)(m_font.LineHeight / 2), m_width - 5, m_height);
                Rectangle _rect = new Rectangle(m_absX + m_guiManager.CornerSize, m_absY + m_guiManager.CornerSize, m_width - m_guiManager.CornerSize, m_height);

                int from = _rect.Width / m_fontWidth;
                string toDraw = m_text;
                if (m_text.Length > from)
                    toDraw = m_text.Substring(m_text.Length - from);
				
				DrawTextInABox(_rect, m_foreColor, toDraw);
                #endregion
            }

            ResetDeviceStates();
        }

        protected override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);
        }

        protected override void OnMouseUp(MouseEventArgs args)
        {
            base.OnMouseUp(args);

            m_ebi.SetFocus(null);
        }

        protected override void OnKeyDown(object focus, KeyEventArgs k)
        {
            base.OnKeyDown(focus, k);
        }

        public string ElementID
        {
            get
            {
                return m_guiElementID;
            }
            set
            {
                m_guiElementID = value;

                m_needsUpdate = true;
            }
        }

        public SpriteParameters Parameters
        {
            get
            {
                return m_params;
            }
            set
            {
                m_params = value;
            }
        }
    }
}

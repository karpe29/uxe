#region License
/*
 *  Xna5D.GUI.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 18, 2006
 */
#endregion

#region Using Statements
using System;
using System.IO;
using System.Xml;
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

namespace Xe.Gui
{
    public partial class SlidePanel : UIControl
    {
        #region Members
        protected Button m_button;

        protected bool m_activated = false;

		Dock Dock;

        #endregion

		public SlidePanel(Game game, IGuiManager guiManager)
            : base(game, guiManager)
        {
            m_button = new Button(game, guiManager);
            m_button.Name = "btnOpenClose";
            m_button.ControlTag = "slide";
            m_button.Click += new ClickHandler(OnOpenCloseClick);
            m_button.Width = 13;
            m_button.Height = 25;

            game.Components.Add(m_button);

            this.Controls.Add(m_button);

            this.Dock = Dock.Right;
        }

        protected virtual void OnOpenCloseClick(object sender, MouseEventArgs args)
        {
            //System.Console.WriteLine("OPEN_CLOSE_CLICK");
            //System.Console.WriteLine("ORIGIN: " + m_miniButton.Parameters.Origin.X.ToString() + ", " + m_miniButton.Parameters.Origin.Y.ToString());

            if (m_activated)
            {
                //this.X = this.GraphicsDevice.Viewport.Width + 1;

				m_button.ControlTag = "slide";
            }
            else
            {
                //this.X = this.GraphicsDevice.Viewport.Width - this.Width;

				m_button.ControlTag = "slide2";
            }

            m_activated = !m_activated;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            try
            {/*
                switch (this.Dock)
                {
                    case Dock.Left:
                        this.X = -this.Width - 1;
                        m_button.Parameters.SpriteEffects = SpriteEffects.FlipHorizontally;
                        break;
                    case Dock.Right:
                        this.X = this.GraphicsDevice.Viewport.Width + 1;
                        m_button.Parameters.SpriteEffects = SpriteEffects.None;
                        break;
                    case Dock.Top:
                        this.Y = -this.Height - 1;
                        m_button.Parameters.SpriteEffects = SpriteEffects.None;
                        break;
                    case Dock.Bottom:
                        this.Y = this.GraphicsDevice.Viewport.Height + 1;
                        m_button.Parameters.SpriteEffects = SpriteEffects.FlipVertically;
                        break;
                    default:
                        this.X = this.GraphicsDevice.Viewport.Width + 1;
                        m_button.Parameters.SpriteEffects = SpriteEffects.None;
                        break;
                }

               */
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, e.Message, e);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_needsUpdate)
            {
                try
                {/*
                    switch (this.Dock)
                    {
                        case Dock.Left:
                            m_button.X = this.X + this.Width;
                            m_button.Y = this.Y + 5;
                            break;
                        case Dock.Right:
                            m_button.X = this.X - (m_button.Width);
                            m_button.Y = this.Y + 5;
                            break;
                        case Dock.Top:
                            m_button.X = this.X + 5;
                            m_button.Y = this.Y + this.Height;
                            //m_miniButton.Y = this.Y + this.Height;
                            //m_miniButton.Parameters.Origin = new Vector2((float)m_miniButton.X, (float)m_miniButton.Y);
                            break;
                        case Dock.Bottom:
                            m_button.X = this.X + 5;
                            m_button.Y = this.Y - m_button.Height;
                            break;
                        default:
                            m_button.X = this.X - (m_button.Width);
                            m_button.Y = this.Y + 5;
                            break;
                    }
					*/

                    m_needsUpdate = false;
                }
                catch (Exception e)
                {
                    if (m_reporter != null)
                        m_reporter.BroadcastError(this, e.Message, e);
                }
            }

            if (m_activated)
            {
                switch (this.Dock)
                {
                    case Dock.Right:
                        if(this.X > (this.GraphicsDevice.Viewport.Width - this.Width))
                            this.X--;
                        break;
                    case Dock.Left:
                        if(this.X < 0)
                            this.X++;
                        break;
                    case Dock.Top:
                        if (this.Y < 0)
                            this.Y++;
                        break;
                    case Dock.Bottom:
                        if (this.Y > (this.GraphicsDevice.Viewport.Height - this.Height))
                            this.Y--;
                        break;
                    default:
                        if (this.X < (this.GraphicsDevice.Viewport.Width + 1))
                            this.X++;
                        break;
                }
            }
            else
            {
                switch (this.Dock)
                {
                    case Dock.Right:
                        if (this.X < (this.GraphicsDevice.Viewport.Width + 1))
                            this.X++;
                        break;
                    case Dock.Left:
                        if (this.X + this.Width > -1)
                            this.X--;
                        break;
                    case Dock.Top:
                        if (this.Y + this.Height > -1)
                            this.Y--;
                        break;
                    case Dock.Bottom:
                        if (this.Y < (this.GraphicsDevice.Viewport.Height + 1))
                            this.Y++;
                        break;
                    default:
                        if (this.X < (this.GraphicsDevice.Viewport.Width + 1))
                            this.X++;
                        break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);
        }
    }
}



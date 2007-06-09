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
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using Xe;
using Xe.Input;
using Xe.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Xe.GUI
{
    public partial class Label : UIControl
    {
		public Label(Game game, GUIManager guiManager)
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

            try
            {
                m_outRects = m_guiManager.CreateControl("label.out", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_overRects = m_guiManager.CreateControl("label.out", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_downRects = m_guiManager.CreateControl("label.out", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_disabledRects = m_guiManager.CreateControl("label.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));
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
                {
                    m_outRects = m_guiManager.CreateControl("label.out", new Rectangle(m_absX, m_absY, m_width, m_height));
                    m_overRects = m_guiManager.CreateControl("label.out", new Rectangle(m_absX, m_absY, m_width, m_height));
                    m_downRects = m_guiManager.CreateControl("label.out", new Rectangle(m_absX, m_absY, m_width, m_height));
                    m_disabledRects = m_guiManager.CreateControl("label.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));

                    m_needsUpdate = false;
                }
                catch (Exception e)
                {
                    if (m_reporter != null)
                        m_reporter.BroadcastError(this, e.Message, e);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs args)
        {
            base.OnMouseDown(args);
        }
    }
}



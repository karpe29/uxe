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
using Xe.Input;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Xe.GUI
{
    public partial class Button : UIControl
    {
		public Button(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            this.IsHoverable = true;
            this.TextAlign = TextAlignment.Center;
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
            m_outRects = m_guiManager.CreateControl("button.out", new Rectangle(m_absX, m_absY, m_width, m_height));
            m_overRects = m_guiManager.CreateControl("button.over", new Rectangle(m_absX, m_absY, m_width, m_height));
            m_downRects = m_guiManager.CreateControl("button.down", new Rectangle(m_absX, m_absY, m_width, m_height));
            m_disabledRects = m_guiManager.CreateControl("button.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_needsUpdate)
            {
                m_outRects = m_guiManager.CreateControl("button.out", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_overRects = m_guiManager.CreateControl("button.over", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_downRects = m_guiManager.CreateControl("button.down", new Rectangle(m_absX, m_absY, m_width, m_height));
                m_disabledRects = m_guiManager.CreateControl("button.disabled", new Rectangle(m_absX, m_absY, m_width, m_height));

                m_needsUpdate = false;
            }
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
    }
}

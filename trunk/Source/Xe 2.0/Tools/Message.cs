#region License
/*
 *  Xna5D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: November 30, 2006
 */
#endregion

#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Tools
{
    public class Message
    {
        #region Members
        private string m_dest = String.Empty;   // The destination ID.
        private string m_source = String.Empty; // The source ID.

        private object m_sender = null;         // A reference to the sender.

        private string m_msg = String.Empty;    // The textual message.

        private object m_data = null;           // Optional data.

        private Message m_innerMsg = null;      // An inner message.
        #endregion

        #region Constructor
        public Message(object sender)
        {
            m_sender = sender;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The ID of the Destination object.
        /// </summary>
        public string Destination
        {
            get
            {
                return m_dest;
            }
            set
            {
                m_dest = value;
            }
        }

        /// <summary>
        /// The ID of the Source object.
        /// </summary>
        public string Source
        {
            get
            {
                return m_source;
            }
            set
            {
                m_source = value;
            }
        }

        /// <summary>
        /// A reference to the Sender object.
        /// </summary>
        public object Sender
        {
            get
            {
                return m_sender;
            }
            set
            {
                m_sender = value;
            }
        }

        /// <summary>
        /// The textual message.
        /// </summary>
        public string Msg
        {
            get
            {
                return m_msg;
            }
            set
            {
                m_msg = value;
            }
        }

        /// <summary>
        /// Optional object data.
        /// </summary>
        public object Data
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
            }
        }

        /// <summary>
        /// Optional Inner Message.
        /// </summary>
        public Message InnerMessage
        {
            get
            {
                return m_innerMsg;
            }
            set
            {
                m_innerMsg = value;
            }
        }
        #endregion
    }
}

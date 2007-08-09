#region License
/*
 *  Xna5D.Data.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: November 30, 2006
*/
#endregion License

#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Xe.Tools;
#endregion

namespace Xe.Tools
{
    public delegate void BeforeClosingHandler();

    public partial class XmlLogger : Microsoft.Xna.Framework.GameComponent, IXmlLoggerService, Xe.Tools.IService
    {
        #region Members
        protected XmlTextWriter m_xmlWriter;
        protected string m_xmlFile;
        protected string m_htmlFile;

        protected bool m_isOpen;

        protected int m_brackets = 0;

        public event BeforeClosingHandler BeforeClosing;

        protected Xe.Tools.IReporterService m_reporter;
        #endregion

        #region Constructor & Initialization
		public XmlLogger(Game game)
            : base(game)
        {
            if (game != null)
            {
                game.Services.AddService(typeof(IXmlLoggerService), this);
                game.Exiting += new EventHandler(Game_Exiting);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));
            
            AddHandlers();
        }
        #endregion

        #region Event Handlers
        protected virtual void AddHandlers()
        {
            if (m_reporter != null)
            {
                m_reporter.MessageReported += new MessageReportedHandler(this.OnMessageReported);
                m_reporter.ErrorReported += new ErrorReportedHandler(this.OnErrorReported);
                m_reporter.WarningReported += new WarningReportedHandler(this.OnWarningReported);
            }
        }

        protected virtual void RemoveHandlers()
        {
            if (m_reporter != null)
            {
                m_reporter.MessageReported -= this.OnMessageReported;
                m_reporter.ErrorReported -= this.OnErrorReported;
                m_reporter.WarningReported -= this.OnWarningReported;
            }
        }

        protected virtual void OnWarningReported(Message msg)
        {
            WriteWarning(msg);
        }

        protected virtual void OnErrorReported(Message msg, Exception except)
        {
            WriteError(except.GetType(), msg);
        }

        protected virtual void OnMessageReported(Message msg)
        {
            WriteMessage(msg);
        }
        #endregion

        void Game_Exiting(object sender, EventArgs e)
        {
            this.CloseFile();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #region IXmlLoggerService Members
        #region Loading
        public void LoadFile(string xmlFile)
        {
            if (m_isOpen)
            {
                if (m_xmlFile.Equals(xmlFile))
                    return;

                this.CloseFile();
            }

            m_xmlFile = xmlFile;

            if (m_xmlFile.EndsWith(".xml"))
                m_htmlFile = m_xmlFile.Substring(0, m_xmlFile.Length - 4) + ".html";

            m_xmlWriter = new XmlTextWriter(m_xmlFile, null);
            m_xmlWriter.Formatting = Formatting.Indented;

            m_xmlWriter.WriteStartDocument();
            m_xmlWriter.WriteStartElement("Log");

            m_isOpen = true;

            Message start = new Message(this);
            start.Source = this.ID;
            start.Destination = this.ID;
            start.Msg = "Log started at " + DateTime.Now.ToString();

            WriteMessage(start);
        }
        #endregion

        #region Closing
        public void CloseFile()
        {
            if (m_xmlWriter != null && m_isOpen)
            {
                if (BeforeClosing != null)
                    BeforeClosing.Invoke();

                Message end = new Message(this);
                end.Source = this.ID;
                end.Destination = this.ID;
                end.Msg = "Log finished at " + DateTime.Now.ToString();
                WriteMessage(end);

                m_xmlWriter.WriteEndElement();
                m_xmlWriter.WriteEndDocument();

                m_xmlWriter.Close();

                Transform();
            }

            m_isOpen = false;
        }

        protected virtual void Transform()
        {

            XslCompiledTransform xslTrans = new XslCompiledTransform();
			
			string s = File.ReadAllText(@"Content\XML\Log.xslt");
			
			if (s != null)
			{
				XmlReader xr = XmlReader.Create(new StringReader(s));
				xslTrans.Load(xr);

				xslTrans.Transform(m_xmlFile, m_htmlFile);
			}
        }
        #endregion

        #region Errors
        public void WriteError(Type exceptionType, string msg)
        {
            if (m_isOpen)
            {
                m_xmlWriter.WriteStartElement("Error");

                m_xmlWriter.WriteStartElement("ExceptionType");
                m_xmlWriter.WriteString(exceptionType.Name);
                m_xmlWriter.WriteEndElement();

                m_xmlWriter.WriteStartElement("Text");
                m_xmlWriter.WriteString(msg);
                m_xmlWriter.WriteEndElement();

                m_xmlWriter.WriteEndElement();
            }
        }

        public void WriteError(Type exceptionType, Message msg)
        {
            if (m_isOpen)
            {
                m_xmlWriter.WriteStartElement("Error");

                m_xmlWriter.WriteStartElement("ExceptionType");
                m_xmlWriter.WriteString(exceptionType.Name);
                m_xmlWriter.WriteEndElement();

                WriteMessage(msg);

                m_xmlWriter.WriteEndElement();
            }
        }
        #endregion

        #region Warnings
        public void WriteWarning(string msg)
        {
            if (m_isOpen)
            {
                m_xmlWriter.WriteStartElement("Warning");

                m_xmlWriter.WriteStartElement("Text");
                m_xmlWriter.WriteString(msg);
                m_xmlWriter.WriteEndElement();

                m_xmlWriter.WriteEndElement();
            }
        }

        public void WriteWarning(Message msg)
        {
            if (m_isOpen)
            {
                m_xmlWriter.WriteStartElement("Warning");

                WriteMessage(msg);

                m_xmlWriter.WriteEndElement();
            }
        }
        #endregion

        #region Messages
        public void WriteMessage(string msg)
        {
            if (m_isOpen)
            {
                m_xmlWriter.WriteStartElement("Message");

                m_xmlWriter.WriteStartElement("Text");
                m_xmlWriter.WriteString(msg);
                m_xmlWriter.WriteEndElement();

                m_xmlWriter.WriteEndElement();
            }
        }

        public void WriteMessage(Message msg)
        {
            if (m_isOpen)
            {
                m_xmlWriter.WriteStartElement("Message");

                m_xmlWriter.WriteStartElement("Source");
                m_xmlWriter.WriteString(msg.Source);
                m_xmlWriter.WriteEndElement();

                m_xmlWriter.WriteStartElement("Destination");
                m_xmlWriter.WriteString(msg.Destination);
                m_xmlWriter.WriteEndElement();

                m_xmlWriter.WriteStartElement("Text");
                m_xmlWriter.WriteString(msg.Msg);
                m_xmlWriter.WriteEndElement();

                if (msg.InnerMessage != null)
                    WriteMessage(msg.InnerMessage);

                m_xmlWriter.WriteEndElement();
            }
        }
        #endregion

        #region Brackets
        public void StartBracket(string name)
        {
            if (m_isOpen)
            {
                m_xmlWriter.WriteStartElement("Bracket");
                m_xmlWriter.WriteStartAttribute("name");
                m_xmlWriter.WriteString(name);
                m_xmlWriter.WriteEndAttribute();

                m_brackets++;
            }
        }

        public void EndBracket()
        {
            if (m_isOpen)
            {
                if (m_brackets > 0)
                {
                    m_xmlWriter.WriteEndElement();

                    m_brackets--;
                }
            }
        }
        #endregion

        public bool IsOpen
        {
            get
            {
                return m_isOpen;
            }
        }

        #endregion

        #region IService Members
        public string ID
        {
            get
            {
                return "XmlLogger";
            }
        }

        public void LoadSettings(XmlNode node)
        {
        }
        #endregion
    }

    #region IXmlLoggerService Interface
    public interface IXmlLoggerService
    {
        /// <summary>
        /// BeforeClosingHandler Event
        /// </summary>
        event BeforeClosingHandler BeforeClosing;

        /// <summary>
        /// Load File creates the Xml File for logging.
        /// </summary>
        /// <param name="xmlFile">The Xml File to log to.</param>
        void LoadFile(string xmlFile);

        /// <summary>
        /// Closes the Xml logging file.
        /// </summary>
        void CloseFile();

        /// <summary>
        /// Writes an Error to the log file.
        /// </summary>
        /// <param name="exceptionType">The type of exception to log.</param>
        /// <param name="msg">The associated textual message.</param>
        void WriteError(Type exceptionType, string msg);

        /// <summary>
        /// Writes an Error to the log file.
        /// </summary>
        /// <param name="exceptionType">The type of exception to log.</param>
        /// <param name="msg">The associated Message object to log.</param>
        void WriteError(Type exceptionType, Message msg);

        /// <summary>
        /// Writes a Warning to the log file.
        /// </summary>
        /// <param name="msg">The textual warning to log.</param>
        void WriteWarning(string msg);

        /// <summary>
        /// Writes a Warning to the log file.
        /// </summary>
        /// <param name="msg">The Message object to log.</param>
        void WriteWarning(Message msg);

        /// <summary>
        /// Write a Message to the log file.
        /// </summary>
        /// <param name="msg">The textual message to log.</param>
        void WriteMessage(string msg);

        /// <summary>
        /// Write a Message to the log file.
        /// </summary>
        /// <param name="msg">The Message object to log.</param>
        void WriteMessage(Message msg);

        /// <summary>
        /// Starts an Element Bracket.
        /// </summary>
        /// <param name="name">The name of the Element to create.</param>
        void StartBracket(string name);

        /// <summary>
        /// Closes an Element Bracket.
        /// </summary>
        void EndBracket();

        /// <summary>
        /// True if the file is open, false if it is not.
        /// </summary>
        bool IsOpen { get; }
    }
    #endregion
}



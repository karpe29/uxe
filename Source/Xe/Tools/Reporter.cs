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
using Xe.Tools;
#endregion

namespace Xe.Tools
{
	#region IService Interface
    public interface IService
    {
        /// <summary>
        /// A unique ID.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Load settings via XML.
        /// </summary>
        /// <param name="node">The appropriate XmlNode.</param>
        void LoadSettings(XmlNode node);
    }
    #endregion

    public partial class Reporter : Microsoft.Xna.Framework.GameComponent, IService, IReporterService
    {
        #region Members
        private ReportLevel m_reportLevel = ReportLevel.Errors | ReportLevel.FatalErrors | ReportLevel.Messages;
        private bool m_throwExceptions = false;

        public event ErrorReportedHandler ErrorReported;
        public event WarningReportedHandler WarningReported;
        public event MessageReportedHandler MessageReported;
        #endregion

        #region Constructor and Destructor
		public Reporter(Game game)
            : base(game)
        {
            if (game != null)
                game.Services.AddService(typeof(IReporterService), this);

			AddHandlers();
        }

        ~Reporter()
        {
            this.Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            RemoveHandlers();
        }
        #endregion

        #region Event Handlers
        protected virtual void AddHandlers()
        {
            this.ErrorReported += new ErrorReportedHandler(this.OnErrorReported);
            this.MessageReported += new MessageReportedHandler(this.OnMessageReported);
            this.WarningReported += new WarningReportedHandler(this.OnWarningReported);
        }

        protected virtual void RemoveHandlers()
        {
            // Remove the Error reported handler.
            if (this.ErrorReported != null)
                this.ErrorReported -= this.OnErrorReported;

            // Remove the Warning Reported handler.
            if (this.WarningReported != null)
                this.WarningReported -= this.OnWarningReported;

            // Remove the Message Reported handler.
            if (this.MessageReported != null)
                this.MessageReported -= this.OnMessageReported;
        }

        protected virtual void OnWarningReported(Message msg)
        {
        }

        protected virtual void OnMessageReported(Message msg)
        {
        }

        protected virtual void OnErrorReported(Message msg, Exception except)
        {
        }
        #endregion

        #region IService Members
        public string ID
        {
            get
            {
                return "Reporter";
            }
        }

        public void LoadSettings(XmlNode node)
        {
        }
        #endregion

        #region IReporterService Members
        #region Errors
        public void BroadcastError(object sender, string msg, Exception exception)
        {
            BroadcastError(sender, msg, exception, false);
        }

        public void BroadcastError(Message msg, Exception exception)
        {
            BroadcastError(msg, exception, false);
        }

        public void BroadcastError(object sender, string msg, Exception exception, bool fatal)
        {
            Message _msg = new Message(sender);
            _msg.Msg = msg;
            _msg.Data = exception;

            IService _sender = sender as IService;
            if (_sender != null)
                _msg.Source = _sender.ID;

            _msg.Destination = this.ID;

            BroadcastError(_msg, exception, fatal);
        }

        public void BroadcastError(Message msg, Exception exception, bool fatal)
        {
            if (fatal)
            {
                if((m_reportLevel & ReportLevel.FatalErrors) != ReportLevel.FatalErrors)
                    return;
            }
            else
            {
                if((m_reportLevel & ReportLevel.Errors) != ReportLevel.Errors)
                    return;
            }

            if (this.ErrorReported != null)
                this.ErrorReported.Invoke(msg, exception);

            if (fatal || m_throwExceptions)
                throw exception;
        }
        #endregion

        #region Warnings
        public void BroadcastWarning(object sender, string msg)
        {
            Message _msg = new Message(sender);
            _msg.Msg = msg;

            IService _sender = sender as IService;
            if (_sender != null)
                _msg.Source = _sender.ID;

            _msg.Destination = this.ID;

            BroadcastWarning(_msg);
        }

        public void BroadcastWarning(Message msg)
        {
            if ((m_reportLevel & ReportLevel.Warnings) != ReportLevel.Warnings)
                return;

            if (this.WarningReported != null)
                this.WarningReported.Invoke(msg);
        }
        #endregion

        #region Messages
        public void BroadcastMessage(object sender, string msg)
        {
            Message _msg = new Message(sender);
            _msg.Msg = msg;

            IService _sender = sender as IService;
            if (_sender != null)
                _msg.Source = _sender.ID;

            _msg.Destination = this.ID;

            BroadcastMessage(_msg);
        }

        public void BroadcastMessage(Message msg)
        {
            if ((m_reportLevel & ReportLevel.Messages) != ReportLevel.Messages)
                return;

            if (this.MessageReported != null)
                this.MessageReported.Invoke(msg);
        }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Determines what gets broadcasted and what does not.
        /// Default setting is Errors and FatalErrors.
        /// </summary>
        public ReportLevel BroadcastLevel
        {
            get
            {
                return m_reportLevel;
            }
            set
            {
                m_reportLevel = value;
            }
        }

        /// <summary>
        /// Determines whether or not exceptions are thrown
        /// via the normal Windows way. If set to True, exceptions
        /// will be thrown after being broadcasted.
        /// </summary>
        public bool ThrowExceptions
        {
            get
            {
                return m_throwExceptions;
            }
            set
            {
                m_throwExceptions = value;
            }
        }
        #endregion
    }

    #region IReporterService Interface
    public interface IReporterService
    {
        event ErrorReportedHandler ErrorReported;
        event WarningReportedHandler WarningReported;
        event MessageReportedHandler MessageReported;

        #region Broadcasting Errors
        /// <summary>
        /// Broadcast an Error.
        /// </summary>
        /// <param name="sender">The object broadcasting the error.</param>
        /// <param name="msg">The textual message to be sent.</param>
        /// <param name="exception">The exception that should be thrown.</param>
        void BroadcastError(object sender, string msg, Exception exception);

        /// <summary>
        /// Broadcast an Error.
        /// </summary>
        /// <param name="msg">A valid Message object.</param>
        /// <param name="exception">The exception that should be thrown.</param>
        void BroadcastError(Message msg, Exception exception);

        /// <summary>
        /// Broadcast an Error.
        /// </summary>
        /// <param name="sender">The object broadcasting the error.</param>
        /// <param name="msg">The textual message to be sent.</param>
        /// <param name="exception">The exception that should be thrown.</param>
        /// <param name="fatal">Whether or not the exception is fatal. If true, the exception
        /// will be thrown no matter what.</param>
        void BroadcastError(object sender, string msg, Exception exception, bool fatal);

        /// <summary>
        /// Broadcast an Error.
        /// </summary>
        /// <param name="msg">A valid Message object.</param>
        /// <param name="exception">The exception that should be thrown.</param>
        /// <param name="fatal">Whether or not the exception is fatal. If true, the exception
        /// will be thrown no matter what.</param>
        void BroadcastError(Message msg, Exception exception, bool fatal);
        #endregion

        #region Broadcasting Warnings
        /// <summary>
        /// Broadcast a Warning.
        /// </summary>
        /// <param name="sender">The object broadcasting the warning.</param>
        /// <param name="msg">The textual message to be passed along.</param>
        void BroadcastWarning(object sender, string msg);

        /// <summary>
        /// Broadcast a Warning.
        /// </summary>
        /// <param name="msg">A valid Message object.</param>
        void BroadcastWarning(Message msg);
        #endregion

        #region Broadcasting Messages
        /// <summary>
        /// Broadcast a Message
        /// </summary>
        /// <param name="sender">The object broadcasting the message.</param>
        /// <param name="msg">The textual message to be passed along.</param>
        void BroadcastMessage(object sender, string msg);

        /// <summary>
        /// Broadcast a Message
        /// </summary>
        /// <param name="msg">A valid Message object.</param>
        void BroadcastMessage(Message msg);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the level that determines what type of messages get
        /// broadcasted.
        /// </summary>
        ReportLevel BroadcastLevel { get; set; }

        /// <summary>
        /// Gets or Sets the boolean that determines whether or not exceptions
        /// get thrown. This is overruled by the 'fatal' boolean.
        /// </summary>
        bool ThrowExceptions { get; set; }
        #endregion
    }
    #endregion

    public enum ReportLevel
    {
		None = 0,
        Messages = 1,
        Warnings = 2,
        Errors = 4,
        FatalErrors = 8
    }

    public delegate void ErrorReportedHandler(Message msg, Exception except);
    public delegate void WarningReportedHandler(Message msg);
    public delegate void MessageReportedHandler(Message msg);
}



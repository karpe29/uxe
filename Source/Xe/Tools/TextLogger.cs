#region License
/*
 *  Xna5D.Data.dll
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
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;
#endregion

namespace Xe.Tools
{
    public partial class TextLogger : Microsoft.Xna.Framework.GameComponent, IService, ITextLoggerService
    {
        #region Members
        protected StreamWriter m_stream;
        private string m_fileName = "log.txt";

        private bool m_isOpen = false;

        private int m_currentBlock = 0;

        protected IReporterService m_reporter;
        #endregion

        #region Constructor
		public TextLogger(Game game)
            : base(game)
        {
            if (game != null)
                game.Services.AddService(typeof(ITextLoggerService), this);
        }
        #endregion

        #region Game Component Overrides
        public override void Initialize()
        {
            base.Initialize();

            m_reporter = (IReporterService)this.Game.Services.GetService(typeof(IReporterService));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        #region ITextLoggerService Members
        #region Writing Errors
        public void WriteError(object sender, Type exceptionType, string message)
        {
            Message _msg = new Message(sender);

            _msg.Msg = String.Format("ERROR ({0}):\n{1}{2}", exceptionType.Name, "{tabs}", message);

            IService _service = sender as IService;
            if (_service != null)
                _msg.Source = _service.ID;

            _msg.Destination = this.ID;

            WriteError(_msg);
        }

        public void WriteError(Message msg)
        {
            Write(msg);
        }
        #endregion

        #region Writing Warnings
        public void WriteWarning(object sender, string message)
        {
            Message _msg = new Message(sender);

            _msg.Msg = String.Format("WARNING:\n{1}{2}", "{tabs}", message);

            IService _service = sender as IService;
            if (_service != null)
                _msg.Source = _service.ID;

            _msg.Destination = this.ID;

            WriteWarning(_msg);
        }

        public void WriteWarning(Message msg)
        {
            Write(msg);
        }
        #endregion

        #region Writing Messages
        public void WriteLine(string line)
        {
            if (m_isOpen && m_stream != null)
                m_stream.WriteLine(GetTabs() + line);
        }

        public void Write(string text)
        {
            if (m_isOpen && m_stream != null)
                m_stream.Write(text);
        }

        public void WriteMessage(object sender, string message)
        {
            Message _msg = new Message(sender);

            _msg.Msg = String.Format("MESSAGE:\n{1}{2}", "{tabs}", message);

            IService _service = sender as IService;
            if (_service != null)
                _msg.Source = _service.ID;

            _msg.Destination = this.ID;

            WriteMessage(_msg);
        }

        public void WriteMessage(Message msg)
        {
            Write(msg);
        }
        #endregion

        #region Blocks
        public void StartBlock()
        {
            m_currentBlock++;
        }

        public void EndBlock()
        {
            if (m_currentBlock > 0)
                m_currentBlock--;
        }
        #endregion

        #region Opening and Closing
        public void OpenLog(string file)
        {
            OpenLog(file, true, true);
        }

        public void OpenLog(string file, bool createDirs)
        {
            OpenLog(file, createDirs, true);
        }

        public void OpenLog(string file, bool createDirs, bool append)
        {
            try
            {
                if (m_isOpen)
                {
                    if (m_fileName.Equals(file))
                        return;

                    CloseLog();
                }

                m_fileName = file;

                m_stream = new StreamWriter(m_fileName, append);

                m_isOpen = true;
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, e.Message, e, false);
            }
        }

        public void CloseLog()
        {
            if (m_isOpen && m_stream != null)
            {
                m_stream.Close();

                m_isOpen = false;
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
                return "TextLogger";
            }
        }

        public void LoadSettings(System.Xml.XmlNode node)
        {
        }
        #endregion

        #region Methods
        private void Write(Message msg)
        {
            if (m_isOpen && m_stream != null)
            {
                string _tabs = GetTabs();
                msg.Msg.Replace("{tabs}", _tabs);

                m_stream.WriteLine(String.Format("{0}Source:{1}\n{2}Destination:{3}\n{4}Message:{5}", _tabs, msg.Source, _tabs, msg.Destination, _tabs, msg.Msg));
            }
        }

        private string GetTabs()
        {
            string _tabs = "";
            for (int i = 0; i < m_currentBlock; i++)
                _tabs += "\t";

            return _tabs;
        }
        #endregion
    }

    #region ITextLoggerService Interface
    public interface ITextLoggerService
    {
        void Write(string text);
        void WriteLine(string line);

        void WriteError(object sender, Type exceptionType, string message);
        void WriteError(Message msg);

        void WriteWarning(object sender, string message);
        void WriteWarning(Message msg);

        void WriteMessage(object sender, string message);
        void WriteMessage(Message msg);

        void StartBlock();
        void EndBlock();

        void OpenLog(string file);
        void OpenLog(string file, bool createDirs);
        void OpenLog(string file, bool createDirs, bool append);

        void CloseLog();

        bool IsOpen { get; }
    }
    #endregion
}



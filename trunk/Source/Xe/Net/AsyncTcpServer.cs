#region License
/*
 *  Xna5D.Net.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 */
#endregion
#if !XBOX360
#region Using Statements
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

using XeFramework;

using Microsoft.Xna.Framework;
#endregion

namespace XeFramework.Net
{
    public partial class AsyncTcpServer : Microsoft.Xna.Framework.GameComponent
    {
        #region Members
        private List<SocketPacket> m_clients = new List<SocketPacket>();
        private AsyncCallback m_workerCallback;

        protected Socket m_listener;

        protected string m_ip = "127.0.0.1";
        protected int m_port = 11000;

        private int m_maxConnections = 100;

        private string m_name = String.Empty;

        protected IReporterService m_reporter;
        #endregion

        #region Events
        public event DataReceivedHandler DataReceived;

        public event ClientConnectedHandler ClientConnected;
        public event ClientDisconnectedHandler ClientDisconnected;

        public event ClientAuthRequestHandler ClientAuthRequest;

        public event ServerStartedHandler ServerStarted;
        public event ServerStoppedHandler ServerStopped;
        #endregion

        #region Constructor
		public AsyncTcpServer(Game game)
            : base(game)
        {
            SetupHandlers(game);
        }

		public AsyncTcpServer(string serverName, Game game)
            : base(game)
        {
            m_name = serverName;

            SetupHandlers(game);
        }

        ~AsyncTcpServer()
        {
            Dispose(true);
        }

		protected void SetupHandlers(Game game)
        {
            #region Add Handlers
            this.DataReceived += new DataReceivedHandler(OnDataReceived);
            this.ClientConnected += new ClientConnectedHandler(OnClientConnected);
            this.ClientDisconnected += new ClientDisconnectedHandler(OnClientDisconnected);
            this.ClientAuthRequest += new ClientAuthRequestHandler(OnClientAuthRequest);
            this.ServerStarted += new ServerStartedHandler(OnServerStarted);
            this.ServerStopped += new ServerStoppedHandler(OnServerStopped);
            #endregion

            game.Exiting += new EventHandler(OnGameExiting);
        }
        #endregion

        #region Virtual Event Handlers
        protected virtual void OnClientAuthRequest(SocketPacket packet)
        {
        }

        protected virtual void OnClientDisconnected(SocketPacket packet)
        {
        }

        protected virtual void OnClientConnected(SocketPacket packet)
        {
        }

        protected virtual void OnDataReceived(string data, SocketPacket packet)
        {
        }

        protected virtual void OnServerStopped()
        {
        }

        protected virtual void OnServerStarted()
        {
        }
        #endregion

        #region Starting and Stopping
        public void Start(int port)
        {
            m_port = port;

            Start();
        }

        public void Start()
        {
            try
            {
                if (m_listener != null)
                    return;

                m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, m_port);

                m_listener.Bind(ipLocal);
                m_listener.Listen(m_maxConnections);

                m_listener.BeginAccept(new AsyncCallback(OnClientConnect), null);

                if (ServerStarted != null)
                    ServerStarted.Invoke();
            }
            catch (SocketException se)
            {
                WriteError(se);
            }
        }

        public void Stop()
        {
            m_listener.Close();

            foreach (SocketPacket _client in m_clients)
            {
                if (_client.Socket.Connected)
                    if (ClientDisconnected != null)
                        ClientDisconnected.Invoke(_client);

                _client.Socket.Close();
            }

            m_clients.Clear();

            if (ServerStopped != null)
                ServerStopped.Invoke();
        }
        #endregion

        #region Callbacks and Client Handling
        protected virtual void OnClientConnect(IAsyncResult ar)
        {
            try
            {
                int _tempID = m_clients.Count + 1;
                SocketPacket _tempClient = new SocketPacket(_tempID.ToString());

                _tempClient.Socket = m_listener.EndAccept(ar);

                m_clients.Add(_tempClient);

                string _str = String.Format("Client #{0} connected.", _tempID);

                if (ClientConnected != null)
                    ClientConnected.Invoke(_tempClient);

                if (DataReceived != null)
                    DataReceived.Invoke(_str, _tempClient);

                WaitForData(_tempClient);

                m_listener.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (ObjectDisposedException ode)
            {
                WriteError(ode, "OnClientConnect: Socket has been closed.");
            }
            catch (SocketException se)
            {
                WriteError(se);
            }
        }

        protected void WaitForData(SocketPacket client)
        {
            try
            {
                if (m_workerCallback == null)
                    m_workerCallback = new AsyncCallback(OnClientDataReceived);

                client.Socket.BeginReceive(client.DataBuffer, 0, client.BufferLength, 
                                           SocketFlags.None, m_workerCallback, client);
            }
            catch(SocketException se)
            {
                WriteError(se);
            }
        }

        protected virtual void OnClientDataReceived(IAsyncResult ar)
        {
            SocketPacket _client = (SocketPacket)ar.AsyncState;

            try
            {
                int iRx = 0;
                iRx = _client.Socket.EndReceive(ar);

                byte[] _data = new byte[_client.BufferLength];
                _client.DataBuffer.CopyTo(_data, 0);

                string _strData = Encoding.ASCII.GetString(_data, 0, iRx);

                if (_client.IsReady)
                {
                    if (DataReceived != null)
                        DataReceived.Invoke(_strData, _client);

                    _client.BufferLength = _client.DefaultBufferLength;
                    _client.ResetBuffer();

                    _client.LastHeader.Message = String.Empty;

                    _client.IsReady = false;
                }
                else
                {
                    _client.LastHeader.Message += _strData;

                    if (_client.LastHeader.Message.Contains("</HEADER>"))
                    {
                        int _bytesNeeded = 256;
                        if (_client.LastHeader.Message.StartsWith("<HEADER>"))
                        {
                            int _first, _last;
                            _first = _client.LastHeader.Message.IndexOf("<BYTES>") + 7;
                            _last = _client.LastHeader.Message.IndexOf("</BYTES>");

                            string _bytes = _client.LastHeader.Message.Substring(_first, _last - _first);
                            if (int.TryParse(_bytes, out _bytesNeeded))
                            {
                                _client.IsReady = true;
                                _client.BufferLength = _bytesNeeded;
                                _client.ResetBuffer();

                                _client.LastHeader.ByteCount = _bytesNeeded;
                            }
                        }
                    }
                }

                #region Old Code
                //    SocketPacket _client = (SocketPacket)ar.AsyncState;

                //    int iRx = 0;
                //    iRx = _client.Socket.EndReceive(ar);

                //    byte[] _data = new byte[_client.BufferLength];
                //    _client.DataBuffer.CopyTo(_data, 0);

                //    //_client.BufferLength = iRx;

                //    string _strData = Encoding.ASCII.GetString(_data, 0, iRx);

                //    if (DataReceived != null)
                //        DataReceived.Invoke(_strData, _client);

                //    _client.ResetBuffer();

                //    WaitForData(_client);
                #endregion

                WaitForData(_client);
            }
            catch (ObjectDisposedException ode)
            {
                WriteError(ode, "OnClientDataReceived: Socket has been closed.");

                if (this.ClientDisconnected != null)
                    ClientDisconnected.Invoke(_client);
            }
            catch (SocketException se)
            {
                WriteError(se);
            }
            catch (Exception e)
            {
                WriteError(e);
            }
        }
        #endregion

        #region Sending Data
        public void SendToAll(string message)
        {
            byte[] _bytes = Encoding.ASCII.GetBytes(message);

            SendToAll(_bytes);
        }

        public void Send(string clientID, string message)
        {
            Send(clientID, message, false);
        }

        public void Send(string clientID, string message, bool allOccurances)
        {
            byte[] _bytes = Encoding.ASCII.GetBytes(message);

            Send(clientID, _bytes, allOccurances);
        }

        public void Send(string clientID, byte[] data)
        {
            Send(clientID, data, false);
        }

        public void Send(string clientID, byte[] data, bool allOccurances)
        {
            try
            {
                string _header = String.Format("<HEADER><BYTES>{0}</BYTES></HEADER>", data.Length);
                byte[] _headerBytes = Encoding.ASCII.GetBytes(_header);

                foreach (SocketPacket _client in m_clients)
                {
                    if (_client != null && _client.Socket.Connected)
                    {
                        if (_client.ID.Equals(clientID))
                        {
                            _client.Socket.BeginSend(_headerBytes, 0, _headerBytes.Length, SocketFlags.None, new AsyncCallback(OnSendHeader), _client);

                            _client.SendBuffer = data;

                            if (!allOccurances)
                                return;
                        }
                    }
                }
            }
            catch (SocketException se)
            {
                WriteError(se);
            }
        }

        public void SendToAll(byte[] data)
        {
            try
            {
                string _header = String.Format("<HEADER><BYTES>{0}</BYTES></HEADER>", data.Length);
                byte[] _headerBytes = Encoding.ASCII.GetBytes(_header);

                foreach (SocketPacket _client in m_clients)
                {
                    if (_client != null && _client.Socket.Connected)
                    {
                        _client.Socket.BeginSend(_headerBytes, 0, _headerBytes.Length, SocketFlags.None, new AsyncCallback(OnSendHeader), _client);

                        _client.SendBuffer = data;
                    }
                }
            }
            catch (SocketException se)
            {
                WriteError(se);
            }
        }

        private void OnSendHeader(IAsyncResult ar)
        {
            try
            {
                SocketPacket _client = (SocketPacket)ar.AsyncState;

                int _sent = _client.Socket.EndSend(ar);

                _client.Socket.Send(_client.SendBuffer);

                _client.SendBuffer = new byte[1];
            }
            catch (ObjectDisposedException ode)
            {
                WriteError(ode);
            }
            catch (SocketException se)
            {
                WriteError(se);
            }
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

        #region Methods (General)
        protected virtual void WriteError(SocketException exception)
        {
            if (m_reporter == null)
                return;

            Message msg = new Message(this);
            msg.Destination = ((IService)m_reporter).ID;
            msg.Source = "AsyncServer";
            msg.Msg = String.Format("SocketErrorCode: {0}, NativeErrorCode: {1}, Message{2}",
                                    exception.SocketErrorCode.ToString(),
                                    exception.NativeErrorCode.ToString(),
                                    exception.Message.ToString());

            m_reporter.BroadcastError(msg, exception);
        }

        protected virtual void WriteError(ObjectDisposedException exception)
        {
            if (m_reporter == null)
                return;

            Message msg = new Message(this);
            msg.Destination = ((IService)m_reporter).ID;
            msg.Source = "AsyncServer";
            msg.Msg = String.Format("Object Disposed Exception! ObjectName: {0}, Message: {1}",
                                    exception.ObjectName, exception.Message);

            m_reporter.BroadcastError(msg, exception);
        }

        protected virtual void WriteError(ObjectDisposedException exception, string message)
        {
            if (m_reporter == null)
                return;

            Message msg = new Message(this);
            msg.Destination = ((IService)m_reporter).ID;
            msg.Source = "AsyncServer";
            msg.Msg = String.Format("Object Disposed Exception! ObjectName: {0}, Message: {1}; {2}",
                                    exception.ObjectName, msg, exception.Message);

            m_reporter.BroadcastError(msg, exception);
        }

        protected virtual void WriteError(Exception exception)
        {
            if (m_reporter == null)
                return;

            Message msg = new Message(this);
            msg.Destination = ((IService)m_reporter).ID;
            msg.Source = "AsyncServer";
            msg.Msg = String.Format("Message: {0}", exception.Message);

            m_reporter.BroadcastError(msg, exception);
        }
        
        public bool SetPort(string port)
        {
            int _port = 0;
            if (int.TryParse(port, out _port))
            {
                m_port = _port;

                return true;
            }

            return false;
        }
        #endregion

        #region Disposing & Exiting
        protected virtual void OnGameExiting(object sender, EventArgs e)
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Stop();
        }
        #endregion

        #region Properties
        public string IP
        {
            get
            {
                return m_ip;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        public int MaxConnections
        {
            get
            {
                return m_maxConnections;
            }
            set
            {
                m_maxConnections = value;
            }
        }

        public int Port
        {
            get
            {
                return m_port;
            }
            set
            {
                m_port = value;
            }
        }
        #endregion
    }
}


#endif
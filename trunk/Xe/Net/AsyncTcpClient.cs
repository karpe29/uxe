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
    public partial class AsyncTcpClient : Microsoft.Xna.Framework.GameComponent
    {
        #region Members
        // Our connection to the server
        protected SocketPacket m_socket;

        // Callback stuff
        private IAsyncResult m_result;
        private AsyncCallback m_clientCallback;

        // The port we are working with
        private int m_port;
        
        // The IP Address we want to connect to.
        private string m_ipAddr;

        // The Reporting service.
        protected IReporterService m_reporter;
        #endregion

        #region Events
        public event DataReceivedHandler DataReceived;
        public event ConnectedHandler Connected;
        public event DisconnectedHandler Disconnected;
        #endregion

        #region Constructor
		public AsyncTcpClient(Game game)
            : base(game)
        {
            this.Connected += new ConnectedHandler(OnConnected);
            this.Disconnected += new DisconnectedHandler(OnDisconnected);

            this.DataReceived += new DataReceivedHandler(OnDataReceived);

            game.Exiting += new EventHandler(OnGameExiting);
        }

        ~AsyncTcpClient()
        {
            Dispose(true);
        }
        #endregion

        #region Virtual Event Handlers
        protected virtual void OnDisconnected()
        {
        }

        protected virtual void OnConnected(SocketPacket server)
        {
        }

        protected virtual void OnDataReceived(string data, SocketPacket packet)
        {
        }
        #endregion

        #region Connecting & Disconnecting
        public void Connect(string ip, int port)
        {
            // Set the IP Address and Port
            m_ipAddr = ip;
            m_port = port;

            // Attempt to connect!
            Connect();
        }

        public void Connect()
        {
            try
            {
                // Make sure the IP Address and Port are valid
                if (String.IsNullOrEmpty(m_ipAddr) || m_port == 0)
                    return;

                // Make sure the IP Address exists
                IPHostEntry entry = Dns.GetHostEntry(m_ipAddr);

                // If it doesn't, throw an exception.
                if (entry == null)
                    throw new HostNotFoundException(m_ipAddr);

                // Create the IP Endpoint object
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(m_ipAddr), m_port);

                // Create our connection object
                m_socket = new SocketPacket("Server");

                // Create the socket
                m_socket.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Attempt to connect
                m_socket.Socket.Connect(ipEnd);

                // If we connected...
                if (m_socket.Socket.Connected)
                {
                    // Wait for data to come in asynchronously
                    WaitForData();

                    // Invoke the Connected event.
                    if (Connected != null)
                        Connected.Invoke(m_socket);
                }

                // If we were not connected, throw an exception.
                SocketException socketException = new SocketException((int)SocketError.ConnectionAborted);

                throw socketException;
            }
            catch (HostNotFoundException hnf)
            {
                WriteError(hnf);
            }
            catch (SocketException se)
            {
                //string _str = "Connection failed, is the server running?\n" + se.Message;

                WriteError(se);
            }
            catch (Exception e)
            {
                WriteError(e);
            }
        }

        public void Disconnect()
        {
            // Close, Disconnect and Shutdown the connection.
            m_socket.Socket.Close();
            m_socket.Socket.Disconnect(false);
            m_socket.Socket.Shutdown(SocketShutdown.Both);

            // Clear the buffer.
            m_socket.ResetBuffer();

            // Invoke the Disconnected event.
            if (Disconnected != null)
                Disconnected.Invoke();
        }
        #endregion

        #region Callbacks and Socket Handling
        protected virtual void WaitForData()
        {
            try
            {
                // Create our callback.
                if (m_clientCallback == null)
                    m_clientCallback = new AsyncCallback(OnClientDataReceived);

                // Begin receiving data
                m_result = m_socket.Socket.BeginReceive(m_socket.DataBuffer, 0, m_socket.BufferLength, SocketFlags.None, m_clientCallback, m_socket);
            }
            catch (SocketException se)
            {
                WriteError(se);
            }
        }

        private void OnClientDataReceived(IAsyncResult ar)
        {
            try
            {
                // Get our connection
                SocketPacket _client = (SocketPacket)ar.AsyncState;

                // Get how many bytes are being received.
                int iRx = 0;
                iRx = _client.Socket.EndReceive(ar);

                // Create a new correctly sized buffer.
                byte[] _data = new byte[_client.BufferLength];
                _client.DataBuffer.CopyTo(_data, 0);

                // Get the textual conversion
                string _strData = Encoding.ASCII.GetString(_data, 0, iRx);

                // If we are ready to receive data
                if (_client.IsReady)
                {
                    // Invoke the event
                    if (DataReceived != null)
                        DataReceived.Invoke(_strData, _client);

                    // Reset the buffers
                    _client.BufferLength = _client.DefaultBufferLength;
                    _client.ResetBuffer();

                    // Reset the header
                    _client.LastHeader.Message = String.Empty;

                    // Reset the server
                    _client.IsReady = false;
                }
                else
                {
                    // Otherwise add to the current header
                    _client.LastHeader.Message += _strData;

                    // If the header has ended
                    if (_client.LastHeader.Message.Contains("</HEADER>"))
                    {
                        int _bytesNeeded = 256;
                        // Make sure we have the whole header
                        if (_client.LastHeader.Message.StartsWith("<HEADER>"))
                        {
                            int _first, _last;
                            _first = _client.LastHeader.Message.IndexOf("<BYTES>") + 7;
                            _last = _client.LastHeader.Message.IndexOf("</BYTES>");

                            // Get the number of bytes needed
                            string _bytes = _client.LastHeader.Message.Substring(_first, _last - _first);
                            if (int.TryParse(_bytes, out _bytesNeeded))
                            {
                                // We are ready!
                                _client.IsReady = true;

                                // Resize our buffer
                                _client.BufferLength = _bytesNeeded;

                                // Clear it too.
                                _client.ResetBuffer();

                                // Set the byte count of the header
                                _client.LastHeader.ByteCount = _bytesNeeded;
                            }
                        }
                    }
                }

                // Wait for data!
                WaitForData();
            }
            catch (ObjectDisposedException ode)
            {
                WriteError(ode, "OnClientDataReceived: Socket has been closed.");
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

        #region Sending
        /// <summary>
        /// Send a textual message to the server.
        /// </summary>
        /// <param name="message">The message you wish to send.</param>
        public void Send(string message)
        {
            // Get the bytes
            byte[] _bytes = Encoding.ASCII.GetBytes(message);

            // Send em!
            Send(_bytes);
        }

        /// <summary>
        /// Send a message to a specific person.
        /// </summary>
        /// <param name="clientID">The client's ID you want to send to.</param>
        /// <param name="message">The message to be sent.</param>
        public void Send(string clientID, string message)
        {
            byte[] _bytes = Encoding.ASCII.GetBytes(message);

            Send(clientID, _bytes);
        }

        /// <summary>
        /// Send raw data to the server.
        /// </summary>
        /// <param name="data">The data to be sent.</param>
        public void Send(byte[] data)
        {
            try
            {
                // Create the header and encode it.
                string _header = String.Format("<HEADER><BYTES>{0}</BYTES></HEADER>", data.Length);
                byte[] _headerBytes = Encoding.ASCII.GetBytes(_header);

                // Set the sending buffer to the data
                m_socket.SendBuffer = data;

                // Send the bytes.
                m_socket.Socket.BeginSend(_headerBytes, 0, _headerBytes.Length, SocketFlags.None, new AsyncCallback(OnSendHeader), m_socket);
            }
            catch (SocketException se)
            {
                WriteError(se);
            }
        }

        /// <summary>
        /// Send raw data to a specific client.
        /// </summary>
        /// <param name="clientID">The client's ID you want to send to.</param>
        /// <param name="data">The data to be sent.</param>
        public void Send(string clientID, byte[] data)
        {
            try
            {
                // Get the header and encode it.
                string _header = String.Format("<HEADER><BYTES>{0}</BYTES><TO>{1}</TO></HEADER>", data.Length, clientID);
                byte[] _headerBytes = Encoding.ASCII.GetBytes(_header);

                // Set the Send buffer
                m_socket.SendBuffer = data;

                // Begin sending the header
                m_socket.Socket.BeginSend(_headerBytes, 0, _headerBytes.Length, SocketFlags.None, new AsyncCallback(OnSendHeader), m_socket);
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
                // Get the connection
                SocketPacket _client = (SocketPacket)ar.AsyncState;

                // Complete the send.
                int _sent = _client.Socket.EndSend(ar);

                // Send the actual data.
                _client.Socket.Send(_client.SendBuffer);

                // Reset the buffer.
                _client.SendBuffer = new byte[1];
            }
            catch(SocketException se)
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

        #region Error Writing
        /// <summary>
        /// Report a SocketException
        /// </summary>
        /// <param name="exception">The SocketException to report.</param>
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

        /// <summary>
        /// Report a HostNotFoundException
        /// </summary>
        /// <param name="exception">The HostNotFoundException to report.</param>
        protected virtual void WriteError(HostNotFoundException exception)
        {
            if (m_reporter == null)
                return;

            Message msg = new Message(this);
            msg.Destination = ((IService)m_reporter).ID;
            msg.Source = "AsyncServer";
            msg.Msg = exception.Message;

            m_reporter.BroadcastError(msg, exception);
        }

        /// <summary>
        /// Report an Exception.
        /// </summary>
        /// <param name="exception">The Exception to report.</param>
        protected virtual void WriteError(Exception exception)
        {
            if (m_reporter == null)
                return;

            Message msg = new Message(this);
            msg.Destination = ((IService)m_reporter).ID;
            msg.Source = "AsyncServer";
            msg.Msg = exception.Message;

            m_reporter.BroadcastError(msg, exception);
        }

        /// <summary>
        /// Report an ObjectDisposedException.
        /// </summary>
        /// <param name="exception">The ObjectDisposedException to report.</param>
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

        /// <summary>
        /// Report an ObjectDisposedException with a message.
        /// </summary>
        /// <param name="exception">The ObjectDisposedException to report.</param>
        /// <param name="message">The message to send.</param>
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
        #endregion

        #region Disposing & Exiting
        protected virtual void OnGameExiting(object sender, EventArgs e)
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Disconnect();
        }
        #endregion
    }
}


#endif
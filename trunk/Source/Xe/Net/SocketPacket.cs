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

#if ! XBOX360
#region Using Statements
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
#endregion

namespace XeFramework.Net
{
    public class SocketPacket
    {
        #region Members
        protected Socket m_socket;
        protected byte[] m_buffer = new byte[256];
        protected byte[] m_sendBuffer = new byte[1];
        protected int m_defaultLength = 256;
        protected string m_message = String.Empty;
        protected string m_id = String.Empty;

        protected SocketHeader m_lastHeader = new SocketHeader();

        protected bool m_isReady = false;
        #endregion

        #region Constructors
        public SocketPacket(string id)
        {
            m_id = id;
        }

        public SocketPacket()
        {
        }
        #endregion

        #region Handling the Buffer
        public void ResetBuffer()
        {
            for (int i = 0; i < m_buffer.Length; i++)
                m_buffer[i] = (byte)'\0';
        }

        public int BufferLength
        {
            get
            {
                return m_buffer.Length;
            }
            set
            {
                if (m_buffer.Length == value)
                    return;

                byte[] _temp = new byte[value];

                if (value > m_buffer.Length)
                    for (int i = 0; i < m_buffer.Length; i++)
                        _temp[i] = m_buffer[i];
                else
                    for (int i = 0; i < value; i++)
                        _temp[i] = m_buffer[i];

                m_buffer = _temp;
            }
        }
        #endregion

        #region Properties
        public string ID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        public string Message
        {
            get
            {
                return m_message;
            }
            set
            {
                m_message = value;
            }
        }

        public byte[] DataBuffer
        {
            get
            {
                return m_buffer;
            }
            set
            {
                m_buffer = value;
            }
        }

        public Socket Socket
        {
            get
            {
                return m_socket;
            }
            set
            {
                m_socket = value;
            }
        }

        public bool IsReady
        {
            get
            {
                return m_isReady;
            }
            set
            {
                m_isReady = value;
            }
        }

        public int DefaultBufferLength
        {
            get
            {
                return m_defaultLength;
            }
            set
            {
                m_defaultLength = value;
            }
        }

        public SocketHeader LastHeader
        {
            get
            {
                return m_lastHeader;
            }
            set
            {
                m_lastHeader = value;
            }
        }

        public byte[] SendBuffer
        {
            get
            {
                return m_sendBuffer;
            }
            set
            {
                m_sendBuffer = value;
            }
        }
        #endregion
    }

    public class SocketHeader
    {
        #region Members
        protected string m_tag = String.Empty;
        protected string m_headerMsg = String.Empty;
        protected int m_byteCount = 256;
        #endregion

        #region Constructors
        public SocketHeader()
        {
        }

        public SocketHeader(string tag)
        {
            m_tag = tag;
        }

        public SocketHeader(string tag, int byteCount)
        {
            m_tag = tag;

            m_byteCount = byteCount;
        }
        #endregion

        #region Properties
        public int ByteCount
        {
            get
            {
                return m_byteCount;
            }
            set
            {
                m_byteCount = value;
            }
        }

        public string Tag
        {
            get
            {
                return m_tag;
            }
            set
            {
                m_tag = value;
            }
        }

        public string Message
        {
            get
            {
                return m_headerMsg;
            }
            set
            {
                m_headerMsg = value;
            }
        }
        #endregion
    }
}

#endif
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
#endregion

namespace Xe.Net
{
    #region Server Only
    public delegate void ClientConnectedHandler(SocketPacket packet);
    public delegate void ClientDisconnectedHandler(SocketPacket packet);
    public delegate void ClientAuthRequestHandler(SocketPacket packet);

    public delegate void ServerStartedHandler();
    public delegate void ServerStoppedHandler();
    #endregion

    public delegate void DataReceivedHandler(string data, SocketPacket packet);

    #region Client Only
    public delegate void ConnectedHandler(SocketPacket server);
    public delegate void DisconnectedHandler();
    #endregion
}
#endif
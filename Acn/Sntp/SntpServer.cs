#region Copyright © 2012 Mark Daniel
//______________________________________________________________________________________________________________
// Simple Network Time Protocol Client
//
// Code taken largely from Code Project
// http://www.codeproject.com/Articles/1005/SNTP-Client-in-C
// SNTP Client in C# by Valer Bocan
// Used with persmission of CPOL.
// See original copyright notice below.
//
// In this derivative version
// Copyright © 2012 Mark Daniel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//______________________________________________________________________________________________________________
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using LXProtocols.Acn.Sntp.Sockets;

namespace LXProtocols.Acn.Sntp
{
    /// <summary>
    /// The SntpServer provides simple response to NTP time requests
    /// </summary>
    public class SntpServer : IDisposable
    {
        protected SntpSocket socket = null;

        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="SntpServer"/> class.
        /// </summary>
        public SntpServer()
        {
            Port = SntpSocket.DefaultPort;
        }

        #endregion

        #region Information


        private IPAddress networkAdapter = IPAddress.Any;

        /// <summary>
        /// Gets or sets the address of the network adapter to listen on.
        /// </summary>
        /// <value>
        /// The network adapter.
        /// </value>
        public IPAddress NetworkAdapter
        {
            get { return networkAdapter; }
            set { networkAdapter = value; }
        }

        private int port;

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port
        {
            get 
            { 
                return port;
            }
            set
            {
                port = value;
                if (socket != null)
                {
                    socket.Port = port;
                }
            }
        }


        /// <summary>
        /// Gets a value indicating whether this <see cref="SntpServer"/> is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        public bool Active
        {
            get { return socket != null; }
        }

        #endregion

        #region Agent

        /// <summary>
        /// Opens a new connection
        /// </summary>
        public virtual void Open()
        {
            try
            {
                if (socket == null)
                {
                    socket = new SntpSocket() { Port = Port };
                    socket.NewPacket += new EventHandler<NewPacketEventArgs>(socket_NewPacket);

                    socket.Open(NetworkAdapter);
                }
            }
            catch (Exception e)
            {
                Close();
                throw e;
            }
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
        public virtual void Close()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
        }

        /// <summary>
        /// Determines whether this instance is open.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is open; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsOpen()
        {
            return (socket != null);
        }

        /// <summary>
        /// Handles the NewPacket event of the socket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Acn.Sntp.Sockets.NewPacketEventArgs"/> instance containing the event data.</param>
        void socket_NewPacket(object sender, NewPacketEventArgs e)
        {
            ProcessPacket(e);
        }



        #endregion


        #region Messaging


        /// <summary>
        /// Processes the packet and returns a response.
        /// </summary>
        /// <param name="packetInfo">The <see cref="Acn.Sntp.Sockets.NewPacketEventArgs"/> instance containing the event data.</param>
        protected void ProcessPacket(NewPacketEventArgs packetInfo)
        {
            NtpData data = packetInfo.Packet;
            data.LeapIndicator = LeapIndicator.NoWarning;
            data.VersionNumber = 4;
            data.Mode = NtpMode.Server;
            data.Stratum = NtpStratum.PrimaryReference;
            data.RootDispersion = 0;
            data.RootDelay = 0;
            data.ReferenceID = "LOCL";
            data.ReceiveTimestamp = packetInfo.RecievedTime;
            data.CopyTransmitToOriginate();
            data.TransmitTimestamp = DateTime.UtcNow;
            socket.Send(packetInfo.SourceEndPoint, data);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Close();
        }

        #endregion
    }
}

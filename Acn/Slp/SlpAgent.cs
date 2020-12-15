#region Copyright © 2011 Oliver Waits
//______________________________________________________________________________________________________________
// Service Location Protocol
// Copyright © 2011 Oliver Waits
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
using System.Text;
using LXProtocols.Acn.Slp.Packets;
using System.Net;
using LXProtocols.Acn.Slp.Sockets;

namespace LXProtocols.Acn.Slp
{
    public abstract class SlpAgent : IDisposable
    {
        protected SlpSocket socket = null;
        protected SlpSocket multicastListenSocket = null;

        #region Setup and Initialisation

        public SlpAgent()
        {
            Language = "en";
        }

        #endregion

        #region Information

        private string scope = "DEFAULT";

        public string Scope
        {
            get { return scope; }
            set { scope = value; }
        }

        public string Language { get; set; }

        private int transactionId = 1;

        public int TransactionId
        {
            get { return transactionId; }
        }

        public short NewTransactionId()
        {
            transactionId++;

            if (transactionId > short.MaxValue)
                transactionId = 1;

            return (short) TransactionId;
        }

        private IPAddress networkAdapter = IPAddress.Any;

        public IPAddress NetworkAdapter
        {
            get { return networkAdapter; }
            set { networkAdapter = value; }
        }


        public bool Active
        {
            get { return socket != null; }
        }

        #endregion

        #region Agent

        /// <summary>
        /// Opens a unicast socket bound to any local port on <see cref="NetworkAdapter"/> for
        /// sending and receiving SLP datagrams, and optionally also opens a socket listening to
        /// the SLP well-known port (<see cref="SlpSocket.Port"/>) to receive multicast SLP
        /// datagrams.
        /// </summary>
        /// <param name="openWellKnownPort">Whether the socket listening on the well-known port
        /// should be opened or not.</param>
        public virtual void Open(bool openWellKnownPort)
        {
            //Open the socket on any available port, this will be used for all unicast communication. 
            //Using an assigned port rather than 427 ensures we are discoverable on a system running multiple SLP clients.
            if (socket == null)
            {
                socket = new SlpSocket();
                socket.NewPacket += new EventHandler<NewPacketEventArgs>(socket_NewPacket);

                socket.Open(new IPEndPoint(NetworkAdapter,0),false);
            }

            //Open a socket to listen to multicast traffic on port 427, this socket is only used to listen for multicast traffic.
            //As unicast traffic is not able to share port 427 we only use this socket to recieve multicast.
            if (openWellKnownPort && multicastListenSocket is null)
            {
                multicastListenSocket = new SlpSocket();
                multicastListenSocket.NewPacket += new EventHandler<NewPacketEventArgs>(socket_NewPacket);
                multicastListenSocket.Open(NetworkAdapter, true);
            }
        }

        public virtual void Close()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }

            if (multicastListenSocket != null)
            {
                multicastListenSocket.Close();
                multicastListenSocket = null;
            }
        }

        public virtual bool IsOpen()
        {
            return (socket != null);
        }

        void socket_NewPacket(object sender, NewPacketEventArgs e)
        {
            ProcessPacket(e);
        }

        protected abstract void ProcessPacket(NewPacketEventArgs packetInfo);

        #endregion


        #region Messaging

        protected void SendDARequest()
        {
            ServiceRequestPacket request = new ServiceRequestPacket();
            FillHeader(request.Header, NewTransactionId());
            request.ServiceType = "service:directory-agent";
            request.Scope = Scope;

            socket.Send(request);
        }

        protected SlpHeaderPacket FillHeader(SlpHeaderPacket header, short transactionId)
        {
            header.Version = 2;
            header.XId = transactionId;
            header.LanguageTag = Language;

            return header;
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }
}

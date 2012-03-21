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
using Acn.Slp.Packets;
using System.Net;
using Acn.Slp.Sockets;

namespace Acn.Slp
{
    public abstract class SlpAgent : IDisposable
    {
        protected SlpSocket socket = null;

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

        public virtual void Open()
        {
            if (socket == null)
            {
                socket = new SlpSocket();
                socket.NewPacket += new EventHandler<NewPacketEventArgs>(socket_NewPacket);

                if(this is SlpUserAgent)
                    socket.Open(new IPEndPoint(NetworkAdapter,0));
                else
                    socket.Open(NetworkAdapter);
            }
        }

        public virtual void Close()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
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
            request.ScopeList = Scope;

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

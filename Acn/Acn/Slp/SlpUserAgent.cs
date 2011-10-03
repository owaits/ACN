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
using System.Threading;
using Acn.Slp.Packets;
using Acn.Slp.Sockets;
using System.Net;

namespace Acn.Slp
{      
    public class SlpUserAgent:SlpAgent
    {
        #region Setup and Initialisation

        public SlpUserAgent ()
        {
        }

        public SlpUserAgent(string scope)
        {
            Scope = scope;
        }

        #endregion       

        #region Contents

        private DirectoryAgentInformation directoryAgent = null;

        public DirectoryAgentInformation DirectoryAgent 
        { 
            get { return directoryAgent; }
            protected set { directoryAgent = value; }
        }

        #endregion

        #region User Agent

        public override void Open()
        {
            base.Open();

            //Discover any DA's we should be talking to.
            SendDARequest();
        }

        public void Find(string serviceType)
        {
            if (socket == null)
                throw new InvalidOperationException("User agent not open. Please open the user agent first before calling Find.");

            SendRequest(serviceType, Scope);
        }

        #endregion        

        #region Messaging

        private void SendRequest(string serviceType, string scope)
        {
            ServiceRequestPacket request = new ServiceRequestPacket();
            FillHeader(request.Header, NewTransactionId());
            request.ServiceType = serviceType;
            request.ScopeList = scope;

            if (DirectoryAgent == null)
                //Multicast the service request.
                socket.Send(request);
            else
                //Request the services directly from the DA.
                socket.Send(DirectoryAgent.EndPoint, request);
        }

        protected override void ProcessPacket(NewPacketEventArgs packetInfo)
        {
            DirectoryAgentAdvertPacket daAdvert = packetInfo.Packet as DirectoryAgentAdvertPacket;
            if (daAdvert != null)
                DirectoryAgent = new DirectoryAgentInformation(daAdvert.Url, packetInfo.SourceEndPoint);


            ServiceReplyPacket serviceReply = packetInfo.Packet as ServiceReplyPacket;
            if (serviceReply != null)
                ProcessServiceReply(serviceReply, packetInfo.SourceEndPoint);

        }

	    #endregion

        #region Events

        public event EventHandler<ServiceFoundEventArgs> ServiceFound;

        protected void ProcessServiceReply(ServiceReplyPacket serviceReply,IPEndPoint ipAddress)
        {
            if (serviceReply.ErrorCode == SlpErrorCode.None)
            {
                if (ServiceFound != null)
                    ServiceFound(this, new ServiceFoundEventArgs(serviceReply.Urls, ipAddress));
            }
        }

        #endregion

        #region IDispose

        public override void Dispose()
        {
            Close();
        }

        #endregion
    }
}

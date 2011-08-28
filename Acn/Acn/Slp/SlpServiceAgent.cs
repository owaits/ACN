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
   
using System.Text;
using Acn.Slp.Sockets;
using Acn.Slp.Packets;
using System.Net;
using System.Collections.Generic;
using System;

namespace Acn.Slp
{
    public class SlpServiceAgent:SlpAgent
    {
        #region Setup and Initialisation
        
        public SlpServiceAgent()
        {
        }
        
        #endregion

        #region Contents

        public string ServiceUrl { get; set; }

        public string ServiceType { get; set; }

        public Dictionary<string,string> Attributes { get; private set; }

        public string AttributeString 
        {
            get 
            {
                StringBuilder sb = new StringBuilder();
                foreach (string aname in Attributes.Keys )
                {
                    if ( sb.Length > 0 ) 
                        sb.Append(",");
                    sb.AppendFormat("({0}={1})", aname, Attributes[aname] );
                }
                return sb.ToString();
            }
        }

        private Dictionary<string, DirectoryAgentInformation> directoryAgents = new Dictionary<string, DirectoryAgentInformation>();

        public Dictionary<string, DirectoryAgentInformation> DirectoryAgents
        {
            get { return directoryAgents; }
        }

        #endregion

        #region Service Agent
        
        public void Open() 
        {
            if (Active)
                throw new InvalidOperationException("The service agent is already active. Either close this agent or do not call Open while active.");

            if (string.IsNullOrEmpty(Scope))
                throw new InvalidOperationException("An attempt was made to open the service without setting the ServiceScope first.");

            if (string.IsNullOrEmpty(ServiceType))
                throw new InvalidOperationException("An attempt was made to open the service without setting the ServiceType first.");

            if (string.IsNullOrEmpty(ServiceUrl))
                throw new InvalidOperationException("An attempt was made to open the service without setting the ServiceUrl first.");

            base.Open();

            SendDARequest();
        }

        #endregion

        #region Messaging

        protected override void ProcessPacket(NewPacketEventArgs packetInfo)
        {
            DirectoryAgentAdvertPacket advert = packetInfo.Packet as DirectoryAgentAdvertPacket;
            if (advert != null)
                ProcessDAAdvert(advert, packetInfo.SourceAddress);

            ServiceRequestPacket request = packetInfo.Packet as ServiceRequestPacket;
            if (IsReplyRequired(request))
                SendServiceReply(packetInfo.SourceAddress, packetInfo.Packet.Header.XId);

            ServiceAcknowledgePacket acknowledge = packetInfo.Packet as ServiceAcknowledgePacket;
            if (acknowledge != null)
                RaiseServiceRegistered();
        }

        private void ProcessDAAdvert(DirectoryAgentAdvertPacket da, IPAddress source)
        {
            if(!DirectoryAgents.ContainsKey(da.Url))
            {
                //Add this DA to the list of DAs we subscribe to.
                DirectoryAgents[da.Url] = new DirectoryAgentInformation(da.Url, source); ;

                //Notify Subscribers that a new DA has been found.
                RaiseNewDirectoryAgentFound();
            }

            //Register with the DA.
            ServiceRegistrationPacket register = new ServiceRegistrationPacket();
            FillHeader(register.Header, da.Header.XId);
            register.Header.Flags |= SlpHeaderFlags.Fresh;
            register.Url = new UrlEntry(ServiceUrl);
            register.ServiceType = ServiceType;
            register.ScopeList = Scope;
  
            socket.Send(source, register);            
        }

        private void RenewDA()
        {

        }

        private bool IsReplyRequired(ServiceRequestPacket request)
        {
            //Check this request is valid.
            if (request == null)
                return false;
            
            //Check that we are not subscribed to a DA
            if (DirectoryAgents.Count > 0)
                return false;

            //Does the scope match the scope of this service.
            if (request.ScopeList != Scope)
                return false;

            //Does the requested service match this service?
            if (request.ServiceType != ServiceType)
                return false;

            //Check that we are not in the exemption list.
            if (request.PRList.Contains(ServiceUrl))
                return false;

            return true;
        }

        private void SendServiceReply(IPAddress target, short transactionId)
        {
            ServiceReplyPacket reply = new ServiceReplyPacket();
            FillHeader(reply.Header, transactionId);
            reply.Urls.Add(new UrlEntry(ServiceUrl));
            socket.Send(target, reply);
        }

        #endregion

        #region Events

        public event EventHandler NewDirectoryAgentFound;

        protected void RaiseNewDirectoryAgentFound()
        {
            if (NewDirectoryAgentFound != null)
                NewDirectoryAgentFound(this, EventArgs.Empty);
        }

        public event EventHandler ServiceRegistered;

        protected void RaiseServiceRegistered()
        {
            if (ServiceRegistered != null)
                ServiceRegistered(this, EventArgs.Empty);
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

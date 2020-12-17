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
using LXProtocols.Acn.Slp.Sockets;
using LXProtocols.Acn.Slp.Packets;
using System.Net;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace LXProtocols.Acn.Slp
{
    public class SlpServiceAgent : SlpAgent
    {
        #region Setup and Initialisation

        public SlpServiceAgent()
        {
            Attributes = new Dictionary<string, string>();
            UseDirectoryAgents = true;
        }

        #endregion

        #region Contents

        /// <summary>
        /// Regex to match a service Url
        /// </summary>
        /// <remarks>
        /// The matching groups are as follows
        ///   0 - Everything
        ///   1 - Abstract service spec including service:abstract:
        ///   2 - Abstract service type including naming authority if supplied
        ///   3 - Full Url including concrete service type
        ///   4 - No use
        ///   5 - Concrete service type
        ///   6 - No use
        ///   7 - Address part of the url
        /// </remarks>
        private static Regex urlMatch = new Regex(@"(service:([^:\s]+):?)?((([^:\s/]+):)?(//(\S*))?)?", RegexOptions.Compiled);

        /// <summary>
        /// Gets the type of the service.
        /// This is the full service type id including the 'service' prefix
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public string ServiceType
        {
            get
            {
                if (string.IsNullOrEmpty(ServiceConcreteType))
                {
                    return string.Format("service:{0}", ServiceAbstractType);
                }
                if (string.IsNullOrEmpty(ServiceAbstractType))
                {
                    return string.Format("service:{0}", ServiceConcreteType);
                }
                return string.Format("service:{0}:{1}", ServiceAbstractType, ServiceConcreteType);
            }
        }

        /// <summary>
        /// Gets or sets the abstract type of the service.
        /// This is a generic service type such as printer, for non standard types
        /// it may include a naming authority eg e133.esta
        /// A service may have an abstract or concrete type or both
        /// </summary>
        /// <value>
        /// The type of the service abstract.
        /// </value>
        public string ServiceAbstractType { get; set; }

        /// <summary>
        /// Gets or sets the concrete type of the service.
        /// This is a specific protocol eg http or ftp
        /// A service may have an abstract or concrete type or both
        /// </summary>
        /// <value>
        /// The concrete type of the service.
        /// </value>
        public string ServiceConcreteType { get; set; }

        /// <summary>
        /// Gets or sets the address of the service.
        /// This is the hostname or ip and the path the service eg
        /// not.wco.ftp.com/cgi-bin/pub-prn
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string ServiceAddress { get; set; }

        /// <summary>
        /// Gets or sets whether we can use a directory agent if available.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use directory agents]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDirectoryAgents { get; set; }

        /// <summary>
        /// Gets or sets the full service URL including all the parts.
        /// A URL must include an address and a concrete or an abstract type.
        /// </summary>
        /// <remarks>
        /// Some examples of legal URLs
        /// 
        /// http://www.ietf.org/rfc/
        /// service:web:http://www.ietf.org/rfc/
        /// service:e133.esta://2.4.9.6/0xaabb11223344
        /// 
        /// </remarks>
        /// <value>
        /// The service URL.
        /// </value>
        public string ServiceUrl
        {
            get
            {
                return string.Format("{0}://{1}", ServiceType, ServiceAddress);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Can't set an empty service url");
                }

                var matches = urlMatch.Match(value);
                if (matches.Success && matches.Captures.Count > 0)
                {
                    if (string.IsNullOrEmpty(matches.Groups[7].Value))
                    {
                        throw new ArgumentNullException("Attempt to set a Url with an empty address");
                    }
                    if (string.IsNullOrEmpty(matches.Groups[2].Value) && string.IsNullOrEmpty(matches.Groups[5].Value))
                    {
                        throw new ArgumentNullException("Attempt to set a Url with neither a concrete nor an abstract type");
                    }
                    ServiceAbstractType = matches.Groups[2].Value;
                    ServiceConcreteType = matches.Groups[5].Value;
                    ServiceAddress = matches.Groups[7].Value;
                }
                else
                {
                    throw new ArgumentNullException("Attempt to set an invlaid Url");
                }
            }
        }

        public Dictionary<string, string> Attributes { get; private set; }

        public string AttributeString
        {
            get
            {
                return JoinAttributeString(Attributes);
            }
        }

        /// <summary>
        /// Joins an attribute dictionary to a string of (a=b) tupples
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public static string JoinAttributeString(IDictionary<string, string> attributes)
        {
            if (attributes == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (string aname in attributes.Keys)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.AppendFormat("({0}={1})", aname, attributes[aname]);
            }
            return sb.ToString();
        }

        private Dictionary<string, DirectoryAgentInformation> directoryAgents = new Dictionary<string, DirectoryAgentInformation>();

        public Dictionary<string, DirectoryAgentInformation> DirectoryAgents
        {
            get { return directoryAgents; }
        }

        #endregion

        #region Service Agent

        /// <summary>
        /// Opens a unicast socket bound to any local port on <see cref="SlpSocket"/> for
        /// sending and receiving SLP datagrams, and optionally also opens a socket listening to
        /// the SLP well-known port (<see cref="SlpSocket.Port"/>) to receive multicast SLP
        /// datagrams.
        ///
        /// Additionally, sends a service request for available discovery agents.
        /// </summary>
        /// <param name="openWellKnownPort">Whether the socket listening on the well-known port
        /// should be opened or not.</param>
        public override void Open(bool openWellKnownPort)
        {
            if (Active)
                throw new InvalidOperationException("The service agent is already active. Either close this agent or do not call Open while active.");

            if (string.IsNullOrEmpty(Scope))
                throw new InvalidOperationException("An attempt was made to open the service without setting the ServiceScope first.");

            if (string.IsNullOrEmpty(ServiceAbstractType) && string.IsNullOrEmpty(ServiceConcreteType))
                throw new InvalidOperationException("An attempt was made to open the service without setting the ServiceType first.");

            if (string.IsNullOrEmpty(ServiceUrl))
                throw new InvalidOperationException("An attempt was made to open the service without setting the ServiceUrl first.");

            base.Open(openWellKnownPort);

            SendDARequest();
        }

        #endregion

        #region Messaging

        protected override void ProcessPacket(NewPacketEventArgs packetInfo)
        {
            DirectoryAgentAdvertPacket advert = packetInfo.Packet as DirectoryAgentAdvertPacket;
            if (advert != null)
                ProcessDAAdvert(advert, packetInfo.SourceEndPoint);

            ServiceRequestPacket request = packetInfo.Packet as ServiceRequestPacket;
            if (IsReplyRequired(request))
                SendServiceReply(packetInfo.SourceEndPoint, packetInfo.Packet.Header.XId);

            ServiceAcknowledgePacket acknowledge = packetInfo.Packet as ServiceAcknowledgePacket;
            if (acknowledge != null)
                RaiseServiceRegistered();

            AttributeRequestPacket attributeRequest = packetInfo.Packet as AttributeRequestPacket;
            if (IsReplyRequired(attributeRequest))
                SendAttributeReply(packetInfo.SourceEndPoint, packetInfo.Packet.Header.XId);
        }

        private void ProcessDAAdvert(DirectoryAgentAdvertPacket da, IPEndPoint source)
        {
            //We should only register with directory agents that either support our scope or those which support all scopes (the scope list is empty).
            if (da.ScopeList.Count != 0 && !da.ScopeList.Contains(Scope))
                return;

            //Determine if we have previously registered with this DA and if not raise a new DA found event.
            if (!DirectoryAgents.ContainsKey(da.Url))
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
            register.AttrList = AttributeString;

            socket.Send(source, register);
        }

        private void RenewDA()
        {

        }

        /// <summary>
        /// Determines whether a reply is required for the specified request.
        /// We should only respond to requests within the scope of the service
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>true</c> if reply is required; otherwise, <c>false</c>.
        /// </returns>
        public bool IsReplyRequired(ServiceRequestPacket request)
        {
            if (!IsReplyRequired((SlpRequestPacket)request))
            {
                return false;
            }

            //Does the requested service match this service?
            if (request.ServiceType != ServiceType &&
                ((string.IsNullOrEmpty(ServiceAbstractType) || request.ServiceType != string.Format("service:{0}", ServiceAbstractType))) &&
                ((string.IsNullOrEmpty(ServiceConcreteType) || request.ServiceType != string.Format("service:{0}", ServiceConcreteType))))
                return false;

            return true;
        }

        /// <summary>
        /// Determines whether a reply is required for the specified request.
        /// We should only respond to requests within the scope of the 
        /// service
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>true</c> if reply is required; otherwise, <c>false</c>.
        /// </returns>
        private bool IsReplyRequired(AttributeRequestPacket request)
        {
            if (!IsReplyRequired((SlpRequestPacket)request))
            {
                return false;
            }

            //Does the requested url match this service?
            //   The URL field can take two forms.  
            //   It can simply be a Service Type such as "http" or "service:tftp". 
            //   In this case, all attributes are returned if the type matches
            //   Or it can be a full URl in which case we only reply if the full URL matches

            return CheckUrlMatch(request.Url, ServiceAbstractType, ServiceConcreteType, ServiceAddress);
        }

        /// <summary>
        /// Checks for a URL match.
        /// </summary>
        /// <param name="requestUrl">The request URL (eg service:printer:lpr://10.0.0.5/queue).</param>
        /// <param name="serviceAbstractType">The abstract service type (eg printer).</param>
        /// <param name="serviceConcreteType">Type of the service concrete (eg lpr).</param>
        /// <param name="serviceAddress">The service URL (eg 10.0.0.5/hp1010).</param>
        /// <returns>True if the request matches the abstract type, the concrete type or the full url</returns>
        public static bool CheckUrlMatch(string requestUrl, string serviceAbstractType, string serviceConcreteType, string serviceAddress)
        {
            if (string.IsNullOrEmpty(requestUrl))
            {
                return false;
            }

            var matches = urlMatch.Match(requestUrl);
            if (matches.Success && matches.Captures.Count > 0)
            {
                string urlAbstractServiceType = matches.Groups[2].Value;
                string urlConcreteServiceType = matches.Groups[5].Value;
                string address = matches.Groups[7].Value;

                if (!string.IsNullOrEmpty(urlAbstractServiceType) && urlAbstractServiceType != serviceAbstractType)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(urlConcreteServiceType) && urlConcreteServiceType != serviceConcreteType)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(address) && address != serviceAddress)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


            return true;
        }

        /// <summary>
        /// Determines whether a reply is required for the specified request.
        /// We should only respond to requests within the scope of the service
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///   <c>true</c> if reply is required; otherwise, <c>false</c>.
        /// </returns>
        private bool IsReplyRequired(SlpRequestPacket request)
        {
            //Check this request is valid.
            if (request == null)
                return false;

            //Check that we are not subscribed to a DA
            if (UseDirectoryAgents && DirectoryAgents.Count > 0)
                return false;

            //Does the scope match the scope of this service.
            if (request.Scope != Scope)
                return false;

            //Check that we are not in the exemption list.
            if (request.PRList.Contains(ServiceUrl))
                return false;

            return true;
        }

        private void SendServiceReply(IPEndPoint target, short transactionId)
        {
            ServiceReplyPacket reply = new ServiceReplyPacket();
            FillHeader(reply.Header, transactionId);
            reply.Urls.Add(new UrlEntry(ServiceUrl));
            socket.Send(target, reply);
        }

        private void SendAttributeReply(IPEndPoint target, short transactionId)
        {
            AttributeReplyPacket reply = new AttributeReplyPacket();
            FillHeader(reply.Header, transactionId);
            reply.AttrList = AttributeString;
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

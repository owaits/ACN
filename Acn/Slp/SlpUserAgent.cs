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
using System.Text.RegularExpressions;

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

        public int Find(string serviceType)
        {
            if (socket == null)
                throw new InvalidOperationException("User agent not open. Please open the user agent first before calling Find.");

           return SendRequest(serviceType, Scope);
        }

        /// <summary>
        /// Sends an attribute request.
        /// This can either be to a specific URL or a general service type.
        /// Results will be returned via the AttributeReply event
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// An id for this request which can be used to match it to the reply
        /// </returns>
        public int RequestAttributes(string url)
        {
            if (socket == null)
                throw new InvalidOperationException("User agent not open. Please open the user agent first before calling.");

            return SendAttributeRequest(Scope, url);
        }

        /// <summary>
        /// Sends an attribute request.
        /// This can either be to a specific URL or a general service type.
        /// Results will be returned via the AttributeReply event
        /// This version targets a specific IP which can cut down the noise on replies.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// An id for this request which can be used to match it to the reply
        /// </returns>
        public int RequestAttributes(IPEndPoint target, string url)
        {
            if (socket == null)
                throw new InvalidOperationException("User agent not open. Please open the user agent first before calling.");

            return SendAttributeRequest(target, Scope, url);
        }

        #endregion        

        #region Messaging

        /// <summary>
        /// Sends a service request.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>The Id of the request</returns>
        private int SendRequest(string serviceType, string scope)
        {
            ServiceRequestPacket request = new ServiceRequestPacket();
            FillHeader(request.Header, NewTransactionId());
            request.ServiceType = serviceType;
            request.Scope = scope;

            if (DirectoryAgent == null)
                //Multicast the service request.
                socket.Send(request);
            else
                //Request the services directly from the DA.
                socket.Send(DirectoryAgent.EndPoint, request);

            return request.Header.XId;
        }



        /// <summary>
        /// Sends an attribute request.
        /// This can either be to a specific URL or a general service URL.
        /// Results will be returned via the AttributeReply event
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// An id for this request which can be used to mtch it to the reply
        /// </returns>
        private int SendAttributeRequest(string scope, string url)
        {
            AttributeRequestPacket request = PrepareAttributeRequest(scope, url);
            socket.Send(request);
            return request.Header.XId;
        }

        /// <summary>
        /// Sends an attribute request.
        /// This can either be to a specific URL or a general service URL.
        /// Results will be returned via the AttributeReply event
        /// </summary>
        /// <param name="target">The target endpoint.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="url">The URL.</param>
        private int SendAttributeRequest(IPEndPoint target, string scope, string url)
        {
            AttributeRequestPacket request = PrepareAttributeRequest(scope, url);

            socket.Send(target, request);

            return request.Header.XId;
        }

        /// <summary>
        /// Prepares the attribute request object.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private AttributeRequestPacket PrepareAttributeRequest(string scope, string url)
        {
            AttributeRequestPacket request = new AttributeRequestPacket();
            FillHeader(request.Header, NewTransactionId());
            request.Scope = scope;
            request.Url = url;
            return request;
        }

        protected override void ProcessPacket(NewPacketEventArgs packetInfo)
        {
            DirectoryAgentAdvertPacket daAdvert = packetInfo.Packet as DirectoryAgentAdvertPacket;
            if (daAdvert != null)
                DirectoryAgent = new DirectoryAgentInformation(daAdvert.Url, packetInfo.SourceEndPoint);


            ServiceReplyPacket serviceReply = packetInfo.Packet as ServiceReplyPacket;
            if (serviceReply != null)
                ProcessServiceReply(serviceReply, packetInfo.SourceEndPoint);

            AttributeReplyPacket attributeReply = packetInfo.Packet as AttributeReplyPacket;
            if (attributeReply != null)
            {
                ProcessAttributeReply(attributeReply, packetInfo.SourceEndPoint);
            }

        }

	    #endregion

        #region Events

        public event EventHandler<ServiceFoundEventArgs> ServiceFound;

        protected void ProcessServiceReply(ServiceReplyPacket serviceReply,IPEndPoint ipAddress)
        {
            if (serviceReply.ErrorCode == SlpErrorCode.None && serviceReply.Urls.Count > 0)
            {
                if (ServiceFound != null)
                    ServiceFound(this, new ServiceFoundEventArgs(serviceReply.Urls, ipAddress) { RequestId = serviceReply.Header.XId });
            }
        }

        /// <summary>
        /// Occurs when a reply to an attribute request is recieved.
        /// </summary>
        public event EventHandler<AttributeReplyEventArgs> AttributeReply;

        /// <summary>
        /// Processes the attribute reply.
        /// </summary>
        /// <param name="attributeReply">The attribute reply packet.</param>
        /// <param name="ipAddress">The ip address the reply originated from.</param>
        protected void ProcessAttributeReply(AttributeReplyPacket attributeReply, IPEndPoint ipAddress)
        {
            if (attributeReply.ErrorCode == SlpErrorCode.None)
            {
                AttributeReplyEventArgs args = new AttributeReplyEventArgs() { Address = ipAddress, RequestId = attributeReply.Header.XId};
                args.Attributes = SplitAttributeList(attributeReply.AttrList);
                if (AttributeReply != null)
                {
                    AttributeReply(this, args);
                }
            }
        }


        // This regex matches a (key=value) string from the attribute list
        // This is much simplified compared to what the RFC calls for but it should work for most common cases
        static Regex attributePairMatch = new Regex(@"\(\s*([^\(\)\,\\\/\!\<\=\>\~]+)\s*=\s*([^\)]*)\s*\)", RegexOptions.Compiled);

        /// <summary>
        /// Splits a string attribute list into a dictionary of key value pairs.
        /// Only supports (key=value) lists not tags.
        /// </summary>
        /// <param name="attributeList">The attribute list.</param>
        /// <returns>A dictionary of string pairs</returns>
        public static Dictionary<string, string> SplitAttributeList(string attributeList)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            var pairs = attributeList.Split(',');
            foreach (string pair in pairs)
            {
                Match match = attributePairMatch.Match(pair);
                if (match.Success)
                {
                    attributes[match.Groups[1].Value] = string.IsNullOrEmpty(match.Groups[2].Value) ? string.Empty : match.Groups[2].Value;
                }
            }
            return attributes;
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

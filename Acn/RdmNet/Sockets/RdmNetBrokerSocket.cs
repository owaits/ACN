using LXProtocols.Acn.IO;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LXProtocols.Acn.RdmNet.Sockets
{
    public class RdmNetBrokerSocket : RdmNetSocket, IProtocolFilter
    {
        public RdmNetBrokerSocket(Socket socket, UId rdmId, Guid senderId, string sourceName)
            : base(rdmId, senderId, sourceName)
        {
            //RegisterProtocolFilter(this);
        }

        public static RdmNetBrokerSocket Connect(RdmEndPoint endpoint, Guid senderId)
        {
            Socket newConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newConnection.Connect(endpoint);

            RdmNetBrokerSocket socket = new RdmNetBrokerSocket(newConnection, endpoint.Id, senderId, string.Empty);
            socket.Open(endpoint);
            return socket;
        }

        #region IProtocolFilter Members

        /// <summary>
        /// Gets a list of protocol ID's that this filter supports.
        /// </summary>
        IEnumerable<int> IProtocolFilter.ProtocolId
        {
            get { return new[] { (int)ProtocolIds.Broker }; }
        }

        /// <summary>
        /// Processes the packet that have been recieved and allocated to this filter.
        /// </summary>
        /// <remarks>
        /// Only packets that have supported protocol ID's will be sent to this function.
        /// </remarks>
        /// <param name="source">The source IP address of the packet.</param>
        /// <param name="header">The header information for the ACN packet.</param>
        /// <param name="data">The data reader for the remaining packet data.</param>
        void IProtocolFilter.ProcessPacket(IPEndPoint source, AcnRootLayer header, AcnBinaryReader data)
        {
        }

        #endregion
    }
}

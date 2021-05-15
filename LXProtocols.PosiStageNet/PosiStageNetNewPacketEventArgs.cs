using LXProtocols.PosiStageNet.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LXProtocols.PosiStageNet
{
    /// <summary>
    /// Event information when a new PSN packet is recieved.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class PosiStageNetNewPacketEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PosiStageNetNewPacketEventArgs"/> class.
        /// </summary>
        /// <param name="localEndPoint">The local end point.</param>
        /// <param name="remoteEndPoint">The remote end point.</param>
        /// <param name="packet">The packet.</param>
        public PosiStageNetNewPacketEventArgs(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint, PosiStageNetPacket packet)
        {
            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remoteEndPoint;
            Packet = packet;
        }

        private IPEndPoint localEndPoint = null;

        /// <summary>
        /// Gets or sets the local end point.
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return localEndPoint; }
            set { localEndPoint = value; }
        }

        private IPEndPoint remoteEndPoint = null;

        /// <summary>
        /// Gets or sets the remote end point.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return remoteEndPoint; }
            set { remoteEndPoint = value; }
        }

        private PosiStageNetPacket packet;

        /// <summary>
        /// Gets the recieved packet.
        /// </summary>
        public PosiStageNetPacket Packet
        {
            get { return packet; }
            private set { packet = value; }
        }
    }
}

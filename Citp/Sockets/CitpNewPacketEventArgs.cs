using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Citp.Packets;

namespace Citp.Sockets
{
    public class CitpNewPacketEventArgs:EventArgs
    {
        public CitpNewPacketEventArgs(IPEndPoint localEndPoint,IPEndPoint remoteEndPoint,CitpPacket packet)
        {
            LocalEndPoint = localEndPoint;
            RemoteEndPoint = remoteEndPoint;
            Packet = packet;
        }

        private IPEndPoint localEndPoint = null;

        public IPEndPoint LocalEndPoint
        {
            get { return localEndPoint; }
            set { localEndPoint = value; }
        }

        private IPEndPoint remoteEndPoint = null;

        public IPEndPoint RemoteEndPoint
        {
            get { return remoteEndPoint; }
            set { remoteEndPoint = value; }
        }

        private CitpPacket packet;

        public CitpPacket Packet
        {
            get { return packet; }
            private set { packet = value; }
        }
    }
}

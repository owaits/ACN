using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.Slp.Packets;
using System.Net;

namespace LXProtocols.Acn.Slp.Sockets
{
    public class NewPacketEventArgs : EventArgs
    {
        public NewPacketEventArgs(SlpPacket packet)
        {
            Packet = packet;
        }

        public IPEndPoint SourceEndPoint { get; set; }

        private SlpPacket packet;

        public SlpPacket Packet
        {
            get { return packet; }
            private set { packet = value; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LXProtocols.TCNet.Sockets
{
    public class NewPacketEventArgs<TPacketType> : EventArgs
    {
        public NewPacketEventArgs(TCNetEndPoint source, TPacketType packet)
        {
            Source = source;
            Packet = packet;
        }

        private TCNetEndPoint source;

        public TCNetEndPoint Source
        {
            get { return source; }
            protected set { source = value; }
        }

        private TPacketType packet;

        public TPacketType Packet
        {
            get { return packet; }
            private set { packet = value; }
        }

    }
}

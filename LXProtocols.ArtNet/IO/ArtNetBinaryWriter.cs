using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace LXProtocols.ArtNet.IO
{
    public class ArtNetBinaryWriter:BinaryWriter
    {
        public ArtNetBinaryWriter(Stream output)
            : base(output)
        {
        }

        public void WriteNetwork(byte value)
        {
            base.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteNetwork(short value)
        {
            base.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteNetwork(int value)
        {
            base.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteNetwork(string value, int length)
        {
            Write(Encoding.UTF8.GetBytes(value.PadRight(length, (char) 0x0)));
        }
    }
}

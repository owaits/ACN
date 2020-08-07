using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Packets.sAcn
{
    public class StreamingAcnDmxPacket:StreamingAcnPacket
    {
        public StreamingAcnDmxPacket()
            : base(ProtocolIds.sACN)
        {
            Dmx.AddressType = 0xa1;
            Dmx.AddressIncrement = 1;

        }
    }
}

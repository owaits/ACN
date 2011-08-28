using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Packets.sAcn
{
    public class DmxPacket:StreamingAcnPacket
    {
        public DmxPacket()
            : base(ProtocolIds.sACN)
        {
            Dmx.AddressType = 0xa1;
            Dmx.AddressIncrement = 1;

        }
    }
}

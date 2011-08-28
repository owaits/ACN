using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Packets.sAcn
{
    public class RdmNetPacket:StreamingAcnPacket
    {
        public RdmNetPacket()
            : base(ProtocolIds.RdmNet)
        {
        }
    }
}

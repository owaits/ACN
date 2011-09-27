using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Management
{
    public enum NackReason
    {
        UnknownPid = 0x0,
        FormatError = 0x1,
        HardwareFault = 0x2,
        ProxyReject = 0x3
    }
}

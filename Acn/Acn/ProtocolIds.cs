using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn
{
    public enum ProtocolIds
    {
        DMP = 0x2,
        sACN = 0x4,

        /// <summary>
        /// RDM data carried via streaming ACN.
        /// </summary>
        RdmNet = 0x05,

        /// <summary>
        /// Null packet used in RDMNet for Heartbeat.
        /// </summary>
        Null = 0x6
    }
}

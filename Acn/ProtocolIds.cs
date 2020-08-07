using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn
{
    public enum ProtocolIds
    {
        /// <summary>
        /// Session Data Transport Protocol
        /// </summary>
        SDT = 0x1,

        /// <summary>
        /// Device Management Protocol
        /// </summary>
        DMP = 0x2,

        /// <summary>
        /// DMX Data streamed via Multicast
        /// </summary>
        sACN = 0x4,

        /// <summary>
        /// RDM data carried via streaming ACN.
        /// </summary>
        RdmNet = 0x05,

        /// <summary>
        /// Null packet used in RDMNet for Heartbeat.
        /// </summary>
        Null = 0x6,

        /// <summary>
        /// The streaming ACN extensions such as discovery and synchronization.
        /// </summary>
        sACNExtended = 0x08
    }
}

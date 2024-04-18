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
        /// RPT transports RDM messages over IP and supports a multi-controller environment.
        /// </summary>
        RdmPacketTransfer = 0x05,

        /// <summary>
        /// Null packet used in RDMNet for Heartbeat.
        /// </summary>
        Null = 0x6,

        /// <summary>
        /// The streaming ACN extensions such as discovery and synchronization.
        /// </summary>
        sACNExtended = 0x08,

        /// <summary>
        /// RDMNet Broker Messages
        /// </summary>
        Broker = 0x09,

        /// <summary>
        /// Simple device discovery and configuration.
        /// </summary>
        LLRP = 0x0A,

        /// <summary>
        /// EPT allows non-RDM data to be transmitted using a Broker.
        /// </summary>
        /// <remarks>
        /// which addresses Components solely by their CID, and allows manufacturer-defined data to be
        /// transmitted between Components free from the message format and behavioral restrictions of RDM.
        /// </remarks
        ExtensiblePacketTransfer = 0x0B
    }
}

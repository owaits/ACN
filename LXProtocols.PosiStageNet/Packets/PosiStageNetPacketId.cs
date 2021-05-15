using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.PosiStageNet.Packets
{
    /// <summary>
    /// The root packet IDs for PosiStageNet.
    /// </summary>
    public enum PosiStageNetPacketId
    {
        /// <summary>
        /// The information packet containing system information.
        /// </summary>
        Information = 0x6756,
        /// <summary>
        /// The marker information packet.
        /// </summary>
        Data = 0x6755
    }
}

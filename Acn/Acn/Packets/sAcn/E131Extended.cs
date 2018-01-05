using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Packets.sAcn
{
    /// <summary>
    /// The extension PDU's for E131
    /// </summary>
    public enum E131Extended
    {
        /// <summary>
        /// The streaming ACN packet synchronization message.
        /// </summary>
        Synchronization = 1,
        /// <summary>
        /// The streaming ACN discovery message containing the universes being transmitted.
        /// </summary>
        Discovery = 2
    }
}

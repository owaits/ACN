using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet
{
    /// <summary>
    /// The various different types of TCNet device.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// An unknown or non existant device type.
        /// </summary>
        None = 0,
        /// <summary>
        /// The master state is negotiated.
        /// </summary>
        Auto = 1,
        /// <summary>
        /// The device acts as a master, feeding TCNet information to the network.
        /// </summary>
        Master = 2,
        /// <summary>
        /// The device acts as a slave, consuming TCNet information from the network.
        /// </summary>
        Slave = 4,
        /// <summary>
        /// The device acts as a relay, forwarding TCNet information.
        /// </summary>
        Repeater = 8
    }
}

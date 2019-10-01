using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// The base class to be used by all DJ Tap packets.
    /// </summary>
    public abstract class TCNetPacket
    {
        #region Information

        /// <summary>
        /// Gets or sets the network ID which is generated from the IP address and node ID.
        /// </summary>
        /// <remarks>
        /// This is a network unique ID that you can use to identify a device.
        /// </remarks>
        public int NetworkID { get; set; }

        /// <summary>
        /// Node ID of sending device.
        /// </summary>
        public ushort NodeID { get; set; }

        /// <summary>
        /// Gets or sets the time stamp assigned at the moment the packet was recieved.
        /// </summary>
        public DateTime RXTimeStamp { get; set; }

        #endregion

        /// <summary>
        /// Reads the data from a memory buffer into this packet object.
        /// </summary>
        /// <remarks>
        /// Decodes the raw data into usable information.
        /// </remarks>
        /// <param name="data">The data to be read.</param>
        public abstract void ReadData(TCNetBinaryReader data);

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public abstract void WriteData(TCNetBinaryWriter data);

        public static int BuildNetworkID(IPEndPoint endpoint, int nodeId)
        {
            return new Tuple<IPAddress, int>(endpoint.Address, nodeId).GetHashCode();
        }
    }
}

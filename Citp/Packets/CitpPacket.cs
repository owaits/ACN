using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets
{
    /// <summary>
    /// The base class for all CITP packets.
    /// </summary>
    /// <remarks>
    /// All CITP packets should derive from this abstract class.
    /// </remarks>
    public abstract class CitpPacket
    {
        /// <summary>
        /// Reads the packet information from the specified stream.
        /// </summary>
        /// <remarks>
        /// Use to create a packet from a network stream.
        /// </remarks>
        /// <param name="data">The stream to read the packet information from.</param>
        public abstract void ReadData(CitpBinaryReader data);

        /// <summary>
        /// Writes the information in this packet to the specified stream.
        /// </summary>
        /// <param name="data">The stream to write the packet information to.</param>
        public abstract void WriteData(CitpBinaryWriter data);   

    }
}

using ProDJTap.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProDJTap.Packets
{
    /// <summary>
    /// The base class to be used by all DJ Tap packets.
    /// </summary>
    public abstract class DJTapPacket
    {
        #region Information

        /// <summary>
        /// Gets or sets the time stamp assigned at the moment the packet was recieved.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        #endregion

        /// <summary>
        /// Reads the data from a memory buffer into this packet object.
        /// </summary>
        /// <remarks>
        /// Decodes the raw data into usable information.
        /// </remarks>
        /// <param name="data">The data to be read.</param>
        public abstract void ReadData(DJTapBinaryReader data);

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public abstract void WriteData(DJTapBinaryWriter data);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace ProDJTap.IO
{
    /// <summary>
    /// Provides extended stream reading capability for the DJTap protocol.
    /// </summary>
    /// <seealso cref="System.IO.BinaryReader" />
    public class DJTapBinaryReader:BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DJTapBinaryReader"/> class.
        /// </summary>
        /// <param name="input">A stream.</param>
        public DJTapBinaryReader(Stream input)
            : base(input)
        { }

        /// <summary>
        /// Reads 16Bit values using the ProDJ tap format.
        /// </summary>
        /// <returns>The resulting 16Bit Value</returns>
        public ushort ReadNetwork16()
        {
            return (ushort) IPAddress.HostToNetworkOrder(ReadInt16());
        }

        /// <summary>
        /// Reads 32Bit values using the ProDJ tap format.
        /// </summary>
        /// <returns>The resulting 32Bit Value</returns>
        public uint ReadNetwork32()
        {
            return ReadUInt32();
        }

        /// <summary>
        /// Reads 16Bit values using the ProDJ tap format.
        /// </summary>
        /// <returns>The resulting 16Bit Value</returns>
        public float ReadNetworkSingle()
        {
            return ReadSingle();
        }
    }
}

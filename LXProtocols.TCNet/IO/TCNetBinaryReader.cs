using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace LXProtocols.TCNet.IO
{
    /// <summary>
    /// Provides extended stream reading capability for the DJTap protocol.
    /// </summary>
    /// <seealso cref="System.IO.BinaryReader" />
    public class TCNetBinaryReader:BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetBinaryReader"/> class.
        /// </summary>
        /// <param name="input">A stream.</param>
        public TCNetBinaryReader(Stream input)
            : base(input)
        { }

        /// <summary>
        /// Reads 16Bit values using the ProDJ tap format.
        /// </summary>
        /// <returns>The resulting 16Bit Value</returns>
        public ushort ReadNetwork16()
        {
            return ReadUInt16();
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
        /// Reads 64Bit values using the ProDJ tap format.
        /// </summary>
        /// <returns>The resulting 64Bit Value</returns>
        public ulong ReadNetwork64()
        {
            return ReadUInt64();
        }

        /// <summary>
        /// Reads 16Bit values using the ProDJ tap format.
        /// </summary>
        /// <returns>The resulting 16Bit Value</returns>
        public float ReadNetworkSingle()
        {
            return ReadSingle();
        }

        /// <summary>
        /// Reads a string of the specified length from the network stream.
        /// </summary>
        /// <returns></returns>
        public string ReadNetworkString(int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(length));
        }

        /// <summary>
        /// Reads the TCNet time from binary data.
        /// </summary>
        /// <returns>The time that was read.</returns>
        public TimeSpan ReadNetworkTime()
        {
            uint milliseconds = ReadNetwork32();
            return TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}

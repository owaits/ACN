using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace LXProtocols.TCNet.IO
{
    /// <summary>
    /// Provides extended stream writing capability for the DJTap protocol.
    /// </summary>
    /// <seealso cref="System.IO.BinaryWriter" />
    public class TCNetBinaryWriter:BinaryWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetBinaryWriter"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public TCNetBinaryWriter(Stream input)
            : base(input)
        { }

        /// <summary>
        /// Writes a 16Bit value in DJTap format to the data stream.
        /// </summary>
        /// <param name="value">The 16Bit value to write.</param>
        public void WriteToNetwork(ushort value)
        {
            Write((short)value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace ProDJTap.IO
{
    /// <summary>
    /// Provides extended stream writing capability for the DJTap protocol.
    /// </summary>
    /// <seealso cref="System.IO.BinaryWriter" />
    public class DJTapBinaryWriter:BinaryWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DJTapBinaryWriter"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public DJTapBinaryWriter(Stream input)
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

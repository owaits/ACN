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
            Write(value);
        }

        /// <summary>
        /// Writes UInt16 to network.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteToNetwork(uint value)
        {
            Write(value);
        }

        /// <summary>
        /// Writes UInt32 to network.
        /// </summary>
        /// <param name="value">The value.</param>
        public void WriteToNetwork(ulong value)
        {
            Write(value);
        }

        /// <summary>
        /// Reads a string of the specified length from the network stream.
        /// </summary>
        /// <returns></returns>
        public void WriteToNetwork(string value,int length)
        {
            value = (value == null ? string.Empty :value);
            Write(Encoding.ASCII.GetBytes(value.PadRight(length,'\0').Substring(0,length)));
        }

        /// <summary>
        /// Writes TCNet to to network.
        /// </summary>
        /// <param name="time">The time.</param>
        public void WriteToNetwork(TimeSpan time)
        {
            WriteToNetwork((uint) time.TotalMilliseconds);
        }
    }
}

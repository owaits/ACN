using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.IO
{
    /// <summary>
    /// Extends the <see cref="BinaryWriter"/> class with methods specific to writting the an ACN stream.
    /// </summary>
    /// <seealso cref="System.IO.BinaryWriter" />
    public class AcnBinaryWriter : BinaryWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AcnBinaryWriter"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public AcnBinaryWriter(Stream input)
            : base(input)
        { }

        /// <summary>
        /// Writes an ACN Octet to the stream.
        /// </summary>
        /// <param name="value">The 16Bit value to write to the stream.</param>
        public void WriteOctet(short value)
        {
            Write(IPAddress.HostToNetworkOrder(value));
        }

        /// <summary>
        /// Writes an ACN Octet to the stream.
        /// </summary>
        /// <param name="value">The 32Bit value to write to the stream.</param>
        public void WriteOctet(int value)
        {
            Write(IPAddress.HostToNetworkOrder(value));
        }

        /// <summary>
        /// Writes the UTF8 string of a certain length to the stream.
        /// </summary>
        /// <param name="value">The string you want to write to the stream.</param>
        /// <param name="length">The length of the string to write, unused characters will be padded with zeros.</param>
        public void WriteUtf8String(string value, int length)
        {
            //Limit the length of the string to the length specified.
            value = value?.Substring(0,Math.Min(value.Length,length));

            Write(UTF8Encoding.UTF8.GetBytes(value));
            Write(new byte[(length - value.Length)]);
        }

        /// <summary>
        /// Writes a UID to the stream.
        /// </summary>
        /// <param name="value">The UID to write to the stream.</param>
        public void Write(UId value)
        {
            WriteOctet((short)value.ManufacturerId);
            WriteOctet((int)value.DeviceId);
        }
    }
}

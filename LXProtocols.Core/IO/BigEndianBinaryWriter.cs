using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LXProtocols.Core.IO
{
    /// <summary>
    /// Writes binary data to a stream in Big Endian format, this is ideal for writing data to a network stream.
    /// </summary>
    /// <seealso cref="System.IO.BinaryReader" />
    public class BigEndianBinaryWriter: BinaryWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BigEndianBinaryWriter"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public BigEndianBinaryWriter(Stream input)
            : base(input)
        { }

        /// <summary>
        /// Writes a two-byte signed integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte signed integer to write.</param>
        public override void Write(short value)
        {
            var buffer = new byte[sizeof(short)];
            BinaryPrimitives.WriteInt16BigEndian(buffer, value);
            base.Write(buffer);
        }

        /// <summary>
        /// Writes a four-byte signed integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte signed integer to write.</param>
        public override void Write(int value)
        {
            var buffer = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(buffer, value);
            base.Write(buffer);
        }

        /// <summary>
        /// Writes an eight-byte signed integer to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte signed integer to write.</param>
        public override void Write(long value)
        {
            var buffer = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64BigEndian(buffer, value);
            base.Write(buffer);
        }

        /// <summary>
        /// Writes a two-byte unsigned integer to the current stream and advances the stream position by two bytes.
        /// </summary>
        /// <param name="value">The two-byte unsigned integer to write.</param>
        public override void Write(ushort value)
        {
            var buffer = new byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
            base.Write(buffer);
        }

        /// <summary>
        /// Writes a four-byte unsigned integer to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        public override void Write(uint value)
        {
            var buffer = new byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
            base.Write(buffer);
        }

        /// <summary>
        /// Writes an eight-byte unsigned integer to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte unsigned integer to write.</param>
        public override void Write(ulong value)
        {
            var buffer = new byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
            base.Write(buffer);
        }

        /// <summary>
        /// Writes a four-byte floating-point value to the current stream and advances the stream position by four bytes.
        /// </summary>
        /// <param name="value">The four-byte floating-point value to write.</param>
        public override void Write(float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            base.Write(buffer);
        }

        /// <summary>
        /// Writes an eight-byte floating-point value to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="value">The eight-byte floating-point value to write.</param>
        public override void Write(double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);
            base.Write(buffer);
        }
    }
}

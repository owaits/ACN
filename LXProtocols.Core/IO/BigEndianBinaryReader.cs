using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LXProtocols.Core.IO
{
    /// <summary>
    /// Reads binary data from a stream in Big Endian format, this is ideal for reading from a network stream.
    /// </summary>
    /// <seealso cref="System.IO.BinaryReader" />
    public class BigEndianBinaryReader: BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BigEndianBinaryReader"/> class.
        /// </summary>
        /// <param name="input">The input stream.</param>
        public BigEndianBinaryReader(Stream input)
            : base(input)
        { 
        }

        /// <summary>
        /// Reads a 2-byte signed integer from the current stream and advances the current position of the stream by two bytes.
        /// </summary>
        /// <returns>
        /// A 2-byte signed integer read from the current stream.
        /// </returns>
        public override short ReadInt16()
        {
            return BinaryPrimitives.ReadInt16BigEndian(ReadBytes(sizeof(short)));
        }

        /// <summary>
        /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <returns>
        /// A 4-byte signed integer read from the current stream.
        /// </returns>
        public override int ReadInt32()
        {
            return BinaryPrimitives.ReadInt32BigEndian(ReadBytes(sizeof(int)));
        }

        /// <summary>
        /// Reads an 8-byte signed integer from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <returns>
        /// An 8-byte signed integer read from the current stream.
        /// </returns>
        public override long ReadInt64()
        {
            return BinaryPrimitives.ReadInt64BigEndian(ReadBytes(sizeof(long)));
        }

        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream using little-endian encoding and advances the position of the stream by two bytes.
        /// </summary>
        /// <returns>
        /// A 2-byte unsigned integer read from this stream.
        /// </returns>
        public override ushort ReadUInt16()
        {
            return BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(sizeof(ushort)));
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream and advances the position of the stream by four bytes.
        /// </summary>
        /// <returns>
        /// A 4-byte unsigned integer read from this stream.
        /// </returns>
        public override uint ReadUInt32()
        {
            return BinaryPrimitives.ReadUInt32BigEndian(ReadBytes(sizeof(uint)));
        }

        /// <summary>
        /// Reads an 8-byte unsigned integer from the current stream and advances the position of the stream by eight bytes.
        /// </summary>
        /// <returns>
        /// An 8-byte unsigned integer read from this stream.
        /// </returns>
        public override ulong ReadUInt64()
        {
            return BinaryPrimitives.ReadUInt64BigEndian(ReadBytes(sizeof(ulong)));
        }

        /// <summary>
        /// Reads a 4-byte floating point value from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <returns>
        /// A 4-byte floating point value read from the current stream.
        /// </returns>
        public override float ReadSingle()
        {
            //Note: BinaryPrimitives version for single is not in .NetStandard 2.1
            byte[] buffer = ReadBytes(sizeof(float));
            Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer);
        }

        /// <summary>
        /// Reads an 8-byte floating point value from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <returns>
        /// An 8-byte floating point value read from the current stream.
        /// </returns>
        public override double ReadDouble()
        {
            //Note: BinaryPrimitives version for single is not in .NetStandard 2.1
            byte[] buffer = ReadBytes(sizeof(double));
            Array.Reverse(buffer);
            return BitConverter.ToDouble(buffer);
        }
    }
}

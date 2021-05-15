using LXProtocols.Core.IO;
using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace LXProtocols.PosiStageNet.IO
{
    /// <summary>
    /// Used to write PosiStageNet specific data to a binary network stream, the reader handles common scenarios such as byte ordering.
    /// </summary>
    /// <remarks>
    /// A number of read functions exist for PSN spcific data types such as chunk header and vectors.
    /// </remarks>
    /// <seealso cref="LXProtocols.Core.IO.BigEndianBinaryWriter" />
    public class PosiStageNetWriter:BigEndianBinaryWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PosiStageNetWriter"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public PosiStageNetWriter(Stream input)
            : base(input)
        { }

        /// <summary>
        /// Writes a chunky to the stream enasuring that the header with payload length is written correctly. You provide chunk payload data through the chunkBody delegate.
        /// </summary>
        /// <param name="id">The ID of the chunk.</param>
        /// <param name="chunkBody">The the logic to write the chunk data to the stream.</param>
        /// <param name="hasSubChunks">Whether this chunk contains sub chunks or data.</param>
        public void WriteChunk(ushort id, Action chunkBody, bool hasSubChunks = true)
        {
            //Record the chunk header position so we can come back and write the header last.
            long startPosition = BaseStream.Position;

            //Skip the header, we will write this later.
            BaseStream.Seek(4, SeekOrigin.Current);

            //Write the contents of the chunk.
            chunkBody();

            long endPosition = BaseStream.Position;

            //Go back to the header.
            BaseStream.Seek(startPosition - endPosition, SeekOrigin.Current);

            long dataLength = endPosition - startPosition - 4;
            WriteChunkHeader(new ChunkHeader(id,(ushort) dataLength,hasSubChunks));

            //Go back to the end of the chunk.
            BaseStream.Seek(dataLength, SeekOrigin.Current);
        }

        /// <summary>
        /// Encodes the chunk header as a 32bit value and writes it to the stream.
        /// </summary>
        /// <param name="header">The chunk header to write to the stream.</param>
        public void WriteChunkHeader(ChunkHeader header)
        {
            uint chunkHeader = (uint) (header.HasSubChunks ? 0x80000000 : 0);
            chunkHeader |= ((uint) header.DataLength & 0x7FFF) << 16;
            chunkHeader |= (uint)header.Id & 0xFFFF;

            Write(chunkHeader);
        }

        /// <summary>
        /// Writes a string to the stream using a chunk header to indicate the length of the string.
        /// </summary>
        /// <remarks>
        /// The string is encoded as ASCII.
        /// </remarks>
        /// <param name="value">The string to be written to the stream.</param>
        /// <param name="chunkId">Optional override for the chunk header ID, this allows you to set a custom ID.</param>
        public void WriteChunkString(string value, ushort? chunkId = null)
        {
            WriteChunkHeader(new ChunkHeader(chunkId ?? 0, (ushort)value.Length, false));
            Write(Encoding.ASCII.GetBytes(value));
        }

        /// <summary>
        /// Writes an XYZ vector value to the stream.
        /// </summary>
        /// <param name="chunkId">The id of the chunk.</param>
        /// <param name="vector">The vector as XYZ.</param>
        public void WriteVector(ushort chunkId, Vector3 vector)
        {
            WriteChunkHeader(new ChunkHeader(chunkId, sizeof(float) * 3, false));
            Write(vector.X);
            Write(vector.Y);
            Write(vector.Z);
        }

        /// <summary>
        /// Writes the time stamp to the stream as total microseconds.
        /// </summary>
        /// <param name="timeStamp">The time stamp to write to the stream.</param>
        public void WriteTimeStamp(TimeSpan timeStamp)
        {
            Write(timeStamp.Ticks);
        }
    }
}

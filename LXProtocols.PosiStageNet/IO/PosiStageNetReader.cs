using LXProtocols.Core.IO;
using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace LXProtocols.PosiStageNet.IO
{
    /// <summary>
    /// Used to read PosiStageNet specific data from a binary network stream, the reader handles common scenarios such as byte ordering.
    /// </summary>
    /// <remarks>
    /// A number of read functions exist for PSN spcific data types such as chunk header and vectors.
    /// </remarks>
    /// <seealso cref="LXProtocols.Core.IO.BigEndianBinaryReader" />
    public class PosiStageNetReader:BigEndianBinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PosiStageNetReader"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public PosiStageNetReader(Stream input)
            : base(input)
        { }

        /// <summary>
        /// Reads the chunk header containing ID, Length and Sub Chunk flag. The header is decoded from a 32bit value.
        /// </summary>
        /// <returns></returns>
        public ChunkHeader ReadChunkHeader()
        {
            uint header = ReadUInt32();
            return new ChunkHeader()
            {
                Id = (ushort)(header & 0xFFFF),
                DataLength = (ushort)((header >> 16) & 0x7FFF),
                HasSubChunks = (header & 0x80000000) > 0
            };           
        }

        /// <summary>
        /// Reads the chunk string which uses a chunk header to indicate the string length and then an ASCII string to follow.
        /// </summary>
        /// <param name="stringHeader">Optional override for the chunk header, the default uses ID as zero.</param>
        /// <returns>The string that has been read from the stream.</returns>
        public string ReadChunkString(ChunkHeader? stringHeader = null)
        {
            if(stringHeader == null)
                stringHeader = ReadChunkHeader();

            return Encoding.ASCII.GetString(ReadBytes(((ChunkHeader)stringHeader).DataLength));
        }

        /// <summary>
        /// Reads an XYV vector value as floats from the stream.
        /// </summary>
        /// <returns>The XYZ vector that was read.</returns>
        public Vector3 ReadVector()
        {
            return new Vector3()
            {
                X = ReadSingle(),
                Y = ReadSingle(),
                Z = ReadSingle(),
            };
        }

        /// <summary>
        /// Reads the time stamp as the time since the sender was started in microseconds.
        /// </summary>
        /// <returns>The time since the sender started.</returns>
        public TimeSpan ReadTimeStamp()
        {
            return TimeSpan.FromTicks(ReadInt64());
        }

        /// <summary>
        /// Helps you write chunks by ensuring the correct chunk header and size are written to the stream.
        /// </summary>
        /// <param name="parentChunk">The parent chunk header information.</param>
        /// <param name="readAction">The read operation to decode the chunk contents after the header.</param>
        public void ForEachChunk(ChunkHeader parentChunk, Action<ChunkHeader> readAction)
        {
            int dataToRead = parentChunk.DataLength;
            while (dataToRead > 0)
            {
                ChunkHeader chunkHeader = ReadChunkHeader();
                readAction(chunkHeader);
                dataToRead -= chunkHeader.DataLength + 4;
            }
        }
    }
}

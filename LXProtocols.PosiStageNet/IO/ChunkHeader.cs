using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.PosiStageNet.IO
{
    /// <summary>
    /// Each PSN packet is split into a number of nested chunks, each chunk contains a header with information susch as the type of data in the chunk and how long each chunk is.
    /// </summary>
    /// <remarks>
    /// A chunk may contain either information or more chunks, this is indicated by the HasSubChunk flag.
    /// 
    /// The packet is split into a number of chunks to allow backwards compatibility as when reading chunk unknown to the reciever they can simply be ignored. 
    /// </remarks>
    public struct ChunkHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkHeader"/> struct.
        /// </summary>
        /// <param name="id">The ID of the chunk, this determines the contents of the chunk and type of data it contains.</param>
        /// <param name="dataLength">The length of data that follows the chunk header.</param>
        /// <param name="hasSubChunks">Whether this chunk contains any child chunks.</param>
        public ChunkHeader(ushort id, ushort dataLength, bool hasSubChunks)
        {
            Id = id;
            DataLength = dataLength;
            HasSubChunks = hasSubChunks;
        }

        /// <summary>
        /// The ID of the chunk, this determines the contents of the chunk and type of data it contains.
        /// </summary>
        public ushort Id;

        /// <summary>
        /// The length of data that follows the chunk header.
        /// </summary>
        public ushort DataLength;

        /// <summary>
        /// Whether this chunk contains any child chunks.
        /// </summary>
        public bool HasSubChunks;
    }
}

using LXProtocols.PosiStageNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.PosiStageNet.Packets
{
    /// <summary>
    /// The base class frtom which all PSN packets derive, contains header information common to all PSN packets.
    /// </summary>
    public abstract class PosiStageNetPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PosiStageNetPacket"/> class.
        /// </summary>
        /// <param name="packetId">The packet identifier.</param>
        public PosiStageNetPacket(PosiStageNetPacketId packetId)
        {
            PacketId = packetId;
        }

        #region Information

        /// <summary>
        /// Gets or sets the packet identifier.
        /// </summary>
        public PosiStageNetPacketId PacketId { get; protected set; }

        /// <summary>
        /// Gets or sets the time stamp at which this packet was sent based on the offset from when the sender first started.
        /// </summary>
        public TimeSpan TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the protocol version.
        /// </summary>
        /// <remarks>
        /// The version ensures that all systems using the same higher version number are compatible.
        /// </remarks>
        public Version ProtocolVersion { get; set; } = new Version(2,3);

        /// <summary>
        /// Gets or sets the current frame number. When packet contents are split across multiple packets this indicates the index of the frame.
        /// </summary>
        /// <remarks>
        /// PSN packets are limited to 1500 bytes so if a packet requires more data then it must be split by setting the FramePacketCount and FrameIndex.
        /// </remarks>
        public byte FrameID { get; set; }

        /// <summary>
        /// Gets or sets the total number of frames this packet is split across. When packet contents are split across multiple packets this indicates the number of frames the packet is split across.
        /// </summary>
        /// <remarks>
        /// PSN packets are limited to 1500 bytes so if a packet requires more data then it must be split by setting the FramePacketCount and FrameIndex.
        /// </remarks>
        public byte FramePacketCount { get; set; }

        #endregion

        #region Read/Write

        /// <summary>
        /// Reads the packet information from the specified stream.
        /// </summary>
        /// <remarks>
        /// Use to create a packet from a network stream.
        /// </remarks>
        /// <param name="data">The stream to read the packet information from.</param>
        public void ReadData(ChunkHeader header, PosiStageNetReader data)
        {
            data.ForEachChunk(header, chunk =>
            {
                if (chunk.Id == 0)
                    ReadHeaderChunk(data);
                else
                    ReadChunk(chunk, data);
            });
        }

        /// <summary>
        /// Reads the header information from the data stream.
        /// </summary>
        /// <param name="data">The data stream to read from.</param>
        protected void ReadHeaderChunk(PosiStageNetReader data)
        {
            TimeStamp = data.ReadTimeStamp();
            ProtocolVersion = new Version(data.ReadByte(),data.ReadByte());
            FrameID = data.ReadByte();
            FramePacketCount = data.ReadByte();
        }

        /// <summary>
        /// Implementing classes should override this to read each chunk specific to the packet type. This will be called for each chunk that forms the packet.
        /// </summary>
        /// <param name="chunk">The chunk header.</param>
        /// <param name="data">The data reader to read the chunk from.</param>
        protected abstract void ReadChunk(ChunkHeader chunk, PosiStageNetReader data);

        /// <summary>
        /// Writes the information in this packet to the specified stream.
        /// </summary>
        /// <param name="data">The stream to write the packet information to.</param>
        public void WriteData(PosiStageNetWriter data)
        {
            data.WriteChunk((ushort) PacketId, () =>
            {
                WriteHeader(data);
                WriteChunk(data);
            });
        }

        /// <summary>
        /// Writes the header information to the data stream. 
        /// </summary>
        /// <param name="data">The data stream to write the header to.</param>
        protected void WriteHeader(PosiStageNetWriter data)
        {
            data.WriteChunk(0, () =>
             {
                data.WriteTimeStamp(TimeStamp);
                data.Write((byte)ProtocolVersion.Major);
                data.Write((byte)ProtocolVersion.Minor);
                data.Write(FrameID);
                data.Write(FramePacketCount);
             });
        }

        /// <summary>
        /// Implementing classes should override this to write each chunk specific to the packet type. This will be called after the header information for the packet has been written.
        /// </summary>
        /// <param name="data">The stream to write the packet data to.</param>
        protected abstract void WriteChunk(PosiStageNetWriter data);

        #endregion
    }
}

using LXProtocols.PosiStageNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.PosiStageNet.Packets
{
    /// <summary>
    /// This packet contains system information such as the name and trackers we will be sending.
    /// </summary>
    /// <seealso cref="LXProtocols.PosiStageNet.Packets.PosiStageNetPacket" />
    public class PosiStageNetInformation : PosiStageNetPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PosiStageNetInformation"/> class.
        /// </summary>
        public PosiStageNetInformation():base(PosiStageNetPacketId.Information)
        {
        }

        #region Packet Contents

        /// <summary>
        /// Gets or sets the name of the system.
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the trackers for which the name will be sent by this packet.
        /// </summary>
        public List<PosiStageNetTracker> Trackers { get; set; } = new List<PosiStageNetTracker>();

        #endregion

        #region Read/Write

        /// <summary>
        /// Implementing classes should override this to read each chunk specific to the packet type. This will be called for each chunk that forms the packet.
        /// </summary>
        /// <param name="chunk">The chunk header.</param>
        /// <param name="data">The data reader to read the chunk from.</param>
        protected override void ReadChunk(ChunkHeader chunk, PosiStageNetReader data)
        {
            switch(chunk.Id)
            {
                case 1:
                    SystemName = data.ReadChunkString(chunk);
                    break;
                case 2:
                    data.ForEachChunk(chunk, tracker =>
                    {
                        string trackerName = data.ReadChunkString();
                        Trackers.Add(new PosiStageNetTracker()
                        {
                            Id = tracker.Id,
                            Name = trackerName
                        });
                    });
                    break;
            }
        }

        /// <summary>
        /// Implementing classes should override this to write each chunk specific to the packet type. This will be called after the header information for the packet has been written.
        /// </summary>
        /// <param name="data">The stream to write the packet data to.</param>
        /// <exception cref="System.InvalidOperationException">The system name has not been set, please set the system name before writting the data.</exception>
        protected override void WriteChunk(PosiStageNetWriter data)
        {
            if (string.IsNullOrEmpty(SystemName))
                throw new InvalidOperationException("The system name has not been set, please set the system name before writting the data.");

            data.WriteChunkString(SystemName, 1);

            data.WriteChunk(2, () =>
            {
                if(Trackers != null)
                {
                    foreach (var tracker in Trackers)
                    {
                        data.WriteChunk(tracker.Id, () =>
                        {
                            data.WriteChunkString(tracker.Name);
                        });
                    }
                }
            });
        }

        #endregion
    }
}

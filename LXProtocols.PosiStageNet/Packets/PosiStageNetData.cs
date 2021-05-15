using LXProtocols.PosiStageNet.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace LXProtocols.PosiStageNet.Packets
{
    /// <summary>
    /// This packet is normally streamed at 60Hz and contain tracker updates.
    /// </summary>
    /// <seealso cref="LXProtocols.PosiStageNet.Packets.PosiStageNetPacket" />
    public class PosiStageNetData: PosiStageNetPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PosiStageNetData"/> class.
        /// </summary>
        public PosiStageNetData() : base(PosiStageNetPacketId.Data)
        {
        }

        #region Packet Contents

        /// <summary>
        /// Gets or sets the tracker data for this packet.
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
            if (chunk.Id == 1)
            {
                data.ForEachChunk(chunk, trackerHeader =>
                {
                    PosiStageNetTracker tracker = new PosiStageNetTracker();
                    tracker.Id = trackerHeader.Id;

                    data.ForEachChunk(trackerHeader, parameter =>
                    {
                        ReadTrackerParameter(parameter, data, tracker);
                    });

                    Trackers.Add(tracker);
                });
            }
        }

        /// <summary>
        /// Reads the tracker parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="data">The data.</param>
        /// <param name="tracker">The tracker.</param>
        private void ReadTrackerParameter(ChunkHeader parameter, PosiStageNetReader data, PosiStageNetTracker tracker)
        {
            switch (parameter.Id)
            {
                case 0:
                    tracker.Position = data.ReadVector();
                    break;
                case 1:
                    tracker.Speed = data.ReadVector();
                    break;
                case 2:
                    tracker.Orientation = data.ReadVector();
                    break;
                case 3:
                    tracker.Validity = data.ReadSingle();
                    break;
                case 4:
                    tracker.Acceleration = data.ReadVector();
                    break;
                case 5:
                    tracker.TargetPosition = data.ReadVector();
                    break;
                case 6:
                    tracker.TimeStamp = data.ReadTimeStamp();
                    break;

            }
        }

        /// <summary>
        /// Implementing classes should override this to write each chunk specific to the packet type. This will be called after the header information for the packet has been written.
        /// </summary>
        /// <param name="data">The stream to write the packet data to.</param>
        protected override void WriteChunk(PosiStageNetWriter data)
        {
            data.WriteChunk(1, () =>
            {
                foreach(var tracker in Trackers)
                {
                    WriteTracker(data,tracker);
                }
            });
        }

        /// <summary>
        /// Writes the tracker to the stream
        /// </summary>
        /// <param name="data">The data stream to write to.</param>
        /// <param name="tracker">The tracker to be written.</param>
        protected void WriteTracker(PosiStageNetWriter data, PosiStageNetTracker tracker)
        {
            data.WriteChunk(tracker.Id,()=>
            {
                data.WriteVector(0,tracker.Position);
                data.WriteVector(1,tracker.Speed);
                data.WriteVector(2,tracker.Orientation);
                data.WriteChunkHeader(new ChunkHeader(3,sizeof(float),false));
                data.Write(tracker.Validity);
                data.WriteVector(4,tracker.Acceleration);
                data.WriteVector(5,tracker.TargetPosition);
                data.WriteChunkHeader(new ChunkHeader(6, sizeof(long), false));
                data.WriteTimeStamp(tracker.TimeStamp);
            });
        }

        #endregion
    }
}

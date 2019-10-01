using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Contains Beat Grid Data of layer
    /// </summary>
    /// <remarks>
    /// This info can be requested from a node by sending a TCNet Request Data packet, with Datatype=8, Layer=The layer you request data from
    /// </remarks>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetDataHeader" />
    public class TCNetBeatGrid:TCNetDataHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetBeatGrid"/> class.
        /// </summary>
        public TCNetBeatGrid() : base(DataTypes.BeatGrid)
        {
        }

        #endregion

        #region Packet Content

        public uint DataSize { get; set; }

        public uint TotalPackets { get; set; }

        public uint PacketNumber { get; set; }

        public uint DataClusterSize { get; set; }

        public class BeatData
        {
            public ushort BeatNumber { get; set; }

            public BeatTypes BeatType { get; set; }

            public TimeSpan BeatTimeStamp { get; set; }
        }

        public IEnumerable<BeatData> Beats { get; protected set; }

        #endregion

        #region Read/Write

        /// <summary>
        /// Reads the data from a memory buffer into this packet object.
        /// </summary>
        /// <param name="data">The data to be read.</param>
        /// <remarks>
        /// Decodes the raw data into usable information.
        /// </remarks>
        public override void ReadData(TCNetBinaryReader data)
        {
            base.ReadData(data);

            DataSize = data.ReadNetwork32();
            TotalPackets = data.ReadNetwork32();
            PacketNumber = data.ReadNetwork32();
            DataClusterSize = data.ReadNetwork32();

            long readPosition = DataClusterSize;
            List<BeatData> beats = new List<BeatData>();
            while(readPosition > 7)
            {
                BeatData beat = new BeatData();
                beat.BeatNumber = data.ReadNetwork16();
                beat.BeatType = (BeatTypes) data.ReadByte();
                data.ReadByte();
                beat.BeatTimeStamp = data.ReadNetworkTime();

                beats.Add(beat);

                readPosition -= 8;
            }

            Beats = beats;
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(DataSize);
            data.Write(TotalPackets);
            data.Write(PacketNumber);
            data.Write(DataClusterSize);

            foreach(BeatData beat in Beats)
            {
                data.WriteToNetwork(beat.BeatNumber);
                data.Write((byte) beat.BeatType);
                data.Write((byte)0);
                data.WriteToNetwork(beat.BeatTimeStamp);
            }
        }

        #endregion
    }
}

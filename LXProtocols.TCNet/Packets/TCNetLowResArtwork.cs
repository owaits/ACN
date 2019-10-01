using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Contains Low Res Artwork file (JPEG Format)
    /// </summary>
    /// <remarks>
    /// This info can be requested from a node by sending a TCNet Request Data packet, with Datatype=12, Layer=The layer you request data from
    /// </remarks>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetDataHeader" />
    public class TCNetLowResArtwork:TCNetDataHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetLowResArtwork"/> class.
        /// </summary>
        public TCNetLowResArtwork() : base(DataTypes.LowResArtworkFile)
        {
        }

        #endregion

        #region Packet Content

        public uint TotalPackets { get; set; }

        public uint PacketNumber { get; set; }

        public uint DataClusterSize { get; set; }

        public byte[] FileData { get; set; }

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

            int dataSize = (int)data.ReadNetwork32();
            TotalPackets = data.ReadNetwork32();
            PacketNumber = data.ReadNetwork32();
            DataClusterSize = data.ReadNetwork32();
            FileData = data.ReadBytes(dataSize);
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(FileData.Length);
            data.Write(TotalPackets);
            data.Write(PacketNumber);
            data.Write(DataClusterSize);
            data.Write(FileData);
        }

        #endregion
    }
}

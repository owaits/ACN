using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    public class TCNetDataHeader:TCNetHeader
    {
        #region Setup and Initialisation

        public TCNetDataHeader(DataTypes data) :base(MessageTypes.Data)
        {
            DataType = data;
        }

        #endregion

        #region Packet Contents

        /// <summary>
        /// Gets the type of the data contained in this packet.
        /// </summary>
        public DataTypes DataType { get; private set; }

        /// <summary>
        /// Gets or sets the deck or master that this timecode refers to.
        /// </summary>
        /// <remarks>
        /// 1 to 4 = Decks 1-4
        /// 6,7 = Layer A, Layer B
        /// 8 = Master
        /// </remarks>
        public byte LayerID { get; set; }

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
            DataType = (DataTypes) data.ReadByte();
            LayerID = data.ReadByte();
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((byte) DataType);
            data.Write(LayerID);
        }

        #endregion
    }
}

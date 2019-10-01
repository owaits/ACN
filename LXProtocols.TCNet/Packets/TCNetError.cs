using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Notifies that a request is not handled
    /// </summary>
    /// <remarks>
    /// Send when a request is not handled or caused an error or for notifications, this message is send back to notify requesting node.
    /// </remarks>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetHeader" />
    public class TCNetError:TCNetHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetError"/> class.
        /// </summary>
        public TCNetError() : base(MessageTypes.Error)
        {
        }

        #endregion

        #region Packet Content


        public DataTypes DataType { get; set; }

        public byte LayerID { get; set; }

        public ushort Code { get; set; }

        public MessageTypes MessageType { get; set; }

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
            LayerID =data.ReadByte();
            Code = data.ReadNetwork16();
            MessageType = (MessageTypes) data.ReadNetwork16();
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
            data.WriteToNetwork(Code);
            data.WriteToNetwork((ushort) MessageType);
        }

        #endregion
    }
}

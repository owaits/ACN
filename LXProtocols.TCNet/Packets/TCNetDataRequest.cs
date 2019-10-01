using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Request Data from other Node
    /// </summary>
    /// <remarks>
    /// Request is send to a master or repeater node. As result the node will send back a packet containing small wave data or a request error message.
    /// </remarks>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetHeader" />
    public class TCNetDataRequest:TCNetHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetDataRequest"/> class.
        /// </summary>
        public TCNetDataRequest() : base(MessageTypes.DataRequest)
        {
        }

        #endregion

        #region Packet Content

        /// <summary>
        /// Gets or sets the type of the data we want to recieve from the remote node.
        /// </summary>
        public DataTypes DataType { get; set; }

        /// <summary>
        /// Layer where Data is requested for
        /// </summary>
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

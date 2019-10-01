using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// The DJ Tap header for all DJ Tap packets. Contains common data to all packets.
    /// </summary>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetPacket" />
    public class TCNetHeader:TCNetPacket
    {
        /// <summary>
        /// The header size.
        /// </summary>
        public const int PacketSize = 24;

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetHeader"/> class.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        public TCNetHeader(MessageTypes contentType)
        {
            ProtocolVersion = new Version(3, 6);
            Header = "TCN";
            MessageType = contentType;
            Timestamp = TimeSpan.Zero;

        }

        #region Packet Content

        /// <summary>
        /// Protocol Version of sending device.
        /// </summary>
        public Version ProtocolVersion { get; set; }

        /// <summary>
        /// TCNet Protocol Header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the Message type of packet.
        /// </summary>
        public MessageTypes MessageType { get; private set; }

        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// Gets or sets the sequence number of this packet with respect to other packets.
        /// </summary>
        public byte SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of the node such as master, slave etc.
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// Gets or sets the node options.
        /// </summary>
        public NodeOptions NodeOptions { get; set; }

        /// <summary>
        /// Timestamp in microseconds that is used to calculate network latency.
        /// </summary>
        public TimeSpan Timestamp { get; set; }


        #endregion

        #region Information

        /// <summary>
        /// Determines if the data with the packet is valid and should be treated as good.
        /// </summary>
        /// <returns>True if the packet passed all checks.</returns>
        public bool IsValid()
        {
            return true;
        }

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
            NodeID = data.ReadNetwork16();
            ProtocolVersion = new Version(data.ReadByte(), data.ReadByte());
            Header = data.ReadNetworkString(3);
            MessageType = (MessageTypes)data.ReadByte();
            NodeName = data.ReadNetworkString(8);
            SequenceNumber = data.ReadByte();
            NodeType = (NodeType)data.ReadByte();
            NodeOptions = (NodeOptions)data.ReadNetwork16();
            Timestamp = TimeSpan.FromTicks(data.ReadNetwork32() * (TimeSpan.TicksPerSecond / 1000000));
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            data.WriteToNetwork(NodeID);
            data.Write((byte) ProtocolVersion.Major);
            data.Write((byte) ProtocolVersion.Minor);
            data.WriteToNetwork(Header,3);
            data.Write((byte) MessageType);
            data.WriteToNetwork(NodeName,8);
            data.Write(SequenceNumber);
            data.Write((byte)NodeType);
            data.WriteToNetwork((ushort)NodeOptions);
            data.WriteToNetwork((uint)(Timestamp.Milliseconds * 1000));
        }

        #endregion


    }
}

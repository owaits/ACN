using System;
using System.Collections.Generic;
using System.Text;
using LXProtocols.TCNet.IO;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Present and keep alive a node into a TCNet network.
    /// </summary>
    /// <remarks>
    /// Broadcast every 1000ms
    /// </remarks>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetHeader" />
    public class TCNetOptIn:TCNetHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// We register the packet type in the static constructor.
        /// </summary>
        static TCNetOptIn()
        {
            TCNetPacketBuilder.RegisterPacket(MessageTypes.OptIn, (() => new TCNetOptIn()));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetOptIn"/> class.
        /// </summary>
        public TCNetOptIn():base(MessageTypes.OptIn)
        {
        }

        #endregion

        #region Packet Content

        /// <summary>
        /// Number of nodes registered by system.
        /// </summary>
        public ushort NodeCount { get; set; }

        /// <summary>
        /// Listener port of node (Used to receive unicast messages)
        /// </summary>
        public ushort NodeListenerPort { get; set; }

        /// <summary>
        /// Gets or sets the uptime of the node which rolls over every 12 hours
        /// </summary>
        public TimeSpan Uptime { get; set; }

        /// <summary>
        /// Name of Vendor of Node
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// Name of Application/Device (Node)
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the application/device version of the node.
        /// </summary>
        public Version DeviceVersion { get; set; }

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

            NodeCount = data.ReadNetwork16();
            NodeListenerPort = data.ReadNetwork16();
            Uptime = TimeSpan.FromSeconds(data.ReadNetwork16());
            data.ReadBytes(2);
            VendorName = data.ReadNetworkString(16);
            DeviceName = data.ReadNetworkString(16);
            DeviceVersion = new Version(data.ReadByte(), data.ReadByte(), data.ReadByte());
            data.ReadBytes(1);
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.WriteToNetwork(NodeCount);
            data.WriteToNetwork(NodeListenerPort);
            data.WriteToNetwork((ushort) Uptime.TotalSeconds);
            data.Write(new byte[] { 0,0});
            data.WriteToNetwork(VendorName,16);
            data.WriteToNetwork(DeviceName,16);
            data.Write((byte) DeviceVersion.Major);
            data.Write((byte) DeviceVersion.Minor);
            data.Write((byte) DeviceVersion.Build);
            data.Write((byte) 0);
        }

        #endregion
    }
}

using ProDJTap.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProDJTap.Packets
{
    /// <summary>
    /// The DJ Tap header for all DJ Tap packets. Contains common data to all packets.
    /// </summary>
    /// <seealso cref="ProDJTap.Packets.DJTapPacket" />
    public class DJTapHeader:DJTapPacket
    {
        /// <summary>
        /// The header size.
        /// </summary>
        public const int PacketSize = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="DJTapHeader"/> class.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        public DJTapHeader(MessageTypes contentType)
        {
            IdentifierNumber = 0x0800;
            FirmwareVersion = 0x1;
            Timer = 0;
            ContentType = contentType;
        }

        #region Packet Content

        /// <summary>
        /// Gets or sets the protocol identifier number.
        /// </summary>
        public ushort IdentifierNumber { get; set; }

        /// <summary>
        /// Gets or sets the firmware version of the transmitting device.
        /// </summary>
        public ushort FirmwareVersion { get; set; }

        /// <summary>
        /// Gets or sets a counter that is incremented by the sender after every packet.
        /// </summary>
        /// <remarks>
        /// The counter is incremented from 0 to 999
        /// </remarks>
        public ushort Timer { get; set; }

        /// <summary>
        /// Gets or sets the content type of this packet.
        /// </summary>
        public MessageTypes ContentType { get; set; }

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
        public override void ReadData(DJTapBinaryReader data)
        {
            IdentifierNumber = data.ReadUInt16();
            FirmwareVersion = data.ReadUInt16();
            Timer = data.ReadUInt16();
            ContentType = (MessageTypes) data.ReadUInt16();        
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(DJTapBinaryWriter data)
        {
            data.WriteToNetwork(IdentifierNumber);
            data.WriteToNetwork(FirmwareVersion);
            data.WriteToNetwork(Timer);
            data.WriteToNetwork((ushort) ContentType);
        }

        #endregion


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Citp.IO;

namespace Citp.Packets
{
    /// <summary>
    /// The standard CITP header packet from which packets wanting to use a CITP header should derive.
    /// </summary>
    /// <remarks>
    /// Most CITP packets will derive from this header packet.
    /// </remarks>
    /// <seealso cref="Citp.Packets.CitpPacket" />
    public class CitpHeader:CitpPacket
    {
        public const int PacketSize = 20;

        #region Setup and Initialisation
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CitpHeader"/> class.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        public CitpHeader(string contentType)
        {
            Cookie = "CITP";
            VersionMajor = 1;
            VersionMinor = 0;
            MessagePartCount = 1;
            ContentType = contentType;
        }
        
        #endregion

        #region Packet Content

        public string Cookie { get; set; }

        public byte VersionMajor { get; set; }

        public byte VersionMinor { get; set; }

        public ushort RequestId { get; set; }

        public uint MessageSize { get; set; }

        public ushort MessagePartCount { get; set; }

        public ushort MessagePart { get; set; }

        public string ContentType { get; set; }

        #endregion

        #region Information

        /// <summary>
        /// Checks the CITP cookie to ensure the packet is a valid CITP packet.
        /// </summary>
        /// <returns>True if the cookie matches the expected value.</returns>
        public bool IsValid()
        {
            return Cookie == "CITP";
        }

        #endregion

        #region Read/Write

        /// <summary>
        /// Reads the packet information from the specified stream.
        /// </summary>
        /// <param name="data">The stream to read the packet information from.</param>
        /// <remarks>
        /// Use to create a packet from a network stream.
        /// </remarks>
        public override void ReadData(CitpBinaryReader data)
        {
            Cookie = data.ReadCookie();
            VersionMajor = data.ReadByte();
            VersionMinor = data.ReadByte();

            RequestId = data.ReadUInt16();
            MessageSize = data.ReadUInt32();
            MessagePartCount = data.ReadUInt16();
            MessagePart = data.ReadUInt16();

            ContentType = data.ReadCookie();            
        }

        /// <summary>
        /// Writes the information in this packet to the specified stream.
        /// </summary>
        /// <param name="data">The stream to write the packet information to.</param>
        public override void WriteData(CitpBinaryWriter data)
        {
            data.WriteCookie(Cookie);
            data.Write(VersionMajor);
            data.Write(VersionMinor);
            data.Write(RequestId);
            data.Write(MessageSize);
            data.Write(MessagePartCount);
            data.Write(MessagePart);
            data.WriteCookie(ContentType);
        }

        /// <summary>
        /// Updates the message size field once the total size of the packet is known.
        /// </summary>
        /// <param name="messageData">The message data.</param>
        public void WriteMessageSize(CitpBinaryWriter messageData)
        {
            messageData.Seek(8, SeekOrigin.Begin);
            messageData.Write((UInt32) messageData.BaseStream.Length);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ContentType;
        }

    }
}

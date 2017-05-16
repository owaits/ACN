using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Citp.IO;

namespace Citp.Packets
{
    public class CitpHeader:CitpPacket
    {
        public const int PacketSize = 20;

        public CitpHeader(string contentType)
        {
            Cookie = "CITP";
            VersionMajor = 1;
            VersionMinor = 0;
            MessagePartCount = 1;
            ContentType = contentType;
        }

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

        public bool IsValid()
        {
            return Cookie == "CITP";
        }

        #endregion

        #region Read/Write

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


        public void WriteMessageSize(CitpBinaryWriter messageData)
        {
            messageData.Seek(8, SeekOrigin.Begin);
            messageData.Write((UInt32) messageData.BaseStream.Length);
        }

        #endregion

        public override string ToString()
        {
            return ContentType;
        }

    }
}

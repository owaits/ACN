using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexRequestStream:CitpMsexHeader
    {
        public const string PacketType = "RqSt";

        #region Constructors

        public CitpMsexRequestStream()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexRequestStream(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public UInt16 SourceIdentifier { get; set; }

        public string FrameFormat { get; set; }

        public UInt16 FrameWidth { get; set; }

        public UInt16 FrameHeight { get; set; }

        public byte FramesPerSecond { get; set; }

        public byte Timeout { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            SourceIdentifier = data.ReadUInt16();
            FrameFormat = data.ReadCookie();
            FrameWidth = data.ReadUInt16();
            FrameHeight = data.ReadUInt16();
            FramesPerSecond = data.ReadByte();
            Timeout = data.ReadByte();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(SourceIdentifier);
            data.WriteCookie(FrameFormat);
            data.Write(FrameWidth);
            data.Write(FrameHeight);
            data.Write(FramesPerSecond);
            data.Write(Timeout);
        }

        #endregion
    }
}

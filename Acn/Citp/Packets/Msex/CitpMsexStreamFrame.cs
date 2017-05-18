using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexStreamFrame:CitpMsexHeader
    {
        public const string PacketType = "StFr";

        #region Constructors

        public CitpMsexStreamFrame()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexStreamFrame(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public string MediaServerUid { get; set; }

        public UInt16 SourceIdentifier { get; set; }

        public string FrameFormat { get; set; }

        public UInt16 FrameWidth { get; set; }

        public UInt16 FrameHeight { get; set; }

        public byte[] FrameBuffer { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            if (MsexVersion > CitpMsexVersions.Msex11Version)
            {
                MediaServerUid = data.ReadUcs1();
            }
            SourceIdentifier = data.ReadUInt16();
            FrameFormat = data.ReadCookie();
            FrameWidth = data.ReadUInt16();
            FrameHeight = data.ReadUInt16();

            int bufferSize = data.ReadUInt16();
            FrameBuffer = data.ReadBytes(bufferSize);
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            if (MsexVersion > CitpMsexVersions.Msex11Version)
            {
                data.WriteUcs1(MediaServerUid);
            }

            data.Write(SourceIdentifier);
            data.WriteCookie(FrameFormat);
            data.Write(FrameWidth);
            data.Write(FrameHeight);

            data.Write((UInt16)FrameBuffer.Length);
            data.Write(FrameBuffer);
        }

        #endregion
    }
}

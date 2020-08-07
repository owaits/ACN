using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.Msex
{
    public class CitpMsexElementLibraryThumbnail:CitpMsexHeader
    {
        public const string PacketType = "ELTh";

        #region Constructors

        public CitpMsexElementLibraryThumbnail()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexElementLibraryThumbnail(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public MsexElementType LibraryType { get; set; }

        public byte LibraryNumber
        {
            get { return LibraryId.ToNumber(); }
        }

        private CitpMsexLibraryId libraryId = new CitpMsexLibraryId();

        public CitpMsexLibraryId LibraryId
        {
            get { return libraryId; }
            set { libraryId = value; }
        }                

        public string ThumbnailFormat { get; set; }

        public UInt16 ThumbnailWidth { get; set; }

        public UInt16 ThumbnailHeight { get; set; }

        public byte[] ThumbnailBuffer { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            LibraryType = (MsexElementType)data.ReadByte();

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                LibraryId.ParseNumber(data.ReadByte());
            else
                LibraryId = data.ReadMsexLibraryId();

            ThumbnailFormat = data.ReadCookie();
            ThumbnailWidth = data.ReadUInt16();
            ThumbnailHeight = data.ReadUInt16();

            int bufferSize = data.ReadUInt16();
            ThumbnailBuffer = data.ReadBytes(bufferSize);
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte)LibraryType);

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                data.Write(LibraryNumber);
            else
                data.WriteMsexLibraryId(LibraryId);

            data.WriteCookie(ThumbnailFormat);
            data.Write(ThumbnailWidth);
            data.Write(ThumbnailHeight);

            data.Write((UInt16)ThumbnailBuffer.Length);
            data.Write(ThumbnailBuffer);
        }

        #endregion
    }
}

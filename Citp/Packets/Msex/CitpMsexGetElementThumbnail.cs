using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.Msex
{
    public enum ThumbnailOptions
    {
        None = 0,
        PreserveAspectRatio = 1
    }
    public class CitpMsexGetElementThumbnail : CitpMsexGetElementThumbnailBase
    {
        public new const string PacketType = "GETh";

        #region Constructors

        public CitpMsexGetElementThumbnail()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexGetElementThumbnail(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public CitpMsexLibraryId LibraryId { get; set; }

        private List<byte> elementNumbers = new List<byte>();

        public List<byte> ElementNumbers
        {
            get { return elementNumbers; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            ThumbnailFormat = data.ReadCookie();
            ThumbnailWidth = data.ReadUInt16();
            ThumbnailHeight = data.ReadUInt16();
            ThumbnailFlags = (ThumbnailOptions) data.ReadByte();
            LibraryType = (MsexElementType) data.ReadByte();

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                LibraryId = new CitpMsexLibraryId(data.ReadByte());
            else
                LibraryId = data.ReadMsexLibraryId();

            int elementCount = (MsexVersion < CitpMsexVersions.Msex12Version) ? data.ReadByte() : data.ReadUInt16();
            for(int n=0;n<elementCount;n++)
                ElementNumbers.Add(data.ReadByte());
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.WriteCookie(ThumbnailFormat);
            data.Write(ThumbnailWidth);
            data.Write(ThumbnailHeight);
            data.Write((byte)ThumbnailFlags);
            data.Write((byte)LibraryType);

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                data.Write(LibraryId.ToNumber());
            else
                data.WriteMsexLibraryId(LibraryId);

            if (MsexVersion < CitpMsexVersions.Msex12Version)
                data.Write((byte) ElementNumbers.Count);
            else
                data.Write((UInt16) ElementNumbers.Count);

            foreach (byte element in ElementNumbers)
                data.Write(element);
        }

        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.Packets;
using Citp.IO;
using Citp.Packets.Msex;

namespace Citp.Packets.Msex
{
    public class CitpMsexElementThumbnail : CitpMsexElementLibraryThumbnail
    {
        public const string PacketType = "EThn";

        #region Constructors

        public CitpMsexElementThumbnail()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexElementThumbnail(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content
                
        public byte ElementNumber { get; set; }  

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
            ElementNumber = data.ReadByte();

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
            data.Write(ElementNumber);

            data.WriteCookie(ThumbnailFormat);
            data.Write(ThumbnailWidth);
            data.Write(ThumbnailHeight);

            data.Write((UInt16) ThumbnailBuffer.Length);
            data.Write(ThumbnailBuffer);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexGetElementLibraryThumbnail : CitpMsexGetElementThumbnailBase
    {
        public const string PacketType = "GELT";

        #region Constructors

        public CitpMsexGetElementLibraryThumbnail()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexGetElementLibraryThumbnail(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public List<byte> LibraryNumbers
        {
            get { return LibraryIds.Select(x => x.ToNumber()).ToList(); }
        }

        private List<CitpMsexLibraryId> libraryIds = new List<CitpMsexLibraryId>();

        public List<CitpMsexLibraryId> LibraryIds
        {
            get { return libraryIds; }            
        }
        
        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            ThumbnailFormat = data.ReadCookie();
            ThumbnailWidth = data.ReadUInt16();
            ThumbnailHeight = data.ReadUInt16();
            ThumbnailFlags = (ThumbnailOptions)data.ReadByte();
            LibraryType = (MsexElementType)data.ReadByte();

            int libraryCount = (MsexVersion < CitpMsexVersions.Msex12Version) ? data.ReadByte() : data.ReadUInt16();
            if (MsexVersion < CitpMsexVersions.Msex11Version)
            {
                for (int i = 0; i < libraryCount; i++)
                {
                    LibraryIds.Add(new CitpMsexLibraryId(data.ReadByte()));
                }
            }
            else
            {
                for (int i = 0; i < libraryCount; i++)
                {
                    LibraryIds.Add(data.ReadMsexLibraryId());
                }
            }
            
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.WriteCookie(ThumbnailFormat);
            data.Write(ThumbnailWidth);
            data.Write(ThumbnailHeight);
            data.Write((byte)ThumbnailFlags);
            data.Write((byte)LibraryType);

            if (MsexVersion < CitpMsexVersions.Msex12Version)
                data.Write((byte)LibraryIds.Count);
            else
                data.Write((UInt16)LibraryIds.Count);

            if (MsexVersion < CitpMsexVersions.Msex11Version)
            {
                foreach (CitpMsexLibraryId id in LibraryIds)
                {
                    data.Write(id.ToNumber());
                }
            }
            else
            {
                foreach (CitpMsexLibraryId id in LibraryIds)
                {
                    data.WriteMsexLibraryId(id);
                }
            }
                
        }

        #endregion
    }
}

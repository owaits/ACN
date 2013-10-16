using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexGetElementLibraryInformation:CitpMsexHeader
    {
        public const string PacketType = "GELI";

        #region Constructors

        public CitpMsexGetElementLibraryInformation()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexGetElementLibraryInformation(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public MsexElementType LibraryType { get; set; }

        public CitpMsexLibraryId LibraryParentId { get; set; }

        public List<byte> LibraryNumbers { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            LibraryType = (MsexElementType) data.ReadByte();
            
            if (MsexVersion > 1.0) LibraryParentId = data.ReadMsexLibraryId();

            int libraryCount = 0;
            if (MsexVersion < 1.2)
                libraryCount = data.ReadByte();
            else
                libraryCount = data.ReadUInt16();

            for (int n = 0; n < libraryCount; n++)
                LibraryNumbers.Add(data.ReadByte());

        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) LibraryType);
            if (MsexVersion > 1.0) data.WriteMsexLibraryId(LibraryParentId);
            if (MsexVersion < 1.2)
                data.Write((byte)LibraryNumbers.Count);
            else
                data.Write((byte)LibraryNumbers.Count);

            foreach (byte number in LibraryNumbers)
                data.Write(number);
        }

        #endregion
    }
}

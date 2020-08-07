using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.Msex
{
    public class CitpMsexGetElementLibraryInformation:CitpMsexHeader
    {
        public new const string PacketType = "GELI";

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

        private List<byte> libraryNumbers = new List<byte>();

        public List<byte> LibraryNumbers
        {
            get { return libraryNumbers; }
            set { libraryNumbers = value; }
        }


        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            LibraryType = (MsexElementType) data.ReadByte();

            if (MsexVersion > CitpMsexVersions.Msex10Version) LibraryParentId = data.ReadMsexLibraryId();

            int libraryCount = 0;
            if (MsexVersion < CitpMsexVersions.Msex12Version)
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
            if (MsexVersion > CitpMsexVersions.Msex10Version) data.WriteMsexLibraryId(LibraryParentId);

            if (MsexVersion < CitpMsexVersions.Msex12Version)
                data.Write((byte)LibraryNumbers.Count);
            else
                data.Write((UInt16)LibraryNumbers.Count);

            foreach (byte number in LibraryNumbers)
                data.Write(number);
        }

        #endregion
    }
}

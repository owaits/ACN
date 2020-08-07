using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.Msex
{
    public class CitpMsexGetElementInformation:CitpMsexHeader
    {
        public new const string PacketType = "GEIn";

        #region Constructors

        public CitpMsexGetElementInformation()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexGetElementInformation(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public MsexElementType LibraryType { get; set; }

        public byte LibraryNumber
        {
            get  {  return LibraryId.ToNumber(); } 
        }

        private CitpMsexLibraryId libraryId = new CitpMsexLibraryId();

        public CitpMsexLibraryId LibraryId 
        {
            get { return libraryId; }
            set { libraryId = value; }
        }

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

            LibraryType = (MsexElementType)data.ReadByte();

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                LibraryId.ParseNumber(data.ReadByte());
            else
                LibraryId = data.ReadMsexLibraryId();

            int elementCount = (MsexVersion < CitpMsexVersions.Msex12Version) ? data.ReadByte() : data.ReadUInt16();

            for(int n=0;n<elementCount;n++)
                ElementNumbers.Add(data.ReadByte());
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) LibraryType);

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                data.Write(LibraryNumber);
            else
                data.WriteMsexLibraryId(LibraryId);

            if (MsexVersion < CitpMsexVersions.Msex12Version)
                data.Write((byte) ElementNumbers.Count);
            else
                data.Write((UInt16) ElementNumbers.Count);

            foreach (byte number in ElementNumbers)
                data.Write(number);

        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.Msex
{
    public class CitpMsexGenericElementInformation:CitpMsexHeader
    {
        public const string PacketType = "GLEI";

        #region Constructors

        public CitpMsexGenericElementInformation()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexGenericElementInformation(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        private CitpMsexLibraryId libraryId = new CitpMsexLibraryId();

        public CitpMsexLibraryId LibraryId
        {
            get { return libraryId; }
            set { libraryId = value; }
        }

        private List<ElementInfomation> elements = new List<ElementInfomation>();

        public List<ElementInfomation> Elements
        {
            get { return elements; }
        }

        public class ElementInfomation : CitpMsexHeader
        {
            public ElementInfomation()
            {
            }

            public ElementInfomation(CitpBinaryReader data, Version msexVersion)
            {
                MsexVersion = msexVersion;
                ReadData(data);
            }

            public byte ElementNumber { get; set; }

            public UInt32 SerialNumber { get; set; }

            public byte DmxRangeMin { get; set; }

            public byte DmxRangeMax { get; set; }

            public string Name { get; set; }

            public UInt64 VersionTimeStamp { get; set; }

            public override void ReadData(CitpBinaryReader data)
            {
                ElementNumber = data.ReadByte();
                if (MsexVersion == CitpMsexVersions.Msex12Version)
                {
                    SerialNumber = data.ReadUInt32();
                }
                DmxRangeMin = data.ReadByte();
                DmxRangeMax = data.ReadByte();
                Name = data.ReadUcs2();
                VersionTimeStamp = data.ReadUInt64();
            }

            public override void WriteData(CitpBinaryWriter data)
            {
                data.Write(ElementNumber);
                if (MsexVersion == CitpMsexVersions.Msex12Version)
                {
                    data.Write(SerialNumber);
                }
                data.Write(DmxRangeMin);
                data.Write(DmxRangeMax);
                data.WriteUcs2(Name);
                data.Write(VersionTimeStamp);
            }
        }
        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            if (MsexVersion > CitpMsexVersions.Msex10Version)
            {
                LibraryId = data.ReadMsexLibraryId();

                int elementCount = MsexVersion < CitpMsexVersions.Msex12Version ? data.ReadByte() : data.ReadUInt16();

                for (int n = 0; n < elementCount; n++)
                    Elements.Add(new ElementInfomation(data, MsexVersion));
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            
            //This message is not in the 1.0 spec, server should ignore the empty header.
            if (MsexVersion > CitpMsexVersions.Msex10Version)
            {                
                data.WriteMsexLibraryId(LibraryId);

                if (MsexVersion < CitpMsexVersions.Msex12Version)
                    data.Write((byte)Elements.Count);
                else
                    data.Write((UInt16)Elements.Count);

                foreach (ElementInfomation info in Elements)
                {
                    info.MsexVersion = MsexVersion;
                    info.WriteData(data);
                }
            }
        }

        #endregion
    }
}

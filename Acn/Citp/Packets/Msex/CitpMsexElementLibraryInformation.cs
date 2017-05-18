using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.Packets;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexElementLibraryInformation:CitpMsexHeader
    {
        public const string PacketType = "ELIn";

        #region Constructors

        public CitpMsexElementLibraryInformation()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexElementLibraryInformation(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        public MsexElementType LibraryType { get; set; }

        private List<ElementLibraryInformation> libraries = new List<ElementLibraryInformation>();

        public List<ElementLibraryInformation> Libraries
        {
            get { return libraries; }
        }

        public class ElementLibraryInformation : CitpMsexHeader
        {
            public ElementLibraryInformation()
            {
            }

            public ElementLibraryInformation(CitpBinaryReader data, Version msexVersion)
            {
                MsexVersion = msexVersion;
                ReadData(data);
            }

            /// <summary>
            /// Library that has been updated.
            /// </summary>
            public byte LibraryNumber
            {
                get { return LibraryId.ToNumber(); }
                set { LibraryId.ParseNumber(value); }
            }

            private CitpMsexLibraryId libraryId = new CitpMsexLibraryId();

            /// <summary>
            /// Library that has been updated.
            /// </summary>
            public CitpMsexLibraryId LibraryId
            {
                get { return libraryId; }
                set { libraryId = value; }
            }

            public UInt32 SerialNumber { get; set; }

            public byte DmxRangeMin;

            public byte DmxRangeMax;

            public string Name;            

            public UInt16 LibraryCount;

            public UInt16 ElementCount;

            public override void ReadData(CitpBinaryReader data)
            {
                if (MsexVersion < CitpMsexVersions.Msex11Version)
                    LibraryNumber = data.ReadByte();
                else
                    LibraryId = data.ReadMsexLibraryId();

                if (MsexVersion > CitpMsexVersions.Msex11Version)
                    SerialNumber = data.ReadUInt32();

                DmxRangeMin = data.ReadByte();
                DmxRangeMax = data.ReadByte();
                Name = data.ReadUcs2();

                if (MsexVersion > CitpMsexVersions.Msex10Version)
                {
                    LibraryCount = MsexVersion < CitpMsexVersions.Msex12Version ? data.ReadByte() : data.ReadUInt16();
                }

                ElementCount = MsexVersion < CitpMsexVersions.Msex12Version ? data.ReadByte() : data.ReadUInt16();
            }

            public override void WriteData(CitpBinaryWriter data)
            {
                if (MsexVersion < CitpMsexVersions.Msex11Version)
                    data.Write(LibraryNumber);
                else
                    data.WriteMsexLibraryId(LibraryId);

                if (MsexVersion > CitpMsexVersions.Msex11Version)
                    data.Write(SerialNumber);

                data.Write(DmxRangeMin);
                data.Write(DmxRangeMax);
                data.WriteUcs2(Name);

                if (MsexVersion >= CitpMsexVersions.Msex12Version)
                {
                    data.Write(LibraryCount);
                    data.Write(ElementCount);
                }
                else if (MsexVersion == CitpMsexVersions.Msex11Version)
                {
                    data.Write((byte) LibraryCount);
                    data.Write((byte) ElementCount);
                }
                else
                    data.Write((byte)ElementCount);                
            }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            LibraryType = (MsexElementType) data.ReadByte();

            int libraryCount = MsexVersion < CitpMsexVersions.Msex12Version ? data.ReadByte() : data.ReadUInt16();

            for (int n = 0; n < libraryCount; n++)
                Libraries.Add(new ElementLibraryInformation(data, MsexVersion));
            
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) LibraryType);

            if (MsexVersion < CitpMsexVersions.Msex12Version)
                data.Write((byte) Libraries.Count);
            else
                data.Write((UInt16) Libraries.Count);

            foreach (ElementLibraryInformation info in Libraries)
            {
                info.MsexVersion = MsexVersion;
                info.WriteData(data);
            }

        }

        #endregion
    }
}

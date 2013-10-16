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

            public ElementLibraryInformation(CitpBinaryReader data)
            {
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
                if (MsexVersion < 1.1)
                    LibraryNumber = data.ReadByte();
                else
                    LibraryId = data.ReadMsexLibraryId();

                if (MsexVersion > 1.1)
                    SerialNumber = data.ReadUInt32();

                DmxRangeMin = data.ReadByte();
                DmxRangeMax = data.ReadByte();
                Name = data.ReadUcs2();

                if (MsexVersion > 1.0)
                {
                    LibraryCount = MsexVersion < 1.2 ? data.ReadByte() : data.ReadUInt16();
                }

                ElementCount = MsexVersion < 1.2 ? data.ReadByte() : data.ReadUInt16();
            }

            public override void WriteData(CitpBinaryWriter data)
            {
                if (MsexVersion < 1.1)
                    data.Write(LibraryNumber);
                else
                    data.WriteMsexLibraryId(LibraryId);

                if (MsexVersion > 1.1)
                    data.Write(SerialNumber);

                data.Write(DmxRangeMin);
                data.Write(DmxRangeMax);
                data.WriteUcs2(Name);

                if (MsexVersion >= 1.2)
                {
                    data.Write(LibraryCount);
                    data.Write(ElementCount);
                }
                else if (MsexVersion == 1.1)
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

            int libraryCount = MsexVersion < 1.2 ? data.ReadByte() : data.ReadUInt16();

            for (int n = 0; n < libraryCount; n++)
                Libraries.Add(new ElementLibraryInformation(data));
            
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) LibraryType);

            if(MsexVersion <1.2)
                data.Write((byte) Libraries.Count);
            else
                data.Write((UInt16) Libraries.Count);

            foreach(ElementLibraryInformation info in Libraries)
                info.WriteData(data);

        }

        #endregion
    }
}

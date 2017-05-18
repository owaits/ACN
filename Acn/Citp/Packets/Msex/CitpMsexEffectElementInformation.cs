using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexEffectElementInformation:CitpMsexHeader
    {
        public const string PacketType = "EEIn";

        #region Constructors

        public CitpMsexEffectElementInformation()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexEffectElementInformation(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content
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

        private List<EffectInfomation> elements = new List<EffectInfomation>();

        public List<EffectInfomation> Elements
        {
            get { return elements; }
        }

        public class EffectInfomation : CitpMsexHeader
        {
            public EffectInfomation()
            {
            }

            public EffectInfomation(CitpBinaryReader data, Version msexVersion)
            {
                MsexVersion = msexVersion;
                ReadData(data);
            }

            public byte ElementNumber { get; set; }

            public UInt32 SerialNumber { get; set; }

            public byte DmxRangeMin { get; set; }

            public byte DmxRangeMax { get; set; }

            public string EffectName { get; set; }

            private List<string> effectParameterNames = new List<string>();

            public List<string> EffectParameterNames
            {
                get { return effectParameterNames; }                
            }

            public override void ReadData(CitpBinaryReader data)
            {
                ElementNumber = data.ReadByte();
                if (MsexVersion == CitpMsexVersions.Msex12Version)
                {
                    SerialNumber = data.ReadUInt32();
                }
                DmxRangeMin = data.ReadByte();
                DmxRangeMax = data.ReadByte();
                EffectName = data.ReadUcs2();
                byte parameterCount = data.ReadByte();
                EffectParameterNames.Clear();
                for (int i = 0; i < parameterCount; i++)
                {
                    EffectParameterNames.Add(data.ReadUcs2());
                }
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
                data.WriteUcs2(EffectName);
                data.Write((byte)EffectParameterNames.Count);
                foreach (string parameterName in EffectParameterNames)
                {
                    data.WriteUcs2(parameterName);
                }
            }
        }
        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                LibraryId.ParseNumber(data.ReadByte());
            else
                LibraryId = data.ReadMsexLibraryId();

            int elementCount = MsexVersion < CitpMsexVersions.Msex12Version ? data.ReadByte() : data.ReadUInt16();

            for (int n = 0; n < elementCount; n++)
                Elements.Add(new EffectInfomation(data, MsexVersion));
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            if (MsexVersion < CitpMsexVersions.Msex11Version)
                data.Write(LibraryNumber);
            else
                data.WriteMsexLibraryId(LibraryId);

            if (MsexVersion < CitpMsexVersions.Msex12Version)
                data.Write((byte)Elements.Count);
            else
                data.Write((UInt16)Elements.Count);

            foreach (EffectInfomation info in Elements)
            {
                info.MsexVersion = MsexVersion;
                info.WriteData(data);
            }
        }

        #endregion
    }
}

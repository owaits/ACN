using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Citp.IO;

namespace Citp.Packets.Msex
{
    public class CitpMsexVideoSources:CitpMsexHeader
    {
        public const string PacketType = "VSrc";

        #region Constructors

        public CitpMsexVideoSources():base()
        {
            LayerContentType = PacketType;
        }

        public CitpMsexVideoSources(CitpBinaryReader data)
        {
            ReadData(data);
        }

        #endregion

        #region Packet Content

        private List<SourceInformation> sources = new List<SourceInformation>();

        public List<SourceInformation> Sources
        {
            get { return sources; }
        }

        public class SourceInformation : CitpMsexHeader
        {
            public SourceInformation():base()
            {
            }

            public SourceInformation(CitpBinaryReader data, Version msexVersion)
            {
                MsexVersion = msexVersion;
                ReadData(data);
            }

            public UInt16 SourceIdentifier;

            public string SourceName;

            public byte PhysicalOutput;

            public byte LayerNumber;

            public UInt16 Flags;

            public UInt16 Width;

            public UInt16 Height;

            public override void ReadData(CitpBinaryReader data)
            {
                SourceIdentifier = data.ReadUInt16();
                SourceName = data.ReadUcs2();
                PhysicalOutput = data.ReadByte();
                LayerNumber = data.ReadByte();
                Flags = data.ReadUInt16();
                Width = data.ReadUInt16();
                Height = data.ReadUInt16();
            }

            public override void WriteData(CitpBinaryWriter data)
            {
                data.Write(SourceIdentifier);
                data.WriteUcs2(SourceName);
                data.Write(PhysicalOutput);
                data.Write(LayerNumber);
                data.Write(Flags);
                data.Write(Width);
                data.Write(Height);
            }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            UInt16 sourceCount = data.ReadUInt16();
            for (int i = 0; i < sourceCount; i++)
            {
                Sources.Add(new SourceInformation(data, MsexVersion));
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((UInt16) Sources.Count);
            foreach (SourceInformation source in Sources)
                source.WriteData(data);
        }

        #endregion
    }
}

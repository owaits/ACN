using Citp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp.Packets.CaEx
{
    public enum FixtureChanges
    {
        Update = 0,
        NewFixture = 1,
        ExchangedFixture = 2
    }

    public enum FixtureLinkType
    {
        RDMDeviceModelId = 0,
        RDMPersonalityId = 1,
        AtlaBaseFixtureId = 2,
        AtlaBaseModeId = 3
    };

    public class FixtureLink
    {
        public FixtureLink()
        {

        }
        public FixtureLink(FixtureLinkType type, Guid id)
        {
            Type = FixtureLinkType.AtlaBaseModeId;           
            Data = id.ToByteArray();
        }

        public FixtureLinkType Type;

        public byte[] Data;

        public Guid ValueAsGuid()
        {
            return new Guid(new byte[] { Data[3], Data[2], Data[1], Data[0],Data[5], Data[4], Data[7], Data[6],   Data[8], Data[9], Data[10], Data[11],Data[12], Data[13],Data[14], Data[15]});
        }
    }

    public class CaExFixtureList : CaExHeader
    {
         #region Setup and Initialisation

        public CaExFixtureList()
            : base(CaExContentCodes.FixtureList)
        {
        }

        #endregion

        #region Fixture Information

        public class FixtureInformation
        {
            public FixtureInformation()
            {
                ManufacturerName = string.Empty;
                FixtureName = string.Empty;
                ModeName = string.Empty;
                Unit = string.Empty;
                Circuit = string.Empty;
                Note = string.Empty;
            }

            public UInt32 FixtureId { get; set; }

            public string ManufacturerName { get; set; }

            public string FixtureName { get; set; }

            public string ModeName { get; set; }

            public UInt16 ChannelCount { get; set; }

            public bool IsDimmer { get; set; }

            private List<FixtureLink> links = new List<FixtureLink>();

            public List<FixtureLink> Links
            {
                get { return links; }
                set { links = value; }
            }

            public bool IsPatched { get; set; }

            public byte DMXUniverse { get; set; }

            public ushort DMXAddress { get; set; }

            public string Unit { get; set; }

            public ushort Channel { get; set; }

            public string Circuit { get; set; }

            public string Note { get; set; }

            public Coordinate Position { get; set; }

            public Coordinate Angle { get; set; }
        }

        #endregion

        #region Packet Content

        public FixtureChanges Type { get; set; }

        private List<FixtureInformation> fixtures = new List<FixtureInformation>();

        public List<FixtureInformation> Fixtures
        {
            get { return fixtures; }
            set { fixtures = value; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            Type = (FixtureChanges) data.ReadByte();
            
            int count = data.ReadUInt16();
            Fixtures.Clear();
            for(int n=0; n<count;n++)
            {
                FixtureInformation information = new FixtureInformation()
                {
                    FixtureId = data.ReadUInt32(),
                    ManufacturerName = data.ReadUcs2(),
                    FixtureName = data.ReadUcs2(),
                    ModeName = data.ReadUcs2(),
                    ChannelCount = data.ReadUInt16(),
                    IsDimmer = data.ReadBoolean(),
                    Links = ReadLinks(data),
                    IsPatched = data.ReadBoolean(),
                    DMXUniverse = data.ReadByte(),
                    DMXAddress = data.ReadUInt16(),
                    Unit = data.ReadUcs2(),
                    Channel = data.ReadUInt16(),
                    Circuit = data.ReadUcs2(),
                    Note = data.ReadUcs2(),
                    Position = new Coordinate() { X = data.ReadSingle(), Y = data.ReadSingle(), Z = data.ReadSingle() },
                    Angle = new Coordinate() { X = data.ReadSingle(), Y = data.ReadSingle(), Z = data.ReadSingle() }
                };
                Fixtures.Add(information);
            }
        }

        public List<FixtureLink> ReadLinks(CitpBinaryReader data)
        {
            List<FixtureLink> links = new List<FixtureLink>();

            int count = data.ReadByte();
            for (int n = 0; n < count;n++)
            {
                FixtureLink link = new FixtureLink()
                {
                    Type = (FixtureLinkType) data.ReadByte()
                };

                int linkDataSize = data.ReadUInt16();
                link.Data = data.ReadBytes(linkDataSize);
                links.Add(link);
            }

            return links;
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte)Type);
            data.Write((UInt16)Fixtures.Count);
            
            foreach(FixtureInformation fixture in Fixtures)
            {
                data.Write(fixture.FixtureId);
                data.WriteUcs2(fixture.ManufacturerName);
                data.WriteUcs2(fixture.FixtureName);
                data.WriteUcs2(fixture.ModeName);
                data.Write(fixture.ChannelCount);
                data.Write(fixture.IsDimmer);
                data.Write((byte) fixture.Links.Count);
                foreach(FixtureLink link in fixture.Links)
                {
                    data.Write((byte)link.Type);
                    data.Write((UInt16) link.Data.Length);
                    data.Write(link.Data);
                };
                data.Write(fixture.IsPatched);
                data.Write(fixture.DMXUniverse);
                data.Write(fixture.DMXAddress);
                data.WriteUcs2(fixture.Unit);
                data.Write(fixture.Channel);
                data.WriteUcs2(fixture.Circuit);
                data.WriteUcs2(fixture.Note);
                data.Write(fixture.Position.X);
                data.Write(fixture.Position.Y);
                data.Write(fixture.Position.Z);
                data.Write(fixture.Angle.X);
                data.Write(fixture.Angle.Y);
                data.Write(fixture.Angle.Z);
            }
        }

        #endregion
    }
}

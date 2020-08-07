using LXProtocols.Citp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Citp.Packets.CaEx
{
    public class CaExFixtureModify : CaExHeader
    {
         #region Setup and Initialisation

        public CaExFixtureModify()
            : base(CaExContentCodes.FixtureModify)
        {
        }

        #endregion

        #region Fixture Modify

        [Flags]
        public enum Modification
        {
            Patch = 0x1,
            UnitNumber = 0x2,
            Channel = 0x4,
            Circuit = 0x8,
            Note = 0x10,
            Position = 0x20
        }

        public class FixtureChange
        {
            public uint FixtureId;
            public Modification ChangedFields;
            public bool Patched;
            public byte DMXUniverse;
            public ushort DMXAddress;
            public string Unit = string.Empty;
            public ushort Channel;
            public string Circuit = string.Empty;
            public string Note = string.Empty;
            public Coordinate Position = new Coordinate();
            public Coordinate Angle= new Coordinate();
        }

        #endregion

        #region Packet Content

        private List<FixtureChange> fixtures = new List<FixtureChange>();
        public List<FixtureChange> Fixtures
        {
            get { return fixtures; }
            set { fixtures = value; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            int count = data.ReadUInt16();
            for (int n = 0; n < count; n++)
            {
                FixtureChange change = new FixtureChange()
                {
                    FixtureId = data.ReadUInt32(),
                    ChangedFields = (Modification)data.ReadByte(),
                    Patched = data.ReadBoolean(),
                    DMXUniverse = data.ReadByte(),
                    DMXAddress = data.ReadUInt16(),
                    Unit = data.ReadUcs2(),
                    Channel = data.ReadUInt16(),
                    Circuit = data.ReadUcs2(),
                    Note = data.ReadUcs2(),
                    Position = new Coordinate() { X = data.ReadSingle(), Y = data.ReadSingle(), Z = data.ReadSingle() },
                    Angle = new Coordinate() { X = data.ReadSingle(), Y = data.ReadSingle(), Z = data.ReadSingle() }
                };
                Fixtures.Add(change);
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((ushort)Fixtures.Count);
            foreach (FixtureChange change in Fixtures)
            {
                data.Write(change.FixtureId);
                data.Write((byte) change.ChangedFields);
                data.Write(change.Patched);
                data.Write(change.DMXUniverse);
                data.Write(change.DMXAddress);
                data.WriteUcs2(change.Unit);
                data.Write(change.Channel);
                data.WriteUcs2(change.Circuit);
                data.WriteUcs2(change.Note);

                data.Write(change.Position.X);
                data.Write(change.Position.Y);
                data.Write(change.Position.Z);

                data.Write(change.Angle.X);
                data.Write(change.Angle.Y);
                data.Write(change.Angle.Z);

            }
        }

        #endregion
    }
}

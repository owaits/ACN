using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.FSel
{
    public class FSelSelect:FSelHeader
    {
        public const string PacketType = "Sele";

        #region Setup and Initialisation

        public FSelSelect()
            : base(PacketType)
        {
        }

        #endregion

        #region Packet Content

        public bool Complete { get; set; }

        private List<ushort> fixtureIdentifiers = new List<ushort>();

        public List<ushort> FixtureIdentifiers
        {
            get { return fixtureIdentifiers; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            Complete = data.ReadBoolean();
            data.ReadByte();

            int fixtureCount = data.ReadUInt16();
            for (int n = 0; n < fixtureCount; n++)
                FixtureIdentifiers.Add(data.ReadUInt16());
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(Complete);
            data.Write((byte)0);

            data.Write((ushort)FixtureIdentifiers.Count);
            foreach (ushort id in FixtureIdentifiers)
                data.Write(id);
        }

        #endregion
    }
}

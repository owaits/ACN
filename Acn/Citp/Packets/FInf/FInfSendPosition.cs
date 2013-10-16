using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.FInf
{
    public class FInfSendPosition : FInfHeader
    {
        public const string PacketType = "SPos";

        #region Setup and Initialisation

        public FInfSendPosition()
            : base(PacketType)
        {
        }

        #endregion

        #region Packet Content

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

            int fixtureCount = data.ReadUInt16();
            for (int n = 0; n < fixtureCount; n++)
                FixtureIdentifiers.Add(data.ReadUInt16());
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((ushort)FixtureIdentifiers.Count);
            foreach (ushort id in FixtureIdentifiers)
                data.Write(id);
        }

        #endregion
    }
}

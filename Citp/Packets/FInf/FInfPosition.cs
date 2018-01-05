using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.FInf
{
    public struct FixturePosition
    {
        public ushort FixtureIdentifier { get; set; }

        public float PositionX { get; set; }

        public float PositionY { get; set; }

        public float PositionZ { get; set; }
    }

    public class FInfPosition : FInfHeader
    {
        public const string PacketType = "Posi";

        #region Setup and Initialisation

        public FInfPosition()
            : base(PacketType)
        {
        }

        #endregion

        #region Packet Content

        private List<FixturePosition> positions = new List<FixturePosition>();

        public List<FixturePosition> Positions
        {
            get { return positions; }
            set { positions = value; }
        }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            int positionCount = data.ReadUInt16();
            Positions.Clear();
            for (int n = 0; n < positionCount; n++)
            {
                FixturePosition position = new FixturePosition()
                {
                    FixtureIdentifier = data.ReadUInt16(),
                    PositionX = data.ReadSingle(),
                    PositionY = data.ReadSingle(),
                    PositionZ = data.ReadSingle()
                };

                Positions.Add(position);
            }
                
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((ushort)Positions.Count);
            foreach (FixturePosition position in Positions)
            {
                data.Write(position.FixtureIdentifier);
                data.Write(position.PositionX);
                data.Write(position.PositionY);
                data.Write(position.PositionZ);
            }                
        }

        #endregion
    }
}

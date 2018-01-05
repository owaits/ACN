using Citp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp.Packets.CaEx
{
    public class CaExFixtureSelection : CaExHeader
    {
         #region Setup and Initialisation

        public CaExFixtureSelection()
            : base(CaExContentCodes.FixtureSelection)
        {
        }

        #endregion

        #region Packet Content

        private List<uint> fixtureIds = new List<uint>();
        public List<uint> FixtureIds
        {
            get { return fixtureIds; }
            set { fixtureIds = value; }
        }

        #endregion

        #region Read/Write        

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            int count = data.ReadUInt16();
            for(int n=0;n<count;n++)
            {
                FixtureIds.Add(data.ReadUInt32());
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((ushort)FixtureIds.Count);
            foreach (uint id in FixtureIds)
                data.Write(id);
        }

        #endregion
    }
}

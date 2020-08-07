using LXProtocols.Citp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Citp.Packets.CaEx
{
    public class CaExFixtureConsoleStatus : CaExHeader
    {
         #region Setup and Initialisation

        public CaExFixtureConsoleStatus()
            : base(CaExContentCodes.FixtureConsoleStatus)
        {
        }

        #endregion

        #region FixtureStatus

        public struct FixtureStatus
        {
            public uint FixtureId;
            public bool Locked;
            public bool Clearable;
        }

        #endregion


        #region Packet Content

        private List<FixtureStatus> fixtures = new List<FixtureStatus>();

        public List<FixtureStatus> Fixtures
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
            Fixtures.Clear();
            for(int n=0;n<count;n++)
            {
                FixtureStatus status = new FixtureStatus()
                {
                    FixtureId = data.ReadUInt32(),
                    Locked = data.ReadBoolean(),
                    Clearable = data.ReadBoolean()
                };

                Fixtures.Add(status);
            }
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((ushort)Fixtures.Count);

            foreach(FixtureStatus status in Fixtures)
            {
                data.Write(status.FixtureId);
                data.Write(status.Locked);
                data.Write(status.Clearable);
            }
        }

        #endregion
    }
}

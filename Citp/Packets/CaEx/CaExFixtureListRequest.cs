using LXProtocols.Citp.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Citp.Packets.CaEx
{
    public class CaExFixtureListRequest : CaExHeader
    {
        #region Setup and Initialisation

        public CaExFixtureListRequest()
            : base(CaExContentCodes.FixtureListRequest)
        {
        }

        #endregion

        #region Packet Content

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
        }

        #endregion
    }
}

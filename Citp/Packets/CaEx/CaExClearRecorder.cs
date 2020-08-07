using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp.IO;
using LXProtocols.Citp;

namespace LXProtocols.Citp.Packets.CaEx
{
    public class CaExClearRecorder : CaExHeader
    {
        #region Setup and Initialisation

        public CaExClearRecorder()
            : base(CaExContentCodes.ClearRecorder)
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

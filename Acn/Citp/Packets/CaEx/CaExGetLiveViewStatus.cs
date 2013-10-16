using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp.IO;

namespace Citp.Packets.CaEx
{
    public class CaExGetLiveViewStatus:CaExHeader
    {
        #region Setup and Initialisation

        public CaExGetLiveViewStatus()
            : base(CaExContentCodes.GetLiveViewStatus)
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

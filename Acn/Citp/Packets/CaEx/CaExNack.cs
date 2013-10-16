using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citp;
using Citp.IO;

namespace Citp.Packets.CaEx
{
    public class CaExNack:CaExHeader
    {
        #region Setup and Initialisation

        public CaExNack()
            : base(CaExContentCodes.Nack)
        {    
        }

        #endregion

        #region Packet Content

        public byte Reason { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            Reason = data.ReadByte();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write(Reason);
        }

        #endregion
    }
}

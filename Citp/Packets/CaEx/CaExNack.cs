using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Packets.CaEx
{
    public enum CaExNackReason
    {
        UnknownRequest = 0,
        MalformedPacket = 1,
        InternalError = 2,
        RequestRefused = 3
    }

    public class CaExNack:CaExHeader
    {
        #region Setup and Initialisation

        public CaExNack()
            : base(CaExContentCodes.Nack)
        {    
        }

        #endregion

        #region Packet Content

        public CaExNackReason Reason { get; set; }

        #endregion

        #region Read/Write

        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);
            Reason = (CaExNackReason) data.ReadByte();
        }

        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((byte) Reason);
        }

        #endregion
    }
}

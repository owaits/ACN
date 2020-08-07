using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Acn.Packets.Sdt
{
    public class StdDisconnecting : AcnPdu
    {
        public StdDisconnecting()
            : base((int) StdVectors.Disconnecting,1)
        {
        }

        #region Packet Contents

        public int ProtocolId { get; set; }
        
        public ReasonCodes Reason { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            ProtocolId = data.ReadOctet4();
            Reason = (ReasonCodes)data.ReadByte();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.WriteOctet(ProtocolId);
            data.Write((byte)Reason);
        }

        #endregion
    }
}

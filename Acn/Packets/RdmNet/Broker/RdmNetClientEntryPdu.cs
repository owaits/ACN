using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public enum RdmNetClientEntryProtocolIds
    {
        RPT = 0x5,
        EPT = 0xB
    }


    public class RdmNetClientEntryPdu : AcnPdu
    {
        public RdmNetClientEntryPdu(RdmNetClientEntryProtocolIds protocolId)
                    : base((int)protocolId,4)
        {
            Flags = PduFlags.Extended;
        }

        #region PDU Contents

        public Guid ClientId { get; set; }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            ClientId = new Guid(data.ReadBytes(16));
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(ClientId.ToByteArray());
        }

        #endregion
    }
}

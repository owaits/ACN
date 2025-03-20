using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using System.IO;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.Packets.RdmNet.LLRP
{
    public enum LLRPProtocolIds
    {
        ProbeRequest = 1,
        ProbeReply = 2,
        RDMCommand = 3
    }

    public class LLRPPdu : AcnPdu
    {
        public LLRPPdu(LLRPProtocolIds protocolId)
            : base((int)protocolId, 4)
        {
            Flags = PduFlags.Extended;
        }

        #region PDU Contents

        public Guid DestinationId { get; set; }

        public int SequenceNumber { get; set; }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            DestinationId = data.ReadCID();
            SequenceNumber = data.ReadOctet4();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(DestinationId);
            data.WriteOctet(SequenceNumber);
        }

        #endregion

    }
}

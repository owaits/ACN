using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.LLRP
{
    public class LLRPProbeRequestPacket : AcnPacket
    {
        public LLRPProbeRequestPacket()
          : base(ProtocolIds.LLRP)
        {
            //All RDM net packets have flags set to F. This results in a 3 byte Flags and Header.
            Root.Flags = PduFlags.Extended;
        }

        #region Packet Contents

        private LLRPPdu llrp = new LLRPPdu(LLRPProtocolIds.ProbeRequest);

        public LLRPPdu LLRP
        {
            get { return llrp; }
        }

        private LLRPProbeRequestPdu probeRequest = new LLRPProbeRequestPdu();

        public LLRPProbeRequestPdu ProbeRequest
        {
            get { return probeRequest; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            LLRP.ReadPdu(data);
            ProbeRequest.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            LLRP.WritePdu(data);
            ProbeRequest.WritePdu(data);
            ProbeRequest.WriteLength(data);
            LLRP.WriteLength(data);
        }

        #endregion
    }
}

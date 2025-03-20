using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.LLRP
{

    public class LLRPProbeReplyPacket : AcnPacket
    {
        public LLRPProbeReplyPacket()
            : base(ProtocolIds.LLRP)
        {
            //All RDM net packets have flags set to F. This results in a 3 byte Flags and Header.
            Root.Flags = PduFlags.Extended;
        }

        #region Packet Contents

        private LLRPPdu llrp = new LLRPPdu(LLRPProtocolIds.ProbeReply);

        public LLRPPdu LLRP
        {
            get { return llrp; }
        }

        private LLRPProbeReplyPdu probeReply = new LLRPProbeReplyPdu();

        public LLRPProbeReplyPdu ProbeReply
        {
            get { return probeReply; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            LLRP.ReadPdu(data);
            ProbeReply.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            LLRP.WritePdu(data);
            ProbeReply.WritePdu(data);
            ProbeReply.WriteLength(data);
            LLRP.WriteLength(data);
        }

        #endregion
    }
}

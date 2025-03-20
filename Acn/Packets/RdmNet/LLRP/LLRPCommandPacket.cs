using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet.RPT;
using LXProtocols.Acn.Rdm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.LLRP
{
    public class LLRPCommandPacket : AcnPacket
    {
        public LLRPCommandPacket()
            : base(ProtocolIds.LLRP)
        {
            //All RDM net packets have flags set to F. This results in a 3 byte Flags and Header.
            Root.Flags = PduFlags.Extended;
        }

        #region Packet Contents

        private LLRPPdu llrp = new LLRPPdu(LLRPProtocolIds.RDMCommand);

        public LLRPPdu LLRP
        {
            get { return llrp; }
        }

        private LLRPCommandPdu command = new LLRPCommandPdu();

        public LLRPCommandPdu Command
        {
            get { return command; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            LLRP.ReadPdu(data);
            Command.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            LLRP.WritePdu(data);
            Command.WritePdu(data);
            Command.WriteLength(data);
            LLRP.WriteLength(data);
        }

        #endregion
    }
}

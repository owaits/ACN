using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public class RdmNetRptRequestPacket : AcnPacket
    {
        public RdmNetRptRequestPacket()
            : base(ProtocolIds.RdmPacketTransfer)
        {
            //All RDM net packets have flags set to F. This results in a 3 byte Flags and Header.
            Root.Flags = PduFlags.Extended;
        }

        #region Packet Contents

        private RdmNetRptPdu rpt = new RdmNetRptPdu(RdmNetRptProtocolIds.Request);

        public RdmNetRptPdu Rpt
        {
            get { return rpt; }
        }

        private RdmNetRptMessagePdu message = new RdmNetRptMessagePdu(RdmNetRptMessageProtocolIds.RdmCommend);

        public RdmNetRptMessagePdu Message
        {
            get { return message; }
        }

        private RdmNetCommandPdu request = new RdmNetCommandPdu(DmxStartCodes.RDM);

        public RdmNetCommandPdu Request
        {
            get { return request; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Rpt.ReadPdu(data);
            Message.ReadPdu(data);
            Request.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Rpt.WritePdu(data);
            Message.WritePdu(data);
            Request.WritePdu(data);
            Request.WriteLength(data);
            Message.WriteLength(data);
            Rpt.WriteLength(data);
        }

        #endregion
    }
}

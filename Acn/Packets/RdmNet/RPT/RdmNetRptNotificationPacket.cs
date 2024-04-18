using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public class RdmNetRptNotificationPacket : AcnPacket
    {
        public RdmNetRptNotificationPacket()
            : base(ProtocolIds.RdmPacketTransfer)
        {
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

        private RdmNetCommandPdu data = new RdmNetCommandPdu(RdmNetCommandProtocolId.RdmData);

        public RdmNetCommandPdu Data
        {
            get { return data; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Rpt.ReadPdu(data);
            Message.ReadPdu(data);
            Data.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Rpt.WritePdu(data);
            Message.WritePdu(data);
            Data.WritePdu(data);
            Data.WriteLength(data);
            Message.WriteLength(data);
            Rpt.WriteLength(data);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public class RdmNetRptStatusPacket : AcnPacket
    {
        public RdmNetRptStatusPacket()
            : base(ProtocolIds.RdmPacketTransfer)
        {
        }

        public RdmNetRptStatusPacket(RdmNetStatusProtocolId status)
            : base(ProtocolIds.RdmPacketTransfer)
        {
            this.status = new RdmNetStatusPdu(status);
        }

        #region Packet Contents

        private RdmNetRptPdu rpt = new RdmNetRptPdu(RdmNetRptProtocolIds.Status);

        public RdmNetRptPdu Rpt
        {
            get { return rpt; }
        }

        private RdmNetStatusPdu status = new RdmNetStatusPdu(RdmNetStatusProtocolId.None);

        public RdmNetStatusPdu Status
        {
            get { return status; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Rpt.ReadPdu(data);
            Status.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Rpt.WritePdu(data);
            Status.WritePdu(data);
            Status.WriteLength(data);
            Rpt.WriteLength(data);
        }

        #endregion
    }
}

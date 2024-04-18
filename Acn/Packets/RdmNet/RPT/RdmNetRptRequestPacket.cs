using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public class RdmNetRptRequestPacket : AcnPacket
    {
        public RdmNetRptRequestPacket()
            : base(ProtocolIds.RdmPacketTransfer)
        {
        }

        #region Packet Contents

        private RdmNetRptPdu rpt = new RdmNetRptPdu(RdmNetRptProtocolIds.Notification);

        public RdmNetRptPdu Rpt
        {
            get { return rpt; }
        }

        private RdmNetCommandPdu request = new RdmNetCommandPdu(RdmNetCommandProtocolId.RdmData);

        public RdmNetCommandPdu Request
        {
            get { return request; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Rpt.ReadPdu(data);
            Request.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Rpt.WritePdu(data);
            Request.WritePdu(data);
            Request.WriteLength(data);
            Rpt.WriteLength(data);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet;
using LXProtocols.Acn.Rdm;

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

        public List<RdmNetCommandPdu> Data { get; protected set; } = new List<RdmNetCommandPdu>();

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Rpt.ReadPdu(data);
            Message.ReadPdu(data);

            int commandPosition = 0;
            while(commandPosition < (Message.Length - 7))
            {
                RdmNetCommandPdu command = new RdmNetCommandPdu(DmxStartCodes.RDM);
                command.ReadPdu(data);
                Data.Add(command);

                commandPosition += command.Length;
            }
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Rpt.WritePdu(data);
            Message.WritePdu(data);
            foreach(var command in Data)
            {
                command.WritePdu(data);
                command.WriteLength(data);
            }
            Message.WriteLength(data);
            Rpt.WriteLength(data);
        }

        #endregion
    }
}

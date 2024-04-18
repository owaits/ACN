using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using System.IO;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public enum RdmNetStatusProtocolId
    {
        None = 0,
        UnknownRPTUID = 1,
        RDMTimeout = 2,
        RDMInvalidResponse = 3,
        UnknownRdmUID = 4,
        UnknownEndpoint = 5,
        BroadcastCOMPLETE = 6,
        UnknownVector = 7,
        InvalidMessage = 8,
        InvalidCommandClass = 9
    }

    public class RdmNetStatusPdu:AcnPdu
    {
        public RdmNetStatusPdu(RdmNetStatusProtocolId protocolId)
            : base((int)protocolId, 2)
        {
        }

        #region PDU Contents

        public string Status { get; set; }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Status = data.ReadUtf8String(Length - 4);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.WriteUtf8String(Status, Status.Length);
        }

        #endregion

    }
}

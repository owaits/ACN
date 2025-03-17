using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using System.IO;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public enum RdmNetStatusCodes
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
        public RdmNetStatusPdu(RdmNetStatusCodes protocolId)
            : base((int)protocolId, 2)
        {
            Flags = PduFlags.Extended;
        }

        #region PDU Contents

        /// <summary>
        /// Gets or sets the status code or error code defined by <see cref="RdmNetStatusCodes"/>
        /// </summary>
        public RdmNetStatusCodes StatusCode 
        {
            get { return (RdmNetStatusCodes) Vector; }
            set { Vector = (int)value; }
        }

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

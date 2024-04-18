using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using System.IO;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public enum RdmNetRptProtocolIds
    {
        Request = 1,
        Status = 2,
        Notification = 3
    }

    public class RdmNetRptPdu : AcnPdu
    {
        public RdmNetRptPdu(RdmNetRptProtocolIds protocolId)
            : base((int)protocolId)
        {
        }

        #region PDU Contents

        public UId SourceId { get; set; }

        public short SourceEndpointId { get; set; }

        public UId DestinationId { get; set; }

        public short DestinationEndpointId { get; set; }

        public int SequenceNumber { get; set; }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            SourceId = data.ReadUId();
            SourceEndpointId = data.ReadOctet2();
            DestinationId = data.ReadUId();
            DestinationEndpointId = data.ReadOctet2();
            SequenceNumber = data.ReadOctet4();
            data.BaseStream.Seek(1, SeekOrigin.Current);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(SourceId);
            data.WriteOctet(SourceEndpointId);
            data.Write(DestinationId);
            data.WriteOctet(DestinationEndpointId);
            data.WriteOctet(SequenceNumber);
            data.BaseStream.Seek(1, SeekOrigin.Current);
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;
using System.IO;

namespace Acn.Packets.RdmNet
{
    public enum RdmNetProtocolIds
    {
        RdmNet,
        Status,
        Controller,
        ChangeNotification
    }

    public class RdmNetFramingPdu:AcnPdu
    {
        public RdmNetFramingPdu(RdmNetProtocolIds protocolId)
            : base((int) protocolId)
        {
        }

        #region PDU Contents

        private string sourceName = string.Empty;

        public string SourceName
        {
            get { return sourceName; }
            set { sourceName = value; }
        }

        private int sequenceNumber = 0;

        public int SequenceNumber
        {
            get { return sequenceNumber; }
            set { sequenceNumber = value; }
        }

        private short endpointID = 0;

        public short EndpointID
        {
            get { return endpointID; }
            set { endpointID = value; }
        }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            SourceName = data.ReadUtf8String(64);
            SequenceNumber = data.ReadOctet4();
            EndpointID = data.ReadOctet2();
            data.BaseStream.Seek(1, SeekOrigin.Current);            
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.WriteUtf8String(SourceName,64);
            data.WriteOctet(SequenceNumber);
            data.WriteOctet(EndpointID);
            data.BaseStream.Seek(1, SeekOrigin.Current);
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;
using System.IO;
using Acn.Rdm;

namespace Acn.Packets.RdmNet
{
    public class RdmNetStatusPdu:AcnPdu
    {
        public RdmNetStatusPdu()
            : base((int)DmxStartCodes.RDM)
        {
        }

        #region PDU Contents

        private short statusCode = 0;

        public short StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }

        private string statusMessage = string.Empty;

        public string StatusMessage
        {
            get { return statusMessage; }
            set { statusMessage = value; }
        }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            StatusCode = data.ReadOctet2();
            StatusMessage = data.ReadUtf8String(Length - 4);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.WriteOctet(StatusCode);
            data.WriteUtf8String(StatusMessage,StatusMessage.Length);
        }

        #endregion

    }
}

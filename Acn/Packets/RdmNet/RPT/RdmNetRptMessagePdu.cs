using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using System.IO;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public enum RdmNetRptMessageProtocolIds
    {
        RdmCommend = 1
    }

    public class RdmNetRptMessagePdu : AcnPdu
    {
        public RdmNetRptMessagePdu(RdmNetRptMessageProtocolIds protocolId)
            : base((int)protocolId,4)
        {
        }

        #region PDU Contents

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
        }

        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using System.IO;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public class RdmNetCommandPdu:AcnPdu
    {
        public RdmNetCommandPdu(DmxStartCodes startCode)
            : base((int)startCode, 1)
        {
            Flags = PduFlags.Extended;
        }

        #region PDU Contents

        private byte[] rdmData = null;

        public byte[] RdmData
        {
            get { return rdmData; }
            set { rdmData = value; }
        }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            RdmData = data.ReadBytes(Length - 4);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            if(RdmData != null)
                data.Write(RdmData);
        }

        #endregion

    }
}

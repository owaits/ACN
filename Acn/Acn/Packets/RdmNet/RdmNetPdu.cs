using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;
using System.IO;
using Acn.Rdm;

namespace Acn.Packets.RdmNet
{
    public class RdmNetPdu:AcnPdu
    {
        public RdmNetPdu()
            : base((int)DmxStartCodes.RDM,1)
        {
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
            RdmData = data.ReadBytes(Length - 3);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            if(RdmData != null)
                data.Write(RdmData);
        }

        #endregion

    }
}

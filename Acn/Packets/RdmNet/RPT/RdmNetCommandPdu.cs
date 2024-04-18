﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;
using System.IO;
using LXProtocols.Acn.Rdm;

namespace LXProtocols.Acn.Packets.RdmNet.RPT
{
    public enum RdmNetCommandProtocolId
    {
        RdmData = 1
    }

    public class RdmNetCommandPdu:AcnPdu
    {
        public RdmNetCommandPdu(RdmNetCommandProtocolId protocolId)
            : base((int)protocolId, 1)
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

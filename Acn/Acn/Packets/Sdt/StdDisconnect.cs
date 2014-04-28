﻿using Acn.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Packets.Sdt
{
    public class StdDisconnect : AcnPdu
    {
        public StdDisconnect()
            : base((int) StdVectors.Disconnect,1)
        {
        }

        #region Packet Contents

        public int ProtocolId { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            ProtocolId = data.ReadOctet4();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.WriteOctet(ProtocolId);
        }

        #endregion
    }
}

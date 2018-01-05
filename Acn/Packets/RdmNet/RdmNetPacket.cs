using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;
using Acn.Packets.RdmNet;

namespace Acn.Packets.sAcn
{
    public class RdmNetPacket : AcnPacket
    {
        public RdmNetPacket()
            : base(ProtocolIds.RdmNet)
        {
        }

        #region Packet Contents

        private RdmNetFramingPdu framing = new RdmNetFramingPdu(RdmNetProtocolIds.RdmNet);

        public RdmNetFramingPdu Framing
        {
            get { return framing; }
        }

        private RdmNetPdu rdmNet = new RdmNetPdu();

        public RdmNetPdu RdmNet
        {
            get { return rdmNet; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Framing.ReadPdu(data);
            RdmNet.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Framing.WritePdu(data);
            RdmNet.WritePdu(data);
            RdmNet.WriteLength(data);
            Framing.WriteLength(data);
        }

        #endregion
    }
}

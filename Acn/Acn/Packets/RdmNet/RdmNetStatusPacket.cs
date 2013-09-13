using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;
using Acn.Packets.RdmNet;

namespace Acn.Packets.sAcn
{
    public class RdmNetStatusPacket : AcnPacket
    {
        public RdmNetStatusPacket()
            : base(ProtocolIds.RdmNet)
        {
        }

        #region Packet Contents

        private RdmNetFramingPdu framing = new RdmNetFramingPdu(RdmNetProtocolIds.Status);

        public RdmNetFramingPdu Framing
        {
            get { return framing; }
        }

        private RdmNetStatusPdu rdmNet = new RdmNetStatusPdu();

        public RdmNetStatusPdu RdmNet
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

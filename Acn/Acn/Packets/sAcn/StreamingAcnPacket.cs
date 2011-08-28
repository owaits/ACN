using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;
using System.IO;
using Acn.Packets.sAcn;
using Acn.Packets.Dmp;

namespace Acn.Packets.sAcn
{
    public abstract class StreamingAcnPacket : AcnPacket
    {
        public StreamingAcnPacket(ProtocolIds protocolId)
            : base(protocolId)
        {
        }

        #region Packet Contents

        private FramingPdu framing = new FramingPdu();

        public FramingPdu Framing
        {
            get { return framing; }
        }

        private DmpSetProperty dmx = new DmpSetProperty();

        public DmpSetProperty Dmx
        {
            get { return dmx; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Framing.ReadPdu(data);
            Dmx.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Framing.WritePdu(data);
            Dmx.WritePdu(data);
        }

        #endregion
    }
}

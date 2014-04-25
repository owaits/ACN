using Acn.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Packets.sAcn
{
    public class StreamingAcnSynchronizationPacket:AcnPacket
    {
        public StreamingAcnSynchronizationPacket()
            : base(ProtocolIds.sACN)
        {
        }

        #region Packet Contents

        private StreamingAcnFramingPdu framing = new StreamingAcnFramingPdu();

        public StreamingAcnFramingPdu Framing
        {
            get { return framing; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Framing.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Framing.WritePdu(data);
            Framing.WriteLength(data);
        }

        #endregion
    }
}

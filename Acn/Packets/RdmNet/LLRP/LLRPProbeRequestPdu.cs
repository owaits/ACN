using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet.RPT;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.RdmNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.LLRP
{
    public class LLRPProbeRequestPdu : AcnPdu
    {
        public LLRPProbeRequestPdu()
           : base((int)1, 4)
        {
            Flags = PduFlags.Extended;
        }

        #region PDU Contents

        public UId LowerIdBound { get; set; }

        public UId UpperIdBound { get; set; }

        public short Filter { get; set; }

        public List<UId> KnownUIDs { get; set; } = new List<UId>();

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            LowerIdBound = data.ReadUId();
            UpperIdBound = data.ReadUId();
            Filter = data.ReadInt16();

            for (int pos = 14; pos < Length; pos += 6)
            {
                KnownUIDs.Add(data.ReadUId());
            }
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(LowerIdBound);
            data.Write(UpperIdBound);
            data.Write(Filter);

            foreach(UId id in KnownUIDs)
                data.Write(id);
        }

        #endregion
    }
}

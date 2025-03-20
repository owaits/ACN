using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet.RPT;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.RdmNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.LLRP
{
    public enum LLRPComponentType : byte
    {
        RPTDevice = 0,
        RPTController = 1,
        Broker = 2,
        NonRDMNet = 0xFF
    }

    public class LLRPProbeReplyPdu : AcnPdu
    {
        public LLRPProbeReplyPdu()
           : base((int)1, 1)
        {
            Flags = PduFlags.Extended;
        }

        #region PDU Contents

        public UId TargetId { get; set; }

        public byte[] HardwareAddress { get; set; }

        public LLRPComponentType ComponentType { get; set; }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            TargetId = data.ReadUId();
            HardwareAddress = data.ReadBytes(6);
            ComponentType = (LLRPComponentType)data.ReadByte();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(TargetId);
            data.Write(HardwareAddress);
            data.Write((byte)ComponentType);
        }

        #endregion
    }
}

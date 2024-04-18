using LXProtocols.Acn.IO;
using LXProtocols.Acn.Rdm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public enum RPTClientType
    {
        Device,
        Controller
    }

    public class RdmNetRPTClientEntryPdu : RdmNetClientEntryPdu
    {
        public RdmNetRPTClientEntryPdu()
                    : base(RdmNetClientEntryProtocolIds.RPT)
        {
        }

        #region PDU Contents

        public UId ClientUId { get; set; }

        public RPTClientType ClientType { get; set; }

        public Guid BindingId { get; set; } = Guid.Empty;

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            base.ReadData(data);

            ClientUId = data.ReadUId();
            ClientType = (RPTClientType)data.ReadByte();
            BindingId =  new Guid(data.ReadBytes(16));
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(ClientUId);
            data.Write((byte) ClientType);
            data.Write(BindingId.ToByteArray());
        }

        #endregion
    }
}

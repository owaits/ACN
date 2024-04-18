using LXProtocols.Acn.IO;
using LXProtocols.Acn.Rdm;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public enum BrokerConnectionCode
    {
        OK = 0x0000,
        ScopeMismatch = 0x0001,
        Exceeded = 0x0002,
        DuplicateUID = 0x0003,
        InvalidClientEntry = 0x0004,
        InvalidUID = 0x0005
    }

    public class RdmNetBrokerConnectReplyPacket : RdmNetBrokerPacket
    {
        public RdmNetBrokerConnectReplyPacket() : base(RdmNetBrokerProtocolIds.ConnectReply)
        {
        }

        #region Packet Contents

        public BrokerConnectionCode ConnectionCode { get; set; }

        public short E133Version { get; set; }

        public UId BrokerUId { get; set; }

        public UId ClientUId { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Broker.ReadPdu(data);

            ConnectionCode = (BrokerConnectionCode) data.ReadOctet2();
            E133Version = data.ReadOctet2();
            BrokerUId = data.ReadUId();
            ClientUId = data.ReadUId();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Broker.WritePdu(data);

            data.WriteOctet((short) ConnectionCode);
            data.WriteOctet(E133Version);
            data.Write(BrokerUId);
            data.Write(ClientUId);

            Broker.WriteLength(data);
        }

        #endregion
    }
}

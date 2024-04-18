using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    [Flags]
    public enum BrokerConnectionFlags
    {
        IncrementalUpdates = 1
    }

    public class RdmNetBrokerConnectPacket : RdmNetBrokerPacket
    {
        public RdmNetBrokerConnectPacket():base(RdmNetBrokerProtocolIds.Connect)
        {
        }

        #region Packet Contents

        public string ClientScope { get; set; }

        public short E133Version { get; set; }

        public string SearchDomain { get; set; }

        public BrokerConnectionFlags ConnectionFlags { get; set; }

        public RdmNetClientEntryPdu Client { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Broker.ReadPdu(data);

            ClientScope = data.ReadUtf8String(63);
            E133Version = data.ReadOctet2();
            SearchDomain = data.ReadUtf8String(231);
            ConnectionFlags = (BrokerConnectionFlags) data.ReadByte();
            Client.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Broker.WritePdu(data);

            data.WriteUtf8String(ClientScope, 63);
            data.WriteOctet(E133Version);
            data.WriteUtf8String(SearchDomain, 231);
            data.Write((byte) ConnectionFlags);
            Client.WritePdu(data);
            Client.WriteLength(data);

            Broker.WriteLength(data);
        }

        #endregion
    }
}

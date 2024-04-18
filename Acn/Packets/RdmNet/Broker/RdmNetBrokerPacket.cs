using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.sAcn;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public abstract class RdmNetBrokerPacket : AcnPacket
    {
        public RdmNetBrokerPacket() : base(ProtocolIds.Broker)
        {
            broker = new RdmNetBrokerPdu(RdmNetBrokerProtocolIds.NULL);
        }

        public RdmNetBrokerPacket(RdmNetBrokerProtocolIds brokerProtocolId) : base(ProtocolIds.Broker)
        {
            broker = new RdmNetBrokerPdu(brokerProtocolId);

            //All RDM net packets have flags set to F. This results in a 3 byte Flags and Header.
            Root.Flags = 0xF;
        }


        #region Packet Contents

        private RdmNetBrokerPdu broker;

        public RdmNetBrokerPdu Broker
        {
            get { return broker; }
        }

        #endregion
    }
}

using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    /// <summary>
    /// A Null message contains no data and is sent by a Component in order to maintain the health of its TCP connection in the event
    /// that no other message has been sent on the connection in the last E133_TCP_HEARTBEAT_INTERVAL
    /// </summary>
    /// <seealso cref="LXProtocols.Acn.Packets.RdmNet.Broker.RdmNetBrokerPacket" />
    public class RdmNetBrokerNullPacket : RdmNetBrokerPacket
    {
        public RdmNetBrokerNullPacket() : base(RdmNetBrokerProtocolIds.NULL)
        {
        }

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Broker.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Broker.WritePdu(data);
            Broker.WriteLength(data);
        }

        #endregion
    }
}

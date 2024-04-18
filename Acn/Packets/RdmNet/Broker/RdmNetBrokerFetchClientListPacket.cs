using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public class RdmNetBrokerFetchClientListPacket : RdmNetBrokerPacket
    {
        public RdmNetBrokerFetchClientListPacket() : base(RdmNetBrokerProtocolIds.FetchClientList)
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

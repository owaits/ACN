using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public class RdmNetBrokerClientEntryUpdatePacket : RdmNetBrokerPacket
    {
        public RdmNetBrokerClientEntryUpdatePacket():base(RdmNetBrokerProtocolIds.ClientEntryUpdate)
        {
        }

        #region Packet Contents

        public BrokerConnectionFlags ConnectionFlags { get; set; }

        public RdmNetClientEntryPdu Client { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Broker.ReadPdu(data);

            ConnectionFlags = (BrokerConnectionFlags) data.ReadByte();
            Client.ReadPdu(data);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Broker.WritePdu(data);

            data.Write((byte) ConnectionFlags);
            Client.WritePdu(data);
            Client.WriteLength(data);

            Broker.WriteLength(data);
        }

        #endregion
    }
}

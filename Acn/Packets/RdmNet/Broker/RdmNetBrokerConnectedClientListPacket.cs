using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public class RdmNetBrokerConnectedClientListPacket : RdmNetBrokerPacket
    {
        public RdmNetBrokerConnectedClientListPacket() : base(RdmNetBrokerProtocolIds.ConnectedClientList)
        {
        }

        #region Packet Contents

        public List<RdmNetClientEntryPdu> Clients { get; set; } = new List<RdmNetClientEntryPdu>();

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Broker.ReadPdu(data);

            for(int pos=6;pos< Broker.Length; pos+=45)
            {
                //TODO: Make this dynamic based on the RPT or EPT type.
                RdmNetRPTClientEntryPdu client = new RdmNetRPTClientEntryPdu();
                client.ReadPdu(data);
                Clients.Add(client);
            }
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Broker.WritePdu(data);

            foreach(var client in Clients)
            {
                client.WritePdu(data);
                client.WriteLength(data);
            }

            Broker.WriteLength(data);
        }

        #endregion
    }
}

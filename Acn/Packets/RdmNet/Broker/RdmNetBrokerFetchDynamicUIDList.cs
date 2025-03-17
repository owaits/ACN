using LXProtocols.Acn.IO;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.RdmNet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public class RdmNetBrokerFetchDynamicUIDListPacket : RdmNetBrokerPacket
    {
        public RdmNetBrokerFetchDynamicUIDListPacket() : base(RdmNetBrokerProtocolIds.FetchDynamicUIDList)
        {
        }

        #region Packet Contents

        public List<UId> DynamicIds { get; set; } = new List<UId>();

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Broker.ReadPdu(data);

            for(int pos=6;pos< Broker.Length; pos+=6)
            {
                UId id = data.ReadUId();
                DynamicIds.Add(id);
            }
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Broker.WritePdu(data);

            foreach(var id in DynamicIds)
            {
                data.Write(id);
            }

            Broker.WriteLength(data);
        }

        #endregion
    }
}

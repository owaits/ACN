using LXProtocols.Acn.IO;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.RdmNet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public class RdmNetBrokerAssignedDynamicUIDsPacket : RdmNetBrokerPacket
    {
        public RdmNetBrokerAssignedDynamicUIDsPacket() : base(RdmNetBrokerProtocolIds.AssignedDynamicUIDS)
        {
        }

        #region Packet Contents

        public List<DynamicUId> DynamicIds { get; set; } = new List<DynamicUId>();

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Broker.ReadPdu(data);

            for(int pos=6;pos< Broker.Length; pos+=22)
            {
                //TODO: Make this dynamic based on the RPT or EPT type.
                DynamicUId id = new DynamicUId();
                id.DynamicId = data.ReadUId();
                id.ResponderId = data.ReadUId();
                DynamicIds.Add(id);
            }
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Broker.WritePdu(data);

            foreach(var id in DynamicIds)
            {
                data.Write(id.DynamicId);
                data.Write(id.ResponderId);
            }

            Broker.WriteLength(data);
        }

        #endregion
    }
}

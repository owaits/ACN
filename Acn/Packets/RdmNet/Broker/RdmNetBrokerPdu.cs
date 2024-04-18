using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public enum RdmNetBrokerProtocolIds
    {
        Connect = 1,
        ConnectReply = 2,
        ClientEntryUpdate = 3,
        RedirectV4 = 4,
        RedirectV6 = 5,
        FetchClientList = 6,
        ConnectedClientList = 7,
        ClientAdd = 8,
        ClientRemove = 9,
        ClientEntryChange = 10,
        RequestDynamicUIDS = 11,
        AassignedDynamicUIDS = 12,
        FetchDynamicUIDList = 13,
        Disconnect = 14,
        NULL = 15
    }

    public class RdmNetBrokerPdu : AcnPdu
    {
        public RdmNetBrokerPdu(RdmNetBrokerProtocolIds protocolId)
            : base((int)protocolId, 2)
        {
            Flags = PduFlags.Extended;
        }

        #region PDU Contents

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
        }

        #endregion

    }
}

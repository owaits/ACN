using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    public class RdmNetBrokerRedirectV4Packet : RdmNetBrokerPacket
    {
        public RdmNetBrokerRedirectV4Packet() : base(RdmNetBrokerProtocolIds.RedirectV4)
        {
        }

        #region Packet Contents

        public IPAddress Address { get; set; }

        public short Port { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Broker.ReadPdu(data);

            Address = new IPAddress(data.ReadBytes(4));
            Port = data.ReadOctet2();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Broker.WritePdu(data);
            data.Write(Address.GetAddressBytes());
            data.WriteOctet(Port);
            Broker.WriteLength(data);
        }

        #endregion
    }
}

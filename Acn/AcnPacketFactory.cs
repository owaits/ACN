using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.Rdm.Packets.Net;
using LXProtocols.Acn.Packets.sAcn;
using LXProtocols.Acn.IO;
using LXProtocols.Acn.Packets.RdmNet.Broker;
using LXProtocols.Acn.Packets.RdmNet.RPT;

namespace LXProtocols.Acn
{
    public class AcnPacketFactory
    {
        static AcnPacketFactory()
        {
            AcnPduFactory e131ExtendedBuilder = new AcnPduFactory();
            e131ExtendedBuilder.RegisterPacketType((int) E131Extended.Synchronization,new AcnPacket.Builder<StreamingAcnSynchronizationPacket>());
            e131ExtendedBuilder.RegisterPacketType((int) E131Extended.Discovery,new AcnPacket.Builder<StreamingAcnDiscoveryPacket>());

            AcnPduFactory e133BrokerBuilder = new AcnPduFactory(2);
            e133BrokerBuilder.RegisterPacketType((int)RdmNetBrokerProtocolIds.Connect, new AcnPacket.Builder<RdmNetBrokerConnectPacket>());
            e133BrokerBuilder.RegisterPacketType((int)RdmNetBrokerProtocolIds.ConnectReply, new AcnPacket.Builder<RdmNetBrokerConnectReplyPacket>());
            e133BrokerBuilder.RegisterPacketType((int)RdmNetBrokerProtocolIds.FetchClientList, new AcnPacket.Builder<RdmNetBrokerFetchClientListPacket>());
            e133BrokerBuilder.RegisterPacketType((int)RdmNetBrokerProtocolIds.ConnectedClientList, new AcnPacket.Builder<RdmNetBrokerConnectedClientListPacket>());
            e133BrokerBuilder.RegisterPacketType((int)RdmNetBrokerProtocolIds.Disconnect, new AcnPacket.Builder<RdmNetBrokerDisconnectPacket>());
            e133BrokerBuilder.RegisterPacketType((int)RdmNetBrokerProtocolIds.NULL, new AcnPacket.Builder<RdmNetBrokerNullPacket>());

            AcnPduFactory e133RptBuilder = new AcnPduFactory(2);
            e133RptBuilder.RegisterPacketType((int)RdmNetRptProtocolIds.Request, new AcnPacket.Builder<RdmNetRptRequestPacket>());
            e133RptBuilder.RegisterPacketType((int)RdmNetRptProtocolIds.Status, new AcnPacket.Builder<RdmNetRptStatusPacket>());
            e133RptBuilder.RegisterPacketType((int)RdmNetRptProtocolIds.Notification, new AcnPacket.Builder<RdmNetRptNotificationPacket>());

            //Port List
            factory.RegisterPacketType((int)ProtocolIds.sACN, new AcnPacket.Builder<StreamingAcnDmxPacket>());
            factory.RegisterPacketType((int)ProtocolIds.sACNExtended, e131ExtendedBuilder);
            factory.RegisterPacketType((int)ProtocolIds.RdmPacketTransfer, new AcnPacket.Builder<RdmNetRptRequestPacket>());
            factory.RegisterPacketType((int)ProtocolIds.Broker, e133BrokerBuilder);
            factory.RegisterPacketType((int)ProtocolIds.RdmPacketTransfer, e133RptBuilder);
        }

        protected struct PacketKey
        {
            public PacketKey(int protocolId)
            {
                this.ProtocolId = protocolId;
            }

            public int ProtocolId;
        }

        private Dictionary<PacketKey, IPacketBuilder> packetStore = new Dictionary<PacketKey, IPacketBuilder>();

        public void RegisterPacketType(int protocolId, IPacketBuilder packetType)
        {
            PacketKey key = new PacketKey();
            key.ProtocolId = protocolId;

            packetStore[key] = packetType;
        }

        protected bool TryGetBuilder(PacketKey key, out IPacketBuilder builder)
        {
            return packetStore.TryGetValue(key, out builder);
        }

        private static AcnPacketFactory factory = new AcnPacketFactory();

        public static AcnPacket Build(AcnRootLayer header, AcnBinaryReader data)
        {
            IPacketBuilder packetType;
            if (factory.TryGetBuilder(new PacketKey(header.ProtocolId), out packetType))
            {
                AcnPacket packet = packetType.Create(header, data);
                return packet;
            }

            return null;
        }
    }

    /// <summary>
    /// Interface for all ACN packet builders.
    /// </summary>
    /// <remarks>
    /// Used by the packet factory to customise creation of different packet types.
    /// </remarks>
    public interface IPacketBuilder
    {
        /// <summary>
        /// Creates the specified root layer.
        /// </summary>
        /// <param name="rootLayer">The root layer.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        AcnPacket Create(AcnRootLayer rootLayer, AcnBinaryReader reader);
    }

    public class AcnPduFactory : AcnPacketFactory, IPacketBuilder
    {
        private int protocolVectorLength = 4;

        public AcnPduFactory(int protocolVectorLength = 4)
        {
            this.protocolVectorLength = protocolVectorLength;
        }

        /// <summary>
        /// This is a dummy PDU header used to read the PDU header to determine the PDU type.
        /// </summary>
        /// <remarks>
        /// This is only ever used to determine the PDU type in the factory.
        /// </remarks>
        /// <seealso cref="Acn.AcnPdu" />
        public class PduHeader : AcnPdu
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PduHeader"/> class.
            /// </summary>
            public PduHeader(int vectorLength)
                : base(0, vectorLength)
            {
                
            }

            /// <summary>
            /// Reads the PDU information from the recieved packet data.
            /// </summary>
            /// <param name="data">The recieved packet data.</param>
            protected override void ReadData(AcnBinaryReader data)
            {
            }

            /// <summary>
            /// Write the PDU information to the packet data to be transmitted.
            /// </summary>
            /// <param name="data">The packet data to be sent.</param>
            protected override void WriteData(AcnBinaryWriter data)
            {
            }
        }

        /// <summary>
        /// Creates the specified root layer.
        /// </summary>
        /// <param name="rootLayer">The root layer.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public AcnPacket Create(AcnRootLayer rootLayer, AcnBinaryReader reader)
        {
            long startPosition = reader.BaseStream.Position;

            PduHeader pduHeader = new PduHeader(protocolVectorLength);
            pduHeader.ReadPdu(reader);

            IPacketBuilder builder;
            if (TryGetBuilder(new PacketKey(pduHeader.Vector), out builder))
            {
                reader.BaseStream.Seek(startPosition, System.IO.SeekOrigin.Begin);
                return builder.Create(rootLayer, reader);
            }

            return null;
        }
    }
}

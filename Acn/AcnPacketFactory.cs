using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm.Packets.Net;
using Acn.Packets.sAcn;
using Acn.IO;

namespace Acn
{
    public class AcnPacketFactory
    {
        static AcnPacketFactory()
        {
            AcnPduFactory e131ExtendedBuilder = new AcnPduFactory();
            e131ExtendedBuilder.RegisterPacketType((int) E131Extended.Synchronization,new AcnPacket.Builder<StreamingAcnSynchronizationPacket>());
            e131ExtendedBuilder.RegisterPacketType((int) E131Extended.Discovery,new AcnPacket.Builder<StreamingAcnDiscoveryPacket>());

            //Port List
            factory.RegisterPacketType((int)ProtocolIds.sACN, new AcnPacket.Builder<StreamingAcnDmxPacket>());
            factory.RegisterPacketType((int)ProtocolIds.sACNExtended, e131ExtendedBuilder);
            factory.RegisterPacketType((int)ProtocolIds.RdmNet, new AcnPacket.Builder<RdmNetPacket>());
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
            public PduHeader()
                : base(0)
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

            PduHeader pduHeader = new PduHeader();
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

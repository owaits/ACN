using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LXProtocols.Acn.IO;

namespace LXProtocols.Acn
{
    /// <summary>
    /// The root class for all ACN packets to inherit from.
    /// </summary>
    public abstract class AcnPacket
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="AcnPacket"/> class.
        /// </summary>
        /// <param name="protocolId">The protocol identifier.</param>
        public AcnPacket(int protocolId)
        {
            Root = new AcnRootLayer();
            Root.ProtocolId = protocolId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcnPacket"/> class.
        /// </summary>
        /// <param name="protocolId">The protocol identifier.</param>
        public AcnPacket(ProtocolIds protocolId)
            : this((int)protocolId)
        {
        }

        #endregion

        #region Packet Contents

        /// <summary>
        /// Gets or sets the ACN root layer information.
        /// </summary>
        public AcnRootLayer Root { get; set; }

        #endregion

        #region Read and Write

        /// <summary>
        /// Reads the packet spcific data from the packet data stream.
        /// </summary>
        /// <remarks>
        /// Each packet type should override this and read the data in the format specified by the protocol.
        /// </remarks>
        /// <param name="data">The data reader for the ACN packet.</param>
        protected abstract void ReadData(AcnBinaryReader data);
        
        /// <summary>
        /// Writes the packet specific data to the packet data stream.
        /// </summary>
        /// <remarks>
        /// Each packet type should override this and write the data in the format specified by the protocol.
        /// </remarks>
        /// <param name="data">The data reader for the ACN packet.</param>
        protected abstract void WriteData(AcnBinaryWriter data);

        /// <summary>
        /// Reads a single packet from the data stream and returns the created packet information.
        /// </summary>
        /// <remarks>
        /// This will create the correct packet class from the registerd packet formats in the packet factory.
        /// </remarks>
        /// <param name="data">The recieved packet data.</param>
        /// <returns>The packet information, cast to the desired packet type.</returns>
        public static AcnPacket ReadPacket(AcnBinaryReader data)
        {
            AcnRootLayer rootLayer = new AcnRootLayer();
            rootLayer.ReadData(data,false);

            return ReadPacket(rootLayer, data); ;
        }

        /// <summary>
        /// Reads a single packet from the data stream and returns the created packet information.
        /// </summary>
        /// <remarks>
        /// This will create the correct packet class from the registerd packet formats in the packet factory.
        /// </remarks>
        /// <param name="data">The recieved packet data.</param>
        /// <returns>The packet information, cast to the desired packet type.</returns>
        public static AcnPacket ReadTcpPacket(AcnBinaryReader data)
        {
            AcnRootLayer rootLayer = new AcnRootLayer();
            rootLayer.ReadData(data, true);

            return ReadPacket(rootLayer, data); ;
        }

        /// <summary>
        /// Reads a single packet from the data stream and returns the created packet information. This version assumes the header
        /// information has already been read and so starts at the PDU layer.
        /// </summary>
        /// <remarks>
        /// This will create the correct packet class from the registerd packet formats in the packet factory.
        /// </remarks>
        /// <param name="header">The header information for the packet to be read.</param>
        /// <param name="data">The recieved packet data.</param>
        /// <returns>The packet information, cast to the desired packet type.</returns>
        public static AcnPacket ReadPacket(AcnRootLayer header, AcnBinaryReader data)
        {
            return AcnPacket.Create(header, data);
        }

        public static void WritePacket(AcnPacket packet, AcnBinaryWriter data)
        {
            packet.Root.WriteData(data,false);
            packet.WriteData(data);
            packet.Root.WriteLength(data);
        }

        public static void WriteTcpPacket(AcnPacket packet, AcnBinaryWriter data)
        {
            packet.Root.WriteData(data, true);
            packet.WriteData(data);
            packet.Root.WriteLength(data);
        }

        #endregion

        /// <summary>
        /// Creates an ACN packet from the header information and the recieved data.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="data">The data.</param>
        /// <returns>The created ACN packet, may be cast to the correct packet type.</returns>
        public static AcnPacket Create(AcnRootLayer header,AcnBinaryReader data)
        {
            return AcnPacketFactory.Build(header, data);
        }

        /// <summary>
        /// Creates the specified root layer.
        /// </summary>
        /// <param name="rootLayer">The root layer.</param>
        /// <param name="packetType">Type of the packet.</param>
        /// <returns></returns>
        public static AcnPacket Create(AcnRootLayer rootLayer, Type packetType)
        {
            AcnPacket packet = (AcnPacket)Activator.CreateInstance(packetType);
            packet.Root = rootLayer;
            return packet;
        }

        /// <summary>
        /// The standard packet builder for most ACN packets.
        /// </summary>
        /// <typeparam name="TPacketType">The type of the packet type.</typeparam>
        /// <seealso cref="Acn.IPacketBuilder" />
        public class Builder<TPacketType> : IPacketBuilder where TPacketType : AcnPacket
        {
            /// <summary>
            /// Creates the specified root layer.
            /// </summary>
            /// <param name="rootLayer">The root layer.</param>
            /// <param name="reader">The reader.</param>
            /// <returns></returns>
            public AcnPacket Create(AcnRootLayer rootLayer, AcnBinaryReader reader)
            {
                AcnPacket packet = (AcnPacket)Activator.CreateInstance(typeof(TPacketType));
                packet.Root = rootLayer;
                packet.ReadData(reader);
                return packet;
            }
        }
        
    }
}

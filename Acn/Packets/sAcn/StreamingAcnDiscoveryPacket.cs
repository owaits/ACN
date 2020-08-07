using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Packets.sAcn
{
    /// <summary>
    /// The sACN discovery message allows devices sending DMX data to publish the universes they are transmitting. This allows recievers
    /// to see what universes are available to be consumed without having to join all the multicast groups.
    /// </summary>
    /// <seealso cref="Acn.AcnPacket" />
    public class StreamingAcnDiscoveryPacket:AcnPacket
    {
        #region Setup and Initialisation
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingAcnDiscoveryPacket"/> class.
        /// </summary>
        public StreamingAcnDiscoveryPacket()
            : base(ProtocolIds.sACNExtended)
        {
        }

        #endregion

        #region Framing Layer

        private FramingPdu framing = new FramingPdu();

        /// <summary>
        /// Gets the framing.
        /// </summary>
        public FramingPdu Framing
        {
            get { return framing; }
        }

        /// <summary>
        /// The ACN framing layer.
        /// </summary>
        /// <seealso cref="Acn.AcnPdu" />
        public class FramingPdu : AcnPdu
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FramingPdu"/> class.
            /// </summary>
            public FramingPdu()
                : base((int)E131Extended.Discovery)
            {
            }

            #region PDU Contents

            private string sourceName = string.Empty;

            /// <summary>
            /// Gets or sets the name of the originator of this packet.
            /// </summary>
            public string SourceName
            {
                get { return sourceName; }
                set { sourceName = value; }
            }

            #endregion

            #region Read and Write

            /// <summary>
            /// Reads the PDU information from the recieved packet data.
            /// </summary>
            /// <param name="data">The recieved packet data.</param>
            protected override void ReadData(AcnBinaryReader data)
            {
                SourceName = data.ReadUtf8String(64);
                data.BaseStream.Seek(4, SeekOrigin.Current);
            }

            /// <summary>
            /// Write the PDU information to the packet data to be transmitted.
            /// </summary>
            /// <param name="data">The packet data to be sent.</param>
            protected override void WriteData(AcnBinaryWriter data)
            {
                data.WriteUtf8String(SourceName, 64);
                data.BaseStream.Seek(4, SeekOrigin.Current);
            }

            #endregion
        }

        #endregion

        #region Universe Discovery Layer

        private UniverseDiscoveryPdu universeDiscovery = new UniverseDiscoveryPdu();

        /// <summary>
        /// The discovery universe information PDU.
        /// </summary>
        public UniverseDiscoveryPdu UniverseDiscovery
        {
            get { return universeDiscovery; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="Acn.AcnPdu" />
        public class UniverseDiscoveryPdu : AcnPdu
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UniverseDiscoveryPdu"/> class.
            /// </summary>
            public UniverseDiscoveryPdu()
                : base((int)1)
            {
            }

            #region PDU Contents

            private byte page = 0;

            /// <summary>
            /// Gets or sets the current page of universes being sent.
            /// </summary>
            public byte Page
            {
                get { return page; }
                set { page = value; }
            }

            private byte totalPages = 0;

            /// <summary>
            /// Gets or sets the total number of pages being sent.
            /// </summary>
            public byte TotalPages
            {
                get { return totalPages; }
                set { totalPages = value; }
            }

            private List<int> universes = new List<int>();

            /// <summary>
            /// Gets or sets a list of sACN universes.
            /// </summary>
            public List<int> Universes
            {
                get { return universes; }
                set { universes = value; }
            }

            #endregion

            #region Read and Write

            /// <summary>
            /// Reads the PDU information from the recieved packet data.
            /// </summary>
            /// <param name="data">The recieved packet data.</param>
            protected override void ReadData(AcnBinaryReader data)
            {
                Page = data.ReadByte();
                TotalPages = data.ReadByte();

                for(int n=2;n<Length;n+=2)
                {
                    Universes.Add(data.ReadOctet2());
                }
            }

            /// <summary>
            /// Write the PDU information to the packet data to be transmitted.
            /// </summary>
            /// <param name="data">The packet data to be sent.</param>
            protected override void WriteData(AcnBinaryWriter data)
            {
                data.Write(Page);
                data.WriteOctet(TotalPages);

                foreach(int universe in universes)
                    data.WriteOctet((short) universe);
            }

            #endregion
        }

        #endregion

        #region Read/Write

        /// <summary>
        /// Reads the packet spcific data from the packet data stream.
        /// </summary>
        /// <param name="data">The data reader for the ACN packet.</param>
        /// <remarks>
        /// Each packet type should override this and read the data in the format specified by the protocol.
        /// </remarks>
        protected override void ReadData(AcnBinaryReader data)
        {
            Framing.ReadPdu(data);
            UniverseDiscovery.ReadPdu(data);
        }

        /// <summary>
        /// Writes the packet specific data to the packet data stream.
        /// </summary>
        /// <param name="data">The data reader for the ACN packet.</param>
        /// <remarks>
        /// Each packet type should override this and write the data in the format specified by the protocol.
        /// </remarks>
        protected override void WriteData(AcnBinaryWriter data)
        {
            Framing.WritePdu(data);
            Framing.WriteLength(data);
            UniverseDiscovery.WritePdu(data);
            UniverseDiscovery.WriteLength(data);
        }

        #endregion
    }
}

using Acn.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Acn.Packets.sAcn
{
    /// <summary>
    /// This packet is sent by a device for every DMX frame and after all the DMX universes have been sent. It
    /// indicates to a reciever that all DMX universes have been sent and the device should respond to that DMX data.
    /// </summary>
    /// <remarks>
    /// By sending a single sync packet for many universes a reciever can ensure the universes are synchronized together.
    /// </remarks>
    /// <seealso cref="Acn.AcnPacket" />
    public class StreamingAcnSynchronizationPacket:AcnPacket
    {
        public StreamingAcnSynchronizationPacket()
            : base(ProtocolIds.sACNExtended)
        {
        }

        #region Framing Layer

        private FramingPdu framing = new FramingPdu();

        /// <summary>
        /// Gets the framing information for this packet.
        /// </summary>
        public FramingPdu Framing
        {
            get { return framing; }
        }

        /// <summary>
        /// The framing information.
        /// </summary>
        /// <seealso cref="Acn.AcnPdu" />
        public class FramingPdu : AcnPdu
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FramingPdu"/> class.
            /// </summary>
            public FramingPdu()
                : base((int)E131Extended.Synchronization)
            {
            }

            #region PDU Contents

            private byte sequenceNumber = 0;

            /// <summary>
            /// Gets or sets the sequence number to determine the order sent by the transmitter.
            /// </summary>
            /// <remarks>
            /// In some network conditions the packets may be recieved out of order and this allows the recieve to determine the correct order.
            /// </remarks>
            public byte SequenceNumber
            {
                get { return sequenceNumber; }
                set { sequenceNumber = value; }
            }

            private short synchronizationAddress = 0;

            /// <summary>
            /// Gets or sets the sACN universe used to send synchronization messages. This packet should have been 
            /// sent on that universe.
            /// </summary>
            public short SynchronizationAddress
            {
                get { return synchronizationAddress; }
                set { synchronizationAddress = value; }
            }

            #endregion

            #region Read and Write

            /// <summary>
            /// Reads the PDU information from the recieved packet data.
            /// </summary>
            /// <param name="data">The recieved packet data.</param>
            protected override void ReadData(AcnBinaryReader data)
            {
                SequenceNumber = data.ReadByte();
                SynchronizationAddress = data.ReadOctet2();
                data.BaseStream.Seek(2, SeekOrigin.Current);
            }

            /// <summary>
            /// Write the PDU information to the packet data to be transmitted.
            /// </summary>
            /// <param name="data">The packet data to be sent.</param>
            protected override void WriteData(AcnBinaryWriter data)
            {
                data.Write(SequenceNumber);
                data.WriteOctet(SynchronizationAddress);
                data.BaseStream.Seek(2, SeekOrigin.Current);
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
        }

        #endregion
    }
}

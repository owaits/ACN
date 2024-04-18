using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LXProtocols.Acn.IO;

namespace LXProtocols.Acn
{
    [Flags]
    public enum PduFlags : byte
    {
        Length = 8,
        Vector = 4,
        Header = 2,
        Data = 1,
        All = 7,
        Extended = 0xF
    }

    /// <summary>
    /// The root PDU class to be implemented by all ACN PDU's.
    /// </summary>
    public abstract class AcnPdu
    {
        private int vectorLength = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcnPdu"/> class.
        /// </summary>
        /// <param name="protocolId">The protocol identifier.</param>
        public AcnPdu(int protocolId)
            :this((int) protocolId,4)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcnPdu"/> class.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="vectorLength">Length of the vector.</param>
        public AcnPdu(int vector, int vectorLength)
        {
            Flags = PduFlags.All;

            this.vectorLength = vectorLength;
            Vector = vector;
        }

        #region Packet Contents

        /// <summary>
        /// Gets or sets the PDU flags.
        /// </summary>
        public PduFlags Flags { get; protected set; }

        private int length = 0;

        /// <summary>
        /// Gets or sets the PDU length including the header data.
        /// </summary>
        public int Length 
        {
            get { return length; }
            set
            {
                if (length != value)
                {
                    length = value;

                    //TODO: need to figure out what to do this in RDMNet.
                    //if (length > 4096)
                    //    Flags |= PduFlags.Length;
                    //else
                    //    Flags &= ~PduFlags.Length;
                }
            }
        }

        private int vector = 0;

        /// <summary>
        /// Gets the PDU vector.
        /// </summary>
        public int Vector
        {
            get { return vector; }
            private set { vector = value; }
        }

        #endregion

        #region Read and Write

        /// <summary>
        /// Reads the PDU information and header from the recieved packet data.
        /// </summary>
        /// <param name="data">The recieved packet data.</param>
        public virtual void ReadPdu(AcnBinaryReader data)
        {
            //Read PDU Header
            Length = data.ReadOctet2();
            var flags = (PduFlags)((Length & 0xF000) >> 12);
            Length &= 0xFFF;

            //TODO: look at ACN protocol on how top deal with this.
            if(flags.HasFlag(PduFlags.Extended))
            {
                Length = ((Length << 8) + data.ReadByte());
            }
            Flags = flags;

            switch (vectorLength)
            {
                case 1:
                    Vector = data.ReadByte();
                    break;
                case 2:
                    Vector = data.ReadOctet2();
                    break;
                case 4:
                    Vector = data.ReadOctet4();
                    break;
            }

            ReadData(data);            
        }

        /// <summary>
        /// Reads the PDU information from the recieved packet data.
        /// </summary>
        /// <param name="data">The recieved packet data.</param>
        protected abstract void ReadData(AcnBinaryReader data);

        long lengthPosition = 0;
        
        /// <summary>
        /// Write the PDU information and header to the packet data to be transmitted.
        /// </summary>
        /// <param name="data">The packet data to be sent.</param>
        public virtual void WritePdu(AcnBinaryWriter data)
        {
            //Save length and skip
            //We save the stream position of the length so we can come back later and write it.
            lengthPosition = data.BaseStream.Position;

            data.BaseStream.Seek((Flags == PduFlags.Extended ? 3 : 2), System.IO.SeekOrigin.Current);   
      
            //Write the PDU Vector.
            switch (vectorLength)
            {
                case 1:
                    data.Write((byte) Vector);
                    break;
                case 2:
                    data.WriteOctet((short)Vector);
                    break;
                case 4:
                    data.WriteOctet(Vector);
                    break;
            }

            //Write the PDU data.
            WriteData(data);
        }

        /// <summary>
        /// Writes the length of this PDU to the PDU header.
        /// </summary>
        /// <param name="data">The packet data to write to.</param>
        public void WriteLength(AcnBinaryWriter data)
        {
            //Return to Length and update
            long endPosition = data.BaseStream.Position;

            var flags = Flags;
            Length = (int) (endPosition - lengthPosition);

            //Write PDU length.
            data.BaseStream.Seek(lengthPosition, System.IO.SeekOrigin.Begin);

            if(flags.HasFlag(PduFlags.Length))
            {
                data.WriteOctet((short)(((short)Flags << 12) + ((Length >> 8) & 0xFFF)));
                data.Write((byte)Length);
            }
            else
            {
                data.WriteOctet((short)((Length & 0x0FFF) + ((int)Flags << 12)));
            }

            //Return to original stream position at end of PDU.
            data.BaseStream.Seek(endPosition, System.IO.SeekOrigin.Begin);
        }

        /// <summary>
        /// Write the PDU information to the packet data to be transmitted.
        /// </summary>
        /// <param name="data">The packet data to be sent.</param>
        protected abstract void WriteData(AcnBinaryWriter data);

        #endregion
    }
}

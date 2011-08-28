using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;

namespace Acn
{
    [Flags]
    public enum PduFlags
    {
        Length = 8,
        Vector = 4,
        Header = 2,
        Data = 1,
        All = 7
    }

    public abstract class AcnPdu
    {
        private int vectorLength = 0;

        public AcnPdu(ProtocolIds protocolId)
            :this((int) protocolId,4)
        {
        }

        public AcnPdu(int vector, int vectorLength)
        {
            Flags = PduFlags.All;

            this.vectorLength = vectorLength;
            Vector = vector;
        }

        #region Packet Contents

        public PduFlags Flags { get; protected set; }

        private int length = 0;

        public int Length 
        {
            get { return length; }
            set
            {
                if (length != value)
                {
                    length = value;

                    if (length > 4096)
                        Flags |= PduFlags.Length;
                    else
                        Flags &= ~PduFlags.Length;
                }
            }
        }

        private int vector = 0;

        public int Vector
        {
            get { return vector; }
            private set { vector = value; }
        }

        #endregion

        #region Read and Write

        public virtual void ReadPdu(AcnBinaryReader data)
        {
            //Read PDU Header
            Length = data.ReadOctet2();
            Flags = (PduFlags)((Length & 0xF000) >> 12);
            Length &= 0xFFF;

            switch (vectorLength)
            {
                case 1:
                    Vector = data.ReadByte();
                    break;
                case 4:
                    Vector = data.ReadOctet4();
                    break;
            }

            ReadData(data);            
        }

        protected abstract void ReadData(AcnBinaryReader data);

        public virtual void WritePdu(AcnBinaryWriter data)
        {
            //Save length and skip
            //We save the stream position of the length so we can come back later and write it.
            long lengthPosition = data.BaseStream.Position;
            data.BaseStream.Seek(2, System.IO.SeekOrigin.Current);          
                       
            //Write the PDU Vector.
            switch (vectorLength)
            {
                case 1:
                    data.Write((byte) Vector);
                    break;
                case 4:
                    data.WriteOctet(Vector);
                    break;
            }

            //Write the PDU data.
            WriteData(data);

            //Return to Length and update
            long endPosition = data.BaseStream.Position;
            Length = (int) (endPosition - lengthPosition - 6);

            //Write PDU length.
            data.BaseStream.Seek(lengthPosition, System.IO.SeekOrigin.Begin);
            data.WriteOctet((short)((Length & 0x0FFF) + ((int)Flags << 12)));

            //Return to original stream position at end of PDU.
            data.BaseStream.Seek(endPosition, System.IO.SeekOrigin.Begin);
        }

        protected abstract void WriteData(AcnBinaryWriter data);

        #endregion
    }
}

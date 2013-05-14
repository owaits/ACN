using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;

namespace Acn
{
    public class AcnRootLayer
    {
        public AcnRootLayer()
        {
            PreambleSize = 16;
            PostambleSize = 0;
            Flags = 0x7;
        }

        #region Packet Contents

        public short PreambleSize { get; set; }

        public short PostambleSize { get; set; }

        public string PacketId 
        { 
            get { return "ASC-E1.17"; } 
        }

        public byte Flags { get; set; }

        public int Length { get; set; }

        public int ProtocolId { get; set; }

        public Guid SenderId { get; set; }

        #endregion

        #region Read and Write

        public void ReadData(AcnBinaryReader data)
        {
            //Read Preamble
            PreambleSize = data.ReadOctet2();
            PostambleSize = data.ReadOctet2();
            string packetId = data.ReadUtf8String(12);
            if (packetId != PacketId)
                throw new InvalidPacketException("The packet ID is not a valid ACN packet Id");

            //Read PDU Header
            Length = data.ReadOctet2();
            Flags = (byte)((Length & 0xF000) >> 12);
            Length &= 0xFFF;
            ProtocolId = data.ReadOctet4();

            //Read CID            
            SenderId = new Guid(data.ReadBytes(16));
        }

        private long lengthPosition = 0;

        public void WriteData(AcnBinaryWriter data)
        {
            ValidatePacket();

            data.WriteOctet(PreambleSize);
            data.WriteOctet(PostambleSize);
            data.WriteUtf8String(PacketId, 12);

            //Save the position of the length so we can come back and fill it in.
            lengthPosition = data.BaseStream.Position;

            data.WriteOctet((short) ((Flags << 12) + Length));
            data.WriteOctet(ProtocolId);
            data.Write(SenderId.ToByteArray());
        }

        public void WriteLength(AcnBinaryWriter data)
        {
            if (lengthPosition == 0)
                throw new InvalidOperationException("You must write the root layer data first before calling WriteLength.");

            //Update the Length
            Length = (int) (data.BaseStream.Position - lengthPosition);

            //Write the updated length to the packet data.
            long savedPosition = data.BaseStream.Position;
            data.BaseStream.Seek(lengthPosition, System.IO.SeekOrigin.Begin);
            data.WriteOctet((short)((Flags << 12) + Length));
            data.BaseStream.Seek(savedPosition, System.IO.SeekOrigin.Begin);
        }

        private void ValidatePacket()
        {
            if (PreambleSize < 16)
                throw new InvalidPacketException("Invalid preamble size for root layer protocol. Preamble must be 16 or greater!");

            if (PostambleSize != 0)
                throw new InvalidPacketException("Invalid postamble size for root layer protocol. Postamble must be zero!");

            if(SenderId == Guid.Empty)
                throw new InvalidPacketException("Invalid sender id for root layer protocol. The sender id is not set!"); 
        }

        #endregion

        
    }
}

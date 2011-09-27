using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Acn.Rdm
{
    public abstract class RdmPacket
    {
        protected RdmPacket()
        {
        }

        public RdmPacket(RdmCommands command, RdmParameters parameterId)
        {
            Header = new RdmHeader();
            Header.Command = command;
            Header.ParameterId = parameterId;
        }

        #region Contents

        public RdmHeader Header { get; protected set; }

        public short Checksum { get; set; }

        #endregion

        #region Read and Write

        protected void ReadHeader(RdmBinaryReader data)
        {
            Header.ReadData(data);
        }

        protected void WriteHeader(RdmBinaryWriter data)
        {
            Header.WriteData(data);
        }

        protected abstract void ReadData(RdmBinaryReader data);

        protected abstract void WriteData(RdmBinaryWriter data);

        public static RdmPacket ReadPacket(RdmBinaryReader data)
        {
            RdmPacket rdmPacket = null;

            RdmHeader header = new RdmHeader();
            header.ReadData(data);

            rdmPacket = RdmPacket.Create(header);
            if (rdmPacket != null)
            {
                rdmPacket.ReadData(data);
                return rdmPacket;
             }

            throw new UnknownRdmPacketException(header);            
        }

        public static bool TryReadPacket(RdmBinaryReader data,out RdmPacket rdmPacket)
        {
            RdmHeader header = new RdmHeader();
            header.ReadData(data);

            rdmPacket = RdmPacket.Create(header);
            if (rdmPacket != null)
            {
                rdmPacket.ReadData(data);
                return true;
            }

            return false;
        }

        public static void WritePacket(RdmPacket packet, RdmBinaryWriter data)
        {
            packet.WriteHeader(data);
            packet.WriteData(data);
            packet.Header.WriteLength(data);        
        }

        public static ushort CalculateChecksum(byte[] data)
        {
            ushort checksum = 0;
            foreach (byte item in data)
                checksum += item;
            return checksum;
        }

        public static ushort CalculateChecksum(byte[] data, int startIndex, int endIndex)
        {
            ushort checksum = 0;
            for (int n=startIndex;n<endIndex;n++)
                checksum += data[n];
            return checksum;
        }

        #endregion


        


        public static RdmPacket Create(RdmHeader header)
        {
            return RdmPacketFactory.Build(header);
        }

        public static RdmPacket Create(RdmHeader header, Type packetType)
        {
            RdmPacket packet = (RdmPacket)Activator.CreateInstance(packetType);
            packet.Header = header;
            return packet;
        }
    }
}

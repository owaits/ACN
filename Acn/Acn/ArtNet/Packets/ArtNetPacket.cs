using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public class ArtNetPacket
    {
        public ArtNetPacket(ArtNetOpCodes opCode)
        {
            OpCode = (short) opCode;
        }

        public ArtNetPacket(ArtNetRecieveData data)
        {
            BinaryReader packetReader = new BinaryReader(new MemoryStream(data.buffer));
            ReadData(packetReader);
        }

        public byte[] ToArray()
        {
            MemoryStream stream = new MemoryStream();
            WriteData(new BinaryWriter(stream));
            return stream.ToArray();
        }

        #region Packet Properties

        private string protocol = "Art-Net";

        public string Protocol
        {
            get { return protocol; }
            protected set 
            {
                if(value.Length > 8)
                    protocol = value.Substring(0, 8);
                else
                    protocol = value; 
            }
        }

        private short opCode = 0;

        public virtual short OpCode
        {
            get { return opCode; }
            protected set { opCode = value; }
        }

        private short version = 14;

        public short Version
        {
            get { return version; }
            protected set { version = value; }
        }

        #endregion
        	
        public virtual void ReadData(ArtNetBinaryReader data)
        {
            Protocol = System.Text.ASCIIEncoding.UTF8.GetString(data.ReadBytes(8));
            OpCode = data.ReadInt16();
            Version = data.ReadInt16();
        }

        public virtual void WriteData(ArtNetBinaryWriter data)
        {
            data.Write(System.Text.ASCIIEncoding.UTF8.GetBytes(Protocol.PadRight(7)));
            data.Write((byte)0);            //Null terminate the string.
            data.Write(OpCode);
            data.Write(IPAddress.HostToNetworkOrder(Version));
        }

        public static ArtNetPacket Create(ArtNetRecieveData data)
        {
            switch ((ArtNetOpCodes) data.OpCode)
            {
                case ArtNetOpCodes.Poll:
                    return new ArtPollPacket(data);
                case ArtNetOpCodes.PollReply:
                    return new ArtPollReplyPacket(data);
                case ArtNetOpCodes.Dmx:
                    return new ArtNetDmxPacket(data);
                case  ArtNetOpCodes.TodRequest:
                    return new ArtTodRequestPacket(data);
                case ArtNetOpCodes.TodData:
                    return new ArtTodDataPacket(data);
                case ArtNetOpCodes.TodControl:
                    return new ArtTodControlPacket(data);
                case ArtNetOpCodes.Rdm:
                    return new ArtRdmPacket(data);
                case ArtNetOpCodes.RdmSub:
                    return new ArtRdmSubPacket(data);
            }

            return null;

        }
    }
}

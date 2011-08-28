using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Net
{
    public class PortToUniverse
    {
        public enum PortType
        {
            Physical = 0,
            Virtual = 1
        }

        public enum UniverseType
        {
            Standard = 0,
            Composite = 1
        }

        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.PortToUniverse)
            {
            }

            public short PortNumber { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
            }
        }

        public class GetReply:RdmResponsePacket
        {
            public GetReply():base(RdmCommands.GetResponse,RdmParameters.PortToUniverse)
            {
            }

            public PortType Gateway { get; set; }

            public UniverseType UniverseType { get; set; }

            public short PortNumber { get; set; }

            public short UniverseNumber { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                Gateway = (PortType) data.ReadByte();
                UniverseType = (UniverseType) data.ReadByte();
                PortNumber = data.ReadNetwork16();
                UniverseNumber = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)Gateway);
                data.Write((byte) UniverseType);
                data.WriteNetwork(PortNumber);
                data.WriteNetwork(UniverseNumber);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set():base(RdmCommands.Set,RdmParameters.PortToUniverse)
            {
            }

            public UniverseType UniverseType { get; set; }

            public short PortNumber { get; set; }

            public short UniverseNumber { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                UniverseType = (UniverseType) data.ReadByte();
                PortNumber = data.ReadNetwork16();
                UniverseNumber = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte) UniverseType);
                data.WriteNetwork(PortNumber);
                data.WriteNetwork(UniverseNumber);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply():base(RdmCommands.SetResponse,RdmParameters.PortToUniverse)
            {
            }

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Net
{
    public class InitiateDiscovery
    {
        public enum DiscoveryState
        {
            NotActive = 0x0,
            Full = 0x1,
            Incremental = 0x2
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.InitiateDiscovery)
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

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.InitiateDiscovery)
            {
            }

            public short PortNumber { get; set; }

            public short DeviceCount { get; set; }

            public DiscoveryState DiscoveryState { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
                DeviceCount = data.ReadNetwork16();
                DiscoveryState = (DiscoveryState)data.ReadByte();

            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
                data.WriteNetwork(DeviceCount);
                data.Write((byte)DiscoveryState);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.InitiateDiscovery)
            {
            }

            public short PortNumber { get; set; }

            public DiscoveryState DiscoveryState { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
                DiscoveryState = (DiscoveryState)data.ReadByte();

            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
                data.Write((byte)DiscoveryState);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.InitiateDiscovery)
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

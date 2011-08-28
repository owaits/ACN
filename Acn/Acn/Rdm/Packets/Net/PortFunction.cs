using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Net
{
    public class PortFunction
    {
        public enum PortFunctions
        {
            Disabled = 0x0,
            Input = 0x1,
            Output = 0x2
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.PortFunction)
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
                : base(RdmCommands.GetResponse, RdmParameters.PortFunction)
            {
            }

            public short PortNumber { get; set; }

            public PortFunctions PortFunction { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
                PortFunction = (PortFunctions) data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
                data.Write((byte) PortFunction);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.PortFunction)
            {
            }

            public short PortNumber { get; set; }

            public PortFunctions PortFunction { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
                PortFunction = (PortFunctions)data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
                data.Write((byte)PortFunction);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.PortFunction)
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

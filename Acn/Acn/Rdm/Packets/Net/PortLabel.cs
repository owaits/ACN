using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Net
{
    public class PortLabel
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.PortLabel)
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
                : base(RdmCommands.GetResponse, RdmParameters.PortLabel)
            {
            }

            public short PortNumber { get; set; }

            public string Label { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
                Label = Encoding.ASCII.GetString(data.ReadBytes(Header.ParameterDataLength-2));
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
                data.Write(Encoding.ASCII.GetBytes(Label));
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.PortLabel)
            {
            }

            public short PortNumber { get; set; }

            public string Label { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
                Label = Encoding.ASCII.GetString(data.ReadBytes(Header.ParameterDataLength - 2));
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
                data.Write(Encoding.ASCII.GetBytes(Label));
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.PortLabel)
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

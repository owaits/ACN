using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Net
{
    public class BindingControlFields
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.BindingControlFields)
            {
            }

            public UId Id { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                Id = data.ReadUId();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(Id);
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.BindingControlFields)
            {
            }

            public UId Id { get; set; }

            public short PortNumber { get; set; }

            public short ControlFields { get; set; }

            public UId BindingId { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                Id = data.ReadUId();
                PortNumber = data.ReadNetwork16();
                ControlFields = data.ReadNetwork16();
                BindingId = data.ReadUId();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(Id);
                data.Write(PortNumber);
                data.Write(ControlFields);
                data.Write(BindingId);
            }
        }
    }
}

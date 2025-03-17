using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Rdm.Packets.Net
{
    /// <summary>
    /// This parameter allows a Controller to retrieve the Binding UID and Control Field information that
    /// is sent as part of the Discovery Mute(DISC_MUTE) message.
    /// </summary>
    public class BindingControlFields
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.BindingControlFields)
            {
            }

            public short EndpointID { get; set; }

            public UId Id { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
                Id = data.ReadUId();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(EndpointID);
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

            public short EndpointID { get; set; }

            public short ControlFields { get; set; }

            public UId BindingId { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
                Id = data.ReadUId();                
                ControlFields = data.ReadNetwork16();
                BindingId = data.ReadUId();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(EndpointID);
                data.Write(Id);                
                data.Write(ControlFields);
                data.Write(BindingId);
            }
        }
    }
}

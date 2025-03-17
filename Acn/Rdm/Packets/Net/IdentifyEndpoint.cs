using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Rdm.Packets.Net
{
    public class IdentifyEndpoint
    {
        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.IdentifyEndpoint)
            {
            }

            public short EndpointID { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.IdentifyEndpoint)
            {
            }

            public short EndpointID { get; set; }

            public bool IdentifyOn { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
                IdentifyOn = data.ReadBoolean();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
                data.Write(IdentifyOn);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set():base(RdmCommands.Set,RdmParameters.IdentifyEndpoint)
            {
            }

            public short EndpointID { get; set; }

            public bool IdentifyOn { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
                IdentifyOn = data.ReadBoolean();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
                data.Write(IdentifyOn);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.IdentifyEndpoint)
            {
            }

            protected override void ReadData(RdmBinaryReader data)
            {
                //Parameter Data Empty
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                //Parameter Data Empty
            }
        }

        
    }
}

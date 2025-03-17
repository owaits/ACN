using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Rdm.Packets.Net
{
    public class EndpointResponderListChange
    {
        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.EndpointResponderListChange)
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

        public class Reply:RdmResponsePacket
        {
            public Reply()
                : base(RdmCommands.GetResponse, RdmParameters.EndpointResponderListChange)
            {             
            }

            public short EndpointID { get; set; }

            public int ListChangeNumber { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
                ListChangeNumber = data.ReadNetwork32();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
                data.WriteNetwork(ListChangeNumber);
            }
        }
    }
}

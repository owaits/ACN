using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Rdm.Packets.Net
{
    public class EndpointMode
    {
        public enum EndpointModes
        {
            /// <summary>
            /// Endpoint is disabled for all traffic.
            /// </summary>
            Disabled = 0x0,
            /// <summary>
            /// Endpoint is configured as an Input, accepting DMX512 communication.
            /// </summary>
            Input = 0x1,
            /// <summary>
            /// Endpoint is configured as an Output generating DMX512 communication.
            /// </summary>
            Output = 0x2
        }

        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.EndpointMode)
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
                : base(RdmCommands.GetResponse, RdmParameters.EndpointMode)
            {
            }

            public short EndpointID { get; set; }

            public EndpointModes EndpointMode { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
                EndpointMode = (EndpointModes) data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
                data.Write((byte) EndpointMode);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.EndpointMode)
            {
            }
            
            public short EndpointID { get; set; }

            public EndpointModes EndpointMode { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                EndpointID = data.ReadNetwork16();
                EndpointMode = (EndpointModes) data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(EndpointID);
                data.Write((byte) EndpointMode);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.EndpointMode)
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
    }
}

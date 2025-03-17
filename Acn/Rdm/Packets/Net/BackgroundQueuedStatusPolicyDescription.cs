using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Rdm.Packets.Net
{
    public class BackgroundQueuedStatusPolicyDescription
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.BackgroundQueuedStatusPolicyDescription)
            {
            }

            public BackgroundQueuedPolicy Policy { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                Policy = (BackgroundQueuedPolicy)data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)Policy);
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.BackgroundQueuedStatusPolicyDescription)
            {
            }

            public BackgroundQueuedPolicy Policy { get; set; }

            public string Description { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                Policy = (BackgroundQueuedPolicy)data.ReadByte();
                Description = data.ReadNetworkString(Header.ParameterDataLength - 1);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)Policy);
                data.WriteNetwork(Description);
            }
        }
    }
}

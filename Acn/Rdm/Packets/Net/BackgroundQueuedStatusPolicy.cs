using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn.Rdm.Packets.Net
{
    public enum BackgroundQueuedPolicy : byte
    {
        StatusNone = 0,
        StatusAdvisory = 1,
        StatusWarning = 2,
        StatusError = 3
    }

    public class BackgroundQueuedStatusPolicy
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.BackgroundQueuedStatusPolicy)
            {
            }

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.BackgroundQueuedStatusPolicy)
            {
            }

            public BackgroundQueuedPolicy CurrentPolicy { get; set; }

            public byte PolicyCount { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                CurrentPolicy = (BackgroundQueuedPolicy)data.ReadByte();
                PolicyCount = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)CurrentPolicy);
                data.Write(PolicyCount);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.BackgroundQueuedStatusPolicy)
            {
            }

            public BackgroundQueuedPolicy CurrentPolicy { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                CurrentPolicy = (BackgroundQueuedPolicy)data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write((byte)CurrentPolicy);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.BackgroundQueuedStatusPolicy)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Net
{
    public class PortTiming
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.PortTiming)
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
                : base(RdmCommands.GetResponse, RdmParameters.PortTiming)
            {
            }

            public short PortNumber { get; set; }

            public byte CurrentSetting { get; set; }

            public byte SettingsCount { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
                CurrentSetting = data.ReadByte();
                SettingsCount = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
                data.Write(CurrentSetting);
                data.Write(SettingsCount);
            }
        }

        public class Set : RdmRequestPacket
        {
            public Set()
                : base(RdmCommands.Set, RdmParameters.PortTiming)
            {
            }

            public short PortNumber { get; set; }

            public byte PortTiming { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
                PortTiming = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
                data.Write(PortTiming);
            }
        }

        public class SetReply : RdmResponsePacket
        {
            public SetReply()
                : base(RdmCommands.SetResponse, RdmParameters.PortTiming)
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

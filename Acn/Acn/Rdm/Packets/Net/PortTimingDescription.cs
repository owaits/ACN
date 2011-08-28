using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Net
{
    public class PortTimingDescription
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get, RdmParameters.PortTimingDescription)
            {
            }

            public byte SettingIndex { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                SettingIndex = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(SettingIndex);
            }
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.PortTimingDescription)
            {
            }

            public byte SettingIndex { get; set; }

            public string Description { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                SettingIndex = data.ReadByte();
                Description = Encoding.ASCII.GetString(data.ReadBytes(Header.ParameterDataLength - 1));
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.Write(SettingIndex);
                data.Write(Encoding.ASCII.GetBytes(Description));
            }
        }
    }
}

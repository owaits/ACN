using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Net
{
    public class IdentifyPort
    {
        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.PortIdentify)
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

        public class Set : RdmRequestPacket
        {
            public Set():base(RdmCommands.Set,RdmParameters.PortIdentify)
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

        public class Reply:RdmResponsePacket
        {
            public Reply():base(RdmCommands.GetResponse,RdmParameters.PortIdentify)
            {
            }

            public short PortNumber { get; set; }

            public bool IdentifyState { get; set; }

            protected override void ReadData(RdmBinaryReader data)
            {
                PortNumber = data.ReadNetwork16();
                IdentifyState = (data.ReadByte() == 0 ? false : true);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(PortNumber);
                data.Write((byte)(IdentifyState? 1:0));
            }            
        }
    }
}

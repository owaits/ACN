using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acn.Rdm.Packets.Net
{
    public class PortList
    {
        public class Get:RdmRequestPacket
        {
            public Get():base(RdmCommands.Get,RdmParameters.PortList)
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
            public Reply():base(RdmCommands.GetResponse,RdmParameters.PortList)
            {             
            }

            List<short> portNumbers = null;

            public List<short> PortNumbers 
            {
                get { return portNumbers; }
                set { portNumbers = value; }
            }

            protected override void ReadData(RdmBinaryReader data)
            {
                List<short> ports = new List<short>();
                for (int n = 0; n < (Header.ParameterDataLength/2); n++)
                {
                    ports.Add(data.ReadNetwork16());
                }

                PortNumbers = ports;
            }

            protected override void WriteData(RdmBinaryWriter data)
            {          
                foreach (short portNumber in PortNumbers)
                {
                    data.WriteNetwork(portNumber);
                }
            }
        }
    }
}

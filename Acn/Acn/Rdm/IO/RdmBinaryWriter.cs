using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Acn.Rdm
{
    public class RdmBinaryWriter:BinaryWriter
    {
        public RdmBinaryWriter(Stream output)
            : base(output)
        {
        }

        public void WriteNetwork(short value)
        {
            base.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteNetwork(int value)
        {
            base.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteNetwork(string value)
        {
            Write(Encoding.ASCII.GetBytes(value));
        }


        public void Write(UId value)
        {
            WriteNetwork((short) value.ManufacturerId);
            WriteNetwork((int) value.DeviceId);
        }
    }
}

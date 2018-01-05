using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Acn.IO
{
    public class AcnBinaryWriter : BinaryWriter
    {
        public AcnBinaryWriter(Stream input)
            : base(input)
        { }

        public void WriteOctet(short value)
        {
            Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteOctet(int value)
        {
            Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteUtf8String(string value, int length)
        {
            Write(UTF8Encoding.UTF8.GetBytes(value));
            Write(new byte[(length - value.Length)]);
        }
    }
}

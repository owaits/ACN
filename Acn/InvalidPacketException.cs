using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Acn
{
    public class InvalidPacketException:InvalidOperationException
    {
        public InvalidPacketException(string message)
            : base(message)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;
using System.Net;

namespace Acn
{
    public interface IProtocolFilter
    {
        int ProtocolId { get; }

        void ProcessPacket(IPEndPoint source, AcnRootLayer header, AcnBinaryReader data);
    }
}

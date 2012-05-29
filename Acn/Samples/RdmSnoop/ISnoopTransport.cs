using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm.Routing;
using System.Net;

namespace RdmSnoop
{
    public interface ISnoopTransport:IRdmTransport
    {
        IPAddress LocalAdapter { get; set; }
        IPAddress SubnetMask { get; set; }
    }
}

using LXProtocols.Acn.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.RdmNet.Sockets
{
    public class NewRdmNetDeviceEventArgs
    {
        public RdmEndPoint DeviceEndpoint { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Sockets;
using Acn.Rdm;
using Acn.Slp;
using System.Net;

namespace RdmSnoop
{
    public interface IRdmTransport
    {
        event EventHandler<DeviceFoundEventArgs> NewDeviceFound;

        IPAddress LocalAdapter { get; }

        void Start(IPAddress localAdapter);
        void Stop();

        IRdmSocket GetDeviceSocket(UId deviceId);
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Sockets;

namespace Acn.Rdm.Routing
{
    public interface IRdmTransport
    {
        event EventHandler<DeviceFoundEventArgs> NewDeviceFound;

        void Start();
        void Stop();
        void Discover();

        IRdmSocket Socket { get; }
    }
}

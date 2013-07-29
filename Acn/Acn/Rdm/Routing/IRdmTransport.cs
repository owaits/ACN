using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Sockets;

namespace Acn.Rdm.Routing
{
    /// <summary>
    /// The type of discovery to perform.
    /// </summary>
    [Flags]
    public enum DiscoveryType
    {
        /// <summary>
        /// Searches the local network for RDM gateways.
        /// </summary>
        GatewayDiscovery,
        /// <summary>
        /// Performs an RDM discovery for devices. This will request all the known gateways to start a full discovery.
        /// </summary>
        DeviceDiscovery
    }

    public interface IRdmTransport
    {
        event EventHandler Starting;
        event EventHandler Stoping;

        event EventHandler<DeviceFoundEventArgs> NewDeviceFound;

        void Start();
        void Stop();
        void Discover(DiscoveryType type);

        IRdmSocket Socket { get; }
    }
}

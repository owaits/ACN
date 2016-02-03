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

        /// <summary>
        /// Gets the universe index for a specific endpoint address.
        /// </summary>
        /// <remarks>
        /// The transport may wish to resolve this to an internal addressing scheme.
        /// </remarks>
        /// <param name="address">The device address.</param>
        /// <returns>The DMX universe that this address resolves to.</returns>
        int ResolveEndpointToUniverse(RdmEndPoint address);

        IEnumerable<IRdmSocket> Sockets { get; }
    }
}

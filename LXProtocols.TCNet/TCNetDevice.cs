using LXProtocols.TCNet.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LXProtocols.TCNet
{
    /// <summary>
    /// Represents a remote TCNet device that we have established connection to.
    /// </summary>
    public class TCNetDevice
    {
        /// <summary>
        /// Gets or sets the node identifier reported by the TCNet device.
        /// </summary>
        /// <remarks>
        /// This may be null as there is no requirement for a device to set this as long as no other TCNet devices exist at the same IP address. We recomend you set 
        /// this to a random number to ensure devices can distinguish you from aother applications runniong on the same IP address.
        /// </remarks>
        public int NodeID { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer name of the remote device.
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// Gets or sets a device name to identify this device.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the device type such as Master, Skave or Auto.
        /// </summary>
        public NodeType NodeType { get; set; }

        /// <summary>
        /// Gets or sets the endpoint information of the remote device such as IP Address and Unicast Port.
        /// </summary>
        public TCNetEndPoint Endpoint { get; set; }

        /// <summary>
        /// Gets or sets whether the device has been authenticated with the master device.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return DeviceName;
        }
    }
}

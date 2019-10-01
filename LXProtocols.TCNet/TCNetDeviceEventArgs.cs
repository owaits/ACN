using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet
{
    /// <summary>
    /// Event arguments used when the status of a TCNet remote device changes.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TCNetDeviceEventArgs: EventArgs
    {
        /// <summary>
        /// Gets or sets the TCNet device that has changed.
        /// </summary>
        public TCNetDevice Device { get; set; }
    }
}

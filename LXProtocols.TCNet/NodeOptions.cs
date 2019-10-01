using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet
{
    /// <summary>
    /// The individual device options.
    /// </summary>
    [Flags]
    public enum NodeOptions:byte
    {
        None = 0,
        /// <summary>
        /// Authentication for extended communication needed.
        /// </summary>
        NeedAuthentication = 1,
        /// <summary>
        /// Listens to TCNet Control Messages
        /// </summary>
        SupportsTCNCM = 2,
        /// <summary>
        /// Listens to TCNet Application Specific Data Packet
        /// </summary>
        SupportsTCNASDP = 4,
        /// <summary>
        /// Do not disturb/Sleeping. Node will request data itself if needed to avoid traffic
        /// </summary>
        DoNotDisturb = 8
    }
}

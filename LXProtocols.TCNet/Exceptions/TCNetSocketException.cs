using LXProtocols.TCNet.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LXProtocols.TCNet.Exceptions
{
    /// <summary>
    /// This ocurrs when an error was caught within a TCNet socket that prevented the socket from operating normally.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class TCNetSocketException: TCNetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetSocketException"/> class.
        /// </summary>
        /// <param name="socket">The socket that has thrown this exception.</param>
        /// <param name="message">The message explaining the error that ocurred.</param>
        /// <param name="innerException">The inner exception.</param>
        public TCNetSocketException(TCNetSocket socket,string message, Exception innerException):base(message,innerException)
        {
            Data["NodeId"] = socket.NodeId;
            Data["NodeName"] = socket.NodeName;
            Data["LocalAddress"] = socket.LocalIP;
            Data["LocalSubnetMask"] = socket.LocalSubnetMask;
            Data["BroadcastAddress"] = socket.BroadcastAddress;
        }
    }
}

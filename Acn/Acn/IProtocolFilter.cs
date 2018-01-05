using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;
using System.Net;

namespace Acn
{
    /// <summary>
    /// Allows a class to listen to and process ACN traffic associated with a specific protocol ID.
    /// </summary>
    public interface IProtocolFilter
    {
        /// <summary>
        /// Gets a list of protocol ID's that this filter supports.
        /// </summary>
        IEnumerable<int> ProtocolId { get; }

        /// <summary>
        /// Processes the packet that have been recieved and allocated to this filter.
        /// </summary>
        /// <remarks>
        /// Only packets that have supported protocol ID's will be sent to this function.
        /// </remarks>
        /// <param name="source">The source IP address of the packet.</param>
        /// <param name="header">The header information for the ACN packet.</param>
        /// <param name="data">The data reader for the remaining packet data.</param>
        void ProcessPacket(IPEndPoint source, AcnRootLayer header, AcnBinaryReader data);
    }
}

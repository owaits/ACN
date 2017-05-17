using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp
{
    /// <summary>
    /// Extensions to help with using GUIDs in CITP comunications.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Returns a GUID in network byte order.
        /// </summary>
        /// <param name="id">The GUID to convert to network byte order.</param>
        /// <returns>The GUID in network byte order.</returns>
        public static Guid ToNetwork (this Guid id)
        {
            byte[] guidAsBytes = id.ToByteArray();
            return new Guid(new byte[] { guidAsBytes[3], guidAsBytes[2], guidAsBytes[1], guidAsBytes[0], guidAsBytes[5], guidAsBytes[4], guidAsBytes[7], guidAsBytes[6], guidAsBytes[8], guidAsBytes[9], guidAsBytes[10], guidAsBytes[11], guidAsBytes[12], guidAsBytes[13], guidAsBytes[14], guidAsBytes[15] });         
        }

        /// <summary>
        /// Returns a GUID in standard byte order when given a GUID in network byte order.
        /// </summary>
        /// <param name="id">The GUID to convert to standard byte order.</param>
        /// <returns>The GUID in standard byte order.</returns>
        public static Guid FromNetwork(this Guid id)
        {
            byte[] guidAsBytes = id.ToByteArray();
            return new Guid(new byte[] { guidAsBytes[3], guidAsBytes[2], guidAsBytes[1], guidAsBytes[0], guidAsBytes[5], guidAsBytes[4], guidAsBytes[7], guidAsBytes[6], guidAsBytes[8], guidAsBytes[9], guidAsBytes[10], guidAsBytes[11], guidAsBytes[12], guidAsBytes[13], guidAsBytes[14], guidAsBytes[15] });
        }
    }
}

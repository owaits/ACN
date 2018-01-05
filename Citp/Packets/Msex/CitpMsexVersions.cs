using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp.Packets.Msex
{
    /// <summary>
    /// Definitions for standard CITP MSEX versions and extension methods for converting between <see cref="Version"/> and <see cref="ushort"/> representations.
    /// </summary>
    public static class CitpMsexVersions
    {

        
        private static Version msex10Version;

        /// <summary>
        /// A representation of 1.0.
        /// </summary>
        /// <value>
        /// Version 1.0.
        /// </value>
        public static Version Msex10Version
        {
            get {
                if (CitpMsexVersions.msex10Version == null)
                {
                    msex10Version = new Version(1, 0);
                }
                return CitpMsexVersions.msex10Version; 
            }            
        }
                
        private static Version msex11Version = new Version(1, 1);

        /// <summary>
        /// A representation of 1.1.
        /// </summary>
        /// <value>
        /// Version 1.1.
        /// </value>
        public static Version Msex11Version
        {
            get
            {
                if (CitpMsexVersions.msex11Version == null)
                {
                    msex11Version = new Version(1, 1);
                }
                return CitpMsexVersions.msex11Version;
            }
        }

        
        private static Version msex12Version = new Version(1, 2);

        /// <summary>
        /// A representation of 1.2.
        /// </summary>
        /// <value>
        /// Version 1.2.
        /// </value>
        public static Version Msex12Version
        {
            get
            {
                if (CitpMsexVersions.msex12Version == null)
                {
                    msex12Version = new Version(1, 2);
                }
                return CitpMsexVersions.msex12Version;
            }
        }

        /// <summary>
        /// Gets all known versions.
        /// </summary>
        /// <value>
        /// All versions.
        /// </value>
        public static IEnumerable<Version> AllVersions
        {
            get
            {
                yield return Msex10Version;
                yield return Msex11Version;
                yield return Msex12Version;
            }
        }

        /// <summary>
        /// Converts the MSEX version to unsigned short.
        /// </summary>
        /// <param name="version">Version object representing an MSEX version.</param>
        /// <returns>The MSEX version as a UInt16.</returns>
        public static ushort ToMsexShort(this Version version)
        {
            return (ushort)((version.Major << 8) | version.Minor);
        }

        /// <summary>
        /// Converts the unsigned short to an MSEX version.
        /// </summary>
        /// <param name="version">Unsigned short representing an MSEX version.</param>
        /// <returns>The MSEX version as a version object.</returns>
        public static Version ToMsexVersion(this ushort version)
        {
            return new Version(version >> 8, version & 0xff);
        }
    }
}

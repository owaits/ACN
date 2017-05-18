using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp.Packets.Msex
{
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
    }
}

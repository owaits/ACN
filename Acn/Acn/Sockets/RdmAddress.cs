using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm;
using System.Net;

namespace Acn.Sockets
{
    public class RdmAddress
    {
        protected RdmAddress():base()
        {
        }

        public RdmAddress(IPAddress ipAddress)
        {
            IpAddress = ipAddress;
        }

        public RdmAddress(IPAddress ipAddress, int universe)
        {
            IpAddress = ipAddress;
            Universe = universe;
        }

        private IPAddress ipAddress = IPAddress.Any;

        public IPAddress IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        private int universe = 0;

        public int Universe
        {
            get { return universe; }
            set { universe = value; }
        }

        public override string ToString()
        {
            return IpAddress.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LXProtocols.TCNet.Sockets
{
    public class TCNetEndPoint : IPEndPoint
    {
        public TCNetEndPoint(IPAddress ipAddress)
            : this(ipAddress, 0, 0)
        {
        }

        public TCNetEndPoint(IPAddress ipAddress, int universe)
            : this(ipAddress, 0, universe)
        {
        }

        public TCNetEndPoint(IPEndPoint ipEndPoint)
            : this(ipEndPoint.Address, ipEndPoint.Port, 0)
        {
        }

        public TCNetEndPoint(IPEndPoint ipEndPoint, int universe)
            : this(ipEndPoint.Address, ipEndPoint.Port, universe)
        {
        }

        public TCNetEndPoint(IPAddress ipAddress, int port, int universe) : base(ipAddress, port)
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

        //private UId id = UId.Empty;

        //public UId Id
        //{
        //    get { return id; }
        //    set { id = value; }
        //}

        //private UId gatewayId = UId.Empty;

        //public UId GatewayId
        //{
        //    get { return gatewayId; }
        //    set { gatewayId = value; }
        //}

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

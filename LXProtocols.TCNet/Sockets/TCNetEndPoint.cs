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

        public TCNetEndPoint(IPAddress ipAddress, int nodeId)
            : this(ipAddress, 0, nodeId)
        {
        }

        public TCNetEndPoint(IPEndPoint ipEndPoint)
            : this(ipEndPoint.Address, ipEndPoint.Port, 0)
        {
        }

        public TCNetEndPoint(IPEndPoint ipEndPoint, int nodeId)
            : this(ipEndPoint.Address, ipEndPoint.Port, nodeId)
        {
        }

        public TCNetEndPoint(IPAddress ipAddress, int port, int universe) : base(ipAddress, port)
        {
        }

        public int NodeID { get; set; }

        public override string ToString()
        {
            return this.Address.ToString();
        }
    }
}

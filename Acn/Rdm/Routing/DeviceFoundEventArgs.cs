using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.Sockets;

namespace LXProtocols.Acn.Rdm.Routing
{
    public class DeviceFoundEventArgs : EventArgs
    {
        public DeviceFoundEventArgs(UId id, RdmEndPoint address)
        {
            this.id = id;
            this.ipAddress = address;
        }

        private UId id = UId.Empty;

        public UId Id
        {
            get { return id; }
            set { id = value; }
        }

        private RdmEndPoint ipAddress;

        public RdmEndPoint IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm;
using System.Net;

namespace RdmSnoop
{
    public class DeviceFoundEventArgs:EventArgs
    {

        public DeviceFoundEventArgs(UId id, IPAddress address)
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

        private IPAddress ipAddress;

        public IPAddress IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

    }
}

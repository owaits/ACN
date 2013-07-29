using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Slp;
using System.Net;
using Acn.Sockets;
using Acn.Rdm;
using Acn.Rdm.Routing;

namespace RdmSnoop.Transports
{
    public class RdmNet : ISnoopTransport
    {
        private SlpUserAgent slpUser = new SlpUserAgent("ACN-DEFAULT");
        private RdmSocket acnSocket = null;

        public event EventHandler<DeviceFoundEventArgs> NewDeviceFound;

        private IPAddress localAdapter;

        public IPAddress LocalAdapter
        {
            get { return localAdapter; }
            set { localAdapter = value; }
        }

        private IPAddress subnetMask;

        public IPAddress SubnetMask
        {
            get { return subnetMask; }
            set { subnetMask = value; }
        }


        public void Start()
        {
            slpUser.NetworkAdapter = localAdapter;
            slpUser.ServiceFound += new EventHandler<ServiceFoundEventArgs>(slpUser_ServiceFound);

            if (acnSocket == null || !acnSocket.PortOpen)
            {
                acnSocket = new RdmSocket(UId.NewUId(0xFF), Guid.NewGuid(), "RDM Snoop");
                acnSocket.Open(LocalAdapter);
            }

            slpUser.Open();
            slpUser.Find("service:e133.esta");
        }

        void slpUser_ServiceFound(object sender, ServiceFoundEventArgs e)
        {
            if(NewDeviceFound != null)
            {
                foreach (UrlEntry url in e.Urls)
                    NewDeviceFound(this, new DeviceFoundEventArgs(UId.ParseUrl(url.Url), new RdmAddress(e.Address.Address)));
            }

        }

        public void  Stop()
        {
            if(reliableSocket != null)
                reliableSocket.Dispose();

            if (slpUser != null)
                slpUser.Close();

            if(acnSocket != null)
                acnSocket.Close();
        }

        public void Discover(DiscoveryType type)
        {

        }

        private RdmReliableSocket reliableSocket = null;

        public IRdmSocket Socket
        {
            get 
            {
                if (reliableSocket == null && acnSocket != null)
                    reliableSocket = new RdmReliableSocket(acnSocket);

                return reliableSocket;
            }
        }

        public event EventHandler Starting;

        public event EventHandler Stoping;
    }
}

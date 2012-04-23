using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Slp;
using System.Net;
using Acn.Sockets;
using Acn.Rdm;

namespace RdmSnoop.Transports
{
    public class RdmNet : IRdmTransport
    {
        private SlpUserAgent slpUser = new SlpUserAgent("ACN-DEFAULT");
        private RdmSocket acnSocket = null;

        public event EventHandler<DeviceFoundEventArgs> NewDeviceFound;

        private IPAddress localAdapter;

        public IPAddress LocalAdapter
        {
            get { return localAdapter; }
            protected set { localAdapter = value; }
        }


        public void Start(IPAddress localAdapter,IPAddress subnetMask)
        {
            LocalAdapter = localAdapter;

            slpUser.NetworkAdapter = localAdapter;
            slpUser.ServiceFound += new EventHandler<ServiceFoundEventArgs>(slpUser_ServiceFound);

            if (acnSocket == null || !acnSocket.PortOpen)
            {
                acnSocket = new RdmSocket(UId.NewUId(0xFF), Guid.NewGuid(), "RDM Snoop");
                acnSocket.Open(localAdapter);
            }

            slpUser.Open();
            slpUser.Find("service:e133.esta");
        }

        void slpUser_ServiceFound(object sender, ServiceFoundEventArgs e)
        {
            if(NewDeviceFound != null)
            {
                foreach (UrlEntry url in e.Urls)
                    NewDeviceFound(this, new DeviceFoundEventArgs(UId.ParseUrl(url.Url), e.Address.Address));
            }

        }

        public void  Stop()
        {
            if (slpUser != null)
                slpUser.Close();

            if(acnSocket != null)
                acnSocket.Close();
        }

        public void Discover()
        {

        }

        public IRdmSocket GetDeviceSocket(UId deviceId)
        {
            return acnSocket;
        }
    }
}

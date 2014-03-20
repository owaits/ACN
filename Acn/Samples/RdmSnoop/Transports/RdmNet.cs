using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Slp;
using System.Net;
using Acn.Sockets;
using Acn.Rdm;
using Acn.Rdm.Routing;
using Acn.RdmNet.Sockets;
using Acn.Rdm.Packets.Net;
using Acn.Rdm.Broker;
using System.Diagnostics;
using Mono.Zeroconf;

namespace RdmSnoop.Transports
{
    public class RdmNet : ISnoopTransport
    {
        private SlpUserAgent slpUser = new SlpUserAgent("ACN-DEFAULT");
        private RdmNetMeshSocket rdmNetSocket = null;
        private ServiceBrowser dnsSD = null;

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

            if (rdmNetSocket == null || !rdmNetSocket.PortOpen)
            {
                rdmNetSocket = new RdmNetMeshSocket(UId.NewUId(0xFF), Guid.NewGuid(), "RDM Snoop");
                rdmNetSocket.UnhandledException += rdmNetSocket_UnhandledException;
                rdmNetSocket.NewRdmPacket += acnSocket_NewRdmPacket;
                rdmNetSocket.Open(new IPEndPoint(LocalAdapter,0));
            }

            slpUser.Open();
            slpUser.Find("service:rdmnet-device");

            dnsSD = new ServiceBrowser();
            dnsSD.ServiceAdded += dnsSD_ServiceAdded;
            dnsSD.Browse("_rdmNet._udp", "local");
        }

        void dnsSD_ServiceAdded(object o, ServiceBrowseEventArgs args)
        {
            args.Service.Resolved += delegate(object sender, ServiceResolvedEventArgs e)
            {
                IResolvableService s = (IResolvableService)e.Service;
                foreach (IPAddress address in s.HostEntry.AddressList)
                {

                    RdmEndPoint controlEndpoint = new RdmEndPoint(new IPEndPoint(address, RdmNetSocket.RdmNetPort), 0) { Id = UId.ParseUrl(s.TxtRecord["id"].ValueString) };
                    ControlEndpoints.Add(controlEndpoint);
                    rdmNetSocket.AddKnownDevice(controlEndpoint);
                    DiscoverEndpoints(controlEndpoint);
                }
            };
            args.Service.Resolve();
        }

        void rdmNetSocket_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.WriteLine(((Exception) e.ExceptionObject).Message);
        }

        void acnSocket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            if (e.Packet is EndpointList.Reply)
                ProcessEndpointList(e.Source, e.Packet);
            if (e.Packet is EndpointListChange.Reply)
                ProcessEndpointListChange(e.Source, e.Packet);

            if (e.Packet is EndpointDevices.Reply)
                ProcessDeviceList(e.Source, e.Packet);           
            if (e.Packet is EndpointDeviceListChange.Reply)
                ProcessDeviceListChange(e.Source, e.Packet);
        }

        void slpUser_ServiceFound(object sender, ServiceFoundEventArgs e)
        {
            if(NewDeviceFound != null)
            {
                foreach (UrlEntry url in e.Urls)
                {
                    RdmEndPoint controlEndpoint = new RdmEndPoint(new IPEndPoint(e.Address.Address, RdmNetSocket.RdmNetPort),0) { Id = UId.ParseUrl(url.Url) };
                    ControlEndpoints.Add(controlEndpoint);
                    rdmNetSocket.AddKnownDevice(controlEndpoint);
                    DiscoverEndpoints(controlEndpoint);
                }
            }
        }

        private void DiscoverEndpoints(RdmEndPoint endpoint)
        {
            EndpointList.Get getEndpoints = new EndpointList.Get();
            getEndpoints.Header.DestinationId = endpoint.Id;
            rdmNetSocket.SendRdm(getEndpoints, endpoint, endpoint.Id);
        }

        public void  Stop()
        {
            if (reliableSocket != null)
            {
                reliableSocket.Dispose();
                reliableSocket = null;
            }                

            if (slpUser != null)
                slpUser.Close();

            if (rdmNetSocket != null)
            {
                rdmNetSocket.Close();
                rdmNetSocket = null;
            }
                
        }

        public void Discover(DiscoveryType type)
        {
            switch(type)
            {
                case DiscoveryType.DeviceDiscovery:
                    foreach(RdmEndPoint endpoint in DiscoveredEndpoints)
                        StartRdmDiscovery(endpoint);
                    break;
            }
        }

        private void StartRdmDiscovery(RdmEndPoint endpoint)
        {
            DiscoveryState.Set request = new DiscoveryState.Set();
            request.EndpointID = (short) endpoint.Universe;
            request.DiscoveryState = DiscoveryState.DiscoveryStates.Full;

            rdmNetSocket.SendRdm(request, new RdmEndPoint(endpoint,0), endpoint.Id);
        }

        private RdmReliableSocket reliableSocket = null;

        public IRdmSocket Socket
        {
            get 
            {
                if (reliableSocket == null && rdmNetSocket != null)
                    reliableSocket = new RdmReliableSocket(rdmNetSocket);

                return reliableSocket;
            }
        }

        public event EventHandler Starting;

        public event EventHandler Stoping;

        private HashSet<RdmEndPoint> controlEndpoints = new HashSet<RdmEndPoint>(new RdmEndpointComparer());

        public HashSet<RdmEndPoint> ControlEndpoints
        {
            get { return controlEndpoints; }
            set { controlEndpoints = value; }
        }

        private HashSet<RdmEndPoint> discoveredEndpoints = new HashSet<RdmEndPoint>(new RdmEndpointComparer());

        public HashSet<RdmEndPoint> DiscoveredEndpoints
        {
            get { return discoveredEndpoints; }
            set { discoveredEndpoints = value; }
        }

        #region RDM Message Handlers

        private void ProcessEndpointList(IPEndPoint endpoint, RdmPacket packet)
        {
            EndpointList.Reply reply = packet as EndpointList.Reply;
            if (reply != null)
            {
                foreach(int endpointId in reply.EndpointIDs)
                {
                    RdmEndPoint target = new RdmEndPoint(endpoint, endpointId) { Id = packet.Header.SourceId };
                    DiscoveredEndpoints.Add(target);

                    EndpointDevices.Get request = new EndpointDevices.Get();
                    request.EndpointID = (short) endpointId;
                    if(Socket != null) Socket.SendRdm(request, new RdmEndPoint(endpoint, 0), packet.Header.SourceId);
                }
            }
        }

        private void ProcessEndpointListChange(IPEndPoint endpoint, RdmPacket packet)
        {
            EndpointListChange.Reply reply = packet as EndpointListChange.Reply;
            if (reply != null)
            {
                DiscoverEndpoints(new RdmEndPoint(endpoint));
            }
        }

        private void ProcessDeviceList(IPEndPoint endpoint, RdmPacket packet)
        {
            EndpointDevices.Reply reply = packet as EndpointDevices.Reply;
            if (reply != null)
            {
                RdmEndPoint source = new RdmEndPoint(endpoint, reply.EndpointID);
                foreach (UId id in reply.DeviceIds)
                {
                    NewDeviceFound(this, new DeviceFoundEventArgs(id, source));
                }
            }
        }

        private void ProcessDeviceListChange(IPEndPoint endpoint, RdmPacket packet)
        {
            EndpointDeviceListChange.Reply reply = packet as EndpointDeviceListChange.Reply;
            if (reply != null && Socket != null)
            {
                RdmEndPoint source = new RdmEndPoint(endpoint, reply.EndpointID);

                EndpointDevices.Get request = new EndpointDevices.Get();
                request.EndpointID = reply.EndpointID;
                Socket.SendRdm(request, new RdmEndPoint(endpoint, 0), packet.Header.SourceId);
            }
        }

        #endregion

    }
}

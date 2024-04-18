//#define SLP_Discovery
#define mDNS_Discovery

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.Slp;
using System.Net;
using LXProtocols.Acn.Sockets;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.Rdm.Routing;
using LXProtocols.Acn.RdmNet.Sockets;
using LXProtocols.Acn.Rdm.Packets.Net;
using LXProtocols.Acn.Rdm.Broker;
using System.Diagnostics;

#if mDNS_Discovery
using Mono.Zeroconf;
#endif

namespace RdmSnoop.Transports
{
    public class RdmNet : ISnoopTransport
    {
        private SlpUserAgent slpUser = null;
        private RdmNetMeshSocket rdmNetSocket = null;
#if mDNS_Discovery
        private ServiceBrowser dnsSD = null;
        #endif

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

            if (rdmNetSocket == null || !rdmNetSocket.PortOpen)
            {
                rdmNetSocket = new RdmNetMeshSocket(UId.NewUId(0xFF), Guid.NewGuid(), "RDM Snoop");
                rdmNetSocket.UnhandledException += rdmNetSocket_UnhandledException;
                rdmNetSocket.NewRdmPacket += acnSocket_NewRdmPacket;
                rdmNetSocket.DeviceFound += RdmNetSocket_DeviceFound;
                rdmNetSocket.Open(new IPEndPoint(LocalAdapter,0));
            }

#if SLP_Discovery   
            slpUser = new SlpUserAgent("ACN-DEFAULT");
            slpUser.NetworkAdapter = localAdapter;
            slpUser.ServiceFound += new EventHandler<ServiceFoundEventArgs>(slpUser_ServiceFound);
            slpUser.Open();
            slpUser.Find("service:rdmnet-device");
#endif

#if mDNS_Discovery
            dnsSD = new ServiceBrowser();
            dnsSD.ServiceAdded += dnsSD_ServiceAdded;
            dnsSD.Browse("_rdmNet._tcp", "local");
            #endif
        }

        private void RdmNetSocket_DeviceFound(object sender, NewRdmNetDeviceEventArgs e)
        {
            DiscoverEndpoints(e.DeviceEndpoint);
        }

#if mDNS_Discovery
        void dnsSD_ServiceAdded(object o, ServiceBrowseEventArgs args)
        {
            args.Service.Resolved += delegate(object sender, ServiceResolvedEventArgs e)
            {
                IResolvableService s = (IResolvableService)e.Service;
                foreach (IPAddress address in s.HostEntry.AddressList)
                {
                    RdmEndPoint controlEndpoint = new RdmEndPoint(new IPEndPoint(address, 8888), 0) { Id = UId.ParseUrl(s.TxtRecord["CID"].ValueString) };
                    ControlEndpoints.Add(controlEndpoint);
                    rdmNetSocket.AddBroker(controlEndpoint);
                    DiscoverEndpoints(controlEndpoint);
                }
            };
            args.Service.Resolve();
        }
#endif

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
                    rdmNetSocket.AddBroker(controlEndpoint);
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
            {
                slpUser.Dispose();
                slpUser = null;
            }

#if mDNS_Discovery
            if(dnsSD != null)
            {
                dnsSD.Dispose();
                dnsSD = null;
            }
#endif                

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

        public IEnumerable<IRdmSocket> Sockets
        {
            get 
            {
                if (reliableSocket == null && rdmNetSocket != null)
                    reliableSocket = new RdmReliableSocket(rdmNetSocket);

                return Enumerable.Repeat(reliableSocket,1);
            }
        }

        public event EventHandler Starting;

        protected void RaiseStarting()
        {
            if (Starting != null)
                Starting(this, EventArgs.Empty);
        }

        public event EventHandler Stoping;

        protected void RaiseStoping()
        {
            if (Stoping != null)
                Stoping(this, EventArgs.Empty);
        }

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

                    foreach(var socket in Sockets)
                        socket.SendRdm(request, new RdmEndPoint(endpoint, 0), packet.Header.SourceId);
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
            if (reply != null)
            {
                RdmEndPoint source = new RdmEndPoint(endpoint, reply.EndpointID);

                EndpointDevices.Get request = new EndpointDevices.Get();
                request.EndpointID = reply.EndpointID;
                foreach(var socket in Sockets)
                    socket.SendRdm(request, new RdmEndPoint(endpoint, 0), packet.Header.SourceId);
            }
        }

        #endregion



        public int ResolveEndpointToUniverse(RdmEndPoint address)
        {
            throw new NotImplementedException();
        }
    }
}

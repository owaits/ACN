//#define SLP_Discovery
#define mDNS_Discovery

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Acn.Rdm;
using Acn.Rdm.Packets.Net;
using Acn.Rdm.Packets.Product;
using Acn.Rdm.Routing;
using Acn.RdmNet.Sockets;
using Acn.Slp;
using Acn.Sockets;
using Mono.Zeroconf;

namespace StreamingAcn.RdmNet
{
    public class RdmNetEndPointExplorer
    {
        private SlpUserAgent slpUser = null;
        private RdmNetSocket acnSocket = null;
#if mDNS_Discovery
        private ServiceBrowser dnsSD = null;
#endif

        public event EventHandler NewEndpointFound;

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
                        if (acnSocket == null || !acnSocket.PortOpen)
            {
                acnSocket = new RdmNetSocket(UId.NewUId(0xFF), Guid.NewGuid(), "RDM Snoop");
                acnSocket.NewRdmPacket += acnSocket_NewRdmPacket;
                acnSocket.Open(new IPEndPoint(LocalAdapter, 0));
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
            dnsSD.Browse("_rdmnet._udp", "local");
#endif
        }

#if mDNS_Discovery
        void dnsSD_ServiceAdded(object o, ServiceBrowseEventArgs args)
        {
            args.Service.Resolved += delegate(object sender, ServiceResolvedEventArgs e)
            {
                IResolvableService s = (IResolvableService)e.Service;
                foreach (IPAddress address in s.HostEntry.AddressList)
                {

                    RdmEndPoint controlEndpoint = new RdmEndPoint(new IPEndPoint(address, RdmNetSocket.RdmNetPort), 0) { Id = UId.ParseUrl(s.TxtRecord["id"].ValueString) };
                    ControlEndpoints.Add(controlEndpoint);
                    DiscoverEndpoints(controlEndpoint);
                }
            };
            args.Service.Resolve();
        }
#endif

        void acnSocket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            if (e.Packet is ManufacturerLabel.GetReply)
                ProcessManufacturerLabel(e.Source, e.Packet);
            if (e.Packet is DeviceLabel.GetReply)
                ProcessDeviceLabel(e.Source, e.Packet);
            if (e.Packet is EndpointLabel.GetReply)
                ProcessEndpointLabel(e.Source, e.Packet);
            if (e.Packet is EndpointMode.GetReply)
                ProcessEndpointMode(e.Source, e.Packet);
            if (e.Packet is EndpointToUniverse.GetReply)
                ProcessEndpointUniverse(e.Source, e.Packet);
            if (e.Packet is EndpointList.Reply)
                ProcessEndpointList(e.Source, e.Packet);
        }

        void slpUser_ServiceFound(object sender, ServiceFoundEventArgs e)
        {
            foreach (UrlEntry url in e.Urls)
            {
                RdmEndPoint controlEndpoint = new RdmEndPoint(new IPEndPoint(e.Address.Address, RdmNetSocket.RdmNetPort), 0) { Id = UId.ParseUrl(url.Url) };
                ControlEndpoints.Add(controlEndpoint);
                DiscoverEndpoints(controlEndpoint);
            }
        }

        private void DiscoverEndpoints(RdmEndPoint endpoint)
        {
            EndpointList.Get getEndpoints = new EndpointList.Get();
            getEndpoints.Header.DestinationId = endpoint.Id;
            acnSocket.SendRdm(getEndpoints, endpoint, endpoint.Id);
        }

        public void Stop()
        {
            if (reliableSocket != null)
                reliableSocket.Dispose();

            if (slpUser != null)
                slpUser.Close();

            if (acnSocket != null)
                acnSocket.Close();
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

        private HashSet<RdmEndPoint> controlEndpoints = new HashSet<RdmEndPoint>(new RdmEndpointComparer());

        public HashSet<RdmEndPoint> ControlEndpoints
        {
            get { return controlEndpoints; }
        }

        private HashSet<RdmNetEndPoint> discoveredEndpoints = new HashSet<RdmNetEndPoint>(new RdmNetEndpointComparer());

        public HashSet<RdmNetEndPoint> DiscoveredEndpoints
        {
            get { return discoveredEndpoints; }
        }

        protected void RegisterEndpoint(RdmNetEndPoint endpoint)
        {
            lock (DiscoveredEndpoints)
            {
                if (!DiscoveredEndpoints.Contains(endpoint))
                {
                    endpoint.PropertyChanged += endpoint_PropertyChanged;
                    DiscoveredEndpoints.Add(endpoint);
                }
            }

        }

        void endpoint_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RdmNetEndPoint endpoint = sender as RdmNetEndPoint;
            if(endpoint != null)
            {
                if (e.PropertyName == "AcnUniverse")
                    SetEndpointUniverse(endpoint);
                if (e.PropertyName == "Direction")
                    SetEndpointMode(endpoint);
                if (e.PropertyName == "PortLabel")
                    SetEndpointLabel(endpoint);
            }
        }

        private void SetEndpointMode(RdmNetEndPoint endpoint)
        {
            EndpointMode.Set setMode = new EndpointMode.Set();
            setMode.EndpointID = (short)endpoint.Port;
            setMode.EndpointMode = endpoint.Direction;

            acnSocket.SendRdm(setMode, new RdmEndPoint(endpoint,0), endpoint.Id);
        }

        private void SetEndpointUniverse(RdmNetEndPoint endpoint)
        {
            EndpointToUniverse.Set setUniverse = new EndpointToUniverse.Set();
            setUniverse.EndpointID = (short) endpoint.Universe;
            setUniverse.UniverseNumber = (short) endpoint.AcnUniverse;

            acnSocket.SendRdm(setUniverse, new RdmEndPoint(endpoint,0), endpoint.Id);
        }

        private void SetEndpointLabel(RdmNetEndPoint endpoint)
        {
            EndpointLabel.Set setLabel = new EndpointLabel.Set();
            setLabel.EndpointID = (short)endpoint.Port;
            setLabel.Label = endpoint.PortLabel;

            acnSocket.SendRdm(setLabel, new RdmEndPoint(endpoint, 0), endpoint.Id);
        }

        #region RDM Message Handlers

        private void ProcessManufacturerLabel(IPEndPoint endpoint, RdmPacket packet)
        {
            ManufacturerLabel.GetReply reply = packet as ManufacturerLabel.GetReply;
            if (reply != null)
            {
                lock (DiscoveredEndpoints)
                {
                    foreach (RdmNetEndPoint port in DiscoveredEndpoints)
                    {
                        if (port.Id.Equals(packet.Header.SourceId))
                            port.ManufacturerLabel = reply.Label;
                    }
                }
            }
        }

        private void ProcessDeviceLabel(IPEndPoint endpoint, RdmPacket packet)
        {
            DeviceLabel.GetReply reply = packet as DeviceLabel.GetReply;
            if (reply != null)
            {
                lock (DiscoveredEndpoints)
                {
                    foreach (RdmNetEndPoint port in DiscoveredEndpoints)
                    {
                        if (port.Id.Equals(packet.Header.SourceId))
                            port.DeviceLabel = reply.Label;
                    }
                }
            }
        }

        private void ProcessEndpointLabel(IPEndPoint endpoint, RdmPacket packet)
        {
            EndpointLabel.GetReply reply = packet as EndpointLabel.GetReply;
            if (reply != null)
            {
                lock (DiscoveredEndpoints)
                {
                    foreach (RdmNetEndPoint port in DiscoveredEndpoints)
                    {
                        if (port.Id.Equals(packet.Header.SourceId) && port.Universe == reply.EndpointID)
                            port.PortLabel = reply.Label;
                    }
                }
            }
        }

        private void ProcessEndpointMode(IPEndPoint endpoint, RdmPacket packet)
        {
            EndpointMode.GetReply reply = packet as EndpointMode.GetReply;
            if (reply != null)
            {
                lock (DiscoveredEndpoints)
                {
                    foreach (RdmNetEndPoint port in DiscoveredEndpoints)
                    {
                        if (port.Id.Equals(packet.Header.SourceId) && port.Universe == reply.EndpointID)
                            port.Direction = reply.EndpointMode;
                    }
                }
            }
        }

        private void ProcessEndpointUniverse(IPEndPoint endpoint, RdmPacket packet)
        {
            EndpointToUniverse.GetReply reply = packet as EndpointToUniverse.GetReply;
            if (reply != null)
            {
                lock (DiscoveredEndpoints)
                {
                    foreach (RdmNetEndPoint port in DiscoveredEndpoints)
                    {
                        if (port.Id.Equals(packet.Header.SourceId) && port.Universe == reply.EndpointID)
                            port.AcnUniverse = reply.UniverseNumber;
                    }
                }
            }
        }


        private void ProcessEndpointList(IPEndPoint endpoint, RdmPacket packet)
        {
            EndpointList.Reply reply = packet as EndpointList.Reply;
            if (reply != null)
            {
                foreach (int endpointId in reply.EndpointIDs)
                {
                    RdmNetEndPoint target = new RdmNetEndPoint(endpoint, endpointId) { Id = packet.Header.SourceId };
                    RegisterEndpoint(target);

                    NewEndpointFound(this, EventArgs.Empty);

                    InterogateEndpoint(packet.Header.SourceId, target);
                }
            }
        }



        private void InterogateEndpoint(UId targetId, RdmNetEndPoint endpoint)
        {
            ManufacturerLabel.Get getManufacturerLabel = new ManufacturerLabel.Get();
            acnSocket.SendRdm(getManufacturerLabel, new RdmEndPoint(endpoint, 0), targetId);

            DeviceLabel.Get getDeviceLabel = new DeviceLabel.Get();
            acnSocket.SendRdm(getDeviceLabel, new RdmEndPoint(endpoint, 0), targetId);

            EndpointLabel.Get getLabel = new EndpointLabel.Get();
            getLabel.EndpointID = (short) endpoint.Universe;
            acnSocket.SendRdm(getLabel, new RdmEndPoint(endpoint, 0), targetId);

            EndpointMode.Get getMode = new EndpointMode.Get();
            getMode.EndpointID = (short)endpoint.Universe;
            acnSocket.SendRdm(getMode, new RdmEndPoint(endpoint, 0), targetId);

            EndpointToUniverse.Get getUniverse = new EndpointToUniverse.Get();
            getUniverse.EndpointID = (short)endpoint.Universe;
            acnSocket.SendRdm(getUniverse, new RdmEndPoint(endpoint, 0), targetId);
        }

        #endregion
    }
}

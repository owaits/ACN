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

namespace RdmSnoop.Transports
{
    public class RdmNet : ISnoopTransport
    {
        private SlpUserAgent slpUser = new SlpUserAgent("ACN-DEFAULT");
        private RdmNetSocket acnSocket = null;

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
                acnSocket = new RdmNetSocket(UId.NewUId(0xFF), Guid.NewGuid(), "RDM Snoop");
                acnSocket.NewRdmPacket += acnSocket_NewRdmPacket;
                acnSocket.Open(LocalAdapter);
            }

            slpUser.Open();
            slpUser.Find("service:e133.esta");
        }

        void acnSocket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            if (e.Packet is EndpointDevices.Reply)
                ProcessDeviceList(e.Source, e.Packet);
            if (e.Packet is EndpointList.Reply)
                ProcessEndpointList(e.Source, e.Packet);
        }

        void slpUser_ServiceFound(object sender, ServiceFoundEventArgs e)
        {
            if(NewDeviceFound != null)
            {
                foreach (UrlEntry url in e.Urls)
                {
                    RdmEndPoint controlEndpoint = new RdmEndPoint(e.Address.Address, 0) { Id = UId.ParseUrl(url.Url) };
                    ControlEndpoints.Add(controlEndpoint);
                    DiscoverEndpoints(controlEndpoint);
                }
            }
        }

        private void DiscoverEndpoints(RdmEndPoint endpoint)
        {
            EndpointList.Get getEndpoints = new EndpointList.Get();
            getEndpoints.Header.DestinationId = endpoint.Id;
            acnSocket.SendRdm(getEndpoints, endpoint, endpoint.Id);
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

            acnSocket.SendRdm(request, new RdmEndPoint(endpoint,0), endpoint.Id);
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
                    acnSocket.SendRdm(request, new RdmEndPoint(endpoint, 0), packet.Header.SourceId);
                }
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

        #endregion

    }

    public class RdmEndpointComparer:IEqualityComparer<RdmEndPoint>
    {

        public bool Equals(RdmEndPoint x, RdmEndPoint y)
        {
            return x.Id.Equals(y.Id) && x.Universe.Equals(y.Universe);
        }

        public int GetHashCode(RdmEndPoint obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}

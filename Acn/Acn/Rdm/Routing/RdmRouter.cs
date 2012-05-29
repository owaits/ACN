using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Sockets;

namespace Acn.Rdm.Routing
{
    public class RdmRouter:IRdmTransport
    {
        #region Setup and Initialisation

        public RdmRouter()
        {
            socket = new RdmRoutingSocket(this);
        }

        private bool running = false;

        public bool Running
        {
            get { return running; }
        }

        private RdmRoutingSocket socket;
        
        public IRdmSocket Socket
        {
            get { return socket; }
        }

        public void Start()
        {
            if (transports.Count == 0)
                throw new InvalidOperationException("You must register a transport before calling start. Did you forget to register a transport?");
            
            running = true;

            foreach (RdmRouteBinding binding in transports.Values)
            {
                binding.Transport.Start();
                socket.Bind(binding.Transport.Socket);
            }
        }


        public void Stop()
        {
            running = false;

            foreach (RdmRouteBinding binding in transports.Values)
            {
                binding.Transport.Stop();
                socket.UnBind(binding.Transport.Socket);
            }
        }

        #endregion

        #region Transports

        private Dictionary<int, RdmRouteBinding> transports = new Dictionary<int, RdmRouteBinding>();

        public IEnumerable<RdmRouteBinding> Transports
        {
            get
            {
                return transports.Values;
            }
        }

        public void RegisterTransport(IRdmTransport transport, string name, string description, int priority)
        {
            RegisterTransport(new RdmRouteBinding(transport, name, description, priority));
        }

        public void RegisterTransport(RdmRouteBinding routeDescription)
        {
            if (running)
                new InvalidOperationException("The router is already running. You can not register a transport while the router is running. You must Stop it first.");

            if (transports.ContainsKey(routeDescription.Priority))
                new InvalidOperationException("A transport with the priority {0} already exists. Each transport must have a unique priority.");

            if (transports.Values.Any <RdmRouteBinding>(item => item.Transport == routeDescription.Transport))
                new InvalidOperationException("This transport has already been registered. You may only register a transport once.");

            routeDescription.Transport.NewDeviceFound += new EventHandler<DeviceFoundEventArgs>(Transport_NewDeviceFound);

            lock (transports)
            {
                transports.Add(routeDescription.Priority, routeDescription);
            }
        }

        void Transport_NewDeviceFound(object sender, DeviceFoundEventArgs e)
        {
            RdmRouteBinding binding = Transports.FirstOrDefault<RdmRouteBinding>(item => item.Transport == sender);
            if (binding != null)
                RegisterDevice(e.Id, binding);

            RaiseNewDeviceFound(e);
        }

        #endregion

        #region Device Discovery

        private Dictionary<UId, RdmRouteBinding> deviceTable = new Dictionary<UId, RdmRouteBinding>();

        public IEnumerable<UId> Devices
        {
            get { return deviceTable.Keys; }
        }

        protected void RegisterDevice(UId deviceId, RdmRouteBinding transport)
        {
            lock (deviceTable)
            {
                RdmRouteBinding existingTransport;
                if (deviceTable.TryGetValue(deviceId, out existingTransport))
                {
                    //Already registered.
                    if (existingTransport == transport)
                        return;

                    //Only register transports with a higher priority.
                    if (existingTransport.Priority >= transport.Priority)
                        return;
                }

                //Register device in device table.
                deviceTable[deviceId] = transport;
            }
        }

        public void Discover()
        {
            //Ask each transport to discover.
            foreach (RdmRouteBinding binding in transports.Values)
                binding.Transport.Discover();
        }

        #endregion

        #region Routing

        internal List<RdmRouteBinding> GetTransportsRoutes(UId targetId)
        {
            List<RdmRouteBinding> transportsToUse = new List<RdmRouteBinding>();

            if (targetId == UId.Broadcast)
            {
                lock (transports)
                {
                    transportsToUse.AddRange(transports.Values);
                }
            }
            else
            {
                lock (deviceTable)
                {
                    //Obtain routed transport.
                    RdmRouteBinding transportRoute;
                    if (deviceTable.TryGetValue(targetId, out transportRoute))
                    {
                        transportsToUse.Add(transportRoute);
                    }
                }
            }

            return transportsToUse;
        }

        #endregion

        #region Events

        public event EventHandler<DeviceFoundEventArgs> NewDeviceFound;

        protected void RaiseNewDeviceFound(DeviceFoundEventArgs args)
        {
            if (NewDeviceFound != null)
                NewDeviceFound(this, args);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.Sockets;

namespace LXProtocols.Acn.Rdm.Routing
{
    public class RdmRouter:IRdmTransport
    {
        #region Setup and Initialisation

        public RdmRouter()
        {
            sockets.Add(new RdmRoutingSocket(this));
        }

        private bool running = false;

        public bool Running
        {
            get { return running; }
        }

        private List<RdmRoutingSocket> sockets = new List<RdmRoutingSocket>();
        
        public IEnumerable<IRdmSocket> Sockets
        {
            get { return sockets; }
        }

        public void Start()
        {
            if (transports.Count == 0)
                throw new InvalidOperationException("You must register a transport before calling start. Did you forget to register a transport?");
            
            running = true;

            foreach (RdmRouteBinding binding in transports.Values)
            {
                binding.Transport.Start();
                Bind(binding);
            }
        }


        public void Stop()
        {
            running = false;

            foreach (RdmRouteBinding binding in transports.Values)
            {
                binding.Transport.Stop();
                UnBind(binding);
            }
        }

        internal void Bind(RdmRouteBinding binding)
        {
            if(running)
            {
                foreach (IRdmSocket transportSocket in binding.Transport.Sockets)
                {
                    foreach(var socket in sockets)
                        socket.Bind(transportSocket);
                }                    
            }
                
        }

        internal void UnBind(RdmRouteBinding binding)
        {
            if (running)
            {
                foreach(IRdmSocket transportSocket in binding.Transport.Sockets)
                {
                    foreach(var socket in sockets)
                        socket.UnBind(transportSocket);
                }                    
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
            RegisterTransport(new RdmRouteBinding(this,transport, name, description, priority));
        }

        public void RegisterTransport(RdmRouteBinding routeDescription)
        {
            if (running)
                throw new InvalidOperationException("The router is already running. You can not register a transport while the router is running. You must Stop it first.");

            if (transports.ContainsKey(routeDescription.Priority))
                throw new InvalidOperationException("A transport with the priority {0} already exists. Each transport must have a unique priority.");

            if (transports.Values.Any <RdmRouteBinding>(item => item.Transport == routeDescription.Transport))
                throw new InvalidOperationException("This transport has already been registered. You may only register a transport once.");

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

        public void Discover(DiscoveryType type)
        {
            //Ask each transport to discover.
            foreach (RdmRouteBinding binding in transports.Values)
                binding.Transport.Discover(type);
        }

        #endregion

        #region Routing

        /// <summary>
        /// Gets the current transport in use for a specific device.
        /// </summary>
        /// <param name="targetId">The id of the device to return the transport for.</param>
        /// <returns>The RDM transport for the device.</returns>
        /// <exception cref="InvalidOperationException">The target id is a broadcast id or another invalid id.</exception>
        public IRdmTransport GetTransportForDevice(UId targetId)
        {
            if (targetId == UId.Broadcast)
                throw new InvalidOperationException("Target device UId can not be broadcast.");

            RdmRouteBinding route = GetTransportsRoutes(targetId).FirstOrDefault();
            return route != null ? route.Transport : null;
        }

        /// <summary>
        /// Gets the transport binding for a specific device.
        /// </summary>
        /// <remarks>
        /// Each device will have a transport binding which determines the transport used to comunicate with that device.
        /// </remarks>
        /// <param name="targetId">The ID of the device we wish to get the binding for.</param>
        /// <returns>The transport binding for the specified device.</returns>
        public RdmRouteBinding GetBindingForDevice(UId targetId)
        {
            if (targetId == UId.Broadcast)
                throw new InvalidOperationException("Target device UId can not be broadcast.");

            RdmRouteBinding route = GetTransportsRoutes(targetId).FirstOrDefault();
            return route;
        }

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

        public event EventHandler Starting;

        public event EventHandler Stoping;


        /// <summary>
        /// Gets the universe index for a specific endpoint address.
        /// </summary>
        /// <param name="address">The device address.</param>
        /// <returns>
        /// The DMX universe that this address resolves to.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">No transport exists for this device address.</exception>
        /// <remarks>
        /// The transport may wish to resolve this to an internal addressing scheme.
        /// </remarks>
        public int ResolveEndpointToUniverse(RdmEndPoint address)
        {
            IRdmTransport transport = GetTransportForDevice(address.Id);
            if (transport == null)
                throw new InvalidOperationException("No transport exists for this device address.");
            return transport.ResolveEndpointToUniverse(address);
        }
    }
}

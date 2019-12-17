using LXProtocols.TCNet.IO;
using LXProtocols.TCNet.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LXProtocols.TCNet.Sockets
{
    /// <summary>
    /// The network socket for sending and recieving data in teh DJ Tap protocol.
    /// </summary>
    /// <remarks>
    /// This socket allows you to connect to the network, advertise your prescence and send and recieve data.
    /// </remarks>
    /// <seealso cref="System.Net.Sockets.Socket" />
    public class TCNetSocket: IDisposable
    {
        public const int MaxPacketSize = 5000;
        public const int ManagementPort = 60000;
        public const int ApplicationSpecificPort = 60001;
        public const int TimecodeStreamPort = 60002;


        public event UnhandledExceptionEventHandler UnhandledException;
        public event EventHandler<NewPacketEventArgs<TCNetPacket>> NewPacket;

        private Socket managementSocket;
        private Socket nodeSocket;
        private Socket applicationSpcificSocket;
        private Socket timecodeStreamSocket;
        private Socket unicastTXSocket;

        private TraceSource tcNetTrace = new TraceSource("LXProtocols.TCNet");

        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetSocket"/> class.
        /// </summary>
        public TCNetSocket(NodeType role)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            NodeId = (ushort) rnd.Next(ushort.MaxValue);
            this.Role = role;

            managementSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            nodeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            applicationSpcificSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            timecodeStreamSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            unicastTXSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            this.NewPacket += TCNetSocket_NewPacket;
        }

        /// <summary>
        /// Handles the NewPacket event of the TCNetSocket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NewPacketEventArgs{TCNetPacket}"/> instance containing the event data.</param>
        private void TCNetSocket_NewPacket(object sender, NewPacketEventArgs<TCNetPacket> e)
        {

            TCNetHeader header = e.Packet as TCNetHeader;
            DataTypes dataType = DataTypes.None;
            if(header != null)
            {
                TraceEventType traceLevel = TraceEventType.Information;
                switch (header.MessageType)
                {
                    case MessageTypes.OptIn:
                        ProcessOptIn(e.Source, (TCNetOptIn) e.Packet);
                        traceLevel = TraceEventType.Verbose;
                        break;
                    case MessageTypes.OptOut:
                        ProcessOptOut((TCNetOptOut)e.Packet);
                        break;
                    case MessageTypes.Time:
                        traceLevel = TraceEventType.Verbose;
                        break;
                    case MessageTypes.Data:
                        {
                            TCNetDataHeader dataHeader = header as TCNetDataHeader;
                            if (dataHeader != null)
                            {
                                dataType = dataHeader.DataType;
                                switch(dataHeader.DataType)
                                {
                                    case DataTypes.Metrics:
                                        traceLevel = TraceEventType.Verbose;
                                        break;
                                }
                            }
                                
                        }
                        break;
                }

                tcNetTrace.TraceEvent(traceLevel, (int) header.MessageType, $"{header.NodeName}: {header.GetType().Name} {(dataType != DataTypes.None? dataType.ToString() :string.Empty)}");
            }
        }

        /// <summary>
        /// Called when [unhandled exception].
        /// </summary>
        /// <param name="ex">The ex.</param>
        protected void OnUnhandledException(Exception ex)
        {
            if (UnhandledException != null) UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }


        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Net.Sockets.Socket" />, and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        public void Dispose()
        {
            StopTimeSync();
            StopAdvertising();
            PortOpen = false;

            managementSocket.Dispose();
            nodeSocket.Dispose();
            applicationSpcificSocket.Dispose();
            timecodeStreamSocket.Dispose();
            unicastTXSocket.Dispose();
        }

        #endregion

        #region Information

        private bool portOpen= false;

        /// <summary>
        /// Gets or sets whether this socket is currently open.
        /// </summary>
        public bool PortOpen
        {
            get { return portOpen; }
            set { portOpen = value; }
        }

        /// <summary>
        /// Gets or sets the IP address of the local adapter to use.
        /// </summary>
        public IPAddress LocalIP { get; protected set; }

        /// <summary>
        /// Gets or sets the subnet mask of the local adapter to use.
        /// </summary>
        public IPAddress LocalSubnetMask { get; protected set; }

        /// <summary>
        /// Gets the IP endpoint used for Unicast traffic.
        /// </summary>
        public IPEndPoint UnicastLocalEndpoint
        {
            get
            {
                if (nodeSocket == null)
                    return null;
                return nodeSocket.LocalEndPoint as IPEndPoint;
            }
        }


        private int sequenceNumber = 0;

        /// <summary>
        /// Increments the packet step number and returns the next step number to use.
        /// </summary>
        /// <returns></returns>
        public byte NextSequenceNumber()
        {
            try
            {
                return (byte) sequenceNumber;
            }
            finally
            {
                sequenceNumber++;
                if (sequenceNumber > byte.MaxValue)
                    sequenceNumber = 0;
            }
        }

        /// <summary>
        /// Determines the broadcast address for the current subnet from the local adapter IP and mask.
        /// </summary>
        /// <param name="address">The IP address within the subnet.</param>
        /// <param name="subnetMask">The subnet mask for the subnet.</param>
        /// <returns>The broadcast address for the requested subnet.</returns>
        /// <exception cref="System.ArgumentException">Lengths of IP address and subnet mask do not match.</exception>
        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        /// <summary>
        /// Gets the broadcast address when sending UDP broadcast on the subnet.
        /// </summary>
        public IPAddress BroadcastAddress
        {
            get
            {
                if (LocalSubnetMask == null)
                    return IPAddress.Broadcast;
                return GetBroadcastAddress(LocalIP, LocalSubnetMask);
            }
        }

        private DateTime? lastPacket = null;

        /// <summary>
        /// Gets or sets the date and time of the last packet recieved by this socket.
        /// </summary>
        public DateTime? LastPacket
        {
            get { return lastPacket; }
            protected set { lastPacket = value; }
        }

        /// <summary>
        /// Unique Node ID. When multiple applications/services are running on same IP, this number must be unique.
        /// </summary>
        ///  <remarks>
        /// The node ID will be automatically appended to packets being sent.
        /// </remarks>
        public ushort NodeId { get; set; }

        /// <summary>
        /// Gets or sets the role being assumed by the sending device.
        /// </summary>
        /// <remarks>
        /// The role will be automatically appended to packets being sent.
        /// </remarks>
        public NodeType Role { get; set; }

        private string nodeName = string.Empty;

        /// <summary>
        /// GW Code of software/machine/source that sends packet. (8 Characters)
        /// </summary>
        /// <exception cref="System.ArgumentException">Node name must be no longer than 8 characters.</exception>
        public string NodeName
        {
            get { return nodeName; }
            set
            {
                if (value.Length > 8)
                    throw new ArgumentException("Node name must be no longer than 8 characters.");
                nodeName = value;
            }
        }

        private string vendorName = string.Empty;

        /// <summary>
        /// Gets or sets the brand to use when advertising ourselves on the DJ tap network.
        /// </summary>
        public string VendorName
        {
            get { return vendorName; }
            set
            {
                if (value.Length > 16)
                    throw new ArgumentException("Vendor name must be no longer than 16 characters.");
                vendorName = value;
            }
        }

        private string deviceName = string.Empty;

        /// <summary>
        /// Gets or sets the model to use when advertising ourselves on the DJ tap network.
        /// </summary>
        public string DeviceName
        {
            get { return deviceName; }
            set
            {
                if (value.Length > 16)
                    throw new ArgumentException("Device name must be no longer than 16 characters.");
                deviceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the device specific version.
        /// </summary>
        public Version DeviceVersion { get; set; }

        #endregion

        #region Communications


        /// <summary>
        /// Opens this socket and starts comunications on the specified network adapter.
        /// </summary>
        /// <param name="localIp">The local ip address of the network adapter to open this socket on.</param>
        /// <param name="localSubnetMask">The local subnet mask of the network adapter to open this socket on.</param>
        public void Open(IPAddress localIp, IPAddress localSubnetMask)
        {
            if (DeviceVersion == null) throw new InvalidOperationException("The device version has not been set this must be set before opening the socket.");
            if (string.IsNullOrEmpty(NodeName)) throw new InvalidOperationException("The node name has not been set this must be set before opening the socket.");
            if (string.IsNullOrEmpty(VendorName)) throw new InvalidOperationException("The vendor name has not been set this must be set before opening the socket.");
            if (string.IsNullOrEmpty(DeviceName)) throw new InvalidOperationException("The device name has not been set this must be set before opening the socket.");

            LocalIP = localIp;
            LocalSubnetMask = localSubnetMask;

            //Management Socket
            managementSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            managementSocket.Bind(new IPEndPoint(LocalIP, ManagementPort));
            managementSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            //Application Spcific Data Socket
            applicationSpcificSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            applicationSpcificSocket.Bind(new IPEndPoint(LocalIP, ApplicationSpecificPort));
            applicationSpcificSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            //Timecode Streaming Socket
            timecodeStreamSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            timecodeStreamSocket.Bind(new IPEndPoint(LocalIP, TimecodeStreamPort));
            timecodeStreamSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            //Unicast Node Socket
            nodeSocket.Bind(new IPEndPoint(LocalIP, 65023));

            unicastTXSocket.Bind(new IPEndPoint(LocalIP, 0));

            PortOpen = true;

            StartRecieve(managementSocket, ManagementPort,false);
            StartRecieve(applicationSpcificSocket, ApplicationSpecificPort, false);
            StartRecieve(timecodeStreamSocket, TimecodeStreamPort, false);
            StartRecieve(nodeSocket, ((IPEndPoint)nodeSocket.LocalEndPoint).Port, true);

            StartAdvertising();
            StartTimeSync();
        }

        /// <summary>
        /// Starts recieving data on an open socket.
        /// </summary>
        /// <param name="socket">The socket to recieve data on.</param>
        /// <param name="port">The port to recieve data on.</param>
        protected void StartRecieve(Socket socket,int port, bool unicast)
        {
            try
            {
                SocketError socketError;
                EndPoint localPort = new IPEndPoint(IPAddress.Any, 0);
                TCNetRecieveData recieveState = new TCNetRecieveData()
                {
                    Socket = socket,
                    Port = port,
                    Unicast = unicast
                };
                recieveState.SetLength(MaxPacketSize);

                socket.BeginReceiveFrom(recieveState.GetBuffer(), 0, (int)recieveState.Length, SocketFlags.None, ref localPort, new AsyncCallback(OnRecieve), recieveState);
                    
            }
            catch (Exception ex)
            {
                OnUnhandledException(new ApplicationException("An error ocurred while trying to start recieving ArtNet.",ex));
            }
        }

        /// <summary>
        /// Called when when new data is recieved on a socket.
        /// </summary>
        /// <param name="state">The recieve data for this transaction.</param>
        private void OnRecieve(IAsyncResult state)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any,0);

            if (PortOpen)
            {
                TCNetRecieveData recieveState = (TCNetRecieveData)(state.AsyncState);
                if (recieveState != null)
                {
                    try
                    {
		    	IPEndPoint localEndpoint = (IPEndPoint)recieveState.Socket.LocalEndPoint;
			
                        SocketError socketError;
                        int dataRecieved;
                        
                        dataRecieved = recieveState.Socket.EndReceiveFrom(state, ref remoteEndPoint);

                        recieveState.SetLength((recieveState.Length - recieveState.ReadNibble) + dataRecieved);

                        //Protect against UDP loopback where we recieve our own packets.
                        if (localEndpoint != remoteEndPoint && recieveState.Valid)
                        {
                            LastPacket = DateTime.UtcNow;

                            TCNetPacket newPacket;
                            while (TCNetPacketBuilder.TryBuild(recieveState, (DateTime) LastPacket, out newPacket))
                            {
                                recieveState.ReadPosition = (int) recieveState.Position;

                                newPacket.NetworkID = TCNetPacket.BuildNetworkID((IPEndPoint)remoteEndPoint, newPacket.NodeID);

                                //Packet has been read successfully.
                                if (NewPacket != null)
                                    NewPacket(this, new NewPacketEventArgs<TCNetPacket>(new TCNetEndPoint((IPEndPoint) remoteEndPoint,newPacket.NodeID), newPacket));
                            }
                        }

                    }
		    catch (ObjectDisposedException ex)
		    {
		    	//It is possible for the socket to become disposed
			OnUnhandledException(ex);
			PortOpen = false;
		    }
                    catch (Exception ex)
                    {
                        OnUnhandledException(ex);
                    }
                    finally
                    {
                        //Attempt to recieve another packet.
                        if(PortOpen) StartRecieve(recieveState.Socket,recieveState.Port, recieveState.Unicast);
                    }
                }
            }
        }


        #region Sending

        /// <summary>
        /// Sends the specified packet using UDP broadcast to all TCNet devices.
        /// </summary>
        /// <param name="packet">The packet to send on the network.</param>
        protected void Broadcast(Socket socket, TCNetPacket packet)
        {
            MemoryStream data = new MemoryStream();
            TCNetBinaryWriter writer = new TCNetBinaryWriter(data);
            IPEndPoint localEndpoint = (IPEndPoint) socket.LocalEndPoint;

            TCNetHeader header = packet as TCNetHeader;
            if (header != null)
            {
                header.NodeID = NodeId;
                header.NodeType = Role;
                header.NodeName = NodeName;
                header.SequenceNumber = NextSequenceNumber();
                header.Timestamp = DateTime.UtcNow.TimeOfDay;
            }

            packet.WriteData(writer);
            try
            {
                socket.SendTo(data.ToArray(), new IPEndPoint(BroadcastAddress, localEndpoint.Port));
            }
            catch (SocketException ex)
            {
                OnUnhandledException(new ApplicationException("Socket error broadcasting TCNet integration.", ex));
            }
        }

        /// <summary>
        /// Sends the specified packet using unicast to a specific DJ Tap device.
        /// </summary>
        /// <param name="packet">The packet to send on the network.</param>
        /// <param name="address">The address of the device to send the packet to.</param>
        protected void Unicast(TCNetPacket packet, TCNetEndPoint address)
        {
            MemoryStream data = new MemoryStream();
            TCNetBinaryWriter writer = new TCNetBinaryWriter(data);
            IPEndPoint localEndpoint = (IPEndPoint)nodeSocket.LocalEndPoint;

            TCNetHeader header = packet as TCNetHeader;
            if(header != null)
            {
                header.NodeID = NodeId;
                header.NodeType = Role;
                header.NodeName = NodeName;
                header.SequenceNumber = NextSequenceNumber();
                header.Timestamp = DateTime.UtcNow.TimeOfDay;

                TraceEventType traceLevel = TraceEventType.Information;
                DataTypes dataType = DataTypes.None;

                TCNetDataHeader dataHeader = header as TCNetDataHeader;
                if (dataHeader != null)
                    dataType = dataHeader.DataType;

                tcNetTrace.TraceEvent(traceLevel, (int)header.MessageType, $"{header.NodeName}: {header.GetType().Name} {(dataType != DataTypes.None ? dataType.ToString() : string.Empty)}");
            }

            packet.WriteData(writer);
            try
            {
                unicastTXSocket.SendTo(data.ToArray(), new IPEndPoint(address.Address, address.Port));
            }
            catch (SocketException ex)
            {
                OnUnhandledException(new ApplicationException("Socket error unicast TCNet integration.", ex));
            }

        }

        /// <summary>
        /// Unicasts a TCNet packet to the specified device.
        /// </summary>
        /// <param name="device">The device to send the packet to.</param>
        /// <param name="packet">The packet to be unicast.</param>
        public void Send(TCNetDevice device, TCNetPacket packet)
        {
            Send(device.Endpoint,packet);
        }

        /// <summary>
        /// Unicasts a TCNet packet to the specified device.
        /// </summary>
        /// <param name="endPoint">The IP address and port to send the packet to.</param>
        /// <param name="packet">The packet to be unicast.</param>
        public void Send(TCNetEndPoint endPoint, TCNetPacket packet)
        {
            Unicast(packet, endPoint);
        }


        #endregion


        #endregion

        #region Advertisement

        private Timer advertTimer = null;

        /// <summary>
        /// Starts the periodic poll to advertise our prescence on the network.
        /// </summary>
        /// <remarks>
        /// We must send a GWOffer packet at least every 2 seconds to maintain our prescence on te network.
        /// </remarks>
        public void StartAdvertising()
        {
            if(advertTimer == null)
            {
                advertTimer = new Timer(new TimerCallback(Poll));
                advertTimer.Change(1000, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Stops the advertising on the DJ Tap network.
        /// </summary>
        public void StopAdvertising()
        {
            if(advertTimer != null)
            {
                advertTimer.Dispose();
                advertTimer = null;
            }

            SendOptOut();
        }

        /// <summary>
        /// Called periodically to send a GWOffer packet and advertise our prescence.
        /// </summary>
        /// <remarks>
        /// The GWOffer is sent every second/
        /// </remarks>
        /// <param name="state">The state.</param>
        private void Poll(object state)
        {
            try 
	        {
                SendOptIn();
	        }
            catch(SocketException ex)
            {
                StopAdvertising();
                OnUnhandledException(new ApplicationException("Socket error while advertising ProDJTap, advertising will stop.", ex));
            }
	        finally
	        {
                if (advertTimer != null)
		            advertTimer.Change(1000, Timeout.Infinite);
	        }
        }

        /// <summary>
        /// Constructs and broadcasts the OptIn packet to all devices on the network.
        /// </summary>
        /// <remarks>
        /// This is sent periodically once a second once StartADvertsising has been called.
        /// </remarks>
        protected void SendOptIn()
        {
            IPEndPoint localEndpoint = (IPEndPoint) nodeSocket.LocalEndPoint;

            TCNetOptIn offerPacket = new TCNetOptIn();
            offerPacket.NodeOptions = NodeOptions.NeedAuthentication;
            offerPacket.NodeListenerPort = (ushort) localEndpoint.Port;
            offerPacket.VendorName = VendorName;
            offerPacket.DeviceName = DeviceName;
            offerPacket.DeviceVersion = DeviceVersion;
            offerPacket.NodeCount = 1;

            Broadcast(managementSocket, offerPacket);            
        }

        /// <summary>
        /// Constructs and broadcasts the OptIn packet to all devices on the network.
        /// </summary>
        /// <remarks>
        /// This should be sent when the device is being shutdown.
        /// </remarks>
        protected void SendOptOut()
        {
            IPEndPoint localEndpoint = (IPEndPoint)nodeSocket.LocalEndPoint;

            TCNetOptOut offerPacket = new TCNetOptOut();
            offerPacket.NodeCount = 1;
            offerPacket.NodeListenerPort = (ushort) localEndpoint.Port;

            Broadcast(managementSocket, offerPacket);
        }

        #endregion

        #region Devices

        private Dictionary<int, TCNetDevice> devices = new Dictionary<int, TCNetDevice>();

        /// <summary>
        /// Ocurrs when a new device is found on the TCNet network.
        /// </summary>
        public event EventHandler<TCNetDeviceEventArgs> NewDeviceFound;

        /// <summary>
        /// Raises the new device found.
        /// </summary>
        /// <param name="device">The device.</param>
        protected void RaiseNewDeviceFound(TCNetDevice device)
        {
            if (NewDeviceFound != null)
                NewDeviceFound(this, new TCNetDeviceEventArgs() { Device = device });
        }

        /// <summary>
        /// Ocurrs when a new device is lost on the TCNet network.
        /// </summary>
        public event EventHandler<TCNetDeviceEventArgs> DeviceLost;

        protected void RaiseDeviceLost(TCNetDevice device)
        {
            if (DeviceLost != null)
                DeviceLost(this, new TCNetDeviceEventArgs() { Device = device });
        }

        /// <summary>
        /// Processes the opt in packet being recieved via broadcast from other devices.
        /// </summary>
        /// <remarks>
        /// If the devices is newly discovered then the NewDeviceFound event will be fired.
        /// </remarks>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="optIn">The opt in packet that was recieved.</param>
        private void ProcessOptIn(IPEndPoint endpoint, TCNetOptIn optIn)
        {
            bool newDevice = false;

            TCNetDevice device;
            if (!devices.TryGetValue(optIn.NetworkID, out device))
            {
                device = new TCNetDevice();
                devices[optIn.NetworkID] = device;
                newDevice = true;
            }

            device.Endpoint = new TCNetEndPoint(endpoint.Address, optIn.NodeListenerPort, optIn.NodeID);
            device.VendorName = optIn.VendorName;
            device.DeviceName = optIn.DeviceName;
            device.NodeType = optIn.NodeType;

            if(newDevice)
            {
                Task.Run(()=>RaiseNewDeviceFound(device));
            }
        }

        /// <summary>
        /// Processes the opt out packet being sent by devices when they leave the network.
        /// </summary>
        /// <param name="optOut">The opt out packet that was recieved.</param>
        private void ProcessOptOut(TCNetOptOut optOut)
        {
            TCNetDevice device;
            if(devices.TryGetValue(optOut.NetworkID,out device))
            {
                devices.Remove(optOut.NetworkID);
                RaiseDeviceLost(device);
            }
        }

        /// <summary>
        /// Tries the get device.
        /// </summary>
        /// <param name="networkID">The network identifier.</param>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        public bool  TryGetDevice(int networkID, out TCNetDevice device)
        {
            return devices.TryGetValue(networkID, out device);
        }

        #endregion

        #region Time Synchronization

        private Timer timeSyncTimer = null;

        /// <summary>
        /// Starts using the TCNet time sync packet to determine the network latency between the two devices.
        /// </summary>
        /// <remarks>
        /// The network latency is estimated using an average of the round trip time between the two devices.
        /// </remarks>
        protected void StartTimeSync()
        {
            if(timeSyncTimer == null)
            {
                timeSyncTimer = new Timer(new TimerCallback(TimeSync));
                TimeSync(null);
            }
        }

        /// <summary>
        /// Stops time synchronization between the devices.
        /// </summary>
        protected void StopTimeSync()
        {
            if (timeSyncTimer != null)
            {
                timeSyncTimer.Dispose();
                timeSyncTimer = null;
            }
        }

        /// <summary>
        /// Called periodically to carry out time sync between the devices.
        /// </summary>
        /// <param name="state">The state.</param>
        private void TimeSync(object state)
        {
            try
            {
                if (PortOpen)
                {
                    foreach (TCNetDevice device in devices.Values.ToList())
                    {
                        if(device.NodeType == NodeType.Master)
                            RequestTimeSync(device);
                    }
                }
                else
                {
                    StopTimeSync();
                }
            }
            catch (Exception ex)
            {
                StopTimeSync();
                OnUnhandledException(new ApplicationException("Socket error during time sync. Stopping time sync.", ex));
            }
            finally
            {
                if(timeSyncTimer != null)
                    timeSyncTimer.Change(TimeSpan.FromSeconds(10), TimeSpan.Zero);
            }

        }

        /// <summary>
        /// Requests the time sync by sending a TCNet time Sync request packet.
        /// </summary>
        /// <param name="device">The device.</param>
        private void RequestTimeSync(TCNetDevice device)
        {            
            TCNetTimeSync timeSync = new TCNetTimeSync();
            timeSync.StepNumber = TCNetTimeSync.TimeSyncSteps.Initialize;
            timeSync.NodeListenerPort = (ushort) UnicastLocalEndpoint.Port;
            timeSync.Timestamp = DateTime.UtcNow.TimeOfDay;

            Send(device, timeSync);
        }

        #endregion

    }
}

using Acn.Sockets;
using Citp.Sockets;
using ProDJTap.IO;
using ProDJTap.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProDJTap.Sockets
{
    /// <summary>
    /// The network socket for sending and recieving data in teh DJ Tap protocol.
    /// </summary>
    /// <remarks>
    /// This socket allows you to connect to the network, advertise your prescence and send and recieve data.
    /// </remarks>
    /// <seealso cref="System.Net.Sockets.Socket" />
    public class DJTapSocket:Socket
    {
        public const int Port = 60000;
        public const int TimecodeStreamPort = 60002;


        public event UnhandledExceptionEventHandler UnhandledException;
        public event EventHandler<NewPacketEventArgs<DJTapPacket>> NewPacket;

        private Socket timecodeStreamSocket;

        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="DJTapSocket"/> class.
        /// </summary>
        public DJTapSocket()
            : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            timecodeStreamSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
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
        protected override void Dispose(bool disposing)
        {
            StopAdvertising();
            PortOpen = false;

            timecodeStreamSocket.Dispose();
            base.Dispose(disposing);
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

        private string brand = string.Empty;

        /// <summary>
        /// Gets or sets the brand to use when advertising ourselves on the DJ tap network.
        /// </summary>
        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }

        private string model = string.Empty;

        /// <summary>
        /// Gets or sets the model to use when advertising ourselves on the DJ tap network.
        /// </summary>
        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        #endregion

        #region Communications


        /// <summary>
        /// Opens this socket and starts comunications on the specified network adapter.
        /// </summary>
        /// <param name="localIp">The local ip address of the network adapter to open this socket on.</param>
        /// <param name="localSubnetMask">The local subnet mask of the network adapter to open this socket on.</param>
        public void Open(IPAddress localIp, IPAddress localSubnetMask)
        {
            LocalIP = localIp;
            LocalSubnetMask = localSubnetMask;

            //General Purpose Socket
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(new IPEndPoint(LocalIP, Port));
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            timecodeStreamSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            timecodeStreamSocket.Bind(new IPEndPoint(LocalIP, TimecodeStreamPort));
            timecodeStreamSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

            PortOpen = true;

            StartRecieve(this,Port);
            StartRecieve(timecodeStreamSocket, TimecodeStreamPort);

            StartAdvertising();
        }

        /// <summary>
        /// Starts recieving data on an open socket.
        /// </summary>
        /// <param name="socket">The socket to recieve data on.</param>
        /// <param name="port">The port to recieve data on.</param>
        protected void StartRecieve(Socket socket,int port)
        {
            try
            {
                EndPoint localPort = new IPEndPoint(IPAddress.Any, port);
                DJTapRecieveData recieveState = new DJTapRecieveData()
                {
                    Socket = socket,
                    Port = port
                };
                recieveState.SetLength(1500);
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
                DJTapRecieveData recieveState = (DJTapRecieveData)(state.AsyncState);
                if (recieveState != null)
                {
                    try
                    {
                        recieveState.SetLength((recieveState.Length - recieveState.ReadNibble) + recieveState.Socket.EndReceiveFrom(state, ref remoteEndPoint));

                            //Protect against UDP loopback where we recieve our own packets.
                            if (LocalEndPoint != remoteEndPoint && recieveState.Valid)
                            {
                                LastPacket = DateTime.UtcNow;

                                DJTapPacket newPacket;
                                while (DJTapPacketBuilder.TryBuild(recieveState, (DateTime) LastPacket, out newPacket))
                                {
                                    recieveState.ReadPosition = (int) recieveState.Position;

                                    //Packet has been read successfully.
                                    if (NewPacket != null)
                                        NewPacket(this, new NewPacketEventArgs<DJTapPacket>((IPEndPoint) remoteEndPoint, newPacket));
                                }
                            }

                    }
                    catch (Exception ex)
                    {
                        OnUnhandledException(ex);
                    }
                    finally
                    {
                        //Attempt to recieve another packet.
                        StartRecieve(recieveState.Socket,recieveState.Port);
                    }
                }
            }
        }


        #region Sending

        /// <summary>
        /// Sends the specified packet using UDP broadcast to all DJ Tap devices.
        /// </summary>
        /// <param name="packet">The packet to send on the network.</param>
        public void Send(DJTapPacket packet)
        {
            MemoryStream data = new MemoryStream();
            DJTapBinaryWriter writer = new DJTapBinaryWriter(data);

            packet.WriteData(writer);
            SendTo(data.ToArray(), new IPEndPoint(BroadcastAddress, Port));
        }

        /// <summary>
        /// Sends the specified packet using unicast to a specific DJ Tap device.
        /// </summary>
        /// <param name="packet">The packet to send on the network.</param>
        /// <param name="address">The address of the device to send the packet to.</param>
        public void Send(DJTapPacket packet, RdmEndPoint address)
        {
            MemoryStream data = new MemoryStream();
            DJTapBinaryWriter writer = new DJTapBinaryWriter(data);

            packet.WriteData(writer);
            SendTo(data.ToArray(), new IPEndPoint(address.IpAddress, Port));
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
		        GWOffer offerPacket = new GWOffer();
                offerPacket.Brand = Brand;
                offerPacket.Model = Model;
                Send(offerPacket);
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

        #endregion
    }
}

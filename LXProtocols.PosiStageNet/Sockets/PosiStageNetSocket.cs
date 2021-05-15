using LXProtocols.PosiStageNet.IO;
using LXProtocols.PosiStageNet.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LXProtocols.PosiStageNet.Sockets
{
    /// <summary>
    /// This socket handles sending and recieving Posi Stage Net data over the network.
    /// </summary>
    /// <remarks>
    /// Use this socket to send and recieve PSN packets over the network. It will subscribe to the correct multicast port and group.
    /// </remarks>
    /// <seealso cref="System.Net.Sockets.Socket" />
    public class PosiStageNetSocket: Socket
    {
        /// <summary>
        /// Winsock ioctl code which will disable ICMP errors from being propagated to a UDP socket.
        /// This can occur if a UDP packet is sent to a valid destination but there is no socket
        /// registered to listen on the given port.
        /// </summary>
        public const int SIO_UDP_CONNRESET = -1744830452;

        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="PosiStageNetSocket"/> class.
        /// </summary>
        public PosiStageNetSocket():base(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp)
        {
            StartTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PosiStageNetSocket"/> class.
        /// </summary>
        /// <param name="userMulticastAddress">The user multicast address.</param>
        /// <param name="userPort">The user port.</param>
        public PosiStageNetSocket(IPAddress userMulticastAddress, int userPort):this()
        {
            MulticastGroup = userMulticastAddress;
            Port = userPort;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Net.Sockets.Socket" />, and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            PortOpen = false;

            LeaveMulticastGroup();
            base.Dispose(disposing);
        }


        #endregion

        #region Information

        /// <summary>
        /// Gets or sets the multicast group. The default group is 236.10.10.10
        /// </summary>
        public IPAddress MulticastGroup { get; set; } = new IPAddress(new byte[] {236, 10, 10, 10});

        /// <summary>
        /// Gets or sets the UDP port. The default port is 56565.
        /// </summary>
        public int Port { get; set; } = 56565;

        /// <summary>
        /// Gets or sets a value indicating whether UDP port is open.
        /// </summary>
        public bool PortOpen { get; set; } = false;

        /// <summary>
        /// Gets or sets the last packet recieve time.
        /// </summary>
        private DateTime? LastPacket { get; set; } = null;

        /// <summary>
        /// Gets or sets the time at whic this socket was opened.
        /// </summary>
        /// <remarks>
        /// This is used to calculate the packet time stamps.
        /// </remarks>
        public DateTime StartTime { get; set; }

        #endregion

        #region Communications

        /// <summary>
        /// Opens the specified adapter ip.
        /// </summary>
        /// <param name="adapterIP">The adapter ip.</param>
        public void Open(IPAddress adapterIP)
        {
            Open(new IPEndPoint(adapterIP, Port));
        }


        /// <summary>
        /// Opens this socket and starts comunications on the specified network adapter.
        /// </summary>
        /// <param name="localIp">The local ip address of the network adapter to open this socket on.</param>
        /// <param name="localSubnetMask">The local subnet mask of the network adapter to open this socket on.</param>
        public void Open(IPEndPoint localEndPoint)
        {
            if (PortOpen)
                throw new InvalidOperationException("This PosiStageNet socket is already open. Did you mean to close the socket first?");

            // Set the SIO_UDP_CONNRESET ioctl to true for this UDP socket. If this UDP socket
            //    ever sends a UDP packet to a remote destination that exists but there is
            //    no socket to receive the packet, an ICMP port unreachable message is returned
            //    to the sender. By default, when this is received the next operation on the
            //    UDP socket that send the packet will receive a SocketException. The native
            //    (Winsock) error that is received is WSAECONNRESET (10054). Since we don't want
            //    to wrap each UDP socket operation in a try/except, we'll disable this error
            //    for the socket with this ioctl call.
            byte[] byteTrue = new byte[4] { 0, 0, 0, 1 };
            IOControl(SIO_UDP_CONNRESET, byteTrue, null);

            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            Bind(localEndPoint);
            JoinMulticastGroup(localEndPoint);
            PortOpen = true;

            StartRecieve(this, null);
        }

        /// <summary>
        /// Joins the multicast group.
        /// </summary>
        /// <param name="localEndPoint">The local end point.</param>
        private void JoinMulticastGroup(IPEndPoint localEndPoint)
        {
            MulticastLoopback = true;

            //Only join local LAN group.
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);

            //Join Group
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastGroup, ((IPEndPoint)localEndPoint).Address));
        }

        /// <summary>
        /// Leaves the multicast group.
        /// </summary>
        private void LeaveMulticastGroup()
        {
            if (IsBound)
            {
                //Leave Group
                SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(MulticastGroup));
            }
        }

        /// <summary>
        /// Starts the recieve.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffer">The buffer.</param>
        public void StartRecieve(Socket socket, MemoryStream buffer)
        {
            try
            {
                EndPoint localPort = new IPEndPoint(IPAddress.Any, 0);

                if (buffer == null)
                {
                    buffer = new MemoryStream(65536);
                    buffer.SetLength(65536);
                }
                buffer.Seek(0, SeekOrigin.Begin);

                Tuple<Socket, MemoryStream> recieveState = new Tuple<Socket, MemoryStream>(socket, buffer);
                socket.BeginReceiveFrom(buffer.GetBuffer(), 0, (int)buffer.Length, SocketFlags.None, ref localPort, new AsyncCallback(OnRecieve), recieveState);
            }
            catch (Exception ex)
            {
                RaiseUnhandledException(new ApplicationException("An error ocurred while trying to start recieving ACN.", ex));
            }
        }

        /// <summary>
        /// Called when [recieve].
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnRecieve(IAsyncResult state)
        {
            if (PortOpen)
            {
                Tuple<Socket, MemoryStream> recieveState = (Tuple<Socket, MemoryStream>)(state.AsyncState);

                try
                {
                    if (recieveState != null)
                    {
                        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        int dataSize = recieveState.Item1.EndReceiveFrom(state, ref remoteEndPoint);

                        //Protect against UDP loopback where we recieve our own packets.
                        //Ensure we have actually recieved data.
                        if (LocalEndPoint != remoteEndPoint && dataSize > 0)
                        {
                            LastPacket = DateTime.Now;

                            IPEndPoint ipEndPoint = (IPEndPoint)remoteEndPoint;

                            //If this is a TCP connection then the returned enpoint will be empty and we must use the connection endpoint.
                            if (ipEndPoint.Port == 0)
                                ipEndPoint = (IPEndPoint)recieveState.Item1.RemoteEndPoint;

                            PosiStageNetReader packetReader = new PosiStageNetReader(recieveState.Item2);
                            ProcessPSNPacket(ipEndPoint, packetReader);
                        }
                    }
                }
                catch (SocketException)
                {
                    Close();
                }
                catch (Exception ex)
                {
                    RaiseUnhandledException(ex);
                }
                finally
                {
                    //Attempt to recieve another packet.
                    if (PortOpen)
                        StartRecieve(recieveState.Item1, recieveState.Item2);
                }
            }
        }

        /// <summary>
        /// Processes the PSN packet.
        /// </summary>
        /// <param name="remoteEndPoint">The remote end point.</param>
        /// <param name="data">The data.</param>
        private void ProcessPSNPacket(IPEndPoint remoteEndPoint, PosiStageNetReader data)
        {
            var chunkHeader = data.ReadChunkHeader();
            if(PosiStageNetPacketFactory.TryBuild(chunkHeader, out PosiStageNetPacket packet))
            {
                packet.ReadData(chunkHeader, data);

                RaiseNewPacket(new PosiStageNetNewPacketEventArgs((IPEndPoint)LocalEndPoint, (IPEndPoint)remoteEndPoint, packet));
            }
        }

        /// <summary>
        /// Sends the packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <param name="destination">The destination.</param>
        public void SendPacket(PosiStageNetPacket packet, IPAddress destination)
        {
            SendPacket(packet, new IPEndPoint(destination, Port));
        }

        /// <summary>
        /// Sends the packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <param name="destination">The destination.</param>
        public void SendPacket(PosiStageNetPacket packet, IPEndPoint destination)
        {
            packet.TimeStamp = DateTime.UtcNow.Subtract(StartTime);

            MemoryStream data = new MemoryStream();
            PosiStageNetWriter writer = new PosiStageNetWriter(data);

            packet.WriteData(writer);

            BeginSendTo(data.GetBuffer(), 0, (int)data.Length, SocketFlags.None, destination, null, null);
        }

        #endregion

        #region Protocol Packets

        /// <summary>
        /// Creates new packet.
        /// </summary>
        public event EventHandler<PosiStageNetNewPacketEventArgs> NewPacket;

        /// <summary>
        /// Raises the new packet.
        /// </summary>
        /// <param name="args">The <see cref="PosiStageNetNewPacketEventArgs"/> instance containing the event data.</param>
        protected void RaiseNewPacket(PosiStageNetNewPacketEventArgs args)
        {
            if (NewPacket != null)
                NewPacket(this, args);
        }

        #endregion

        #region Information Streaming

        /// <summary>
        /// Gets or sets the update rate at which information messages are sent in Hz. Default is 1Hz.
        /// </summary>
        public int InformationRate { get; set; } = 1;

        #endregion

        #region Tracker Streaming


        /// <summary>
        /// Gets or sets the rate that tracker updates are sent in Hz. Default is 60Hz.
        /// </summary>
        public int TrackerRate { get; set; } = 60;

        private List<PosiStageNetTracker> trackers = new List<PosiStageNetTracker>();

        /// <summary>
        /// Adds the trackers.
        /// </summary>
        /// <param name="trackers">The trackers.</param>
        public void AddTrackers(IEnumerable<PosiStageNetTracker> trackers)
        {
            this.trackers.AddRange(trackers);
        }

        /// <summary>
        /// Removes the trackers.
        /// </summary>
        /// <param name="trackers">The trackers.</param>
        public void RemoveTrackers(IEnumerable<PosiStageNetTracker> trackers)
        {
            foreach (var item in trackers)
                this.trackers.Remove(item);
        }

        /// <summary>
        /// Clears the trackers.
        /// </summary>
        public void ClearTrackers()
        {
            trackers.Clear();            
        }

        #endregion

        #region Exceptions

        /// <summary>
        /// Occurs when [unhandled exception].
        /// </summary>
        public event UnhandledExceptionEventHandler UnhandledException;

        /// <summary>
        /// Raises the unhandled exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <exception cref="System.ApplicationException">Exception is unhandled by user code. Please subscribe to the UnhandledException event.</exception>
        protected void RaiseUnhandledException(Exception ex)
        {
            if (UnhandledException == null)
                throw new ApplicationException("Exception is unhandled by user code. Please subscribe to the UnhandledException event.", ex);
            UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        #endregion
    }
}

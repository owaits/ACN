using Acn.Packets.sAcn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Acn.Sockets
{
    /// <summary>
    /// Handles creating <see cref="StreamingAcnSocket"/> 
    /// </summary>
    /// <typeparam name="T">The type of the Streaming ACN socket.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public class MultiStreamingAcnSocket<T> : IDisposable where T : StreamingAcnSocket
    {
        #region Properties

        /// <summary>
        /// Delegate to create new instances of the socket.
        /// </summary>
        private readonly Func<T> SocketFactory;

        /// <summary>
        /// The currently active Streaming ACN sockets.
        /// </summary>
        private readonly Dictionary<IPAddress, T> streamingAcnSockets = new Dictionary<IPAddress, T>();
        /// <summary>
        /// The network adapter addresses to use.
        /// </summary>
        private readonly List<IPAddress> networkAdapterAddresses = new List<IPAddress>();

        /// <summary>
        /// Gets or sets a value indicating whether to open the sockets for receiving.
        /// </summary>
        /// <value>
        ///   <c>true</c> to open for receiving; otherwise, <c>false</c>.
        /// </value>
        public bool OpenReceive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use loopback adapter in addition to the adapters specified.
        /// </summary>
        /// <value>
        ///   <c>true</c> to use loopback adapter; otherwise, <c>false</c>.
        /// </value>
        public bool UseLoopback { get; set; }

        #endregion Properties

        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiStreamingAcnSocket{T}" /> class.
        /// </summary>
        /// <param name="socketFactory">The socket factory.</param>
        public MultiStreamingAcnSocket(Func<T> socketFactory)
        {
            SocketFactory = socketFactory;
            UseLoopback = true;
        }

        /// <summary>
        /// Sets the network adapters.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        public void SetNetworkAdapters(IEnumerable<IPAddress> addresses)
        {
            networkAdapterAddresses.Clear();
            networkAdapterAddresses.AddRange(addresses);
        }

        /// <summary>
        /// Sets to use all network adapters.
        /// </summary>
        public void SetAllNetworkAdapters()
        {
            SetNetworkAdapters(NetworkInterface.GetAllNetworkInterfaces()
                .Where(adapter => adapter.OperationalStatus == OperationalStatus.Up)
                .Where(adapter => adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                .Select(ni => ni.GetIPProperties())
               .Where(ip => ip != null).SelectMany(ip => ip.UnicastAddresses)
               .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a.Address)).Select(a => a.Address));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        #endregion Setup and Initialisation

        #region Start/Stop

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (streamingAcnSockets.Count == 0)
            {
                IEnumerable<IPAddress> ipAddresses = UseLoopback ? networkAdapterAddresses.Union(Enumerable.Repeat(IPAddress.Loopback, 1)) : networkAdapterAddresses;
                foreach (IPAddress ipAddress in ipAddresses.Distinct())
                {
                    T dmxSocket = null;
                    try
                    {
                        dmxSocket = SocketFactory();
                        dmxSocket.UnhandledException += OnUnhandledException;
                        if (OpenReceive)
                        {
                            dmxSocket.NewPacket += OnNewPacket;
                            dmxSocket.Open(new IPEndPoint(ipAddress, dmxSocket.Port));
                        }
                        else
                        {
                            dmxSocket.Open(new IPEndPoint(ipAddress, 0));
                        }

                        streamingAcnSockets[ipAddress] = dmxSocket;
                    }
                    catch (SocketException ex)
                    {
                        if (dmxSocket != null)
                        {
                            dmxSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                            dmxSocket.Dispose();
                            dmxSocket = null;
                        }
                        RaiseUnhandledException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            foreach (StreamingAcnSocket socket in streamingAcnSockets.Values)
            {
                socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                socket.Close();
                if (OpenReceive)
                {
                    socket.NewPacket -= OnNewPacket;
                }
                socket.UnhandledException -= OnUnhandledException;
                socket.Dispose();
            }
            streamingAcnSockets.Clear();
        }

        #endregion Start/Stop

        #region Events

        /// <summary>
        /// Occurs when a new packet is received.
        /// </summary>
        public event EventHandler<NewPacketEventArgs<StreamingAcnDmxPacket>> NewPacket;

        /// <summary>
        /// Called when a new packet is received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NewPacketEventArgs{StreamingAcnDmxPacket}"/> instance containing the event data.</param>
        private void OnNewPacket(object sender, NewPacketEventArgs<StreamingAcnDmxPacket> e)
        {
            if (NewPacket != null)
                NewPacket(sender, e);
        }

        /// <summary>
        /// Occurs when an unhandled exception occurs.
        /// </summary>
        public event UnhandledExceptionEventHandler UnhandledException;

        /// <summary>
        /// Raises the unhandled exception.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <exception cref="ApplicationException">Exception is unhandled by user code. Please subscribe to the UnhandledException event.</exception>
        protected void RaiseUnhandledException(Exception e)
        {
            RaiseUnhandledException(new UnhandledExceptionEventArgs(e, false));
        }

        /// <summary>
        /// Raises the unhandled exception.
        /// </summary>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        /// <exception cref="ApplicationException">Exception is unhandled by user code. Please subscribe to the UnhandledException event.</exception>
        protected void RaiseUnhandledException(UnhandledExceptionEventArgs e)
        {
            if (UnhandledException == null)
                throw new ApplicationException("Exception is unhandled by user code. Please subscribe to the UnhandledException event.", (Exception)e.ExceptionObject);

            UnhandledException(this, e);
        }

        /// <summary>
        /// Called when an unhandled exception occurs in one of the sockets.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            RaiseUnhandledException(e);
        }

        #endregion Events

        #region DMX

        /// <summary>
        /// Sends the DMX.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <param name="dmxData">The DMX data.</param>
        public void SendDmx(int universe, byte[] dmxData)
        {
            foreach (StreamingAcnSocket socket in streamingAcnSockets.Values)
            {
                if (socket.DmxUniverses.Contains(universe))
                    socket.SendDmx(universe, dmxData);
            }
        }

        /// <summary>
        /// Sends the DMX.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <param name="dmxData">The DMX data.</param>
        /// <param name="priority">The priority.</param>
        public void SendDmx(int universe, byte[] dmxData, byte priority)
        {
            foreach (StreamingAcnSocket socket in streamingAcnSockets.Values)
            {
                if (socket.DmxUniverses.Contains(universe))
                    socket.SendDmx(universe, dmxData, priority);
            }
        }

        /// <summary>
        /// Sends the DMX.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <param name="startCode">The start code.</param>
        /// <param name="dmxData">The DMX data.</param>
        /// <param name="priority">The priority.</param>
        public void SendDmx(int universe, byte startCode, byte[] dmxData, byte priority)
        {
            foreach (StreamingAcnSocket socket in streamingAcnSockets.Values)
            {
                if (socket.DmxUniverses.Contains(universe))
                    socket.SendDmx(universe, startCode, dmxData, priority);
            }
        }

        #endregion DMX

        #region Universes

        /// <summary>
        /// Gets the DMX universes or <c>null</c> if there are no sockets.
        /// </summary>
        /// <value>
        /// The DMX universes.
        /// </value>
        public IEnumerable<int> DmxUniverses
        {
            get { return streamingAcnSockets.SelectMany(kvp => kvp.Value.DmxUniverses).Distinct(); }
        }

        /// <summary>
        /// Joins a DMX universe.
        /// </summary>
        /// <param name="universe">The universe.</param>
        public void JoinDmxUniverse(int universe)
        {
            foreach (StreamingAcnSocket socket in streamingAcnSockets.Values)
            {
                if (!socket.DmxUniverses.Contains(universe))
                    socket.JoinDmxUniverse(universe);
            }
        }

        /// <summary>
        /// Joins a DMX universe for an address.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <param name="address">The address.</param>
        public void JoinDmxUniverse(int universe, IPAddress address)
        {
            streamingAcnSockets[address].JoinDmxUniverse(universe);
        }

        /// <summary>
        /// Drops a DMX universe.
        /// </summary>
        /// <param name="universe">The universe.</param>
        public void DropDmxUniverse(int universe)
        {
            foreach (StreamingAcnSocket socket in streamingAcnSockets.Values)
            {
                if (socket.DmxUniverses.Contains(universe))
                    socket.DropDmxUniverse(universe);
            }
        }

        /// <summary>
        /// Drops a DMX universe from an address.
        /// </summary>
        /// <param name="universe">The universe.</param>
        /// <param name="address">The address.</param>
        public void DropDmxUniverse(int universe, IPAddress address)
        {
            streamingAcnSockets[address].DropDmxUniverse(universe);
        }

        /// <summary>
        /// Sets the DMX universes.
        /// </summary>
        /// <param name="addressUniverses">The universes for each address.</param>
        public void SetDmxUniverses(ILookup<IPAddress, int> addressUniverses)
        {
            foreach (IGrouping<IPAddress, int> address in addressUniverses)
            {
                SetDmxUniverses(address.Key, address);
            }
        }

        /// <summary>
        /// Sets the DMX universes.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="universes">The universes.</param>
        public void SetDmxUniverses(IPAddress address, IEnumerable<int> universes)
        {
            T socket = streamingAcnSockets[address];
            HashSet<int> oldUniverses = new HashSet<int>(socket.DmxUniverses);
            foreach (int universe in universes)
            {
                if (!oldUniverses.Remove(universe))
                {
                    socket.JoinDmxUniverse(universe);
                }
            }
            foreach (int universe in oldUniverses)
            {
                socket.DropDmxUniverse(universe);
            }
        }

        /// <summary>
        /// Sets the DMX universes.
        /// </summary>
        /// <param name="universes">The universes.</param>
        public void SetDmxUniverses(IEnumerable<int> universes)
        {
            foreach (IPAddress address in streamingAcnSockets.Keys)
            {
                SetDmxUniverses(address, universes);
            }
        }

        #endregion
    }
}

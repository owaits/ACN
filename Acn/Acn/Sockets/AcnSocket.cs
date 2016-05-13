﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Acn.IO;
using Acn.Packets.sAcn;
using System.Diagnostics;
using System.Threading;

namespace Acn.Sockets
{
    public class AcnSocket:Socket
    {
        /// <summary>
        /// Winsock ioctl code which will disable ICMP errors from being propagated to a UDP socket.
        /// This can occur if a UDP packet is sent to a valid destination but there is no socket
        /// registered to listen on the given port.
        /// </summary>
        public const int SIO_UDP_CONNRESET = -1744830452;

        public event UnhandledExceptionEventHandler UnhandledException;

        #region Setup and Initialisation

        public AcnSocket(Guid senderId)
            : base(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp)
        {
            if (senderId == Guid.Empty)
                throw new ArgumentException("Invalid sender ID!", "senderId");

            this.senderId = senderId;
        }

        #endregion

        #region Information

        public virtual int Port
        {
            get { return 5568; }
        }

        private Guid senderId = Guid.Empty;

        public Guid SenderId
        {
            get { return senderId; }
        }

        private bool portOpen = false;

        public bool PortOpen
        {
            get { return portOpen; }
            set { portOpen = value; }
        }

        private DateTime? lastPacket = null;

        public DateTime? LastPacket
        {
            get { return lastPacket; }
            protected set { lastPacket = value; }
        }

        #endregion

        #region Filters

        private Dictionary<int, IProtocolFilter> filters = new Dictionary<int, IProtocolFilter>();

        public void RegisterProtocolFilter(IProtocolFilter filter)
        {
            filters.Add(filter.ProtocolId, filter);
        }

        #endregion

        #region Traffic

        protected virtual bool TcpTraffic
        {
            get { return false; }
        }

        public void Open(IPAddress adapterIP)
        {
            Open(new IPEndPoint(adapterIP, Port));
        }

        public virtual void Open(IPEndPoint localEndPoint)
        {
            if (PortOpen)
                throw new InvalidOperationException("This ACN socket is already open. Did you mean to close the socket first?");

            // Set the SIO_UDP_CONNRESET ioctl to true for this UDP socket. If this UDP socket
            //    ever sends a UDP packet to a remote destination that exists but there is
            //    no socket to receive the packet, an ICMP port unreachable message is returned
            //    to the sender. By default, when this is received the next operation on the
            //    UDP socket that send the packet will receive a SocketException. The native
            //    (Winsock) error that is received is WSAECONNRESET (10054). Since we don't want
            //    to wrap each UDP socket operation in a try/except, we'll disable this error
            //    for the socket with this ioctl call.
            try
            {
            byte[] byteTrue = new byte[4] {0,0,0, 1};
            IOControl(SIO_UDP_CONNRESET, byteTrue, null);
            }
            catch (SocketException)
            {
                Trace.WriteLine("Unable to set SIO_UDP_CONNRESET, maybe not supported.");
            }

            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(localEndPoint);

            //Multi-cast socket settings
            MulticastLoopback = true;            
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);        //Only join local LAN group.

            PortOpen = true;

            StartRecieve(this,null);
        }

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

                            AcnBinaryReader packetReader = new AcnBinaryReader(recieveState.Item2);
                            ProcessAcnPacket(ipEndPoint, packetReader);
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
                    if(PortOpen)
                        StartRecieve(recieveState.Item1, recieveState.Item2);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether to override the default root layer.
        /// </summary>
        /// <value>
        ///   <c>true</c> to override root layer; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool OverrideRootLayer { get { return false; } }

        /// <summary>
        /// Allows the root layer part of the ACN packet to be edited.
        /// </summary>
        /// <returns>The root layer of the packet.</returns>
        protected virtual AcnRootLayer GetRootLayer()
        {
            return new AcnRootLayer();
        }

        private void ProcessAcnPacket(IPEndPoint source, AcnBinaryReader data)
        {
            AcnRootLayer rootLayer = GetRootLayer();
            rootLayer.ReadData(data, TcpTraffic);

            IProtocolFilter filter;
            if (filters.TryGetValue(rootLayer.ProtocolId, out filter))
            {
                filter.ProcessPacket(source,rootLayer, data);
            }
        }

        public void SendPacket(AcnPacket packet, IPAddress destination)
        {
            SendPacket(packet, new IPEndPoint(destination, Port));
        }

        public void SendPacket(AcnPacket packet, IPEndPoint destination)
        {
            //Set the senders CID.
            packet.Root.SenderId = SenderId;

            MemoryStream data = new MemoryStream();
            AcnBinaryWriter writer = new AcnBinaryWriter(data);

            AcnPacket.WritePacket(packet, writer);

            BeginSendTo(data.GetBuffer(), 0, (int)data.Length, SocketFlags.None, destination, null, null);
        }

        protected void RaiseUnhandledException(Exception ex)
        {
            if (UnhandledException == null)
                throw new ApplicationException("Exception is unhandled by user code. Please subscribe to the UnhandledException event.",ex);
            UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        /// <summary>
        /// Closes the <see cref="T:System.Net.Sockets.Socket" /> connection and releases all associated resources.
        /// </summary>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        protected new void Close()
        {
            PortOpen = false;
            base.Close();
        }

        /// <summary>
        /// Closes the <see cref="T:System.Net.Sockets.Socket" /> connection and releases all associated resources with a specified timeout to allow queued data to be sent.
        /// </summary>
        /// <param name="timeout">Wait up to <paramref name="timeout" /> seconds to send any remaining data, then close the socket.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// </PermissionSet>
        protected new void Close(int timeout)
        {
            PortOpen = false;
            base.Close(timeout);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Net.Sockets.Socket" />, and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            PortOpen = false;
            base.Dispose(disposing);
        }

        #endregion
    }
}

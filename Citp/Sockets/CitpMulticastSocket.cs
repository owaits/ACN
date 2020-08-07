using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using LXProtocols.Citp.Packets;
using System.IO;
using LXProtocols.Citp.Sockets;
using LXProtocols.Citp.IO;

namespace LXProtocols.Citp.Sockets
{
    public class CitpMulticastSocket:Socket, IDisposable
    {
        public const int Port = 4809;
        public IPAddress MulticastGroup = IPAddress.Parse("224.0.0.180");

        public event UnhandledExceptionEventHandler UnhandledException;
        public event EventHandler<CitpNewPacketEventArgs> NewPacket;

        /// <summary>
        /// Winsock ioctl code which will disable ICMP errors from being propagated to a UDP socket.
        /// This can occur if a UDP packet is sent to a valid destination but there is no socket
        /// registered to listen on the given port.
        /// </summary>
        public const int SIO_UDP_CONNRESET = -1744830452;

        public CitpMulticastSocket()
            : base(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp)
        {

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


        public void Open(IPAddress networkAdapter)
        {
            IPEndPoint localEndPoint = new IPEndPoint(networkAdapter, Port);

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

            StartRecieve();
        }

        private void JoinMulticastGroup(IPEndPoint localEndPoint)
        {
            MulticastLoopback = true;

            //Only join local LAN group.
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
            
            //Join Group
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastGroup, ((IPEndPoint)localEndPoint).Address)); 
        }

        private void LeaveMulticastGroup()
        {
            if (IsBound)
            {
                //Leave Group
                SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(MulticastGroup));
            }
        }

        public void StartRecieve()
        {
            try
            {
                EndPoint localPort = new IPEndPoint(IPAddress.Any, Port);
                CitpRecieveData recieveState = new CitpRecieveData();
                recieveState.SetLength(recieveState.Capacity);
                BeginReceiveFrom(recieveState.GetBuffer(), 0, recieveState.ReadNibble, SocketFlags.None, ref localPort, new AsyncCallback(OnRecieve), recieveState);
            }
            catch (Exception ex)
            {
                OnUnhandledException(new ApplicationException("An error ocurred while trying to start recieving CITP.", ex));
            }
        }

        private void OnRecieve(IAsyncResult state)
        {
            CitpPacket newPacket;
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            if (PortOpen)
            {
                try
                {
                    CitpRecieveData recieveState = (CitpRecieveData)(state.AsyncState);

                    if (recieveState != null)
                    {
                        recieveState.SetLength(EndReceiveFrom(state, ref remoteEndPoint));

                        //Protect against UDP loopback where we recieve our own packets.
                        if (LocalEndPoint != remoteEndPoint && recieveState.Valid)
                        {
                            LastPacket = DateTime.Now;

                            if (NewPacket != null)
                            {
                                if (CitpPacketBuilder.TryBuild(recieveState, out newPacket))
                                {
                                    NewPacket(this, new CitpNewPacketEventArgs((IPEndPoint) LocalEndPoint ,(IPEndPoint) remoteEndPoint,newPacket));

                                    CitpTrace.RxPacket((IPEndPoint)remoteEndPoint, newPacket);
                                }
                            }
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
                    StartRecieve();
                }
            }
        }

        protected void OnUnhandledException(Exception ex)
        {
            if (UnhandledException != null) UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        public void BeginSend(CitpHeader citpMessage)
        {
            MemoryStream data = new MemoryStream();
            CitpBinaryWriter writer = new CitpBinaryWriter(data);

            citpMessage.WriteData(writer);
            citpMessage.WriteMessageSize(writer);

            IPEndPoint targetEndpoint = new IPEndPoint(MulticastGroup, Port);
            BeginSendTo(data.GetBuffer(), 0, (int)data.Length, SocketFlags.None, targetEndpoint, null, null);

            CitpTrace.RxPacket(targetEndpoint, citpMessage);
        }

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;

            LeaveMulticastGroup();
            base.Dispose(disposing);
        }
       

    }
}

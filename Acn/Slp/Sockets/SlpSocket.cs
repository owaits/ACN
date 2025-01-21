using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using LXProtocols.Acn.IO;
using System.IO;
using LXProtocols.Acn.Packets.sAcn;
using LXProtocols.Acn.Slp.Packets;
using LXProtocols.Acn.Slp.IO;

namespace LXProtocols.Acn.Slp.Sockets
{
    public class SlpSocket:Socket
    {
        public event UnhandledExceptionEventHandler UnhandledException;
        internal event EventHandler<NewPacketEventArgs> NewPacket;

        #region Setup and Initialisation

        public SlpSocket()
            : base(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp)
        {
        }

        #endregion

        #region Information

        public const int Port = 427;

        public IPAddress MulticastGroup
        {
            get
            {
                return new IPAddress(new byte[] { 239,255,255,253});
            }
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

        #region Traffic

        /// <summary>
        /// Opens the this socket and starts recieving data, if multicast then the multicast group is joined.
        /// </summary>
        /// <param name="ipAddress">The local end point to bind to, this specifies the network adapter to use.</param>
        /// <param name="multicast">When true the socket will be setup to recieve and send multicast data.</param>
        public void Open(IPAddress ipAddress, bool multicast)
        {
            Open(new IPEndPoint(ipAddress, Port), multicast);
        }

        /// <summary>
        /// Opens the this socket and starts recieving data, if multicast then the multicast group is joined.
        /// </summary>
        /// <param name="localEndPoint">The local end point to bind to, this specifies the etwork adapter and port to use.</param>
        /// <param name="multicast">When true the socket will be setup to recieve and send multicast data.</param>
        public void Open(IPEndPoint localEndPoint, bool multicast)
        {
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(localEndPoint);
            if(multicast)
                JoinMulticastGroup();
            PortOpen = true;

            StartRecieve();
        }

        private void JoinMulticastGroup()
        {
            //Setting this to true allows us to test on one PC.
            MulticastLoopback = true;
            
            //Only join local LAN group.
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 20);

            //Join Group
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastGroup,((IPEndPoint)LocalEndPoint).Address));
        }

        public void StartRecieve()
        {
            try
            {
                EndPoint remotePort = new IPEndPoint(IPAddress.Any, 0);
                MemoryStream recieveState = new MemoryStream(SlpPacket.MaxSize);
                recieveState.SetLength(SlpPacket.MaxSize);
                BeginReceiveFrom(recieveState.GetBuffer(), 0, (int)recieveState.Length, SocketFlags.None, ref remotePort, new AsyncCallback(OnRecieve), recieveState);
            }
            catch (Exception ex)
            {
                OnUnhandledException(new ApplicationException("An error ocurred while trying to start recieving CITP.", ex));
            }
        }

        private void OnRecieve(IAsyncResult state)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            if (PortOpen)
            {
                try
                {
                    MemoryStream recieveState = (MemoryStream)(state.AsyncState);

                    if (recieveState != null)
                    {
                        EndReceiveFrom(state, ref remoteEndPoint);

                        LastPacket = DateTime.Now;

                        if (NewPacket != null)
                        {
                            SlpBinaryReader dataReader = new SlpBinaryReader(recieveState);

                            //Read the Header
                            SlpPacket packet = SlpPacket.ReadPacket(dataReader);

                            NewPacketEventArgs args = new NewPacketEventArgs(packet);
                            args.SourceEndPoint = (IPEndPoint) remoteEndPoint;

                            NewPacket(this, args);
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

        public void Send(SlpPacket packet)
        {
            packet.Header.Flags |= SlpHeaderFlags.RequestMulticast;
            Send(new IPEndPoint(MulticastGroup,Port), packet);
        }

        public void Send(IPEndPoint target, SlpPacket packet)
        {
            MemoryStream data = new MemoryStream();
            SlpBinaryWriter writer = new SlpBinaryWriter(data);

            SlpPacket.WritePacket(packet, writer);

            BeginSendTo(data.GetBuffer(), 0, (int)data.Length, SocketFlags.None, target, new AsyncCallback(OnSend), null);
        }

        /// <summary>
        /// Called when SendTo completes and handles any errors that may have ocurred.
        /// </summary>
        /// <param name="state">The state.</param>
        private void OnSend(IAsyncResult state)
        {
            try
            {
                //Throws an exception if the send was not successful or returns the bytes sent.
                int bytesSent = EndSendTo(state);
            }
            catch (Exception ex)
            {
                OnUnhandledException(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;
            base.Dispose(disposing);
        }

        #endregion
    }
}

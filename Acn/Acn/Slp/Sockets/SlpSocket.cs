using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Acn.IO;
using System.IO;
using Acn.Packets.sAcn;
using Acn.Slp.Packets;
using Acn.Slp.IO;

namespace Acn.Slp.Sockets
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

        public void Open(IPAddress adapterIPAddress)
        {
            IPEndPoint localEndPoint = new IPEndPoint(adapterIPAddress, Port);

            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(localEndPoint);
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
                EndPoint remotePort = new IPEndPoint(IPAddress.Any, Port);
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
                            args.SourceAddress = ((IPEndPoint)remoteEndPoint).Address;
                            args.SourcePort = ((IPEndPoint)remoteEndPoint).Port;

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
            //packet.Header.Flags |= SlpHeaderFlags.RequestMulticast;
            Send(MulticastGroup, packet);
        }

        public void Send(IPAddress target, SlpPacket packet)
        {
            MemoryStream data = new MemoryStream();
            SlpBinaryWriter writer = new SlpBinaryWriter(data);

            SlpPacket.WritePacket(packet, writer);

            BeginSendTo(data.GetBuffer(), 0, (int)data.Length, SocketFlags.None, new IPEndPoint(target, Port), null, null);
        }

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;
            base.Dispose(disposing);
        }

        #endregion
    }
}

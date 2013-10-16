using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Citp.Packets;
using System.IO;
using Citp.Sockets;
using Citp.IO;

namespace Citp.Sockets
{
    public class CitpMulticastSocket:Socket
    {
        public const int Port = 4809;
        public IPAddress MulticastGroup = IPAddress.Parse("224.0.0.180");

        public event UnhandledExceptionEventHandler UnhandledException;
        public event EventHandler<CitpNewPacketEventArgs> NewPacket;

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

            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(localEndPoint);
            JoinMulticastGroup();            
            PortOpen = true;

            StartRecieve();
        }

        private void JoinMulticastGroup()
        {
            MulticastLoopback = true;

            //Only join local LAN group.
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive,20);
            
            //Join Group
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastGroup)); 
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

            BeginSendTo(data.GetBuffer(), 0, (int) data.Length, SocketFlags.None, new IPEndPoint(MulticastGroup, Port), null, null);
        }

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;
            base.Dispose(disposing);
        }
       

    }
}

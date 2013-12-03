using Acn.IO;
using Acn.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using Acn.Rdm;
using Acn.Packets.sAcn;
using Acn.Packets.RdmNet;

namespace Acn.RdmNet.Sockets
{
    public class HealthCheckedTcpSocket: RdmNetSocket, IProtocolFilter
    {
        private Socket socket = null;
        private Timer heartbeatTimer = null;
        private HeartbeatProtocolFilter heartbeatFilter = null;

        public HealthCheckedTcpSocket(Socket socket, UId rdmId, Guid senderId, string sourceName)
            : base(rdmId,senderId,sourceName)
        {
            this.socket = socket;

            heartbeatTimer = new Timer(new TimerCallback(Heartbeat));
            heartbeatFilter = new HeartbeatProtocolFilter(this);

            NewRdmPacket += HealthCheckedTcpSocket_NewRdmPacket;

            RegisterProtocolFilter(heartbeatFilter);
        }

        public override void Open(IPEndPoint localEndPoint)
        {
            PortOpen = true;
            StartRecieve(socket, null);            
            
            if(Healthy)
                HeartbeatEnabled = true;
        }

        public static HealthCheckedTcpSocket Connect(RdmEndPoint endpoint,Guid senderId)
        {
            Socket newConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newConnection.Connect(endpoint);

            HealthCheckedTcpSocket socket = new HealthCheckedTcpSocket(newConnection, endpoint.Id, senderId, string.Empty);
            socket.Open(endpoint);
            return socket;
        }

        protected override void Dispose(bool disposing)
        {
            HeartbeatEnabled = false;
            base.Dispose(disposing);
        }

        #region Information

        private TimeSpan heartbeatInterval = new TimeSpan(0, 0, 5);

        public TimeSpan HeartbeatInterval
        {
            get { return heartbeatInterval; }
            set { heartbeatInterval = value; }
        }

        private TimeSpan heartbeatTimeout = new TimeSpan(0, 0, 16);

        public TimeSpan HeartbeatTimeout
        {
            get { return heartbeatTimeout; }
            set { heartbeatTimeout = value; }
        }


        private DateTime lastContact = DateTime.Now;

        public DateTime LastContact
        {
            get { return lastContact; }
            protected set { lastContact = value; }
        }

        #endregion       

        #region Health Check

        public bool Healthy
        {
            get { return socket.Connected && DateTime.Now.Subtract(LastContact) <= HeartbeatTimeout; }
        }

        private bool heartbeatEnabled = false;

        public bool HeartbeatEnabled
        {
            get { return heartbeatEnabled; }
            set 
            {
                if (heartbeatEnabled != value)
                {
                    if(value && !Healthy)
                        throw new InvalidOperationException("Unable to start the heartbeat on a socket that is not Healthy!");

                    heartbeatEnabled = value;
                    if (heartbeatEnabled && Healthy)
                        heartbeatTimer.Change(TimeSpan.Zero, HeartbeatInterval);
                    else
                        heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }                
            }
        }

        private void Heartbeat(object state)
        {
            if(Healthy)
                SendHeartbeat();
            else
                heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void SendHeartbeat()
        {
            RdmNetHeartbeat heartbeatPacket = new RdmNetHeartbeat();
            SendPacket(heartbeatPacket);
        }


        void HealthCheckedTcpSocket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            LastContact = DateTime.Now;
        }

        #endregion

        #region RDM

        public void SendPacket(AcnPacket packet)
        {
            //Set the senders CID.
            packet.Root.SenderId = SenderId;

            MemoryStream data = new MemoryStream();
            AcnBinaryWriter writer = new AcnBinaryWriter(data);

            AcnPacket.WritePacket(packet, writer);

            socket.Send(data.GetBuffer(), 0, (int)data.Length, SocketFlags.None);
        }

        #endregion

        private class HeartbeatProtocolFilter: IProtocolFilter
        {
            private HealthCheckedTcpSocket parent;

            public HeartbeatProtocolFilter(HealthCheckedTcpSocket parent)
            {
                this.parent = parent;
            }

            int IProtocolFilter.ProtocolId
            {
                get { return (int) ProtocolIds.Null; }
            }

            void IProtocolFilter.ProcessPacket(IPEndPoint source, AcnRootLayer header, AcnBinaryReader data)
            {
                parent.LastContact = DateTime.Now;
            }
        }

    }
}

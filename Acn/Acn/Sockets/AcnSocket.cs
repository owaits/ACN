using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Acn.IO;
using Acn.Packets.sAcn;

namespace Acn.Sockets
{
    public class AcnSocket:Socket
    {
        public event UnhandledExceptionEventHandler UnhandledException;

        #region Setup and Initialisation

        public AcnSocket(Guid senderId)
            : base(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp)
        {
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

        public void Open(IPAddress adapterIP)
        {
            Open(new IPEndPoint(adapterIP, Port));
        }

        public void Open(IPEndPoint localEndPoint)
        {           
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(localEndPoint);

            //Multi-cast socket settings
            MulticastLoopback = true;            
            SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 20);        //Only join local LAN group.

            PortOpen = true;

            StartRecieve(null);
        }

        public void StartRecieve(MemoryStream recieveState)
        {
            try
            {
                EndPoint localPort = new IPEndPoint(IPAddress.Any, Port);

                if (recieveState == null)
                {
                    recieveState = new MemoryStream(1024);
                    recieveState.SetLength(1024);
                }
                recieveState.Seek(0, SeekOrigin.Begin);

                BeginReceiveFrom(recieveState.GetBuffer(), 0,(int) recieveState.Length, SocketFlags.None, ref localPort, new AsyncCallback(OnRecieve), recieveState);
            }
            catch (Exception ex)
            {
                RaiseUnhandledException(new ApplicationException("An error ocurred while trying to start recieving CITP.", ex));
            }
        }

        private void OnRecieve(IAsyncResult state)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            if (PortOpen)
            {
                MemoryStream recieveState = (MemoryStream)(state.AsyncState);
                
                try
                {
                    if (recieveState != null)
                    {
                        EndReceiveFrom(state, ref remoteEndPoint);

                        //Protect against UDP loopback where we recieve our own packets.
                        if (LocalEndPoint != remoteEndPoint)
                        {
                            LastPacket = DateTime.Now;
                            ProcessAcnPacket((IPEndPoint) remoteEndPoint,new AcnBinaryReader(recieveState));
                        }
                    }
                }
                catch (Exception ex)
                {
                    RaiseUnhandledException(ex);
                }
                finally
                {
                    //Attempt to recieve another packet.
                    StartRecieve(recieveState);
                }
            }
        }

        private void ProcessAcnPacket(IPEndPoint source, AcnBinaryReader data)
        {
            AcnRootLayer rootLayer = new AcnRootLayer();
            rootLayer.ReadData(data);

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
            if (UnhandledException != null) 
                UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;
            base.Dispose(disposing);
        }

        #endregion
    }
}

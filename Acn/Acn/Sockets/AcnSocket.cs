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
            byte[] byteTrue = new byte[4] {0,0,0, 1};
            IOControl(SIO_UDP_CONNRESET, byteTrue, null);

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
                RaiseUnhandledException(new ApplicationException("An error ocurred while trying to start recieving CITP.", ex));
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
                        recieveState.Item1.EndReceiveFrom(state, ref remoteEndPoint);

                        //Protect against UDP loopback where we recieve our own packets.
                        if (LocalEndPoint != remoteEndPoint)
                        {
                            LastPacket = DateTime.Now;

                            IPEndPoint ipEndPoint = (IPEndPoint)remoteEndPoint;

                            //If this is a TCP connection then the returned enpoint will be empty and we must use the connection endpoint.
                            if (ipEndPoint.Port == 0)
                                ipEndPoint = (IPEndPoint)recieveState.Item1.RemoteEndPoint;

                            ProcessAcnPacket(ipEndPoint, new AcnBinaryReader(recieveState.Item2));
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

        private void ProcessAcnPacket(IPEndPoint source, AcnBinaryReader data)
        {
            AcnRootLayer rootLayer = new AcnRootLayer();
            rootLayer.ReadData(data,false);

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

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;
            base.Dispose(disposing);
        }

        #endregion
    }
}

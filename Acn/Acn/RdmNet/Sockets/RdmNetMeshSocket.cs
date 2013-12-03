using Acn.Rdm;
using Acn.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Acn.RdmNet.Sockets
{
    public class RdmNetMeshSocket:RdmNetSocket
    {
        private TcpListener connectionListener = null;

        #region Setup and Initialisation

        public RdmNetMeshSocket(UId rdmId, Guid sourceId, string sourceName)
            : base(rdmId, sourceId, sourceName)
        {
        }

        public void Start(IPAddress networkAdapter)
        {
            connectionListener = new TcpListener(networkAdapter, TcpPort);
            connectionListener.BeginAcceptSocket(new AsyncCallback(DoNewConnection), connectionListener);
        }

        #endregion

        #region Information

        private int tcpPort = RdmNetSocket.RdmNetPort;

        public int TcpPort
        {
            get { return tcpPort; }
            set { tcpPort = value; }
        }

        #endregion

        #region TCP Traffic

        private void DoNewConnection(IAsyncResult state)
        {
            TcpListener listener = (TcpListener)state.AsyncState;
            Socket clientSocket = listener.EndAcceptSocket(state);
        }

        #endregion
        
        #region Device Management

        private Dictionary<RdmEndPoint, HealthCheckedTcpSocket> devices = new Dictionary<RdmEndPoint, HealthCheckedTcpSocket>(new RdmEndpointComparer());

        public void AddKnownDevice(RdmEndPoint endpoint)
        {
            if(!devices.ContainsKey(endpoint))
            {
                HealthCheckedTcpSocket device = HealthCheckedTcpSocket.Connect(endpoint,SenderId);
                device.NewRdmPacket += device_NewRdmPacket;
                devices.Add(endpoint,device);
            }
        }

        public void RemoveKnownDevice(RdmEndPoint endpoint)
        {

        }

        void device_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            RaiseNewRdmPacket((RdmEndPoint) e.Source, e.Packet);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

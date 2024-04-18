﻿using LXProtocols.Acn.Packets.RdmNet;
using LXProtocols.Acn.Packets.RdmNet.Broker;
using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LXProtocols.Acn.RdmNet.Sockets
{
    public class RdmNetMeshSocket:RdmNetSocket
    {
        private TcpListener connectionListener = null;

        public event EventHandler<NewRdmNetDeviceEventArgs> DeviceFound;
        public event EventHandler<NewRdmNetDeviceEventArgs> ControllerFound;

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

        private Dictionary<RdmEndPoint, HealthCheckedTcpSocket> brokers = new Dictionary<RdmEndPoint, HealthCheckedTcpSocket>(new RdmEndpointComparer());

        public void AddBroker(RdmEndPoint endpoint)
        {
            if(!brokers.ContainsKey(endpoint))
            {
                HealthCheckedTcpSocket broker = HealthCheckedTcpSocket.Connect(endpoint,SenderId);
                //RdmNetBrokerSocket broker = RdmNetBrokerSocket.Connect(endpoint, SenderId);
                broker.UnhandledException += device_UnhandledException;
                broker.NewRdmPacket += device_NewRdmPacket;
                broker.DeviceFound += Broker_DeviceFound;
                broker.ControllerFound += Broker_ControllerFound;
                brokers.Add(endpoint, broker);

                broker.SendPacket(new RdmNetBrokerConnectPacket()
                {
                    ClientScope = "default",
                    E133Version = 1,
                    SearchDomain = "",
                    ConnectionFlags = BrokerConnectionFlags.IncrementalUpdates,
                    Client = new RdmNetRPTClientEntryPdu()
                    {
                        ClientId = SenderId,
                        ClientUId = RdmSourceId,
                        ClientType = RPTClientType.Controller
                    }
                });
            }
        }

        private void Broker_ControllerFound(object sender, NewRdmNetDeviceEventArgs e)
        {
            ControllerFound(sender, e);
        }

        private void Broker_DeviceFound(object sender, NewRdmNetDeviceEventArgs e)
        {
            DeviceFound(sender, e);
        }

        void device_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            RaiseUnhandledException((Exception) e.ExceptionObject);
        }

        public void RemoveKnownDevice(RdmEndPoint endpoint)
        {

        }

        void device_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            RaiseNewRdmPacket((RdmEndPoint) e.Source, e.Packet);
        }

        #endregion

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                foreach(var socket in brokers.Values)
                {
                    socket.Dispose();
                }
                brokers.Clear();
            }
            base.Dispose(disposing);
        }
    }
}

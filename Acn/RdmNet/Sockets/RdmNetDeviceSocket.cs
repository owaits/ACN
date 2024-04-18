using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;
using LXProtocols.Acn.Packets.RdmNet.RPT;

namespace LXProtocols.Acn.RdmNet.Sockets
{
    public class RdmNetDeviceSocket:RdmNetSocket
    {
        private TcpListener connectionListener = null;

        #region Setup and Initialisation
		 
        public RdmNetDeviceSocket(UId rdmId, Guid sourceId, string sourceName)
            : base(rdmId, sourceId, sourceName)
        {
        }

        public override void Open(System.Net.IPEndPoint localEndPoint)
        {
            base.Open(localEndPoint);

            connectionListener = new TcpListener(localEndPoint);
            connectionListener.Start();
            connectionListener.BeginAcceptSocket(new AsyncCallback(DoNewConnection), connectionListener);
        }

        public bool IsDisposed = false;

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                if (connectionListener != null)
                    connectionListener.Stop();

                AliveTcpSocket = null;
            }
            if(disposing && AliveTcpSocket != null)
            {
                AliveTcpSocket.Dispose();
            }

            base.Dispose(disposing);
        }

	    #endregion

        #region TCP Traffic

        private object tcpLock = new object();
        private HealthCheckedTcpSocket aliveTcpSocket = null;

        public HealthCheckedTcpSocket AliveTcpSocket
        {
            get { return aliveTcpSocket; }
            protected set 
            { 
                if(aliveTcpSocket != value)
                {
                    if (aliveTcpSocket != null)
                    {
                        aliveTcpSocket.Dispose();
                    }
                    
                    aliveTcpSocket = value; 
                }
                
            }
        }

        public bool IsTcpConnectionAlive()
        {
            return AliveTcpSocket != null && AliveTcpSocket.Healthy;
        }

        private void DoNewConnection(IAsyncResult state)
        {
            if (!IsDisposed)
            {
                TcpListener listener = (TcpListener)state.AsyncState;
                if (listener.Server.IsBound)
                {
                    try
                    {
                        Socket clientSocket = listener.EndAcceptSocket(state);

                        if (IsTcpConnectionAlive())
                        {
                            clientSocket.Close();
                        }
                        else
                        {
                            HealthCheckedTcpSocket socket = new HealthCheckedTcpSocket(clientSocket, RdmSourceId, SenderId, SourceName);
                            socket.UnhandledException += socket_UnhandledException;
                            socket.Open(new IPEndPoint(IPAddress.Any, 0));
                            AliveTcpSocket = socket;
                        }
                    }
                    finally
                    {
                        listener.BeginAcceptSocket(new AsyncCallback(DoNewConnection), listener);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the UnhandledException event of the socket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        void socket_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            RaiseUnhandledException((Exception)e.ExceptionObject);
        }


        #endregion

        #region RDM

        public void SendBackgroundRdm(RdmPacket packet)
        {
            if (!IsTcpConnectionAlive())
                throw new InvalidOperationException("No healthy TCP connection exists for RDMNet broadcast.");

            //Fill in addition details
            packet.Header.SourceId = RdmSourceId;
            packet.Header.DestinationId = UId.Broadcast;

            //Create Rdm Packet
            MemoryStream rdmData = new MemoryStream();
            RdmBinaryWriter rdmWriter = new RdmBinaryWriter(rdmData);

            //Write the RDM sub-start code.
            rdmWriter.Write((byte)RdmVersions.SubMessage);

            //Write the RDM packet
            RdmPacket.WritePacket(packet, rdmWriter);

            //Write the checksum
            rdmWriter.WriteNetwork((short)RdmPacket.CalculateChecksum(rdmData.GetBuffer()));

            //Create sACN Packet
            RdmNetRptRequestPacket dmxPacket = new RdmNetRptRequestPacket();
            dmxPacket.Rpt.SourceId = RdmSourceId;
            dmxPacket.Rpt.DestinationId = packet.Header.SourceId;
            dmxPacket.Request.RdmData = rdmData.GetBuffer();

            AliveTcpSocket.SendPacket(dmxPacket);

            RaiseRdmPacketSent(new NewPacketEventArgs<RdmPacket>(null, packet)); 
        }

        #endregion

    }
}

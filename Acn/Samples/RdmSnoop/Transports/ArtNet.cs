using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Acn.ArtNet.Sockets;
using Acn.Rdm;
using Acn.ArtNet.Packets;
using Acn.ArtNet;

namespace RdmSnoop.Transports
{
    public class ArtNet : IRdmTransport
    {
        private ArtNetSocket socket = new ArtNetSocket(UId.NewUId(0));

        public event EventHandler<DeviceFoundEventArgs> NewDeviceFound;

        public ArtNet()
        {
            socket.NewPacket += new EventHandler<Acn.Sockets.NewPacketEventArgs<ArtNetPacket>>(socket_NewPacket);
        }

        void socket_NewPacket(object sender, Acn.Sockets.NewPacketEventArgs<ArtNetPacket> e)
        {
            switch ((ArtNetOpCodes)e.Packet.OpCode)
            {
                case ArtNetOpCodes.PollReply:
                    ProcessPollReply((ArtPollReplyPacket)e.Packet);
                    break;
                case ArtNetOpCodes.TodData:
                    ProcessTodData((ArtTodDataPacket)e.Packet);
                    break;
            }
        }


        private IPAddress localAdapter;

        public IPAddress LocalAdapter
        {
            get { return localAdapter; }
            protected set { localAdapter = value; }
        }

        public void Start(System.Net.IPAddress localAdapter)
        {
            if (!socket.PortOpen)
            {
                LocalAdapter = localAdapter;

                socket.Open(localAdapter, IPAddress.Broadcast);

                SendTodRequest();
            }
        }

        public void Stop()
        {
            socket.Close();
        }

        public Acn.Sockets.IRdmSocket GetDeviceSocket(Acn.Rdm.UId deviceId)
        {
            return socket;
        }

        #region Art Net


        private void ProcessPollReply(ArtPollReplyPacket packet)
        {
            //Does device support RDM?
            //if ((packet.Status & PollReplyStatus.RdmCapable) > 0)
            //{
            //    //Request RDM Table of Devices

            //}
        }

        private void ProcessTodData(ArtTodDataPacket packet)
        {
            
        }

        private void SendTodRequest()
        {
            ArtTodRequestPacket packet = new ArtTodRequestPacket();
            socket.Send(packet);
        }

        #endregion
    }
}

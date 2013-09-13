using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Acn.ArtNet.Sockets;
using Acn.Rdm;
using Acn.ArtNet.Packets;
using Acn.ArtNet;
using Acn.Sockets;
using Acn.Rdm.Routing;

namespace RdmSnoop.Transports
{
    public class ArtNet : ISnoopTransport, IDisposable
    {
        private ArtNetSocket socket = null;

        public event EventHandler<DeviceFoundEventArgs> NewDeviceFound;


        public ArtNet()
        {
        }

        void socket_NewPacket(object sender, Acn.Sockets.NewPacketEventArgs<ArtNetPacket> e)
        {
            switch ((ArtNetOpCodes)e.Packet.OpCode)
            {
                case ArtNetOpCodes.PollReply:
                    ProcessPollReply((ArtPollReplyPacket)e.Packet, e.Source);
                    break;
                case ArtNetOpCodes.TodData:
                    ProcessTodData((ArtTodDataPacket)e.Packet,e.Source);
                    break;
            }
        }


        private IPAddress localAdapter;

        public IPAddress LocalAdapter
        {
            get { return localAdapter; }
            set { localAdapter = value; }
        }

        private IPAddress subnetMask;

        public IPAddress SubnetMask
        {
            get { return subnetMask; }
            set { subnetMask = value; }
        }

        public void Start()
        {
            if (socket == null || !socket.PortOpen)
            {
                LocalAdapter = localAdapter;

                socket = new ArtNetSocket(UId.NewUId(32));
                socket.NewPacket += new EventHandler<Acn.Sockets.NewPacketEventArgs<ArtNetPacket>>(socket_NewPacket);
                socket.Open(LocalAdapter, SubnetMask);

                Discover(DiscoveryType.GatewayDiscovery);
            }
        }

        public void Discover(DiscoveryType type)
        {
            if (type == DiscoveryType.GatewayDiscovery)
                SendArtPoll();
            if (type == DiscoveryType.DeviceDiscovery)
            {
                for (int n = 0; n < byte.MaxValue;n++)
                    SendTodControl(n,ArtTodControlCommand.AtcFlush);
            }
        }

        public void Stop()
        {
            if (reliableSocket != null)
                reliableSocket.Dispose();
            if (socket != null)
                socket.Close();
        }

        private RdmReliableSocket reliableSocket = null;

        public IRdmSocket Socket
        {
            get 
            {
                if (reliableSocket == null && socket != null)
                    reliableSocket = new RdmReliableSocket(socket);

                return reliableSocket; 
            }
        }

        #region Art Net


        private void ProcessPollReply(ArtPollReplyPacket packet,IPEndPoint endPoint)
        {
            //Does device support RDM?
            if ((packet.Status & PollReplyStatus.RdmCapable) > 0)
            {
                //if (NewDeviceFound != null)
                //    NewDeviceFound(this, new DeviceFoundEventArgs(id, endPoint.Address));

                //Request RDM Table of Devices

                //Find out which universes the device supports.
                List<byte> universes = new List<byte>();
                for(int n=0; n<4;n++)
                {
                    if((packet.PortTypes[n] & 0x80)>0)
                    {
                        universes.Add(packet.SwOut[n]);
                    }
                }

                //Request TOD for each input universe.
                SendTodRequest(endPoint.Address, universes);
            }
        }

        private void ProcessTodData(ArtTodDataPacket packet,IPEndPoint endPoint)
        {
            foreach (UId id in packet.Devices)
            {
                if (NewDeviceFound != null)
                    NewDeviceFound(this, new DeviceFoundEventArgs(id,new RdmEndPoint(endPoint.Address,(int) packet.Universe)));
            }
        }

        private void SendArtPoll()
        {
            ArtPollPacket packet = new ArtPollPacket();
            packet.TalkToMe = 6;
            socket.Send(packet);
        }

        private void SendTodRequest(IPAddress address, List<byte> universes)
        {
            ArtTodRequestPacket packet = new ArtTodRequestPacket();
            packet.RequestedUniverses = universes;
            socket.Send(packet);
        }

        private void SendTodControl(int port, ArtTodControlCommand command)
        {
            ArtTodControlPacket packet = new ArtTodControlPacket();
            packet.Address = (byte) port;
            packet.Command = command;
            socket.Send(packet);
        }

        #endregion

        public void Dispose()
        {
            Stop();
        }

        public event EventHandler Starting;

        public event EventHandler Stoping;
    }
}

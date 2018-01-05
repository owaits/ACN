using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Sockets;
using Acn.Rdm.Packets;
using System.Net;
using Acn.Rdm.Packets.Net;
using Acn.Rdm;
using Acn.Rdm.Broker;
using Acn.RdmNet.Sockets;

namespace SandboxAcnDevice
{
    public class RdmDevice
    {
        RdmNetDeviceSocket socket = null;
        RdmMessageBroker broker = new RdmMessageBroker();

        public RdmDevice(RdmNetDeviceSocket socket)
        {
            UId = Guid.NewGuid();
            this.socket = socket;

            SetupBroker();

            socket.NewRdmPacket += new EventHandler<NewPacketEventArgs<Acn.Rdm.RdmPacket>>(socket_NewRdmPacket);
        }

        private void SetupBroker()
        {
            //Port List Reply
            EndpointList.Reply portListReply = new EndpointList.Reply();
            portListReply.EndpointIDs = new List<short>() { 1, 2, 3, 4 };
            broker.RegisterResponse(RdmParameters.EndpointList, portListReply);

        }

        void socket_NewRdmPacket(object sender, NewPacketEventArgs<Acn.Rdm.RdmPacket> e)
        {
            RdmPacket replyPacket = broker.ProcessPacket(e.Packet);
            if (replyPacket != null)
            {
                socket.SendRdm(replyPacket,new RdmEndPoint(e.Source),e.Packet.Header.SourceId);
            }
        }


        public Guid UId { get; protected set; }

        public int Ports { get; set; }


    }
}

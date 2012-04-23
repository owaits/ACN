using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Acn.Sockets;
using Acn.Rdm;
using Acn.Rdm.Packets;
using Acn.Rdm.Packets.Net;

namespace RdmNetworkMonitor
{
    public class RdmDeviceBroker
    {
        RdmSocket socket = null;

        public event EventHandler PortsChanged;

        public RdmDeviceBroker(RdmSocket socket, UId id,RdmAddress address)
        {
            Id = id;
            Address = address;
            this.socket = socket;

            socket.NewRdmPacket += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_NewRdmPacket);
        }

        void socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            PortList.Reply ports = e.Packet as PortList.Reply;
            if (ports != null)
            {
                Ports = ports.PortNumbers;
                if (PortsChanged != null)
                    PortsChanged(this, EventArgs.Empty);
            }
        }

        public UId Id { get; set; }

        public RdmAddress Address { get; set; }

        public List<short> Ports { get; set; }

        public void Identify()
        {
            PortList.Get getPorts = new PortList.Get();
            socket.SendRdm(getPorts,Address,Id);
        }
    }
}

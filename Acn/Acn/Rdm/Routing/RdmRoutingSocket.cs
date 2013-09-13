using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Sockets;

namespace Acn.Rdm.Routing
{
    public class RdmRoutingSocket:IRdmSocket
    {
        private RdmRouter router = null;

        public RdmRoutingSocket(RdmRouter router)
        {
            this.router = router; 
        }

        public void Bind(IRdmSocket socket)
        {
            socket.NewRdmPacket += Socket_NewRdmPacket;
        }

        public void UnBind(IRdmSocket socket)
        {
            socket.NewRdmPacket -= Socket_NewRdmPacket;
        }

        private void Socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            ProcessPacket(e);
        }

        protected void ProcessPacket(NewPacketEventArgs<RdmPacket> e)
        {
            RaiseNewRdmPacket(e);
        }

        #region IRdmSocket

        public event EventHandler<NewPacketEventArgs<RdmPacket>> NewRdmPacket;

        protected void RaiseNewRdmPacket(NewPacketEventArgs<RdmPacket> packetInfo)
        {
            if (NewRdmPacket != null)
                NewRdmPacket(this, packetInfo);
        }

        public event EventHandler<NewPacketEventArgs<RdmPacket>> RdmPacketSent;

        protected void RaiseRdmPacketSent(NewPacketEventArgs<RdmPacket> packetInfo)
        {
            if (RdmPacketSent != null)
                RdmPacketSent(this, packetInfo);
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId)
        {
            List<RdmRouteBinding> transportsToUse = router.GetTransportsRoutes(targetId);

            //Send the packet on all transports.
            foreach (RdmRouteBinding binding in transportsToUse)
            {
                binding.Transport.Socket.SendRdm(packet, targetAddress, targetId);
            }
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId, UId sourceId)
        {
            List<RdmRouteBinding> transportsToUse = router.GetTransportsRoutes(targetId);

            //Send the packet on all transports.
            foreach (RdmRouteBinding binding in transportsToUse)
            {
                binding.Transport.Socket.SendRdm(packet, targetAddress, targetId, sourceId);
            }
        }
        #endregion
    }
}

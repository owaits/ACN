using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.Sockets;

namespace LXProtocols.Acn.Rdm.Routing
{
    public class RdmRoutingSocket:IRdmSocket
    {
        private RdmRouter router = null;

        public RdmRoutingSocket(RdmRouter router)
        {
            this.router = router; 
        }

        /// <summary>
        /// Gets or sets whether RDM packets are blocked by this socket.
        /// </summary>
        public bool BlockRDM { get; set; }

        public void Bind(IRdmSocket socket)
        {
            socket.NewRdmPacket += Socket_NewRdmPacket;
            socket.RdmPacketSent += socket_RdmPacketSent;
        }

        public void UnBind(IRdmSocket socket)
        {
            socket.NewRdmPacket -= Socket_NewRdmPacket;
            socket.RdmPacketSent -= socket_RdmPacketSent;
        }

        private void Socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            ProcessPacket(e);
        }

        protected void ProcessPacket(NewPacketEventArgs<RdmPacket> e)
        {
            RaiseNewRdmPacket(e);
        }

        void socket_RdmPacketSent(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            RaiseRdmPacketSent(e);
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
            if (BlockRDM)
                return;

            List<RdmRouteBinding> transportsToUse = router.GetTransportsRoutes(targetId);

            //Send the packet on all transports.
            foreach (RdmRouteBinding binding in transportsToUse)
            {
                foreach(IRdmSocket socket in binding.Transport.Sockets)
                    socket.SendRdm(packet, targetAddress, targetId);
            }
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId, UId sourceId)
        {
            if (BlockRDM)
                return;

            List<RdmRouteBinding> transportsToUse = router.GetTransportsRoutes(targetId);

            //Send the packet on all transports.
            foreach (RdmRouteBinding binding in transportsToUse)
            {
                foreach (IRdmSocket socket in binding.Transport.Sockets)
                    socket.SendRdm(packet, targetAddress, targetId, sourceId);
            }
        }
        #endregion
    }
}

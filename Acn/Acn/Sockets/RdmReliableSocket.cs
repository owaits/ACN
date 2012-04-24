using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm;

namespace Acn.Sockets
{
    /// <summary>
    /// This RDM socket provides a reliable means of transporting RDM packets over an unreliable network. It
    /// will wrap an unreliable RDM socket.
    /// </summary>
    /// <remarks>
    /// Ensures that a transaction is completed by re-requesting packets for which no response has been recieved.
    /// </remarks>
    public class RdmReliableSocket:IRdmSocket
    {
        private IRdmSocket socket = null;
        private Dictionary<int, RdmPacket> transactionQueue = new Dictionary<int, RdmPacket>();

        public RdmReliableSocket(IRdmSocket socket)
        {
            this.socket = socket;

            socket.NewRdmPacket += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_NewRdmPacket);
            socket.RdmPacketSent += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_RdmPacketSent);
        }

        private int transactionNumber = 1;

        public int TransactionNumber
        {
            get { return transactionNumber; }
            protected set { transactionNumber = value; }
        }

        private void RegisterTransaction(RdmPacket packet)
        {
            if (packet.Header.Command == RdmCommands.Get || packet.Header.Command == RdmCommands.Set)
            {
                transactionQueue.Add(TransactionNumber, packet);
                TransactionNumber++;
            }
        }

        void socket_RdmPacketSent(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            if (RdmPacketSent != null)
                RdmPacketSent(sender, e);
        }

        void socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            if (e.Packet.Header.TransactionNumber > 0)
            {
                if (e.Packet.Header.Command == RdmCommands.GetResponse || e.Packet.Header.Command == RdmCommands.SetResponse)
                {
                    transactionQueue.Remove(e.Packet.Header.TransactionNumber);
                }
            }

            if (NewRdmPacket != null)
                NewRdmPacket(sender, e);
        }

        #region Events

        public event EventHandler<NewPacketEventArgs<Rdm.RdmPacket>> NewRdmPacket;
        public event EventHandler<NewPacketEventArgs<Rdm.RdmPacket>> RdmPacketSent;

        #endregion

        #region Communications
        
        public void SendRdm(RdmPacket packet, RdmAddress targetAddress, UId targetId)
        {
            RegisterTransaction(packet);

            socket.SendRdm(packet, targetAddress, targetId);
        }

        public void SendRdm(RdmPacket packet, RdmAddress targetAddress, UId targetId, UId sourceId)
        {
            RegisterTransaction(packet);

            socket.SendRdm(packet, targetAddress, targetId,sourceId);
        }

        #endregion



    }
}

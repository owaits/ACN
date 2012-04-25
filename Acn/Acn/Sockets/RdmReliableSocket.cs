using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.Rdm;
using System.ComponentModel;
using System.Threading;

namespace Acn.Sockets
{
    /// <summary>
    /// This RDM socket provides a reliable means of transporting RDM packets over an unreliable network. It
    /// will wrap an unreliable RDM socket.
    /// </summary>
    /// <remarks>
    /// Ensures that a transaction is completed by re-requesting packets for which no response has been recieved.
    /// </remarks>
    public class RdmReliableSocket:IRdmSocket,INotifyPropertyChanged,IDisposable
    {
        private IRdmSocket socket = null;
        private Dictionary<byte, Transaction> transactionQueue = new Dictionary<byte, Transaction>();
        private Timer retryTimer;

        private class Transaction
        {
            public Transaction(byte number, RdmPacket packet, RdmAddress address, UId id)
            {
                Number = number;
                Packet = packet;
                TargetAddress = address;
                TargetId = id;
                Attempts = 0;
                LastAttempt = DateTime.Now;
            }

            public byte Number;
            public RdmPacket Packet;
            public RdmAddress TargetAddress;
            public UId TargetId;

            public int Attempts = 0;
            public DateTime LastAttempt;
        }

        public RdmReliableSocket(IRdmSocket socket)
        {
            this.socket = socket;

            socket.NewRdmPacket += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_NewRdmPacket);
            socket.RdmPacketSent += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_RdmPacketSent);

            retryTimer = new Timer(new TimerCallback(Retry));
        }

        public IRdmSocket InnerSocket
        {
            get { return socket; }
        }

        private TimeSpan retryInterval = new TimeSpan(0, 0, 3);

        public TimeSpan RetryInterval
        {
            get { return retryInterval; }
            set { retryInterval = value; }
        }

        private int retryAttempts = 4;

        public int RetryAttempts
        {
            get { return retryAttempts; }
            set { retryAttempts = value; }
        }



        private byte transactionNumber = 1;

        public byte TransactionNumber
        {
            get { return transactionNumber; }
            protected set { transactionNumber = value; }
        }

        private byte AllocateTransactionNumber()
        {
            lock (transactionQueue)
            {
                do
                {
                    TransactionNumber++;
                }
                while (transactionQueue.ContainsKey(TransactionNumber));

                return TransactionNumber;
            }
        }

        private int packetsSent = 0;

        public int PacketsSent
        {
            get { return packetsSent; }
            protected set 
            {
                if (packetsSent != value)
                {
                    packetsSent = value;
                    RaisePropertyChanged("PacketsSent");
                }
            }
        }

        private int packetsRecieved = 0;

        public int PacketsRecieved
        {
            get { return packetsRecieved; }
            protected set
            {
                if (packetsRecieved != value)
                {
                    packetsRecieved = value;
                    RaisePropertyChanged("PacketsRecieved");
                }
            }
        }

        private int packetsDropped = 0;

        public int PacketsDropped
        {
            get { return packetsDropped; }
            protected set
            {
                if (packetsDropped != value)
                {
                    packetsDropped = value;
                    RaisePropertyChanged("PacketsDropped");
                }
            }
        }

        private int transactionsStarted = 0;

        public int TransactionsStarted
        {
            get { return transactionsStarted; }
            protected set
            {
                if (transactionsStarted != value)
                {
                    transactionsStarted = value;
                    RaisePropertyChanged("TransactionsStarted");
                }
            }
        }

        private int transactionsFailed = 0;

        public int TransactionsFailed
        {
            get { return transactionsFailed; }
            protected set
            {
                if (transactionsFailed != value)
                {
                    transactionsFailed = value;
                    RaisePropertyChanged("TransactionsFailed");
                }
            }
        }

        private void RegisterTransaction(RdmPacket packet, RdmAddress address, UId id)
        {
            lock (transactionQueue)
            {
                if (packet.Header.Command == RdmCommands.Get || packet.Header.Command == RdmCommands.Set)
                {
                    byte number = AllocateTransactionNumber();
                    transactionQueue.Add(number, new Transaction(number,packet, address, id));
                    packet.Header.TransactionNumber = number;

                    if (transactionQueue.Count == 1)
                        retryTimer.Change(RetryInterval, RetryInterval);

                    TransactionsStarted++;
                }
            }
        }

        private void Retry(object state)
        {
            DateTime timeStamp = DateTime.Now;
            List<byte> failedTransactions = new List<byte>();
            List<Transaction> retryTransactions = new List<Transaction>();

            lock (transactionQueue)
            {
                foreach (Transaction item in transactionQueue.Values)
                {
                    if (item.Attempts > RetryAttempts)
                        failedTransactions.Add(item.Number);
                    else
                    {
                        retryTransactions.Add(item);
                        item.Attempts++;
                        item.LastAttempt = timeStamp;
                    }
                }

                foreach (byte transactionId in failedTransactions)
                    transactionQueue.Remove(transactionId);
            }

            PacketsDropped += retryTransactions.Count + failedTransactions.Count;
            TransactionsFailed += failedTransactions.Count;
            
            foreach (Transaction transaction in retryTransactions)
            {
                socket.SendRdm(transaction.Packet, transaction.TargetAddress, transaction.TargetId);
            }
        }

        void socket_RdmPacketSent(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            PacketsSent++;

            if (RdmPacketSent != null)
                RdmPacketSent(sender, e);
        }

        void socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            PacketsRecieved++;

            if (e.Packet.Header.TransactionNumber > 0)
            {
                if (e.Packet.Header.Command == RdmCommands.GetResponse || e.Packet.Header.Command == RdmCommands.SetResponse)
                {
                    lock (transactionQueue)
                    {
                        transactionQueue.Remove(e.Packet.Header.TransactionNumber);

                        if (transactionQueue.Count == 0)
                            retryTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
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
            RegisterTransaction(packet, targetAddress, targetId);

            socket.SendRdm(packet, targetAddress, targetId);
        }

        public void SendRdm(RdmPacket packet, RdmAddress targetAddress, UId targetId, UId sourceId)
        {            
            RegisterTransaction(packet, targetAddress, targetId);

            socket.SendRdm(packet, targetAddress, targetId, sourceId);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
        public void Dispose()
        {
            retryTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}

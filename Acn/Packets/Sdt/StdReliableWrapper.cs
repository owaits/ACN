using Acn.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Packets.Sdt
{
    public class StdReliableWrapper:AcnPdu
    {
        public StdReliableWrapper()
            : base((int) StdVectors.ReliableWrapper,1)
        {
        }

        protected StdReliableWrapper(int vector)
            : base(vector, 1)
        {
        }

        #region Packet Contents

        public short ChannelNumber { get; set; }

        public int TotalSequenceNumber { get; set; }

        public int ReliableSequenceNumber { get; set; }

        public int OldestAvailableWrapper { get; set; }

        public short FirstMemberToAck { get; set; }

        public short LastMemberToAck { get; set; }

        public short MAKThreshold { get; set; }

        public AcnPdu Pdu { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            ChannelNumber = data.ReadOctet2();
            TotalSequenceNumber = data.ReadOctet4();
            ReliableSequenceNumber = data.ReadOctet4();
            OldestAvailableWrapper = data.ReadOctet4();
            FirstMemberToAck = data.ReadOctet2();
            LastMemberToAck = data.ReadOctet2();
            MAKThreshold = data.ReadOctet2();
            throw new NotImplementedException();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.WriteOctet(ChannelNumber);
            data.WriteOctet(TotalSequenceNumber);
            data.WriteOctet(ReliableSequenceNumber);
            data.WriteOctet(OldestAvailableWrapper);
            data.WriteOctet(FirstMemberToAck);
            data.WriteOctet(LastMemberToAck);
            data.WriteOctet(MAKThreshold);
            throw new NotImplementedException();
        }

        #endregion
    }
}

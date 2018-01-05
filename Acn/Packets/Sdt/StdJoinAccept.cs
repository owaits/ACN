using Acn.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Packets.Sdt
{
    public class StdJoinAccept:AcnPdu
    {
        public StdJoinAccept()
            : base((int) StdVectors.JoinAccept,1)
        {
        }

        #region Packet Contents

        public Guid LeaderId { get; set; }

        public short ChannelNumber { get; set; }

        public short MemberId { get; set; }

        public int ReliableSequenceNumber { get; set; }

        public short ReciprocalChannel { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            LeaderId = new Guid(data.ReadBytes(16));
            ChannelNumber = data.ReadOctet2();
            MemberId = data.ReadOctet2();
            ReliableSequenceNumber = data.ReadOctet4();
            ReciprocalChannel = data.ReadOctet2();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(LeaderId.ToByteArray());
            data.WriteOctet(ChannelNumber);
            data.WriteOctet(MemberId);
            data.WriteOctet(ReliableSequenceNumber);
            data.WriteOctet(ReciprocalChannel);
        }

        #endregion
    }
}

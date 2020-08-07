using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Acn.Packets.Sdt
{
    public class StdJoinRefuse : AcnPdu
    {
        public StdJoinRefuse()
            : base((int) StdVectors.JoinRefuse,1)
        {
        }

        #region Packet Contents

        public Guid LeaderId { get; set; }

        public short ChannelNumber { get; set; }

        public short MemberId { get; set; }

        public int ReliableSequenceNumber { get; set; }

        public ReasonCodes RefuseCode { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            LeaderId = new Guid(data.ReadBytes(16));
            ChannelNumber = data.ReadOctet2();
            MemberId = data.ReadOctet2();
            ReliableSequenceNumber = data.ReadOctet4();
            RefuseCode = (ReasonCodes) data.ReadByte();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(LeaderId.ToByteArray());
            data.WriteOctet(ChannelNumber);
            data.WriteOctet(MemberId);
            data.WriteOctet(ReliableSequenceNumber);
            data.Write((byte) RefuseCode);
        }

        #endregion
    }
}

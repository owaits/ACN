using Acn.IO;
using Acn.Packets.Sdt.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Packets.Sdt
{
    public class StdJoin:AcnPdu
    {
        public StdJoin()
            : base((int) StdVectors.Join,1)
        {
        }

        #region Packet Contents

        public Guid ComponentId { get; set; }

        public short MemberId { get; set; }

        public short ChannelNumber { get; set; }

        public short ReciprocalChannel { get; set; }

        public int TotalSequenceNumber { get; set; }

        public int ReliableSequenceNumber { get; set; }

        public SdtAddress DestinationAddress { get; set; }

        public ChannelParameterBlock ChannelParameterBlock { get; set; }

        public byte AdHocExpiry { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            ComponentId = new Guid(data.ReadBytes(16));
            MemberId = data.ReadOctet2();
            ChannelNumber = data.ReadOctet2();
            ReciprocalChannel = data.ReadOctet2();
            TotalSequenceNumber = data.ReadOctet4();
            ReliableSequenceNumber = data.ReadOctet4();
            DestinationAddress = SdtAddress.ReadData(data);
            ChannelParameterBlock = ChannelParameterBlock.ReadData(data);
            AdHocExpiry = data.ReadByte();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(ComponentId.ToByteArray());
            data.WriteOctet(MemberId);
            data.WriteOctet(ChannelNumber);
            data.WriteOctet(ReciprocalChannel);
            data.WriteOctet(TotalSequenceNumber);
            data.WriteOctet(ReliableSequenceNumber);
            DestinationAddress.WriteData(data);
            ChannelParameterBlock.WriteData(data);
            data.Write(AdHocExpiry);
        }

        #endregion
    }
}

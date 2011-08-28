using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;
using System.IO;

namespace Acn.Packets.sAcn
{
    public class FramingPdu:AcnPdu
    {
        public FramingPdu()
            : base(ProtocolIds.DMP)
        {
        }

        #region PDU Contents

        private string sourceName = string.Empty;

        public string SourceName
        {
            get { return sourceName; }
            set { sourceName = value; }
        }

        private byte priority = 0;

        public byte Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        private byte sequenceNumber = 0;

        public byte SequenceNumber
        {
            get { return sequenceNumber; }
            set { sequenceNumber = value; }
        }

        private byte options = 0;

        public byte Options
        {
            get { return options; }
            set { options = value; }
        }

        private short universe = 0;

        public short Universe
        {
            get { return universe; }
            set { universe = value; }
        }

        #endregion

        #region Read and Write

        protected override void ReadData(AcnBinaryReader data)
        {
            SourceName = data.ReadUtf8String(64);
            Priority = data.ReadByte();
            data.BaseStream.Seek(2, SeekOrigin.Current);
            SequenceNumber = data.ReadByte();
            Options = data.ReadByte();
            Universe = data.ReadOctet2();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.WriteUtf8String(SourceName,64);
            data.Write(Priority);
            data.BaseStream.Seek(2, SeekOrigin.Current);
            data.Write(SequenceNumber);
            data.Write(Options);
            data.WriteOctet(Universe);
        }

        #endregion

    }
}

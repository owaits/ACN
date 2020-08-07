using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LXProtocols.Acn.IO;

namespace LXProtocols.Acn.Packets.Dmp
{
    public class DmpGetProperty : AcnPdu
    {
        public DmpGetProperty()
            : base((int) DmpMessages.GetProperty,1)
        {
        }

        #region Packet Contents

        private byte addressType = 0;

        public byte AddressType
        {
            get { return addressType; }
            set { addressType = value; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            AddressType = data.ReadByte();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(AddressType);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acn.IO;

namespace Acn.Packets.Dmp
{
    public class DmpSetProperty : AcnPdu
    {
        public DmpSetProperty()
            : base((int) DmpMessages.SetProperty,1)
        {
        }

        #region Packet Contents

        private byte addressType = 0;

        public byte AddressType
        {
            get { return addressType; }
            set { addressType = value; }
        }

        private short firstPropertyAddress = 0;

        public short FirstPropertyAddress
        {
            get { return firstPropertyAddress; }
            set { firstPropertyAddress = value; }
        }

        private short addressIncrement = 0;

        public short AddressIncrement
        {
            get { return addressIncrement; }
            set { addressIncrement = value; }
        }

        public short PropertyLength
        {
            get { return (short) PropertyData.Length; }
        }

        private byte[] propertyData = null;

        public byte[] PropertyData
        {
            get { return propertyData; }
            set { propertyData = value; }
        }

        public byte[] Data
        {
            get { return PropertyData; }
            set { PropertyData = value; }
        }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            AddressType = data.ReadByte();
            FirstPropertyAddress = data.ReadOctet2();
            AddressIncrement = data.ReadOctet2();

            int propertyLength = data.ReadOctet2();
            PropertyData = data.ReadBytes(propertyLength);
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(AddressType);
            data.WriteOctet(FirstPropertyAddress);
            data.WriteOctet(AddressIncrement);
            data.WriteOctet(PropertyLength);
            WriteContent(data);
        }

        /// <summary>
        /// Writes the Property data to the packet stream.
        /// </summary>
        /// <remarks>
        /// This allows customization of the property data.
        /// </remarks>
        /// <param name="data">The packet data stream.</param>
        protected virtual void WriteContent(AcnBinaryWriter data)
        {
            data.Write(PropertyData);
        }

        #endregion
    }
}

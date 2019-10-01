using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    public class TCNetApplicationSpecificData:TCNetHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetOptIn"/> class.
        /// </summary>
        public TCNetApplicationSpecificData() : base(MessageTypes.ApplicationSpecificData)
        {
        }

        #endregion

        #region Packet Content

        public ushort ApplicationSignature { get; set; }

        public virtual uint DataSize
        {
            get { return 0; }
        }

        public uint TotalPackets { get; set; }

        public uint PacketNumber { get; set; }

        #endregion

        #region Read/Write

        /// <summary>
        /// Reads the data from a memory buffer into this packet object.
        /// </summary>
        /// <param name="data">The data to be read.</param>
        /// <remarks>
        /// Decodes the raw data into usable information.
        /// </remarks>
        public override void ReadData(TCNetBinaryReader data)
        {
            base.ReadData(data);

            ApplicationSignature = data.ReadNetwork16();
            int dataSize = (int) data.ReadNetwork32();
            TotalPackets = data.ReadNetwork32();
            PacketNumber = data.ReadNetwork32();

            ReadApplicationData(data,dataSize);
        }

        public virtual void ReadApplicationData(TCNetBinaryReader data, int dataSize)
        {
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.WriteToNetwork(ApplicationSignature);
            data.WriteToNetwork(DataSize);
            data.WriteToNetwork(TotalPackets);
            data.WriteToNetwork(PacketNumber);

            WriteApplicationData(data);
        }

        public virtual void WriteApplicationData(TCNetBinaryWriter data)
        {
        }

        #endregion
    }
}

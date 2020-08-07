using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXProtocols.Citp;
using LXProtocols.Citp.IO;
using LXProtocols.Citp.Packets;

namespace LXProtocols.Citp.Packets.CaEx
{
    public class CaExHeader:CitpHeader
    {
        public const string PacketType = "CAEX";

        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="CaExHeader"/> class.
        /// </summary>
        /// <param name="contentCode">The content code.</param>
        public CaExHeader(CaExContentCodes contentCode)
            : base(PacketType)
        {
            ContentCode = contentCode;
        }

        #endregion

        #region Packet Content

        public CaExContentCodes ContentCode { get; set; }

        #endregion

        #region Read/Write

        /// <summary>
        /// Reads the packet information from the specified stream.
        /// </summary>
        /// <param name="data">The stream to read the packet information from.</param>
        /// <remarks>
        /// Use to create a packet from a network stream.
        /// </remarks>
        public override void ReadData(CitpBinaryReader data)
        {
            base.ReadData(data);

            ContentCode = (CaExContentCodes) data.ReadUInt32();
        }

        /// <summary>
        /// Writes the information in this packet to the specified stream.
        /// </summary>
        /// <param name="data">The stream to write the packet information to.</param>
        public override void WriteData(CitpBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((uint) ContentCode);

        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return base.ToString() + " " + ContentCode;
        }
    }
}

using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Sent by devices wishing to take part in the DJ Tap network.
    /// </summary>
    /// <remarks>
    /// This packet must be sent at least every 2 seconds or the device will be deemed to no longer
    /// be present on the network.
    /// </remarks>
    /// <seealso cref="ProDJTap.Packets.DJTapHeader" />
    public class GWOffer:TCNetHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="GWOffer"/> class.
        /// </summary>
        public GWOffer():base(MessageTypes.GWOffer)
        {

        }

        #endregion

        #region Packet Content

        private string brand = string.Empty;

        /// <summary>
        /// Gets or sets the manufacturer of the transmitting decide.
        /// </summary>
        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }

        private string model = string.Empty;

        /// <summary>
        /// Gets or sets the model of the transmitting device.
        /// </summary>
        public string Model
        {
            get { return model; }
            set { model = value; }
        }


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

            Brand = ASCIIEncoding.ASCII.GetString(data.ReadBytes(8));
            Model = ASCIIEncoding.ASCII.GetString(data.ReadBytes(8));
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(ASCIIEncoding.ASCII.GetBytes(Brand.PadRight(8)));
            data.Write(ASCIIEncoding.ASCII.GetBytes(Model.PadRight(8)));
            data.Write((byte)0);
        }

        #endregion

    }
}

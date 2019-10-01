using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Send and Receive Realtime Keyboard Data Packets to control nodes remotely.
    /// </summary>
    /// <remarks>
    /// Response Required
    /// </remarks>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetHeader" />
    public class TCNetKeyboard:TCNetHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetKeyboard"/> class.
        /// </summary>
        public TCNetKeyboard() : base(MessageTypes.KeyboardData)
        {
        }

        #endregion

        #region Packet Content

        public enum Steps
        {
            Initialize = 0,
            Response = 1
        }

        /// <summary>
        /// Current step in process (0=Initialize, 1=Response)
        /// </summary>
        public Steps Step { get; set; }

        /// <summary>
        /// Raw text data string
        /// </summary>
        public string KeyboardData { get; set; }


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

            Step = (Steps)data.ReadByte();
            data.ReadByte();
            int length = (int)data.ReadNetwork32();
            data.ReadBytes(12);
            KeyboardData = data.ReadNetworkString(length);
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte)Step);
            data.Write((byte)0);
            data.WriteToNetwork((uint)KeyboardData.Length);
            data.Write(new byte[12]);
            data.WriteToNetwork(KeyboardData, KeyboardData.Length);
        }

        #endregion
    }
}

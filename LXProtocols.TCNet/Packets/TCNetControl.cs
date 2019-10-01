using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Send and Receive Control Packets to control nodes remotely.
    /// </summary>
    /// <remarks>
    /// Response Required
    /// </remarks>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetHeader" />
    public class TCNetControl:TCNetHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetControl"/> class.
        /// </summary>
        public TCNetControl() : base(MessageTypes.ControlMessages)
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
        /// String with Control Path
        /// </summary>
        /// <remarks>
        /// Examples: 
        ///     To stop a layer remotely: layer/1/state=6; (6=stop) 
        ///     To set layer A source layer 1: layer/5/source=1; 
        ///     To set layer M source layer A: layer/7/source=5;
        ///     To set state to “play” on layer 2 and force a resync on layer 2: layer/2/state=3; layer/2/resync; 
        ///     
        /// As control paths differ per application, contact your software vendor to obtain correct control path’s.
        /// </remarks>
        public string ControlPath { get; set; }


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

            Step = (Steps) data.ReadByte();
            data.ReadByte();
            int length = (int) data.ReadNetwork32();
            data.ReadBytes(12);
            ControlPath = data.ReadNetworkString(length);
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) Step);
            data.Write((byte)0);
            data.WriteToNetwork((uint) ControlPath.Length);
            data.Write(new byte[12]);
            data.WriteToNetwork(ControlPath, ControlPath.Length);
        }

        #endregion
    }
}

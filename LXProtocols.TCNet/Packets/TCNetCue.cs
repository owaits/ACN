using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    /// <summary>
    /// Contains Cue Data of Layer
    /// </summary>
    /// <remarks>
    /// This info can be requested from a node by sending a TCNet Request Data packet, with Datatype=12, Layer=The layer you request data from
    /// </remarks>
    /// <seealso cref="LXProtocols.TCNet.Packets.TCNetDataHeader" />
    public class TCNetCue:TCNetDataHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetCue"/> class.
        /// </summary>
        public TCNetCue() : base(DataTypes.Cue)
        {
        }

        #endregion

        #region Packet Content

        /// <summary>
        /// Time of Loop IN
        /// </summary>
        public TimeSpan LoopIn { get; set; }

        /// <summary>
        /// Time of Loop OUT
        /// </summary>
        public TimeSpan LoopOut { get; set; }


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

            data.ReadByte();
            LoopIn = data.ReadNetworkTime();
            LoopOut = data.ReadNetworkTime();
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte)0);
            data.WriteToNetwork(LoopIn);
            data.WriteToNetwork(LoopOut);
        }

        #endregion
    }
}

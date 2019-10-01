using LXProtocols.TCNet.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.TCNet.Packets
{
    public class TCNetTimeSync:TCNetHeader
    {
        #region Setup and Initialisation

        /// <summary>
        /// Initializes a new instance of the <see cref="TCNetTimeSync"/> class.
        /// </summary>
        public TCNetTimeSync() : base(MessageTypes.TimeSync)
        {
        }

        #endregion

        #region Packet Content

        public enum TimeSyncSteps
        {
            Initialize = 0,
            Response = 1
        }

        public TimeSyncSteps StepNumber { get; set; }

        public ushort NodeListenerPort { get; set; }

        public TimeSpan RemoteTimestamp { get; set; }

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

            StepNumber = (TimeSyncSteps) data.ReadByte();
            data.ReadByte();
            NodeListenerPort = data.ReadNetwork16();
            RemoteTimestamp = TimeSpan.FromTicks(data.ReadNetwork32() * (TimeSpan.TicksPerSecond / 1000000));
        }

        /// <summary>
        /// Writes the contents of this packet into a memory buffer.
        /// </summary>
        /// <param name="data">The data buffer to write the packet contents to.</param>
        public override void WriteData(TCNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write((byte) StepNumber);
            data.Write((byte)0);
            data.WriteToNetwork(NodeListenerPort);
            data.WriteToNetwork((uint)(RemoteTimestamp.Ticks / (TimeSpan.TicksPerSecond / 1000000)));
        }

        #endregion
    }
}

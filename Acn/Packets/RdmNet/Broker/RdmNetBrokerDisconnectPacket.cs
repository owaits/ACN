using LXProtocols.Acn.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.Acn.Packets.RdmNet.Broker
{
    [Flags]
    public enum BrokerDisconnectReason
    {
        /// <summary>
        /// Sent by Components to indicate that they are about to shut down.
        /// </summary>
        Shutdown = 0x0000,
        /// <summary>
        /// Sent by Components when they do not have the ability to support this connection. Note that a Component must reserve certain resources to be able to send this message when it is in such a state.
        /// </summary>
        CapacityExhausted = 0x0001,
        /// <summary>
        /// Sent by Components which must terminate a connection due to an internal hardware fault.
        /// </summary>
        HardwareFault = 0x0002,
        /// <summary>
        /// Sent by Components which must terminate a connection due to a software fault.
        /// </summary>
        SaoftwareFault = 0x0003,
        /// <summary>
        /// Sent by Components which must terminate a connection because of a software reset.This message should not be sent in the case of a reboot, as the Shutdown message is preferred.
        /// </summary>
        SoftwareReset = 0x0004,
        /// <summary>
        /// Sent by Brokers that are not on the desired Scope.
        /// </summary>
        IncorrectScope = 0x0005,
        /// <summary>
        /// Sent by Components which must terminate a connection because they were reconfigured using RPT.
        /// </summary>
        RPTReconfigure = 0x0006,
        /// <summary>
        /// Sent by Components which must terminate a connection because they were reconfigured using LLRP.
        /// </summary>
        LLRPReconfigure = 0x0007,
        /// <summary>
        /// Sent by Components which must terminate a connection because they were reconfigured through some means outside the scope of this standard (i.e.front panel configuration)
        /// </summary>
        UserReconfigure = 0x0008,
    }

    public class RdmNetBrokerDisconnectPacket : RdmNetBrokerPacket
    {
        public RdmNetBrokerDisconnectPacket() : base(RdmNetBrokerProtocolIds.Disconnect)
        {
        }

        #region Packet Contents

        public BrokerDisconnectReason DisconnectReason { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            Broker.ReadPdu(data);
            DisconnectReason = (BrokerDisconnectReason) data.ReadOctet2();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            Broker.WritePdu(data);

            data.Write((short)DisconnectReason);

            Broker.WriteLength(data);
        }

        #endregion
    }
}

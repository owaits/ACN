using LXProtocols.PosiStageNet.IO;
using LXProtocols.PosiStageNet.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace LXProtocols.PosiStageNet
{
    /// <summary>
    /// Responsible for creating packet instances based on the ID of the packet.
    /// </summary>
    public class PosiStageNetPacketFactory
    {
        /// <summary>
        /// Tries to create a packet instance from the packet ID, if the packet is not supported it will return false.
        /// </summary>
        /// <param name="header">The chunk header to decode the packet type from.</param>
        /// <param name="packet">The the newly created packet instance.</param>
        /// <returns>True if the packet type is supported.</returns>
        public static bool TryBuild(ChunkHeader header, out PosiStageNetPacket packet)
        {
            packet = null;
            switch((PosiStageNetPacketId) header.Id)
            {
                case PosiStageNetPacketId.Information:
                    packet= new PosiStageNetInformation();
                    break;
                case PosiStageNetPacketId.Data:
                    packet= new PosiStageNetData();
                    break;
            }
            return packet != null;
        }
    }
}

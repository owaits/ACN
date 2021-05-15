using LXProtocols.PosiStageNet.IO;
using LXProtocols.PosiStageNet.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.PosiStageNet.Test
{
    /// <summary>
    /// Test helpers to send and receive packets by writing the packets to a memory stream and reading them back.
    /// </summary>
    public class PosiStageNetPacketTester
    {
        /// <summary>
        /// Sends and receives a PSN packet.
        /// Performs some basic asserts on the header.
        /// </summary>
        /// <param name="packet">The PSN packet to test.</param>
        /// <returns>The read packet.</returns>
        public static PosiStageNetPacket SendAndReceivePacket(PosiStageNetPacket sent)
        {
            MemoryStream messageStream = WriteToMemoryStream(sent);
            messageStream.Seek(0, SeekOrigin.Begin);
            PosiStageNetPacket received = ReceiveFromStream(messageStream);
            Assert.AreEqual(sent.ProtocolVersion, received.ProtocolVersion);
            Assert.AreEqual(sent.TimeStamp, received.TimeStamp);
            Assert.AreEqual(sent.FrameID, received.FrameID);
            Assert.AreEqual(sent.FramePacketCount, received.FramePacketCount);
            return received;
        }

        /// <summary>
        /// Similar to the send method in the client.
        /// Fills out the size field and adds the data to a memory stream.
        /// </summary>
        /// <param name="citpMessage">The citp message.</param>
        /// <returns>The read packet.</returns>
        private static MemoryStream WriteToMemoryStream(PosiStageNetPacket packet)
        {
            MemoryStream data = new MemoryStream();
            PosiStageNetWriter writer = new PosiStageNetWriter(data);

            packet.WriteData(writer);

            return data;
        }

        /// <summary>
        /// Receives from stream.
        /// </summary>
        /// <param name="packetStream">The packet stream.</param>
        /// <returns>The read packet.</returns>
        private static PosiStageNetPacket ReceiveFromStream(MemoryStream packetStream)
        {
            PosiStageNetReader reader = new PosiStageNetReader(packetStream);
            PosiStageNetPacket readPacket;

            ChunkHeader header = reader.ReadChunkHeader();
            Assert.IsTrue(PosiStageNetPacketFactory.TryBuild(header, out readPacket), "Error building packet");
            Assert.IsNotNull(readPacket, "Returned packet is null.");

            readPacket.ReadData(header, reader);
            return readPacket;
        }
    }
}

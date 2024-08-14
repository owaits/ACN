using LXProtocols.Citp.IO;
using LXProtocols.Citp.Packets;
using LXProtocols.Citp.Packets.CaEx;
using LXProtocols.Citp.Packets.PInf;
using LXProtocols.Citp.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Citp.Test
{
    /// <summary>
    /// Test helpers to send and receive packets by writing the packets to a memory stream and reading them back.
    /// </summary>
    public class CitpPacketTester
    {
        /// <summary>
        /// Sends and receives an Msex packet.
        /// Performs some basic asserts on the header.
        /// </summary>
        /// <param name="citpMessage">The citp message.</param>
        /// <param name="mtuSize">The MTU size to simulate when writing an reading the packet. The MTU size determines the size at which the packet is fragmented.</param>
        /// <returns>The read packet.</returns>
        public static CitpPacket SendAndReceiveMsexVersionedPacket(CitpMsexHeader citpMessage, Version msexVersion, int mtuSize = 1500)
        {
            citpMessage.MsexVersion = msexVersion;
            CitpMsexHeader received = SendAndReceivePacket(citpMessage, mtuSize) as CitpMsexHeader;
            Assert.AreEqual(received.MsexVersion, citpMessage.MsexVersion);
            Assert.AreEqual(received.LayerContentType, citpMessage.LayerContentType);
            return received;
        }

        /// <summary>
        /// Sends and receives an Msex packet.
        /// Performs some basic asserts on the header.
        /// </summary>
        /// <param name="citpMessage">The citp message.</param>
        /// <param name="mtuSize">The MTU size to simulate when writing an reading the packet. The MTU size determines the size at which the packet is fragmented.</param>
        /// <returns>The read packet.</returns>
        public static CitpPacket SendAndReceiveCaExPacket(CaExHeader citpMessage, int mtuSize = 1500)
        {
            CaExHeader received = SendAndReceivePacket(citpMessage,mtuSize) as CaExHeader;
            Assert.AreEqual(received.ContentCode, citpMessage.ContentCode);
            Assert.AreEqual(received.ContentType, citpMessage.ContentType);
            return received;
        }

        /// <summary>
        /// Sends and receives a Peer Information packet.
        /// Performs some basic asserts on the header.
        /// </summary>
        /// <param name="citpMessage">The citp message.</param>
        /// <returns>The read packet.</returns>
        public static CitpPacket SendAndReceivePeerInformationPacket(CitpPInfHeader citpMessage)
        {            
            CitpPInfHeader received = SendAndReceivePacket(citpMessage) as CitpPInfHeader;
            Assert.AreEqual(received.LayerContentType, citpMessage.LayerContentType);
            return received;
        }

        /// <summary>
        /// Sends and receives a CITP packet.
        /// Performs some basic asserts on the header.
        /// </summary>
        /// <param name="citpMessage">The citp message.</param>
        /// <param name="mtuSize">The MTU size to simulate when writing an reading the packet. The MTU size determines the size at which the packet is fragmented.</param>
        /// <returns>The read packet.</returns>
        public static CitpPacket SendAndReceivePacket(CitpHeader citpMessage, int mtuSize = 1500)
        {
            CitpRecieveData messageStream = WriteToMemoryStream(citpMessage);
            CitpHeader received = ReceiveFromStream(messageStream, mtuSize);
            Assert.AreEqual(received.ContentType, citpMessage.ContentType);
            Assert.AreEqual(received.VersionMajor, citpMessage.VersionMajor);
            Assert.AreEqual(received.VersionMinor, citpMessage.VersionMinor);
            return received;
        }

        /// <summary>
        /// Similar to the send method in the client.
        /// Fills out the size field and adds the data to a memory stream.
        /// </summary>
        /// <param name="citpMessage">The citp message.</param>
        /// <returns>The read packet.</returns>
        private static CitpRecieveData WriteToMemoryStream(CitpHeader citpMessage)
        {
            CitpRecieveData data = new CitpRecieveData();
            CitpBinaryWriter writer = new CitpBinaryWriter(data);

            citpMessage.WriteData(writer);
            citpMessage.WriteMessageSize(writer);

            return data;
        }

        /// <summary>
        /// Receives from stream.
        /// </summary>
        /// <param name="packetStream">The packet stream.</param>
        /// <param name="mtuSize">The MTU size that determines the size of the fragmented packets.</param>
        /// <returns>The read packet.</returns>
        private static CitpHeader ReceiveFromStream(CitpRecieveData packetStream, int mtuSize = 1500)
        {
            CitpPacket readPacket;

            var fragments = FragmentStream(packetStream, mtuSize);

            foreach(var fragment in fragments.Take(fragments.Count() - 1))
                Assert.IsFalse(CitpPacketBuilder.TryBuild(fragment, out readPacket), "Packet builder reported a successful packet built from incomplete fragment. Was expecting EndOfStreamException when reading packet.");

            Assert.IsTrue(CitpPacketBuilder.TryBuild(packetStream, out readPacket), "Error building packet");
            Assert.IsNotNull(readPacket, "Returned packet is null.");
            return readPacket as CitpHeader;
        }

        /// <summary>
        /// Simulates a fragmented packet stream by breaking up the received data into separate fragments of the MTU size.
        /// </summary>
        /// <param name="packetStream">The packet stream.</param>
        /// <param name="mtuSize">The MTU size that determines the size of the fragmented packets.</param>
        /// <returns>A list of packets that are fragments of the original packet data.</returns>
        private static IEnumerable<CitpRecieveData> FragmentStream(CitpRecieveData packetStream, int mtuSize = 1500)
        {
            List<CitpRecieveData> fragments = new List<CitpRecieveData>();

            if (mtuSize >= packetStream.Length)
            {
                fragments.Add(packetStream);
            }
            else
            {
                for (int position = 0; position < packetStream.Length; position+=mtuSize)
                {
                    var fragment = new CitpRecieveData();
                    fragment.Write(packetStream.GetBuffer(), 0, position + mtuSize);
                    fragments.Add(fragment);
                }
            }
            return fragments;            
        }
    }
}

﻿using Citp.IO;
using Citp.Packets;
using Citp.Packets.PInf;
using Citp.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citp.Test
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
        /// <returns>The read packet.</returns>
        public static CitpPacket SendAndReceiveMsexVersionedPacket(CitpMsexHeader citpMessage, Version msexVersion)
        {
            citpMessage.MsexVersion = msexVersion;
            CitpMsexHeader received = SendAndReceivePacket(citpMessage) as CitpMsexHeader;
            Assert.AreEqual(received.MsexVersion, citpMessage.MsexVersion);
            Assert.AreEqual(received.LayerContentType, citpMessage.LayerContentType);
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
        /// <returns>The read packet.</returns>
        public static CitpPacket SendAndReceivePacket(CitpHeader citpMessage)
        {
            CitpRecieveData messageStream = WriteToMemoryStream(citpMessage);
            CitpHeader received = ReceiveFromStream(messageStream);
            Assert.AreEqual(received.ContentType, citpMessage.ContentType);
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
        /// <returns>The read packet.</returns>
        private static CitpHeader ReceiveFromStream(CitpRecieveData packetStream)
        {
            CitpPacket readPacket;
            Assert.IsTrue(CitpPacketBuilder.TryBuild(packetStream, out readPacket), "Error building packet");
            Assert.IsNotNull(readPacket, "Returned packet is null.");
            return readPacket as CitpHeader;
        }
    }
}

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Citp.Packets.Msex;
using Citp.Packets;
using System.IO;
using Citp.IO;
using Citp.Sockets;
using Citp.Packets.PInf;

namespace Citp.Test
{
    /// <summary>
    /// Tests for the Peer Information packets
    /// </summary>
    [TestClass]
    public class CitpPinfPacketTests
    {
        /// <summary>
        /// Citps the msex client information test.
        /// </summary>
        [TestMethod, TestCategory("CitpPinf")]
        public void CitpPInfPeerNameTest()
        {
            CitpPInfPeerName sentPacket = new CitpPInfPeerName();
            sentPacket.Name = "Zaphod";

            CitpPInfPeerName receivedPacket = CitpPacketTester.SendAndReceivePeerInformationPacket(sentPacket) as CitpPInfPeerName;

            Assert.AreEqual(sentPacket.Name, receivedPacket.Name);
        }

        /// <summary>
        /// Citps the msex client information test.
        /// </summary>
        [TestMethod, TestCategory("CitpPinf")]
        public void CitpPInfPeerLocationTest()
        {
            CitpPInfPeerLocation sentPacket = new CitpPInfPeerLocation();
            sentPacket.ListeningTCPPort = 0;
            sentPacket.Type = "LightingConsole";
            sentPacket.Name = "Avolites Titan";
            sentPacket.State = "Running";

            CitpPInfPeerLocation receivedPacket = CitpPacketTester.SendAndReceivePeerInformationPacket(sentPacket) as CitpPInfPeerLocation;

            Assert.AreEqual(sentPacket.ListeningTCPPort, receivedPacket.ListeningTCPPort);
            Assert.AreEqual(sentPacket.Name, receivedPacket.Name);
            Assert.AreEqual(sentPacket.State, receivedPacket.State);
        }
    }
}

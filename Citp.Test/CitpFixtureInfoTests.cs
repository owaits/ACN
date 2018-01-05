using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Citp.Packets.FInf;

namespace Citp.Test
{
    /// <summary>
    /// Tests for CITP Fixture Info messages. Data is written out to memory then read back in again.
    /// </summary>
    [TestClass]
    public class CitpFixtureInfoTests
    {
        /// <summary>
        /// Tests the Send Frames message.
        /// </summary>
        [TestMethod, TestCategory("CitpFinf")]
        public void CitpFinfSendFramesTest()
        {
            FInfSendFrames sentPacket = new FInfSendFrames();
            sentPacket.FixtureIdentifiers.Add(201);
            sentPacket.FixtureIdentifiers.Add(6002);

            FInfSendFrames receivedPacket = CitpPacketTester.SendAndReceivePacket(sentPacket) as FInfSendFrames;

            Assert.IsTrue(sentPacket.FixtureIdentifiers.SequenceEqual(receivedPacket.FixtureIdentifiers));
        }

        [TestMethod, TestCategory("CitpFinf")]
        public void CitpFinfFramesTest()
        {
            FInfFrames sentPacket = new FInfFrames();
            sentPacket.FixtureIdentifier = 201;
            sentPacket.FrameFilters.Add("Open");
            sentPacket.FrameFilters.Add("Cyan");
            sentPacket.FrameGobos.Add("Stars");
            sentPacket.FrameGobos.Add("Breakup");


            FInfFrames receivedPacket = CitpPacketTester.SendAndReceivePacket(sentPacket) as FInfFrames;
            Assert.AreEqual(sentPacket.FixtureIdentifier, receivedPacket.FixtureIdentifier, "Fixture identifiers are not equal.");
            Assert.IsTrue(sentPacket.FrameFilters.SequenceEqual(receivedPacket.FrameFilters), "Different filters.");
            Assert.IsTrue(sentPacket.FrameGobos.SequenceEqual(receivedPacket.FrameGobos), "Different Gobos.");
        }
    }
}

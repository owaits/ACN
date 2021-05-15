using LXProtocols.PosiStageNet.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace LXProtocols.PosiStageNet.Test
{
    [TestClass]
    public class PacketTests
    {
        private List<PosiStageNetTracker> BuildTrackers()
        {
            List<PosiStageNetTracker> trackers = new List<PosiStageNetTracker>();
            trackers.Add(new PosiStageNetTracker()
            {
                Id = 0x0101,
                Name = "Tracker 1",
                Position = new Vector3(1.1f, 2.2f, 3.3f),
                Orientation = new Vector3(10.1f, 20.2f, 30.3f),
                TargetPosition = new Vector3(100.1f, 200.2f, 300.3f),
                Speed = new Vector3(1000.1f, 2000.2f, 3000.3f),
                Acceleration = new Vector3(5.1f, 6.2f, 7.3f),
                Validity = 34f,
                TimeStamp = TimeSpan.FromSeconds(222),
            }); ;
            trackers.Add(new PosiStageNetTracker()
            {
                Id = 0x0202,
                Name = "Tracker 2",
                Position = new Vector3(1.1f, 2.2f, 3.3f),
                Orientation = new Vector3(10.1f, 20.2f, 30.3f),
                TargetPosition = new Vector3(100.1f, 200.2f, 300.3f),
                Speed = new Vector3(1000.1f, 2000.2f, 3000.3f),
                Acceleration = new Vector3(5.1f, 6.2f, 7.3f),
                Validity = 34f,
                TimeStamp = TimeSpan.FromSeconds(222),
            });
            return trackers;
        }

        [TestMethod]
        public void PSNInformationPacketTest()
        {
            PosiStageNetInformation sentPacket = new PosiStageNetInformation()
            {
                SystemName = "LXProtocols Test",
                Trackers = BuildTrackers()
            };

            PosiStageNetInformation received = PosiStageNetPacketTester.SendAndReceivePacket(sentPacket) as PosiStageNetInformation;

            Assert.AreEqual("LXProtocols Test", received.SystemName);
            Assert.AreEqual(sentPacket.Trackers[0].Id, received.Trackers[0].Id);
            Assert.AreEqual(sentPacket.Trackers[0].Name, received.Trackers[0].Name);
            Assert.AreEqual(sentPacket.Trackers[1].Id, received.Trackers[1].Id);
            Assert.AreEqual(sentPacket.Trackers[1].Name, received.Trackers[1].Name);
        }

        [TestMethod]
        public void PSNDataPacketTest()
        {
            PosiStageNetData sentPacket = new PosiStageNetData()
            {
                Trackers = BuildTrackers()
            };

            PosiStageNetData received = PosiStageNetPacketTester.SendAndReceivePacket(sentPacket) as PosiStageNetData;
            Assert.AreEqual(sentPacket.Trackers[0].Id, received.Trackers[0].Id);
            Assert.AreEqual(sentPacket.Trackers[0].Validity, received.Trackers[0].Validity);
            Assert.AreEqual(sentPacket.Trackers[0].TimeStamp, received.Trackers[0].TimeStamp);
            Assert.AreEqual(sentPacket.Trackers[0].Position, received.Trackers[0].Position);
            Assert.AreEqual(sentPacket.Trackers[0].Orientation, received.Trackers[0].Orientation);
            Assert.AreEqual(sentPacket.Trackers[0].Speed, received.Trackers[0].Speed);
            Assert.AreEqual(sentPacket.Trackers[0].Acceleration, received.Trackers[0].Acceleration);
            Assert.AreEqual(sentPacket.Trackers[0].TargetPosition, received.Trackers[0].TargetPosition);

            Assert.AreEqual(sentPacket.Trackers[1].Id, received.Trackers[1].Id);
            Assert.AreEqual(sentPacket.Trackers[1].Validity, received.Trackers[1].Validity);
            Assert.AreEqual(sentPacket.Trackers[1].TimeStamp, received.Trackers[1].TimeStamp);
            Assert.AreEqual(sentPacket.Trackers[1].Position, received.Trackers[1].Position);
            Assert.AreEqual(sentPacket.Trackers[1].Orientation, received.Trackers[1].Orientation);
            Assert.AreEqual(sentPacket.Trackers[1].Speed, received.Trackers[1].Speed);
            Assert.AreEqual(sentPacket.Trackers[1].Acceleration, received.Trackers[1].Acceleration);
            Assert.AreEqual(sentPacket.Trackers[1].TargetPosition, received.Trackers[1].TargetPosition);
        }
    }
}

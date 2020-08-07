using LXProtocols.ArtNet.IO;
using LXProtocols.ArtNet.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Acn.Test.ArtNet
{
    [TestClass]
    public class ArtNetDmxTests
    {
        [TestMethod]
        public void TestDMXPacket()
        {
            byte[] testDmx = new byte[512];
            for (int n = 0; n < 512; n++)
                testDmx[n] = (byte) n;

            ArtNetDmxPacket sourcePacket = new ArtNetDmxPacket()
            {
                Universe = 258,
                DmxData = testDmx
            };
            ArtNetDmxPacket targetPacket = new ArtNetDmxPacket();

            using (MemoryStream data = new MemoryStream())
            {
                sourcePacket.WriteData(new ArtNetBinaryWriter(data));
                data.Seek(0, SeekOrigin.Begin);
                targetPacket.ReadData(new ArtNetBinaryReader(data));
            }

            Assert.AreEqual(sourcePacket.Universe, targetPacket.Universe);
            Assert.IsTrue(Enumerable.SequenceEqual(sourcePacket.DmxData, targetPacket.DmxData));


        }
    }
}

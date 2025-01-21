using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LXProtocols.Citp.Packets.Msex;
using LXProtocols.Citp.Packets;
using System.IO;
using LXProtocols.Citp.IO;
using LXProtocols.Citp.Sockets;
using LXProtocols.Citp.Packets.PInf;

namespace LXProtocols.Citp.Test
{
    [TestClass]
    public class CitpMsexPacketTests
    {
        /// <summary>
        /// Client Information packet test.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexClientInformationTest()
        {
            CitpMsexClientInformation clientInformation = new CitpMsexClientInformation();
            clientInformation.SupportedMSEXVersions.AddRange(CitpMsexVersions.AllVersions);
            CitpMsexClientInformation received = CitpPacketTester.SendAndReceiveMsexVersionedPacket(clientInformation, CitpMsexVersions.Msex10Version) as CitpMsexClientInformation;

            foreach (Version version in clientInformation.SupportedMSEXVersions)
            {
                Assert.IsTrue(received.SupportedMSEXVersions.Contains(version), string.Format("Missing supported MSEX version {0}.", version));
            }
        }

        /// <summary>
        /// Tests both versions of the Server Information Message.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexServerInformationTest()
        {
            CitpMsexServerInformation serverInfo = new CitpMsexServerInformation();
            serverInfo.UUID = Acn.Rdm.UId.NewUId(10).ToString();
            serverInfo.ProductName = "Bob";
            serverInfo.ProductVersionMajor = 10;
            serverInfo.ProductVersionMinor = 1;
            serverInfo.ProductVersionBugfix = 42;
            serverInfo.SupportedMsexVersions.AddRange(CitpMsexVersions.AllVersions);
            serverInfo.SupportedLibraryTypes = 1;
            serverInfo.ThumbnailFormats.Add("RGB8");
            serverInfo.ThumbnailFormats.Add("JPEG");
            serverInfo.ThumbnailFormats.Add("PNG ");
            serverInfo.StreamFormats.Add("RGB8");
            serverInfo.StreamFormats.Add("JPEG");
            serverInfo.StreamFormats.Add("PNG ");
            serverInfo.DmxLayers.Add(new DmxDescriptor() { Protocol="ArtNet", Universe = 0, Net = 0, Channel = 1, PersonalityID = Guid.Parse("A023DB01-3B0B-4428-9C36-1CE14CDC9B8D") });
            serverInfo.DmxLayers.Add(new DmxDescriptor() { Protocol = "ArtNet", Universe = 0, Net = 0, Channel = 101, PersonalityID = Guid.Parse("A023DB01-3B0B-4428-9C36-1CE14CDC9B8D") });
            serverInfo.DmxLayers.Add(new DmxDescriptor() { Protocol = "BSRE1.31", Universe = 10, Channel = 101, PersonalityID = Guid.Parse("A023DB01-3B0B-4428-9C36-1CE14CDC9B8D") });            

            CitpMsexServerInformation received = CitpPacketTester.SendAndReceiveMsexVersionedPacket(serverInfo, CitpMsexVersions.Msex10Version) as CitpMsexServerInformation;
            AssertServerInformationPacket(serverInfo, received, CitpMsexVersions.Msex10Version);

            received = CitpPacketTester.SendAndReceiveMsexVersionedPacket(serverInfo, CitpMsexVersions.Msex12Version) as CitpMsexServerInformation;
            AssertServerInformationPacket(serverInfo, received, CitpMsexVersions.Msex12Version);
        }

        /// <summary>
        /// Asserts the server information packet.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertServerInformationPacket(CitpMsexServerInformation sent, CitpMsexServerInformation received, Version version)
        {
            Assert.AreEqual(sent.ProductName, received.ProductName, "Product name not received correctly");
            Assert.AreEqual(sent.ProductVersionMajor, received.ProductVersionMajor, "Major product versions are not equal.");
            Assert.AreEqual(sent.ProductVersionMinor, received.ProductVersionMinor, "Minor product vesions are not equal.");

            //Test the DMX Layers
            int dmxLayerCount = sent.DmxLayers.Count;
            Assert.AreEqual(dmxLayerCount, received.DmxLayers.Count, "Number of DMX layers are not equal.");
            for (int i = 0; i < dmxLayerCount; i++)
            {
                Assert.AreEqual(sent.DmxLayers[i].Universe, received.DmxLayers[i].Universe, "Universes are not equal.");
                Assert.AreEqual(sent.DmxLayers[i].Net, received.DmxLayers[i].Net, "Subnets are not equal.");
                Assert.AreEqual(sent.DmxLayers[i].Channel, received.DmxLayers[i].Channel, "Channels are not equal.");

                if (version >= CitpMsexVersions.Msex12Version)
                {
                    Assert.AreEqual(sent.DmxLayers[i].PersonalityID, received.DmxLayers[i].PersonalityID, "Personality IDs are not equal.");
                }
                else
                {
                    //In the older versions the id should not be sent.
                    Assert.AreNotEqual(sent.DmxLayers[i].PersonalityID, received.DmxLayers[i].PersonalityID, "Personality IDs are equal, id should not be sent before version 1.2.");
                }
            }

            //Test values unique to the 1.2 packet.
            if (version >= CitpMsexVersions.Msex12Version)
            {
                Assert.AreEqual(sent.UUID, received.UUID, "Unique IDs are not equal");
                Assert.AreEqual(sent.ProductVersionBugfix, received.ProductVersionBugfix, "Bug fix versions are not equal.");
                Assert.IsTrue(sent.SupportedMsexVersions.SequenceEqual(received.SupportedMsexVersions), "Supported version lists are not equal");
                Assert.IsTrue(sent.ThumbnailFormats.SequenceEqual(received.ThumbnailFormats), "Supported ThumbnailFormats lists are not equal");
                Assert.IsTrue(sent.StreamFormats.SequenceEqual(received.StreamFormats), "Supported StreamFormats lists are not equal");
            }
        }

        /// <summary>
        /// Tests the Nack packet.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexNackTest()
        {
            CitpMsexNack sentPacket = new CitpMsexNack();
            sentPacket.ReceivedContentType = "GELT";

            CitpMsexNack receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, CitpMsexVersions.Msex12Version) as CitpMsexNack;
            Assert.AreEqual(sentPacket.ReceivedContentType, receivedPacket.ReceivedContentType, "Received content type is incorrect");
        }

        /// <summary>
        /// Tests the layer status packet.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexLayerStatusTest()
        {
            CitpMsexLayerStatus sentPacket = new CitpMsexLayerStatus();

            for (int i = 0; i < 3; i++)
            {
                CitpMsexLayerStatus.LayerStatus layerStatus = new CitpMsexLayerStatus.LayerStatus();
                layerStatus.LayerNumber = (byte)i;
                layerStatus.PhysicalOutput = 0;
                layerStatus.MediaLibraryType = 2;
                layerStatus.MediaLibraryId = new CitpMsexLibraryId(1, 0, 1, 2);
                layerStatus.MediaLibraryNumber = 5;
                layerStatus.MediaNumber = 3;
                layerStatus.MediaName = "Test Media";
                layerStatus.MediaPosition = 76;
                layerStatus.MediaLength = 675;
                layerStatus.MediaFPS = 25;
                layerStatus.LayerStatusFlags = 1;
                sentPacket.Layers.Add(layerStatus);
            }

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexLayerStatus receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexLayerStatus;
            AssertLayerStatus(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex10Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexLayerStatus;
            AssertLayerStatus(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the layer status.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertLayerStatus(CitpMsexLayerStatus sent, CitpMsexLayerStatus received, Version version)
        {
            int layerCount = sent.Layers.Count;
            Assert.AreEqual(layerCount, received.Layers.Count, "A different number of layers were sent to received.");

            for(int i = 0; i < layerCount; i++)
            {
                Assert.AreEqual(sent.Layers[i].LayerNumber, received.Layers[i].LayerNumber, "Layer number does not match");
                Assert.AreEqual(sent.Layers[i].PhysicalOutput, received.Layers[i].PhysicalOutput, "Physical output does not match");                
                Assert.AreEqual(sent.Layers[i].MediaNumber, received.Layers[i].MediaNumber, "MediaNumber does not match");
                Assert.AreEqual(sent.Layers[i].MediaName, received.Layers[i].MediaName, "Media name does not match");
                Assert.AreEqual(sent.Layers[i].MediaPosition, received.Layers[i].MediaPosition, "Media position does not match");
                Assert.AreEqual(sent.Layers[i].MediaLength, received.Layers[i].MediaLength, "Media Length does not match");
                Assert.AreEqual(sent.Layers[i].MediaFPS, received.Layers[i].MediaFPS, "FPS does not match");
                Assert.AreEqual(sent.Layers[i].LayerStatusFlags, received.Layers[i].LayerStatusFlags, "Layer status flag does not match");

                if (version >= CitpMsexVersions.Msex12Version)
                {                    
                    Assert.IsTrue(sent.Layers[i].MediaLibraryId == received.Layers[i].MediaLibraryId, "MediaLibraryId does not match");
                    Assert.AreEqual(sent.Layers[i].MediaLibraryType, received.Layers[i].MediaLibraryType, "MediaLibraryType does not match");
                }
                else
                {
                    Assert.AreEqual(sent.Layers[i].MediaLibraryNumber, received.Layers[i].MediaLibraryNumber, "MediaLibraryNumber does not match");
                }
            }
        }

        /// <summary>
        /// Tests the get element library information packet.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexGetElementLibraryInformationTest()
        {
            CitpMsexGetElementLibraryInformation sentPacket = new CitpMsexGetElementLibraryInformation();
            sentPacket.LibraryType = MsexElementType.Effects;
            sentPacket.LibraryParentId = new CitpMsexLibraryId(2);
            sentPacket.LibraryNumbers.Add(1);
            sentPacket.LibraryNumbers.Add(42);
            sentPacket.LibraryNumbers.Add(84);

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexGetElementLibraryInformation receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementLibraryInformation;
            AssertCitpMsexGetElementLibraryInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementLibraryInformation;
            AssertCitpMsexGetElementLibraryInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex10Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementLibraryInformation;
            AssertCitpMsexGetElementLibraryInformation(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the get element library information packet.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexGetElementLibraryInformation(CitpMsexGetElementLibraryInformation sent, CitpMsexGetElementLibraryInformation received, Version version)
        {
            Assert.AreEqual(sent.LibraryType, received.LibraryType, "Library type does not match");
            Assert.IsTrue(sent.LibraryNumbers.SequenceEqual(received.LibraryNumbers), "Library numbers do not match");

            if (version > CitpMsexVersions.Msex10Version)
            {
                Assert.IsTrue(sent.LibraryParentId == received.LibraryParentId, "Parent Id does not match");
            }
        }

        /// <summary>
        /// Tests the element library information packet.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexElementLibraryInformationTest()
        {
            CitpMsexElementLibraryInformation sentPacket = new CitpMsexElementLibraryInformation();
            sentPacket.LibraryType = MsexElementType.Media;

            for (int i = 0; i < 3; i++)
            {
                CitpMsexElementLibraryInformation.ElementLibraryInformation info = new CitpMsexElementLibraryInformation.ElementLibraryInformation();
                info.LibraryNumber = (byte)i;
                info.SerialNumber = 1234;
                info.DmxRangeMax = 10;
                info.DmxRangeMin = 5;
                info.Name = "Test Clip " + i;
                info.LibraryCount = 11;
                info.ElementCount = 6;
                info.LibraryId = new CitpMsexLibraryId((byte)i);
                sentPacket.Libraries.Add(info);
            }

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexElementLibraryInformation receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementLibraryInformation;
            AssertCitpMsexElementLibraryInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementLibraryInformation;
            AssertCitpMsexElementLibraryInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex10Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementLibraryInformation;
            AssertCitpMsexElementLibraryInformation(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the element library information packet.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexElementLibraryInformation(CitpMsexElementLibraryInformation sent, CitpMsexElementLibraryInformation received, Version version)
        {
            Assert.AreEqual(sent.LibraryType, received.LibraryType, "Library type does not match");
            int libraryCount = sent.Libraries.Count;
            Assert.AreEqual(libraryCount, received.Libraries.Count, "A different number of libraries were sent and received.");

            for (int i = 0; i < libraryCount; i++)
            {
                Assert.AreEqual(sent.Libraries[i].DmxRangeMax, received.Libraries[i].DmxRangeMax, "Dmx range Max does not match.");
                Assert.AreEqual(sent.Libraries[i].DmxRangeMin, received.Libraries[i].DmxRangeMin, "Dmx range Min does not match.");
                Assert.AreEqual(sent.Libraries[i].Name, received.Libraries[i].Name, "Name does not match.");
                Assert.AreEqual(sent.Libraries[i].ElementCount, received.Libraries[i].ElementCount, "Element count does not match.");

                if (sent.MsexVersion == CitpMsexVersions.Msex10Version)
                {
                    Assert.AreEqual(sent.Libraries[i].LibraryNumber, received.Libraries[i].LibraryNumber, "Library number does not match.");
                }
                else
                {
                    Assert.IsTrue(sent.Libraries[i].LibraryId == received.Libraries[i].LibraryId, "Library Id does not match.");

                    if (sent.MsexVersion > CitpMsexVersions.Msex11Version)
                    {
                        Assert.IsTrue(sent.Libraries[i].SerialNumber == received.Libraries[i].SerialNumber, "Serial number does not match.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests the Element Library Updated packet
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexElementLibraryUpdatedTest()
        {
            CitpMsexElementLibraryUpdated sentPacket = new CitpMsexElementLibraryUpdated();
            sentPacket.LibraryType = MsexElementType.Media;            
            sentPacket.LibraryId = new CitpMsexLibraryId(12);
            sentPacket.UpdateFlags = MsexUpdateFlags.AllElements;
            sentPacket.AffectedElements = new System.Collections.BitArray(256, true);
            sentPacket.AffectedLibraries = new System.Collections.BitArray(256, true);

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexElementLibraryUpdated receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementLibraryUpdated;
            AssertCitpMsexElementLibraryUpdated(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementLibraryUpdated;
            AssertCitpMsexElementLibraryUpdated(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex10Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementLibraryUpdated;
            AssertCitpMsexElementLibraryUpdated(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the Element Library Updated packet
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexElementLibraryUpdated(CitpMsexElementLibraryUpdated sent, CitpMsexElementLibraryUpdated received, Version version)
        {
            Assert.AreEqual(sent.LibraryType, received.LibraryType, "Library type does not match");
            Assert.AreEqual(sent.UpdateFlags, received.UpdateFlags, "Update flags do not match");

            if (version == CitpMsexVersions.Msex10Version)
            {
                Assert.AreEqual(sent.LibraryNumber, received.LibraryNumber, "Library type does not match");
            }
            else
            {
                Assert.IsTrue(sent.LibraryId == received.LibraryId, "Library Id does not match.");
                if (version == CitpMsexVersions.Msex12Version)
                {
                    AssertBitArray(sent.AffectedLibraries, received.AffectedLibraries);
                    AssertBitArray(sent.AffectedElements, received.AffectedElements);
                }
            }
        }

        /// <summary>
        /// Asserts the bit array.
        /// </summary>
        /// <param name="array1">The array1.</param>
        /// <param name="array2">The array2.</param>
        private void AssertBitArray(System.Collections.BitArray array1, System.Collections.BitArray array2)
        {
            int length = array1.Count;
            Assert.AreEqual(length, array2.Count, "Arrays are a different length");

            for (int i = 0; i < length; i++)
            {
                Assert.AreEqual(array1[i], array2[i], "Array element not equal");
            }

        }

        /// <summary>
        /// Gets the element information message test.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void GetElementInformationTest()
        {
            CitpMsexGetElementInformation sentPacket = new CitpMsexGetElementInformation();
            sentPacket.LibraryType = MsexElementType.Effects;
            sentPacket.LibraryId = new CitpMsexLibraryId(42);
            sentPacket.ElementNumbers.Add(32);
            sentPacket.ElementNumbers.Add(64);
            sentPacket.ElementNumbers.Add(96);

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexGetElementInformation receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementInformation;
            AssertCitpMsexGetElementInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementInformation;
            AssertCitpMsexGetElementInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex10Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementInformation;
            AssertCitpMsexGetElementInformation(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the get element information message.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexGetElementInformation(CitpMsexGetElementInformation sent, CitpMsexGetElementInformation received, Version version)
        {
            Assert.AreEqual(sent.LibraryType, received.LibraryType, "Library type does not match");
            Assert.IsTrue(sent.ElementNumbers.SequenceEqual(received.ElementNumbers), "Correct sequence was not received.");

            if (version == CitpMsexVersions.Msex10Version)
            {
                Assert.AreEqual(sent.LibraryNumber, received.LibraryNumber, "Library number does not match");
            }
            else
            {
                Assert.IsTrue(sent.LibraryId == received.LibraryId, "Library Id does not match.");
            }
        }

        /// <summary>
        /// Tests the media element information.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexMediaElementInformationTest()
        {
            CitpMsexMediaElementInformation sentPacket = new CitpMsexMediaElementInformation();
            sentPacket.LibraryId = new CitpMsexLibraryId(36);

            for (int i = 0; i < 4; i++)
            {
                CitpMsexMediaElementInformation.MediaInformation mediaInfo = new CitpMsexMediaElementInformation.MediaInformation();
                mediaInfo.Number = 22;
                mediaInfo.DmxRangeMin = 115;
                mediaInfo.DmxRangeMax = 120;
                mediaInfo.MediaName = "Test Media " + i;
                mediaInfo.MediaVersionTimestamp = 80542367546;
                mediaInfo.MediaWidth = 1920;
                mediaInfo.MediaHeight = 1080;
                mediaInfo.MediaHeight = 26000;
                mediaInfo.MediaFPS = 25;
                mediaInfo.SerialNumber = 32;
                sentPacket.Elements.Add(mediaInfo);
            }

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexMediaElementInformation receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexMediaElementInformation;
            AssertCitpMsexMediaElementInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexMediaElementInformation;
            AssertCitpMsexMediaElementInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex10Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexMediaElementInformation;
            AssertCitpMsexMediaElementInformation(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the media element information packet.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexMediaElementInformation(CitpMsexMediaElementInformation sent, CitpMsexMediaElementInformation received, Version version)
        {
            if (version == CitpMsexVersions.Msex10Version)
            {
                Assert.AreEqual(sent.LibraryNumber, received.LibraryNumber, "Library number does not match");
            }
            else
            {
                Assert.IsTrue(sent.LibraryId == received.LibraryId, "Library Id does not match.");
            }

            int elementCount = sent.Elements.Count;
            Assert.AreEqual(elementCount, received.Elements.Count, "Some elements have been lost in the post.");

            for (int i = 0; i < elementCount; i++)
            {
                Assert.AreEqual(sent.Elements[i].Number, received.Elements[i].Number, "Element number doesn't match.");
                Assert.AreEqual(sent.Elements[i].DmxRangeMin, received.Elements[i].DmxRangeMin, "Dmx Range Min doesn't match.");
                Assert.AreEqual(sent.Elements[i].DmxRangeMax, received.Elements[i].DmxRangeMax, "Dmx Range Max doesn't match.");
                Assert.AreEqual(sent.Elements[i].MediaName, received.Elements[i].MediaName, "MediaName doesn't match.");
                Assert.AreEqual(sent.Elements[i].MediaVersionTimestamp, received.Elements[i].MediaVersionTimestamp, "MediaVersionTimestamp doesn't match.");
                Assert.AreEqual(sent.Elements[i].MediaWidth, received.Elements[i].MediaWidth, "MediaWidth doesn't match.");
                Assert.AreEqual(sent.Elements[i].MediaLength, received.Elements[i].MediaLength, "MediaLength doesn't match.");
                Assert.AreEqual(sent.Elements[i].MediaFPS, received.Elements[i].MediaFPS, "MediaFPS doesn't match.");

                if (version == CitpMsexVersions.Msex12Version)
                {
                    Assert.AreEqual(sent.Elements[i].SerialNumber, received.Elements[i].SerialNumber, "SerialNumber doesn't match.");
                }
            }
        }

        /// <summary>
        /// Tests the effect element information packet.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexEffectElementInformationTest()
        {
            CitpMsexEffectElementInformation sentPacket = new CitpMsexEffectElementInformation();
            sentPacket.LibraryId = new CitpMsexLibraryId(36);

            for (int i = 0; i < 4; i++)
            {
                CitpMsexEffectElementInformation.EffectInfomation effectInfo = new CitpMsexEffectElementInformation.EffectInfomation();
                effectInfo.ElementNumber = 22;
                effectInfo.DmxRangeMin = 115;
                effectInfo.DmxRangeMax = 120;
                effectInfo.EffectName = "Test Effect " + i;                
                effectInfo.SerialNumber = 32;
                effectInfo.EffectParameterNames.Add("Red");
                effectInfo.EffectParameterNames.Add("Green");
                effectInfo.EffectParameterNames.Add("Blue");
                sentPacket.Elements.Add(effectInfo);
            }

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexEffectElementInformation receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexEffectElementInformation;
            AssertCitpMsexEffectElementInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexEffectElementInformation;
            AssertCitpMsexEffectElementInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex10Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexEffectElementInformation;
            AssertCitpMsexEffectElementInformation(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts  the effect element information packet.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The version under test.</param>
        private void AssertCitpMsexEffectElementInformation(CitpMsexEffectElementInformation sent, CitpMsexEffectElementInformation received, Version version)
        {
            if (version == CitpMsexVersions.Msex10Version)
            {
                Assert.AreEqual(sent.LibraryNumber, received.LibraryNumber, "Library number does not match");
            }
            else
            {
                Assert.IsTrue(sent.LibraryId == received.LibraryId, "Library Id does not match.");
            }

            int elementCount = sent.Elements.Count;
            Assert.AreEqual(elementCount, received.Elements.Count, "Some elements have been lost in the post.");

            for (int i = 0; i < elementCount; i++)
            {
                Assert.AreEqual(sent.Elements[i].ElementNumber, received.Elements[i].ElementNumber, "Element number doesn't match.");
                Assert.AreEqual(sent.Elements[i].DmxRangeMin, received.Elements[i].DmxRangeMin, "Dmx Range Min doesn't match.");
                Assert.AreEqual(sent.Elements[i].DmxRangeMax, received.Elements[i].DmxRangeMax, "Dmx Range Max doesn't match.");
                Assert.AreEqual(sent.Elements[i].EffectName, received.Elements[i].EffectName, "Name doesn't match.");
                int parameterCount = sent.Elements[i].EffectParameterNames.Count;
                Assert.AreEqual(parameterCount, received.Elements[i].EffectParameterNames.Count, "Paramter count different.");
                Assert.IsTrue(sent.Elements[i].EffectParameterNames.SequenceEqual(received.Elements[i].EffectParameterNames), "Sent and received parameter list doesn't match.");

                if (version == CitpMsexVersions.Msex12Version)
                {
                    Assert.AreEqual(sent.Elements[i].SerialNumber, received.Elements[i].SerialNumber, "SerialNumber doesn't match.");
                }
            }
        }

        /// <summary>
        /// Tests the Generic Element information message
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexGenericElementInformationTest()
        {
            CitpMsexGenericElementInformation sentPacket = new CitpMsexGenericElementInformation();
            sentPacket.LibraryId = new CitpMsexLibraryId(36);

            for (int i = 0; i < 4; i++)
            {
                CitpMsexGenericElementInformation.ElementInfomation elementInfo = new CitpMsexGenericElementInformation.ElementInfomation();
                elementInfo.ElementNumber = 22;
                elementInfo.DmxRangeMin = 115;
                elementInfo.DmxRangeMax = 120;
                elementInfo.Name = "Test Element " + i;
                elementInfo.SerialNumber = 32;
                elementInfo.VersionTimeStamp = 98716734263489;
                sentPacket.Elements.Add(elementInfo);
            }

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexGenericElementInformation receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGenericElementInformation;
            AssertCitpMsexGenericElementInformation(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGenericElementInformation;
            AssertCitpMsexGenericElementInformation(sentPacket, receivedPacket, msexVersion);            
        }

        /// <summary>
        /// Asserts the Generic Element information message
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexGenericElementInformation(CitpMsexGenericElementInformation sent, CitpMsexGenericElementInformation received, Version version)
        {            
            Assert.IsTrue(sent.LibraryId == received.LibraryId, "Library Id does not match.");            

            int elementCount = sent.Elements.Count;
            Assert.AreEqual(elementCount, received.Elements.Count, "Some elements have been lost in the post.");

            for (int i = 0; i < elementCount; i++)
            {
                Assert.AreEqual(sent.Elements[i].ElementNumber, received.Elements[i].ElementNumber, "Element number doesn't match.");
                Assert.AreEqual(sent.Elements[i].DmxRangeMin, received.Elements[i].DmxRangeMin, "Dmx Range Min doesn't match.");
                Assert.AreEqual(sent.Elements[i].DmxRangeMax, received.Elements[i].DmxRangeMax, "Dmx Range Max doesn't match.");
                Assert.AreEqual(sent.Elements[i].Name, received.Elements[i].Name, "Name doesn't match.");
                Assert.AreEqual(sent.Elements[i].VersionTimeStamp, received.Elements[i].VersionTimeStamp, "Time doesn't match.");

                if (version == CitpMsexVersions.Msex12Version)
                {
                    Assert.AreEqual(sent.Elements[i].SerialNumber, received.Elements[i].SerialNumber, "SerialNumber doesn't match.");
                }
            }
        }

        /// <summary>
        /// Tests the get element library thumbnail message.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexGetElementLibraryThumbnailTest()
        {
            CitpMsexGetElementLibraryThumbnail sentPacket = new CitpMsexGetElementLibraryThumbnail();
            sentPacket.ThumbnailFormat = "RGB8";
            sentPacket.ThumbnailWidth = 70;
            sentPacket.ThumbnailHeight = 80;
            sentPacket.ThumbnailFlags = ThumbnailOptions.PreserveAspectRatio;
            sentPacket.LibraryType = MsexElementType.Media;
            sentPacket.LibraryIds.Add(new CitpMsexLibraryId(12));
            sentPacket.LibraryIds.Add(new CitpMsexLibraryId(13));
            sentPacket.LibraryIds.Add(new CitpMsexLibraryId(14));            

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexGetElementLibraryThumbnail receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementLibraryThumbnail;
            AssertCitpMsexGetElementLibraryThumbnail(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementLibraryThumbnail;
            AssertCitpMsexGetElementLibraryThumbnail(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex10Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementLibraryThumbnail;
            AssertCitpMsexGetElementLibraryThumbnail(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the get element library thumbnail message.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexGetElementLibraryThumbnail(CitpMsexGetElementLibraryThumbnail sent, CitpMsexGetElementLibraryThumbnail received, Version version)
        {
            AssertCitpMsexGetElementThumbnailBase(sent, received, version);

            int libraryIdCount = sent.LibraryIds.Count;
            Assert.AreEqual(libraryIdCount, received.LibraryIds.Count, "Some library Ids were lost in the post.");
            for (int i = 0; i < libraryIdCount; i++)
                Assert.IsTrue(sent.LibraryIds[i] == received.LibraryIds[i], "Library Ids do not match.");
        }

        /// <summary>
        /// Asserts the get element thumbnail base.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexGetElementThumbnailBase(CitpMsexGetElementThumbnailBase sent, CitpMsexGetElementThumbnailBase received, Version version)
        {
            Assert.AreEqual(sent.ThumbnailFormat, received.ThumbnailFormat, "Thumbnail formats are note equal.");
            Assert.AreEqual(sent.ThumbnailWidth, received.ThumbnailWidth, "Thumbnail width is note equal.");
            Assert.AreEqual(sent.ThumbnailHeight, received.ThumbnailHeight, "Thumbnail height is note equal.");
            Assert.AreEqual(sent.ThumbnailFlags, received.ThumbnailFlags, "Thumbnail height is note equal.");
            Assert.AreEqual(sent.LibraryType, received.LibraryType, "LibraryType is note equal.");
        }

        /// <summary>
        /// Tests the element library thumbnail message.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexElementLibraryThumbnailTest()
        {
            CitpMsexElementLibraryThumbnail sentPacket = new CitpMsexElementLibraryThumbnail();
            sentPacket.LibraryId = new CitpMsexLibraryId(56);
            sentPacket.ThumbnailFormat = "RGB8";
            sentPacket.ThumbnailWidth = 70;
            sentPacket.ThumbnailHeight = 80;
            sentPacket.LibraryType = MsexElementType.Media;
            sentPacket.ThumbnailBuffer = new byte[]{12,3,55,78,96};

            Version msexVersion = CitpMsexVersions.Msex10Version;
            CitpMsexElementLibraryThumbnail receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementLibraryThumbnail;
            AssertCitpMsexElementLibraryThumbnail(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementLibraryThumbnail;
            AssertCitpMsexElementLibraryThumbnail(sentPacket, receivedPacket, msexVersion);

            //Test that when the packet is fragmented it still reads the data correctly.
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion, 43) as CitpMsexElementLibraryThumbnail;
            AssertCitpMsexElementLibraryThumbnail(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the element library thumbnail message.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexElementLibraryThumbnail(CitpMsexElementLibraryThumbnail sent, CitpMsexElementLibraryThumbnail received, Version version)
        {
            Assert.AreEqual(sent.LibraryType, received.LibraryType, "LibraryType is note equal.");            
            Assert.AreEqual(sent.ThumbnailFormat, received.ThumbnailFormat, "Thumbnail formats are note equal.");
            Assert.AreEqual(sent.ThumbnailWidth, received.ThumbnailWidth, "Thumbnail width is note equal.");
            Assert.AreEqual(sent.ThumbnailHeight, received.ThumbnailHeight, "Thumbnail height is note equal.");
            Assert.IsTrue(sent.ThumbnailBuffer.SequenceEqual(received.ThumbnailBuffer), "Thumbnail buffer is note equal.");

            if (sent.MsexVersion < CitpMsexVersions.Msex11Version)
            {
                Assert.AreEqual(sent.LibraryNumber, received.LibraryNumber, "Library numbers are not equal.");
            }
            else
            {
                Assert.IsTrue(sent.LibraryId == received.LibraryId, "Library IDs are not equal");
            }
        }

        private void AssertCitpMsexElementThumbnail(CitpMsexElementThumbnail sent, CitpMsexElementThumbnail received, Version version)
        {
            Assert.AreEqual(sent.LibraryType, received.LibraryType, "LibraryType is note equal.");
            Assert.AreEqual(sent.ThumbnailFormat, received.ThumbnailFormat, "Thumbnail formats are note equal.");
            Assert.AreEqual(sent.ThumbnailWidth, received.ThumbnailWidth, "Thumbnail width is note equal.");
            Assert.AreEqual(sent.ThumbnailHeight, received.ThumbnailHeight, "Thumbnail height is note equal.");
            Assert.IsTrue(sent.ThumbnailBuffer.SequenceEqual(received.ThumbnailBuffer), "Thumbnail buffer is note equal.");

            if (sent.MsexVersion < CitpMsexVersions.Msex11Version)
            {
                Assert.AreEqual(sent.LibraryNumber, received.LibraryNumber, "Library numbers are not equal.");
            }
            else
            {
                Assert.IsTrue(sent.LibraryId == received.LibraryId, "Library IDs are not equal");
            }
        }

        /// <summary>
        /// Tests the element thumbnail message.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexElementThumbnailTest()
        {
            CitpMsexElementThumbnail sentPacket = new CitpMsexElementThumbnail();
            sentPacket.LibraryId = new CitpMsexLibraryId(56);
            sentPacket.ElementNumber = 12;
            sentPacket.ThumbnailFormat = "RGB8";
            sentPacket.ThumbnailWidth = 70;
            sentPacket.ThumbnailHeight = 80;
            sentPacket.LibraryType = MsexElementType.Media;
            sentPacket.ThumbnailBuffer = new byte[] { 12, 3, 55, 78, 96 };

            Version msexVersion = CitpMsexVersions.Msex10Version;
            CitpMsexElementThumbnail receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementThumbnail;
            AssertCitpMsexElementThumbnail(sentPacket, receivedPacket, msexVersion);
            Assert.AreEqual(sentPacket.ElementNumber, receivedPacket.ElementNumber, "Element number is not equal in version 1.0.");

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexElementThumbnail;
            AssertCitpMsexElementThumbnail(sentPacket, receivedPacket, msexVersion);
            Assert.AreEqual(sentPacket.ElementNumber, receivedPacket.ElementNumber, "Element number is not equal in version 1.1.");
        }

        /// <summary>
        /// Tests the get element thumbnail message.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexGetElementThumbnailTest()
        {
            CitpMsexGetElementThumbnail sentPacket = new CitpMsexGetElementThumbnail();
            sentPacket.ThumbnailFormat = "JPEG";
            sentPacket.ThumbnailWidth = 70;
            sentPacket.ThumbnailHeight = 80;
            sentPacket.ThumbnailFlags = ThumbnailOptions.PreserveAspectRatio;
            sentPacket.LibraryType = MsexElementType.Media;
            sentPacket.LibraryId = new CitpMsexLibraryId(88);
            sentPacket.ElementNumbers.Add(12);
            sentPacket.ElementNumbers.Add(13);
            sentPacket.ElementNumbers.Add(14);

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexGetElementThumbnail receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementThumbnail;
            AssertCitpMsexGetElementThumbnail(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex11Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementThumbnail;
            AssertCitpMsexGetElementThumbnail(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex10Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetElementThumbnail;
            AssertCitpMsexGetElementThumbnail(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the get element thumbnail message.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexGetElementThumbnail(CitpMsexGetElementThumbnail sent, CitpMsexGetElementThumbnail received, Version version)
        {
            AssertCitpMsexGetElementThumbnailBase(sent, received, version);
            Assert.IsTrue(sent.LibraryId == received.LibraryId, "Library Ids do not match.");
            
            int elementNumberCount = sent.ElementNumbers.Count;
            Assert.AreEqual(elementNumberCount, received.ElementNumbers.Count, "Some element numbers were lost in the post.");
            Assert.IsTrue(sent.ElementNumbers.SequenceEqual(received.ElementNumbers), "Sent and received element numbers are not the same");
        }

        /// <summary>
        /// Tests the get video sources message.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexGetVideoSourcesTest()
        {
            CitpMsexGetVideoSources sentPacket = new CitpMsexGetVideoSources();
            
            //All the required asserts happen within send and receive.
            Version msexVersion = CitpMsexVersions.Msex10Version;
            CitpMsexGetVideoSources receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexGetVideoSources;            
        }

        /// <summary>
        /// Tests the video sources message.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexVideoSourcesTest()
        {
            CitpMsexVideoSources sentPacket = new CitpMsexVideoSources();
            for (int i = 0; i < 4; i++)
            {
                CitpMsexVideoSources.SourceInformation sourceInfo = new CitpMsexVideoSources.SourceInformation();
                sourceInfo.SourceIdentifier = 42;
                sourceInfo.SourceName = "Marvin Left Diode " + i;
                sourceInfo.PhysicalOutput = 1;
                sourceInfo.LayerNumber = 8;
                sourceInfo.Flags = 1;
                sourceInfo.Width = 640;
                sourceInfo.Height = 480;
            }

            Version msexVersion = CitpMsexVersions.Msex10Version;
            CitpMsexVideoSources receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexVideoSources;
            AssertCitpMsexVideoSources(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the video sources message.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexVideoSources(CitpMsexVideoSources sent, CitpMsexVideoSources received, Version version)
        {
            int sourceCount = sent.Sources.Count;
            Assert.AreEqual(sourceCount, received.Sources.Count, "Number of sources are not equal.");

            for (int i = 0; i < sourceCount; i++)
            {
                Assert.AreEqual(sent.Sources[i].SourceIdentifier, received.Sources[i].SourceIdentifier, "Source identifiers don't match.");
                Assert.AreEqual(sent.Sources[i].SourceName, received.Sources[i].SourceName, "Source names don't match.");
                Assert.AreEqual(sent.Sources[i].PhysicalOutput, received.Sources[i].PhysicalOutput, "Source outputs don't match.");
                Assert.AreEqual(sent.Sources[i].LayerNumber, received.Sources[i].LayerNumber, "Source layer numbers don't match.");
                Assert.AreEqual(sent.Sources[i].Flags, received.Sources[i].Flags, "Source flags don't match.");
                Assert.AreEqual(sent.Sources[i].Width, received.Sources[i].Width, "Source widths don't match.");
                Assert.AreEqual(sent.Sources[i].Height, received.Sources[i].Height, "Source heights don't match.");
            }
        }

        /// <summary>
        /// Tests the request stream message.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexGetRequestStreamTest()
        {
            CitpMsexRequestStream sentPacket = new CitpMsexRequestStream();
            sentPacket.SourceIdentifier = 72;
            sentPacket.FrameFormat = "JPEG";
            sentPacket.FrameWidth = 640;
            sentPacket.FrameHeight = 480;
            sentPacket.FramesPerSecond = 3;
            sentPacket.Timeout = 5;

            Version msexVersion = CitpMsexVersions.Msex12Version;
            CitpMsexRequestStream receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexRequestStream;

            Assert.AreEqual(sentPacket.SourceIdentifier, receivedPacket.SourceIdentifier, "Source Identifiers don't match.");
            Assert.AreEqual(sentPacket.FrameFormat, receivedPacket.FrameFormat, "FrameFormat doesn't match.");
            Assert.AreEqual(sentPacket.FrameWidth, receivedPacket.FrameWidth, "FrameWidth doesn't match.");
            Assert.AreEqual(sentPacket.FrameHeight, receivedPacket.FrameHeight, "FrameHeight doesn't match.");
            Assert.AreEqual(sentPacket.FramesPerSecond, receivedPacket.FramesPerSecond, "FramesPerSecond don't match.");
            Assert.AreEqual(sentPacket.Timeout, receivedPacket.Timeout, "Timeout doesn't match.");

        }

        /// <summary>
        /// Tests the stream frame message.
        /// </summary>
        [TestMethod, TestCategory("CitpMsex")]
        public void CitpMsexStreamFrameMessageTest()
        {
            CitpMsexStreamFrame sentPacket = new CitpMsexStreamFrame();
            sentPacket.MediaServerUid = Acn.Rdm.UId.NewUId(80).ToString();
            sentPacket.SourceIdentifier = 5;
            sentPacket.FrameFormat = "JPEG";
            sentPacket.FrameWidth = 640;
            sentPacket.FrameHeight = 480;
            sentPacket.FrameBuffer = new byte[] { 12, 3, 55, 78, 96 };

            Version msexVersion = CitpMsexVersions.Msex10Version;
            CitpMsexStreamFrame receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexStreamFrame;
            AssertCitpMsexStreamFrame(sentPacket, receivedPacket, msexVersion);

            msexVersion = CitpMsexVersions.Msex12Version;
            receivedPacket = CitpPacketTester.SendAndReceiveMsexVersionedPacket(sentPacket, msexVersion) as CitpMsexStreamFrame;
            AssertCitpMsexStreamFrame(sentPacket, receivedPacket, msexVersion);
        }

        /// <summary>
        /// Asserts the stream frame message.
        /// </summary>
        /// <param name="sent">The sent packet.</param>
        /// <param name="received">The received packet.</param>
        /// <param name="version">The MSEX version under test.</param>
        private void AssertCitpMsexStreamFrame(CitpMsexStreamFrame sent, CitpMsexStreamFrame received, Version version)
        {
            Assert.AreEqual(sent.SourceIdentifier, received.SourceIdentifier, "SourceIdentifier is note equal.");
            Assert.AreEqual(sent.FrameFormat, received.FrameFormat, "FrameFormat is note equal.");
            Assert.AreEqual(sent.FrameWidth, received.FrameWidth, "Frame width is note equal.");
            Assert.AreEqual(sent.FrameHeight, received.FrameHeight, "Frame height is note equal.");
            Assert.IsTrue(sent.FrameBuffer.SequenceEqual(received.FrameBuffer), "Frame buffer is note equal.");

            if (sent.MsexVersion > CitpMsexVersions.Msex11Version)
            {
                Assert.AreEqual(sent.MediaServerUid, received.MediaServerUid, "Media server ID is not equal.");
            }
        }
        
    }
}

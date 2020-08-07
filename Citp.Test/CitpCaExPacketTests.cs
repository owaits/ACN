using LXProtocols.Citp.Packets.CaEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXProtocols.Citp.Test
{
    /// <summary>
    /// Tests for the capture extension packets.
    /// </summary>
    [TestClass]
    public class CitpCaExPacketTests
    {
        /// <summary>
        /// Tests the Nack packet.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExNackTest()
        {
            List<CaExNackReason> nackReason = new List<CaExNackReason>() { CaExNackReason.UnknownRequest, CaExNackReason.MalformedPacket, CaExNackReason.InternalError, CaExNackReason.RequestRefused };

            foreach (CaExNackReason reason in nackReason)
            {
                CaExNack clientInformation = new CaExNack();
                clientInformation.Reason = reason;
                CaExNack received = CitpPacketTester.SendAndReceiveCaExPacket(clientInformation) as CaExNack;
                Assert.AreEqual(clientInformation.Reason, received.Reason, "Nack Reason is not equal.");
            }
        }

        #region Live View Messages.

        /// <summary>
        /// Tests the Get Live View Status.
        /// This message is all in the header at the present time.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExGetLiveViewStatusTest()
        {
            CaExGetLiveViewStatus sentPacket = new CaExGetLiveViewStatus();
            CaExGetLiveViewStatus received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExGetLiveViewStatus;                     
        }

        /// <summary>
        /// Test for the Live View status message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExLiveViewStatusTest()
        {
            List<LiveViewAvailability> options = new List<LiveViewAvailability>() { LiveViewAvailability.None, LiveViewAvailability.Alpha, LiveViewAvailability.Beta, LiveViewAvailability.Gamma };

            foreach (LiveViewAvailability option in options)
            {
                CaExLiveViewStatus sentPacket = new CaExLiveViewStatus();
                sentPacket.Availability = option;
                sentPacket.Width = 1920;
                sentPacket.Height = 1080;
                sentPacket.CameraFocusX = 10;
                sentPacket.CameraFocusY = 15;
                sentPacket.CameraFocusZ = 20;
                sentPacket.CameraPositionX = 0.75f;
                sentPacket.CameraPositionY = 3.5f;
                sentPacket.CameraPositionZ = 15;
                CaExLiveViewStatus received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExLiveViewStatus;
                Assert.AreEqual(sentPacket.Availability, received.Availability, "Availability is not equal.");
                Assert.AreEqual(sentPacket.Width, received.Width, "Width is not equal.");
                Assert.AreEqual(sentPacket.Height, received.Height, "Height is not equal.");
                Assert.AreEqual(sentPacket.CameraFocusX, received.CameraFocusX, "CameraFocusX is not equal.");
                Assert.AreEqual(sentPacket.CameraFocusY, received.CameraFocusY, "CameraFocusY is not equal.");
                Assert.AreEqual(sentPacket.CameraFocusZ, received.CameraFocusZ, "CameraFocusZ is not equal.");
                Assert.AreEqual(sentPacket.CameraPositionX, received.CameraPositionX, "CameraPositionX is not equal.");
                Assert.AreEqual(sentPacket.CameraPositionY, received.CameraPositionY, "CameraPositionY is not equal.");
                Assert.AreEqual(sentPacket.CameraPositionZ, received.CameraPositionZ, "Availability is not equal.");
            }
        }

        /// <summary>
        /// Tests the Get Live View Image message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExGetLiveViewImageTest()
        {
            CaExGetLiveViewImage sentPacket = new CaExGetLiveViewImage();
            sentPacket.Format = LiveViewImageFormat.JPEG;
            sentPacket.Width = 1920;
            sentPacket.Height = 1080;
            sentPacket.CameraFocusX = 10;
            sentPacket.CameraFocusY = 15;
            sentPacket.CameraFocusZ = 20;
            sentPacket.CameraPositionX = 0.75f;
            sentPacket.CameraPositionY = 3.5f;
            sentPacket.CameraPositionZ = 15;
            CaExGetLiveViewImage received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExGetLiveViewImage;
            Assert.AreEqual(sentPacket.Format, received.Format, "Format is not equal.");
            Assert.AreEqual(sentPacket.Width, received.Width, "Width is not equal.");
            Assert.AreEqual(sentPacket.Height, received.Height, "Height is not equal.");
            Assert.AreEqual(sentPacket.CameraFocusX, received.CameraFocusX, "CameraFocusX is not equal.");
            Assert.AreEqual(sentPacket.CameraFocusY, received.CameraFocusY, "CameraFocusY is not equal.");
            Assert.AreEqual(sentPacket.CameraFocusZ, received.CameraFocusZ, "CameraFocusZ is not equal.");
            Assert.AreEqual(sentPacket.CameraPositionX, received.CameraPositionX, "CameraPositionX is not equal.");
            Assert.AreEqual(sentPacket.CameraPositionY, received.CameraPositionY, "CameraPositionY is not equal.");
            Assert.AreEqual(sentPacket.CameraPositionZ, received.CameraPositionZ, "Availability is not equal.");
        }

        /// <summary>
        /// Tests the Live View Image Message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExLiveViewImageTest()
        {
            CaExLiveViewImage sentPacket = new CaExLiveViewImage();
            sentPacket.Format = LiveViewImageFormat.JPEG;
            sentPacket.Data = new byte[]{26,66,77,89,66};            
            CaExLiveViewImage received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExLiveViewImage;
            Assert.AreEqual(sentPacket.Format, received.Format, "Format is not equal.");
            Assert.IsTrue(sentPacket.Data.SequenceEqual(received.Data), "Data is not equal.");           
        }

        #endregion

        #region Cue Recording Messages.

        /// <summary>
        /// Test for the Recording Cue Capabilities message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExSetCueRecordingCapabilitiesTest()
        {
            CaExSetCueRecordingCapabilities sentPacket = new CaExSetCueRecordingCapabilities();
            sentPacket.Availability = true;   
            for (int i = 0; i < 4; i++)
            {
                CueRecordingOption newOption = new CueRecordingOption();
                newOption.Name = "Paddington";
                newOption.Choices = new List<string>();
                newOption.Choices.Add("Marmalade");
                newOption.Choices.Add("Marmalade Sandwiches");
                newOption.Help = "All the things a bear could want.";
                sentPacket.Options.Add(newOption);
            }
                     
            CaExSetCueRecordingCapabilities received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExSetCueRecordingCapabilities;
            Assert.AreEqual(sentPacket.Availability, received.Availability, "Availability is not equal.");
            int optionCount = sentPacket.Options.Count();
            Assert.AreEqual(optionCount, received.Options.Count(), "Option count mismatch");
            for (int i = 0; i < optionCount; i++)
            {
                Assert.AreEqual(sentPacket.Options[i].Name, sentPacket.Options[i].Name, "Name is not equal.");
                Assert.AreEqual(sentPacket.Options[i].Help, sentPacket.Options[i].Help, "Help is not equal.");
                Assert.IsTrue(sentPacket.Options[i].Choices.SequenceEqual(received.Options[i].Choices), "Choice mismatch");
            }
        }

        /// <summary>
        /// Tests the Record Cue message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExRecordCueTest()
        {
            CaExRecordCue sentPacket = new CaExRecordCue();
            for (int i = 0; i < 4; i++)
            {
                RecordCueOption newOption = new RecordCueOption();
                newOption.Name = "Paddington";
                newOption.Value = "Marmalade " + i;                
                sentPacket.Options.Add(newOption);
            }

            CaExRecordCue received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExRecordCue;            
            int optionCount = sentPacket.Options.Count();
            Assert.AreEqual(optionCount, received.Options.Count(), "Option count mismatch");
            for (int i = 0; i < optionCount; i++)
            {
                Assert.AreEqual(sentPacket.Options[i].Name, sentPacket.Options[i].Name, "Name is not equal.");
                Assert.AreEqual(sentPacket.Options[i].Value, sentPacket.Options[i].Value, "Value is not equal.");
            }
        }

        /// <summary>
        /// Tests the Recorder Clear capability message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExSetRecorderClearCapabilityTest()
        {
            List<RecorderClearingAvailability> availabilityOptions = new List<RecorderClearingAvailability>() { RecorderClearingAvailability.Available, RecorderClearingAvailability.Unavailable, RecorderClearingAvailability.Unsupported };

            foreach (RecorderClearingAvailability option in availabilityOptions)
            {
                CaExSetRecorderClearingCapabilities clientInformation = new CaExSetRecorderClearingCapabilities();
                clientInformation.Availability = option;
                CaExSetRecorderClearingCapabilities received = CitpPacketTester.SendAndReceiveCaExPacket(clientInformation) as CaExSetRecorderClearingCapabilities;
                Assert.AreEqual(clientInformation.Availability, received.Availability, "Availability is not equal.");
            }
        }

        /// <summary>
        /// Test for the programmer clear message.
        /// This message is all in the header at the present time.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExClearRecorderTest()
        {
            CaExClearRecorder sentPacket = new CaExClearRecorder();
            CaExClearRecorder received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExClearRecorder;
            Assert.IsNotNull(received, "Message not successfully received.");
        }
        #endregion

        #region Show Sync Messages.

        /// <summary>
        /// Tests the enter show message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExEnterShowTest()
        {
            CaExEnterShow sentPacket = new CaExEnterShow();
            sentPacket.Name = "Test Show";
            CaExEnterShow received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExEnterShow;
            Assert.AreEqual(sentPacket.Name, received.Name, "Show Name mismatch.");
        }

        /// <summary>
        /// Tests the leave show message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExLeaveShowTest()
        {
            CaExLeaveShow sentPacket = new CaExLeaveShow();            
            CaExLeaveShow received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExLeaveShow;
            Assert.IsNotNull(received, "No message received");
        }

        /// <summary>
        /// Tests the leave show message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExSetFixtureTransformationSpace()
        {
            CaExSetFixtureTransformationSpace sentPacket = new CaExSetFixtureTransformationSpace() {  TransformationSpace = TransformSpaces.PanHome };
            CaExSetFixtureTransformationSpace received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExSetFixtureTransformationSpace;
            Assert.IsNotNull(received, "No message received");
            Assert.AreEqual(TransformSpaces.PanHome, received.TransformationSpace);
        }

        /// <summary>
        /// Tests the fixture list request message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExFixtureListRequestTest()
        {
            CaExFixtureListRequest sentPacket = new CaExFixtureListRequest();
            CaExFixtureListRequest received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExFixtureListRequest;
            Assert.IsNotNull(received, "No message received");
        }

        /// <summary>
        /// Tests the fixture list message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExFixtureListTest()
        {
            List<FixtureChanges> options = new List<FixtureChanges>() { FixtureChanges.Update, FixtureChanges.NewFixture, FixtureChanges.ExchangedFixture };
            List<FixtureLinkType> linkTypes = new List<FixtureLinkType>(){ FixtureLinkType.AtlaBaseFixtureId, FixtureLinkType.AtlaBaseModeId, FixtureLinkType.CaptureId, FixtureLinkType.RDMDeviceModelId, FixtureLinkType.RDMPersonalityId};
            Guid identifierId = Guid.NewGuid();

            foreach (FixtureChanges option in options)
            {
                CaExFixtureList sentPacket = new CaExFixtureList();
                sentPacket.Type = option;
                for (int i = 0; i < 4; i++)
                {
                    CaExFixtureList.FixtureInformation newFixture = new CaExFixtureList.FixtureInformation();
                    newFixture.FixtureId = 1690;
                    newFixture.ManufacturerName = "Robe";
                    newFixture.FixtureName = "MMX Spot";
                    newFixture.ModeName = "Mode 1";
                    newFixture.ChannelCount = 24;
                    newFixture.IsDimmer = false;

                    foreach (FixtureLinkType linkType in linkTypes)
                    {
                        FixtureLink link = null;
                        if(linkType != FixtureLinkType.RDMDeviceModelId && linkType != FixtureLinkType.RDMPersonalityId){
                            link = new FixtureLink(linkType, identifierId);
                        }
                        else
                        {
                            link = new FixtureLink();
                            link.Type = linkType;
                            link.Data = new byte[] { 26, 72 };
                        }
                        newFixture.Links.Add(link);
                    }
                    newFixture.IsPatched = true;
                    newFixture.DMXUniverse = 12;
                    newFixture.DMXAddress = (ushort)(1 + (i * 24));
                    newFixture.Unit = "16";
                    newFixture.Channel = 10;
                    newFixture.Circuit = "H1.1";
                    newFixture.Note = "A fixture of some sort";
                    newFixture.Position = new Coordinate() { X = 5, Y = 2, Z = -4 };
                    newFixture.Angle = new Coordinate() { X = 180, Y = 90, Z = 90 };
                    sentPacket.Fixtures.Add(newFixture);
                }

                CaExFixtureList received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExFixtureList;
                Assert.AreEqual(sentPacket.Type, received.Type, "Availability is not equal.");
                int fixtureCount = sentPacket.Fixtures.Count();
                Assert.AreEqual(fixtureCount, received.Fixtures.Count(), "Option count mismatch");
                for (int i = 0; i < fixtureCount; i++)
                {
                    Assert.AreEqual(sentPacket.Fixtures[i].FixtureId, received.Fixtures[i].FixtureId, "Id is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].ManufacturerName, received.Fixtures[i].ManufacturerName, "ManufacturerName is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].FixtureName, received.Fixtures[i].FixtureName, "FixtureName is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].ModeName, received.Fixtures[i].ModeName, "ModeName is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].ChannelCount, received.Fixtures[i].ChannelCount, "ChannelCount is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].IsDimmer, received.Fixtures[i].IsDimmer, "IsDimmer is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].IsPatched, received.Fixtures[i].IsPatched, "IsPatched is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].DMXUniverse, received.Fixtures[i].DMXUniverse, "DMXUniverse is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].DMXAddress, received.Fixtures[i].DMXAddress, "DMXAddress is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].Unit, received.Fixtures[i].Unit, "Unit is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].Channel, received.Fixtures[i].Channel, "ManufacturerName is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].Circuit, received.Fixtures[i].Circuit, "ManufacturerName is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].Note, received.Fixtures[i].Note, "Note is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].Position, received.Fixtures[i].Position, "Position is not equal.");
                    Assert.AreEqual(sentPacket.Fixtures[i].Angle, received.Fixtures[i].Angle, "Angle is not equal.");
                    int linkCount = sentPacket.Fixtures[i].Links.Count;
                    Assert.AreEqual(linkCount, received.Fixtures[i].Links.Count, "Uneven number of identifiers.");
                    for (int j = 0; j < linkCount; j++)
                    {
                        Assert.AreEqual(sentPacket.Fixtures[i].Links[j].Type, received.Fixtures[i].Links[j].Type, "Identifier Type is not equal.");
                        if (received.Fixtures[i].Links[j].Type != FixtureLinkType.RDMDeviceModelId && received.Fixtures[i].Links[j].Type != FixtureLinkType.RDMPersonalityId)
                        {
                            Assert.AreEqual(identifierId, received.Fixtures[i].Links[j].ValueAsGuid(), "Identifier is not equal.");
                        }
                        else
                        {
                            Assert.IsTrue(received.Fixtures[i].Links[j].Data.SequenceEqual(sentPacket.Fixtures[i].Links[j].Data), "Id data is not equal");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fixture Modify Message test.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExFixtureModifyTest()
        {
            CaExFixtureModify sentPacket = new CaExFixtureModify();
            
            for (int i = 0; i < 4; i++)
            {
                CaExFixtureModify.FixtureChange newFixture = new CaExFixtureModify.FixtureChange();
                newFixture.FixtureId = 1690;
                newFixture.ChangedFields = CaExFixtureModify.Modification.Circuit & CaExFixtureModify.Modification.UnitNumber;                
                newFixture.Patched = true;
                newFixture.DMXUniverse = 12;
                newFixture.DMXAddress = (ushort)(1 + (i * 24));
                newFixture.Unit = "16";
                newFixture.Channel = 10;
                newFixture.Circuit = "H1.1";
                newFixture.Note = "A fixture of some sort";
                newFixture.Position = new Coordinate() { X = 5, Y = 2, Z = -4 };
                newFixture.Angle = new Coordinate() { X = 180, Y = 90, Z = 90 };
                sentPacket.Fixtures.Add(newFixture);
            }

            CaExFixtureModify received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExFixtureModify;            
            int fixtureCount = sentPacket.Fixtures.Count();
            Assert.AreEqual(fixtureCount, received.Fixtures.Count(), "Option count mismatch");
            for (int i = 0; i < fixtureCount; i++)
            {
                Assert.AreEqual(sentPacket.Fixtures[i].FixtureId, received.Fixtures[i].FixtureId, "Id is not equal.");                
                Assert.AreEqual(sentPacket.Fixtures[i].Patched, received.Fixtures[i].Patched, "IsPatched is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].DMXUniverse, received.Fixtures[i].DMXUniverse, "DMXUniverse is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].DMXAddress, received.Fixtures[i].DMXAddress, "DMXAddress is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].Unit, received.Fixtures[i].Unit, "Unit is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].Channel, received.Fixtures[i].Channel, "ManufacturerName is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].Circuit, received.Fixtures[i].Circuit, "ManufacturerName is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].Note, received.Fixtures[i].Note, "Note is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].Position, received.Fixtures[i].Position, "Position is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].Angle, received.Fixtures[i].Angle, "Angle is not equal.");                    
            }            
        }

        /// <summary>
        /// Tests the fixture list request message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExFixtureRemoveTest()
        {
            CaExFixtureRemove sentPacket = new CaExFixtureRemove();
            sentPacket.FixtureIds.Add(1620);
            sentPacket.FixtureIds.Add(1621);
            sentPacket.FixtureIds.Add(1622);
            CaExFixtureRemove received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExFixtureRemove;
            Assert.IsTrue(sentPacket.FixtureIds.SequenceEqual(received.FixtureIds), "Different fixture ids sent and received.");
        }

        /// <summary>
        /// Tests the fixture selection message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExFixtureSelectionTest()
        {
            CaExFixtureSelection sentPacket = new CaExFixtureSelection();
            sentPacket.FixtureIds.Add(1620);
            sentPacket.FixtureIds.Add(1621);
            sentPacket.FixtureIds.Add(1622);
            CaExFixtureSelection received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExFixtureSelection;
            Assert.IsTrue(sentPacket.FixtureIds.SequenceEqual(received.FixtureIds), "Different fixture ids sent and received.");
        }

        /// <summary>
        /// Tests the fixture console status message.
        /// </summary>
        [TestMethod, TestCategory("CitpCaEx")]
        public void CitpCaExFixtureConsoleStatusTest()
        {
            CaExFixtureConsoleStatus sentPacket = new CaExFixtureConsoleStatus();
            for (int i = 0; i < 3; i++)
            {
                CaExFixtureConsoleStatus.FixtureStatus status = new CaExFixtureConsoleStatus.FixtureStatus();
                status.FixtureId = (uint)(1620+i);
                status.Clearable = true;
                status.Locked = false;
                sentPacket.Fixtures.Add(status);
            }

            CaExFixtureConsoleStatus received = CitpPacketTester.SendAndReceiveCaExPacket(sentPacket) as CaExFixtureConsoleStatus;
            Assert.AreEqual(3, received.Fixtures.Count, "Incorrect number of fixtures received.");
            for(int i = 0; i < 3; i++)
            {
                Assert.AreEqual(sentPacket.Fixtures[i].FixtureId, received.Fixtures[i].FixtureId, "Id is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].Clearable, received.Fixtures[i].Clearable, "Clearable is not equal.");
                Assert.AreEqual(sentPacket.Fixtures[i].Locked, received.Fixtures[i].Locked, "Locked is not equal.");
            }
        }
        #endregion
    }
}

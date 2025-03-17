using LXProtocols.Acn.Rdm;
using LXProtocols.Acn.Rdm.Packets.Discovery;
using LXProtocols.Acn.Rdm.Packets.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Test
{
    /// <summary>
    /// Test to ensure that RDM packet factory is functioning correctly.
    /// </summary>
    [TestClass()]
    public class RdmPacketFactoryTests
    {
        /// <summary>
        /// This test ensures that packets are correctly identified as requests or responses.
        /// </summary>
        [TestMethod]
        public void IsResponseTest() 
        {
            Assert.IsFalse(RdmPacketFactory.IsResponse(new DiscoveryMute.Request().Header));
            Assert.IsTrue(RdmPacketFactory.IsResponse(new DiscoveryMute.Reply().Header));

            Assert.IsFalse(RdmPacketFactory.IsResponse(new IdentifyEndpoint.Get().Header));
            Assert.IsTrue(RdmPacketFactory.IsResponse(new IdentifyEndpoint.GetReply().Header));

            Assert.IsFalse(RdmPacketFactory.IsResponse(new IdentifyEndpoint.Set().Header));
            Assert.IsTrue(RdmPacketFactory.IsResponse(new IdentifyEndpoint.SetReply().Header));
        }

        /// <summary>
        /// This test ensures that packets are correctly identified as error responses.
        /// </summary>
        [TestMethod]
        public void IsErrorResponseTest()
        {
            Assert.IsFalse(RdmPacketFactory.IsErrorResponse(new IdentifyEndpoint.Get().Header));
            Assert.IsFalse(RdmPacketFactory.IsErrorResponse(new IdentifyEndpoint.GetReply().Header));
            Assert.IsTrue(RdmPacketFactory.IsErrorResponse(new IdentifyEndpoint.GetReply() { ResponseType = RdmResponseTypes.NackReason }.Header));
            
            
            Assert.IsFalse(RdmPacketFactory.IsErrorResponse(new IdentifyEndpoint.Set().Header));
            Assert.IsFalse(RdmPacketFactory.IsErrorResponse(new IdentifyEndpoint.SetReply().Header));
            Assert.IsTrue(RdmPacketFactory.IsErrorResponse(new IdentifyEndpoint.SetReply() { ResponseType = RdmResponseTypes.NackReason }.Header));
        }

    }
}

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

            Assert.IsFalse(RdmPacketFactory.IsResponse(new EndpointIdentify.Get().Header));
            Assert.IsTrue(RdmPacketFactory.IsResponse(new EndpointIdentify.GetReply().Header));

            Assert.IsFalse(RdmPacketFactory.IsResponse(new EndpointIdentify.Set().Header));
            Assert.IsTrue(RdmPacketFactory.IsResponse(new EndpointIdentify.SetReply().Header));
        }

        /// <summary>
        /// This test ensures that packets are correctly identified as error responses.
        /// </summary>
        [TestMethod]
        public void IsErrorResponseTest()
        {
            Assert.IsFalse(RdmPacketFactory.IsErrorResponse(new EndpointIdentify.Get().Header));
            Assert.IsFalse(RdmPacketFactory.IsErrorResponse(new EndpointIdentify.GetReply().Header));
            Assert.IsTrue(RdmPacketFactory.IsErrorResponse(new EndpointIdentify.GetReply() { ResponseType = RdmResponseTypes.NackReason }.Header));
            
            
            Assert.IsFalse(RdmPacketFactory.IsErrorResponse(new EndpointIdentify.Set().Header));
            Assert.IsFalse(RdmPacketFactory.IsErrorResponse(new EndpointIdentify.SetReply().Header));
            Assert.IsTrue(RdmPacketFactory.IsErrorResponse(new EndpointIdentify.SetReply() { ResponseType = RdmResponseTypes.NackReason }.Header));
        }

    }
}

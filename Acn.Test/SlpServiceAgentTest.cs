#region Copyright © 2011 Mark Daniel
//______________________________________________________________________________________________________________
// Service Location Protocol
// Copyright © 2011 Mark Daniel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//______________________________________________________________________________________________________________
#endregion

using Acn.Slp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Acn.Slp.Packets;

namespace Acn.Test
{
    
    
    /// <summary>
    ///This is a test class for SlpServiceAgentTest and is intended
    ///to contain all SlpServiceAgentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SlpServiceAgentTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for CheckUrlMatch
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Acn.dll")]
        public void CheckUrlMatchTest()
        {
            Assert.IsFalse(SlpServiceAgent_Accessor.CheckUrlMatch("", "", "", ""));
            Assert.IsTrue(SlpServiceAgent_Accessor.CheckUrlMatch("service:printer:lpr://igore.wco.ftp.com:515/draft", "printer", "lpr", "igore.wco.ftp.com:515/draft"));
            Assert.IsFalse(SlpServiceAgent_Accessor.CheckUrlMatch("service:printer:lpr://igore.wco.ftp.com:515/draft", "printer", "lpr", "10.0.0.1:515/draft"));
            Assert.IsFalse(SlpServiceAgent_Accessor.CheckUrlMatch("service:printer:lpr://igore.wco.ftp.com:515/draft", "e133.esta", "lpr", "igore.wco.ftp.com:515/draft"));
            Assert.IsTrue(SlpServiceAgent_Accessor.CheckUrlMatch("service:e133.esta", "e133.esta", string.Empty, "2.4.9.8:5568/0xaabb11223344"));
            Assert.IsTrue(SlpServiceAgent_Accessor.CheckUrlMatch("service:e133.esta://2.4.9.8:5568/0xaabb11223344", "e133.esta", string.Empty, "2.4.9.8:5568/0xaabb11223344"));
            Assert.IsFalse(SlpServiceAgent_Accessor.CheckUrlMatch("service:e133.esta://2.7.9.8:5568/0xaabb11223344", "e133.esta", string.Empty, "2.4.9.8:5568/0xaabb11223344"));
            Assert.IsTrue(SlpServiceAgent_Accessor.CheckUrlMatch("nfs://max.net/znoo", "filestore", "nfs", "max.net/znoo"));
            Assert.IsFalse(SlpServiceAgent_Accessor.CheckUrlMatch("nfs://max.org/znoo", "filestore", "nfs", "max.net/znoo"));
    
        }

        /// <summary>
        ///A test for ServiceUrl
        ///</summary>
        [TestMethod()]
        public void ServiceUrlTest()
        {
            SlpServiceAgent target = new SlpServiceAgent(); 
            string expected = "service:printer:lpr://igore.wco.ftp.com:515/draft"; 
            string actual;
            target.ServiceUrl = expected;
            actual = target.ServiceUrl;
            Assert.AreEqual(expected, actual, "Full service url did not match when parsed and output");
            Assert.AreEqual<string>("service:printer:lpr", target.ServiceType, "Full service type did not match");
            Assert.AreEqual<string>("printer", target.ServiceAbstractType, "Abstract service type did not match");
            Assert.AreEqual<string>("lpr", target.ServiceConcreteType, "Concrete service type did not match");
            Assert.AreEqual<string>("igore.wco.ftp.com:515/draft", target.ServiceAddress, "Address service type did not match");
        }

        /// <summary>
        /// Test for ServiceUrl
        ///   This tests that acn urls parse (no concrete type)
        /// </summary>
        [TestMethod()]
        public void ServiceUrlTest_AcnUrls()
        {
            SlpServiceAgent target = new SlpServiceAgent();
            string expected = "service:e133.esta://2.4.3.127/0xaabb11223344";
            string actual;
            target.ServiceUrl = expected;
            actual = target.ServiceUrl;
            Assert.AreEqual(expected, actual, "Full service url did not match when parsed and output");
            Assert.AreEqual<string>("service:e133.esta", target.ServiceType, "Full service type did not match");
            Assert.AreEqual<string>("e133.esta", target.ServiceAbstractType, "Abstract service type did not match");
            Assert.AreEqual<string>(string.Empty, target.ServiceConcreteType, "Concrete service type did not match");
            Assert.AreEqual<string>("2.4.3.127/0xaabb11223344", target.ServiceAddress, "Address service type did not match");
        }

        /// <summary>
        /// Test for ServiceUrl
        ///   This tests that concrete only Urls parse
        /// </summary>
        [TestMethod()]
        public void ServiceUrlTest_Concrete()
        {
            SlpServiceAgent target = new SlpServiceAgent();
            string expected = "http://www.ietf.org:80/rfc";
            string actual;
            target.ServiceUrl = expected;
            actual = target.ServiceUrl;
            Assert.AreEqual("service:http://www.ietf.org:80/rfc", actual, "Full service url did not match when parsed and output");
            Assert.AreEqual<string>("service:http", target.ServiceType, "Full service type did not match");
            Assert.AreEqual<string>(string.Empty, target.ServiceAbstractType, "Abstract service type did not match");
            Assert.AreEqual<string>("http", target.ServiceConcreteType, "Concrete service type did not match");
            Assert.AreEqual<string>("www.ietf.org:80/rfc", target.ServiceAddress, "Address service type did not match");

        }

        /// <summary>
        ///A test for IsReplyRequired
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Acn.dll")]
        public void IsReplyRequiredTest()
        {
            SlpServiceAgent_Accessor target = new SlpServiceAgent_Accessor() { ServiceUrl = "service:e133.esta://2.4.3.127/0xaabb11223344" };
            ServiceRequestPacket request = new ServiceRequestPacket() { ServiceType = "service:e133.esta", ScopeList = target.Scope }; 
            Assert.IsTrue(target.IsReplyRequired(request));
            request.ServiceType = "service:directory-agent";
            Assert.IsFalse(target.IsReplyRequired(request));

            request.ServiceType = "service:e133.esta";
            target.ServiceUrl = "service:printer:lpr://igore.wco.ftp.com:515/draft";
            Assert.IsFalse(target.IsReplyRequired(request));
            request.ServiceType = "service:printer";
            Assert.IsTrue(target.IsReplyRequired(request));
            request.ServiceType = "service:lpr";
            Assert.IsTrue(target.IsReplyRequired(request));
            request.ServiceType = "service:printer:lpr";
            Assert.IsTrue(target.IsReplyRequired(request));

            request.ServiceType = "service:print6er";
            Assert.IsFalse(target.IsReplyRequired(request));
            request.ServiceType = "service:lp2r";
            Assert.IsFalse(target.IsReplyRequired(request));
            request.ServiceType = "service:prin4ter:lpr";
            Assert.IsFalse(target.IsReplyRequired(request));
            request.ServiceType = "service:printer:lp9r";
            Assert.IsFalse(target.IsReplyRequired(request));

            
        }
    }
}

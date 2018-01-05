using Acn.Sntp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Acn.Test
{
    
    
    /// <summary>
    ///This is a test class for NtpDataTest and is intended
    ///to contain all NtpDataTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NtpDataTest
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
        ///A test for VersionNumber
        ///</summary>
        [TestMethod()]
        public void VersionNumberTest()
        {
            NtpData validate = new NtpData(new byte[] { 0x23 });
            Assert.AreEqual(4, validate.VersionNumber);
            Assert.AreEqual(NtpMode.Client, validate.Mode);

            NtpData target = new NtpData(); 
            byte expected = 4; 
            target.VersionNumber = expected;
            Assert.AreEqual(expected, target.VersionNumber);

            target.Mode = NtpMode.Client;
            Assert.AreEqual<NtpMode>(NtpMode.Client, target.Mode);
            Assert.AreEqual(expected, target.VersionNumber, "Version number changed by setting mode");

            Assert.AreEqual(0x23, target.ToArray()[0]);
        }

        /// <summary>
        ///A test for RootDispersion
        ///</summary>
        [TestMethod()]
        public void RootDispersionTest()
        {
            NtpData target = new NtpData();
            double actual = 250;
            target.RootDispersion = actual;
            Assert.AreEqual<double>(actual, target.RootDispersion);
            actual = -250;
            target.RootDispersion = actual;
            Assert.AreEqual<double>(actual, target.RootDispersion);
            actual = -31.25d;
            target.RootDispersion = actual;
            Assert.AreEqual<double>(actual, target.RootDispersion);
            actual = 3.90625;
            target.RootDispersion = actual;
            Assert.AreEqual<double>(actual, target.RootDispersion);
            actual = 8000;
            target.RootDispersion = actual;
            Assert.AreEqual<double>(actual, target.RootDispersion);
            actual = -8250;
            target.RootDispersion = actual;
            Assert.AreEqual<double>(actual, target.RootDispersion);
        }
    }
}

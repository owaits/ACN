using LXProtocols.Acn.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acn.Test
{
    /// <summary>
    /// Tests for <see cref="AcnBinaryWriter"/>
    /// </summary>
    [TestClass]
    public class ACNBinaryWriterTests
    {
        /// <summary>
        /// Ensures that the ACN method to write a fixed length string to a stream functions correctly.
        /// </summary>
        [TestMethod]
        public void WriteUtf8StringTest()
        {
            //Tests that a normal string within the length is written correctly.
            using (var stream = new MemoryStream())
            {
                var writer = new AcnBinaryWriter(stream);
                writer.WriteUtf8String("ABCD", 5);

                Assert.AreEqual(5, stream.Length);

                var data = stream.ToArray();
                Assert.AreEqual(65, data[0]);
                Assert.AreEqual(0, data[4]);
            }

            //Tests that a normal string that exceeds the length is written correctly.
            using (var stream = new MemoryStream())
            {
                var writer = new AcnBinaryWriter(stream);
                writer.WriteUtf8String("ABCDEF", 5);

                Assert.AreEqual(5, stream.Length);

                var data = stream.ToArray();
                Assert.AreEqual(65, data[0]);
                Assert.AreEqual(69, data[4]);
            }
        }
    }
}

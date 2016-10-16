using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class ByteArrayPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0, new ByteArrayPoco(new byte[0]).Orrey.Count);
            var data = new byte[] { 17, 88 };
            var poco = new ByteArrayPoco(data);
            Assert.AreEqual(2, poco.Orrey.Count);
            data[0] = 0; // Ensures poco made a copy
            Assert.AreEqual(17, poco.Orrey[0]);
            Assert.AreEqual(88, poco.Orrey[1]);
            Assert.True(poco.Orrey.IsReadOnly);
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new ByteArrayPoco(new byte[] { 1, 3 }));
            Assert.AreEqual(new ByteArrayPoco(new byte[] { 1, 3 }), new ByteArrayPoco(new byte[] { 1, 3 }));
            Assert.AreNotEqual(new ByteArrayPoco(new byte[] { 1, 6 }), new ByteArrayPoco(new byte[] { 1, 3 }));
            Assert.AreNotEqual(new ByteArrayPoco(new byte[] { 1, 3, 4 }), new ByteArrayPoco(new byte[] { 1, 3 }));
            Assert.AreNotEqual(new ByteArrayPoco(new byte[] { 9 }), new ByteArrayPoco(new byte[] { 1, 3 }));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual("ByteArrayPoco\r\n\tOrrey: <empty>", new ByteArrayPoco(new byte[] { }).ToString());
            Assert.AreEqual("ByteArrayPoco\r\n\tOrrey: 44", new ByteArrayPoco(new byte[] { 44 }).ToString());
            Assert.AreEqual("ByteArrayPoco\r\n\tOrrey: 44, 88", new ByteArrayPoco(new byte[] { 44, 88 }).ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new ByteArrayPoco(new byte[] { }));
            TestUtils.TestRoundTrip(new ByteArrayPoco(new byte[] { 44 }));
            TestUtils.TestRoundTrip(new ByteArrayPoco(new byte[] { 44, 88 }));
        }
    }
}

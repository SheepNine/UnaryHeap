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
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<System.ArgumentNullException>(() => new ByteArrayPoco(null));
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new ByteArrayPoco(new byte[] { 1, 3 }));
            Assert.AreEqual(new ByteArrayPoco(new byte[] { 1, 3 }),
                new ByteArrayPoco(new byte[] { 1, 3 }));
            Assert.AreNotEqual(new ByteArrayPoco(new byte[] { 1, 6 }),
                new ByteArrayPoco(new byte[] { 1, 3 }));
            Assert.AreNotEqual(new ByteArrayPoco(new byte[] { 1, 3, 4 }),
                new ByteArrayPoco(new byte[] { 1, 3 }));
            Assert.AreNotEqual(new ByteArrayPoco(new byte[] { 9 }),
                new ByteArrayPoco(new byte[] { 1, 3 }));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual("{\r\n\tOrrey = []\r\n}",
                new ByteArrayPoco(new byte[] { }).ToString());
            Assert.AreEqual("{\r\n\tOrrey = [44]\r\n}",
                new ByteArrayPoco(new byte[] { 44 }).ToString());
            Assert.AreEqual("{\r\n\tOrrey = [44, 88]\r\n}",
                new ByteArrayPoco(new byte[] { 44, 88 }).ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new ByteArrayPoco(new byte[] { }));
            TestUtils.TestRoundTrip(new ByteArrayPoco(new byte[] { 44 }));
            TestUtils.TestRoundTrip(new ByteArrayPoco(new byte[] { 44, 88 }));
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<ByteArrayPoco>(@"{""Orrey"":[]}");
            TestUtils.TestJsonRoundTrip<ByteArrayPoco>(@"{""Orrey"":[44]}");
            TestUtils.TestJsonRoundTrip<ByteArrayPoco>(@"{""Orrey"":[44,88]}");
        }
    }
}

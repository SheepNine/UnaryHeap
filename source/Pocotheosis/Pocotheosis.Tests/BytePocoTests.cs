using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class BytePocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(byte.MinValue, new BytePoco(byte.MinValue).Cheese);
            Assert.AreEqual(byte.MaxValue, new BytePoco(byte.MaxValue).Cheese);
        }

        [Test]
        public void Checksum()
        {
            Assert.AreEqual("5c62e091b8c0565f1bafad0dad5934276143ae2ccef7a5381e8ada5b1a8d26d2",
                new BytePoco(80).Checksum);
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new BytePoco(15));
            Assert.AreEqual(new BytePoco(15), new BytePoco(15));
            Assert.AreNotEqual(new BytePoco(30), new BytePoco(15));
        }

        [Test]
        public void StringFormat()
        {
            TestUtils.TestToString(
                new BytePoco(19),
@"{
    Cheese = 19
}");
            TestUtils.TestToString(
                new BytePoco(44),
@"{
    Cheese = 44
}");
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new BytePoco(byte.MinValue));
            TestUtils.TestRoundTrip(new BytePoco(42));
            TestUtils.TestRoundTrip(new BytePoco(byte.MaxValue));
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<BytePoco>(@"{""Cheese"":0}");
            TestUtils.TestJsonRoundTrip<BytePoco>(@"{""Cheese"":42}");
            TestUtils.TestJsonRoundTrip<BytePoco>(@"{""Cheese"":255}");
        }
    }
}

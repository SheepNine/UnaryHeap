using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System.Globalization;

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
        public void Equality()
        {
            Assert.AreNotEqual(null, new BytePoco(15));
            Assert.AreEqual(new BytePoco(15), new BytePoco(15));
            Assert.AreNotEqual(new BytePoco(30), new BytePoco(15));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual("{\r\n\tCheese = 19\r\n}",
                new BytePoco(19).ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual("{\r\n\tCheese = 44\r\n}",
                new BytePoco(44).ToString(CultureInfo.InvariantCulture));
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

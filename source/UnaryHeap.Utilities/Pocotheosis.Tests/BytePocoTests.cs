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
        public void Equality()
        {
            Assert.AreNotEqual(null, new BytePoco(15));
            Assert.AreEqual(new BytePoco(15), new BytePoco(15));
            Assert.AreNotEqual(new BytePoco(30), new BytePoco(15));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual("{\r\n\tCheese = 19\r\n}", new BytePoco(19).ToString());
            Assert.AreEqual("{\r\n\tCheese = 44\r\n}", new BytePoco(44).ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new BytePoco(byte.MinValue));
            TestUtils.TestRoundTrip(new BytePoco(42));
            TestUtils.TestRoundTrip(new BytePoco(byte.MaxValue));
        }
    }
}

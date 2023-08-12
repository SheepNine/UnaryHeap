using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System.Globalization;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class EmptyPocoTests
    {
        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new EmptyPoco());
            Assert.AreEqual(new EmptyPoco(), new EmptyPoco());
            Assert.AreNotEqual(new EmptyPoco(), new BoolPoco(false));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual("{ }", new EmptyPoco().ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new EmptyPoco());
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<EmptyPoco>("{}");
        }
    }
}

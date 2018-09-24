using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class BoolPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(true, new BoolPoco(true).Value);
            Assert.AreEqual(false, new BoolPoco(false).Value);
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new BoolPoco(false));
            Assert.AreEqual(new BoolPoco(false), new BoolPoco(false));
            Assert.AreNotEqual(new BoolPoco(true), new BoolPoco(false));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual(
@"{
	""Value"": True
}",
            new BoolPoco(true).ToString());

            Assert.AreEqual(
@"{
	""Value"": False
}",
            new BoolPoco(false).ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new BoolPoco(true));
            TestUtils.TestRoundTrip(new BoolPoco(false));
        }

        [Test]
        public void Builder()
        {
            var start = new BoolPoco(true);
            Assert.IsTrue(start.Value);
            var endBuilder = start.ToBuilder();
            Assert.IsTrue(endBuilder.Value);
            endBuilder.Value = false;
            var end = endBuilder.Build();
            Assert.IsFalse(end.Value);
        }
    }
}

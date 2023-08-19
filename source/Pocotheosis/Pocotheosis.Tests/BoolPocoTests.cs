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
        public void Checksum()
        {
            Assert.AreEqual("a8100ae6aa1940d0b663bb31cd466142ebbdbd5187131b92d93818987832eb89",
                new BoolPoco(true).Checksum);
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
            TestUtils.TestToString(
                new BoolPoco(true), 
@"{
    Value = True
}");
            TestUtils.TestToString(
                new BoolPoco(false),
@"{
    Value = False
}");
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new BoolPoco(true));
            TestUtils.TestRoundTrip(new BoolPoco(false));
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<BoolPoco>(@"{""Value"":true}");
            TestUtils.TestJsonRoundTrip<BoolPoco>(@"{""Value"":false}");
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

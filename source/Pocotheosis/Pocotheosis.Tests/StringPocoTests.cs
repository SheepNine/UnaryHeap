using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class StringPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(string.Empty, new StringPoco(string.Empty).Twine);
            Assert.AreEqual("12 Foot", new StringPoco("12 Foot").Twine);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<System.ArgumentNullException>(() => new StringPoco(null));
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new StringPoco("Alpha"));
            Assert.AreEqual(new StringPoco("Alpha"), new StringPoco("Alpha"));
            Assert.AreNotEqual(new StringPoco("Beta"), new StringPoco("Alpha"));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual("{\r\n\tTwine = 'Fortune'\r\n}",
                new StringPoco("Fortune").ToString());
            Assert.AreEqual("{\r\n\tTwine = 'A value\r\nwith newlines'\r\n}",
                new StringPoco("A value\r\nwith newlines").ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new StringPoco(string.Empty));
            TestUtils.TestRoundTrip(new StringPoco("string.NotEmpty"));
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<StringPoco>(@"{""Twine"":""""}");
            TestUtils.TestJsonRoundTrip<StringPoco>(@"{""Twine"":""12 Foot""}");
        }

        [Test]
        public void Builder()
        {
            var start = new StringPoco("alice");
            Assert.AreEqual("alice", start.Twine);
            var endBuilder = start.ToBuilder();
            Assert.AreEqual("alice", endBuilder.Twine);
            endBuilder.Twine = "bob";
            var end = endBuilder.Build();
            Assert.AreEqual("bob", end.Twine);
        }

        [Test]
        public void Builder_NullValue()
        {
            Assert.Throws<System.ArgumentNullException>(() => new StringPoco.Builder(null));
            Assert.Throws<System.ArgumentNullException>(() =>
                { new StringPoco.Builder("not null").Twine = null; });
        }
    }
}

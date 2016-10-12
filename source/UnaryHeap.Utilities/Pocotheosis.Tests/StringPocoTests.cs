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
        [Ignore("TODO: Make this test pass with appropriate check")]
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
            Assert.AreEqual("StringPoco\r\n\tTwine: Fortune", new StringPoco("Fortune").ToString());
            Assert.AreEqual("StringPoco\r\n\tTwine: A value\r\nwith newlines", new StringPoco("A value\r\nwith newlines").ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new StringPoco(string.Empty));
            TestUtils.TestRoundTrip(new StringPoco("string.NotEmpty"));
        }
    }
}

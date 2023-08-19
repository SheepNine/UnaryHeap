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
            var sut = new StringPoco(string.Empty, null);
            Assert.IsEmpty(sut.Twine);
            Assert.IsNull(sut.NullTwine);
            sut = new StringPoco("Twelve", "feet");
            Assert.AreEqual("Twelve", sut.Twine);
            Assert.AreEqual("feet", sut.NullTwine);
        }

        [Test]
        public void Checksum()
        {
            Assert.AreEqual("afd7d76b1100bc71bdc760d6bde3c71be94a18bd1a2ebef6ea5dfd94f4756380",
                new StringPoco("keyval", null).Checksum);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<System.ArgumentNullException>(() =>
                new StringPoco(null, "This one is OK"));
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new StringPoco("Alpha", "Beta"));
            Assert.AreEqual(new StringPoco("Alpha", "Beta"), new StringPoco("Alpha", "Beta"));
            Assert.AreEqual(new StringPoco("Alpha", null), new StringPoco("Alpha", null));
            Assert.AreNotEqual(new StringPoco("Alpha", "Beta"), new StringPoco("Alpha", null));
            Assert.AreNotEqual(new StringPoco("Alpha", "Beta"), new StringPoco("Beta", "Beta"));
        }

        [Test]
        public void StringFormat()
        {
            TestUtils.TestToString(new() { {
                new StringPoco("Fortune", "favors"),
                @"{
                    Twine = 'Fortune'
                    NullTwine = 'favors'
                }"
            }, {
                new StringPoco("Fortune", null),
                @"{
                    Twine = 'Fortune'
                    NullTwine = null
                }"
            }, {
                new StringPoco("A value\r\nwith newlines", string.Empty),
                @"{
                    Twine = 'A value
                with newlines'
                    NullTwine = ''
                }"
            } });
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(
                new StringPoco(string.Empty, null),
                new StringPoco("string.NotEmpty", "also present")
            );
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<StringPoco>(@"{
                ""Twine"": """",
                ""NullTwine"": null
            }", @"{
                ""Twine"": ""Twelve"",
                ""NullTwine"": ""Feet""
            }");
        }

        [Test]
        public void Builder()
        {
            var start = new StringPoco("alice", "bob");
            Assert.AreEqual("alice", start.Twine);
            Assert.AreEqual("bob", start.NullTwine);
            var endBuilder = start.ToBuilder();
            Assert.AreEqual("alice", endBuilder.Twine);
            Assert.AreEqual("bob", endBuilder.NullTwine);
            endBuilder.Twine = "bob";
            endBuilder.NullTwine = null;
            var end = endBuilder.Build();
            Assert.AreEqual("bob", end.Twine);
            Assert.IsNull(end.NullTwine);
        }

        [Test]
        public void BuilderNullValue()
        {
            Assert.Throws<System.ArgumentNullException>(() =>
                new StringPoco.Builder(null, "Not this one"));
            Assert.Throws<System.ArgumentNullException>(() =>
                { new StringPoco.Builder("not null", "or this").Twine = null; });
        }
    }
}

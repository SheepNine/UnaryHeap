using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    [TestFixture]
    public class StringValueTests
    {
        [Test]
        public void Constructor()
        {
            var sut = new StringValue("woo");
            Assert.AreEqual("woo", sut.Str);
            sut = new StringValue(string.Empty);
            Assert.AreEqual(string.Empty, sut.Str);
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(new StringValue("Alpha"), new StringValue("Alpha"));
            Assert.AreNotEqual(new StringValue("Alpha"), new StringValue("Beta"));
        }

        [Test]
        [Ignore("TODO")]
        public void Builder()
        {
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new StringValue("bacon"),
                "ca769d5f992b9ba679dac921f05da88c43ad4a879e868c89dcb45e2520c82128");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new StringValue("Fortune"),
                @"{
                    Str = 'Fortune'
                }"
            }, {
                new StringValue("A value\r\nwith newlines"),
                @"{
                    Str = 'A value
                with newlines'
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new StringValue("woo"),
                new StringValue(string.Empty)
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<StringValue>(@"{
                ""Str"": ""woo""
            }", @"{
                ""Str"": """"
            }");
        }
    }
}

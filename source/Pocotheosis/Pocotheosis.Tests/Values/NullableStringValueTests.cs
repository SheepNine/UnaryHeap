using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    [TestFixture]
    public class NullableStringValueTests
    {
        [Test]
        public void Constructor()
        {
            var sut = new NullableStringValue("woo");
            Assert.AreEqual("woo", sut.Value);
            sut = new NullableStringValue(string.Empty);
            Assert.AreEqual(string.Empty, sut.Value);
            sut = new NullableStringValue(null);
            Assert.AreEqual(null, sut.Value);
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(new NullableStringValue("Alpha"), new NullableStringValue("Alpha"));
            Assert.AreNotEqual(new NullableStringValue("Alpha"), new NullableStringValue("Beta"));
            Assert.AreNotEqual(new NullableStringValue("Alpha"), new NullableStringValue(null));
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
                new NullableStringValue("bacon"),
                "ca769d5f992b9ba679dac921f05da88c43ad4a879e868c89dcb45e2520c82128");
            PocoTest.Checksum(
                new NullableStringValue(null),
                "ad95131bc0b799c0b1af477fb14fcf26a6a9f76079e48bf090acb7e8367bfd0e");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new NullableStringValue("Fortune"),
                @"{
                    Value = 'Fortune'
                }"
            }, {
                new NullableStringValue(null),
                @"{
                    Value = null
                }"
            }, {
                new NullableStringValue("A value\r\nwith newlines"),
                @"{
                    Value = 'A value
                with newlines'
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new NullableStringValue("woo"),
                new NullableStringValue(string.Empty),
                new NullableStringValue(null)
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<NullableStringValue>(@"{
                ""Value"": ""woo""
            }", @"{
                ""Value"": """"
            }", @"{
                ""Value"": null
            }");
        }
    }
}

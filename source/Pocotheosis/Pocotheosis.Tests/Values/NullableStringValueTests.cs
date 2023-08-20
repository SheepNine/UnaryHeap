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
            Assert.AreEqual("woo", sut.MaybeString);
            sut = new NullableStringValue(string.Empty);
            Assert.AreEqual(string.Empty, sut.MaybeString);
            sut = new NullableStringValue(null);
            Assert.AreEqual(null, sut.MaybeString);
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
                    MaybeString = 'Fortune'
                }"
            }, {
                new NullableStringValue(null),
                @"{
                    MaybeString = null
                }"
            }, {
                new NullableStringValue("A value\r\nwith newlines"),
                @"{
                    MaybeString = 'A value
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
                ""MaybeString"": ""woo""
            }", @"{
                ""MaybeString"": """"
            }", @"{
                ""MaybeString"": null
            }");
        }
    }
}

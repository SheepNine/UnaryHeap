using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    [TestFixture]
    public class PrimitiveValueTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(byte.MinValue, new PrimitiveValue(byte.MinValue).Primitive);
            Assert.AreEqual(byte.MaxValue, new PrimitiveValue(byte.MaxValue).Primitive);
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(new PrimitiveValue(15), new PrimitiveValue(15));
            Assert.AreNotEqual(new PrimitiveValue(30), new PrimitiveValue(15));
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new PrimitiveValue(80),
                "5c62e091b8c0565f1bafad0dad5934276143ae2ccef7a5381e8ada5b1a8d26d2");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new PrimitiveValue(19),
                @"{
                    Primitive = 19
                }"
            }, {
                new PrimitiveValue(44),
                @"{
                    Primitive = 44
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new PrimitiveValue(byte.MinValue),
                new PrimitiveValue(42),
                new PrimitiveValue(byte.MaxValue)
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<PrimitiveValue>(@"{
                ""Primitive"": 0
            }", @"{
                ""Primitive"": 42
            }", @"{
                ""Primitive"": 255
            }");
        }
    }
}

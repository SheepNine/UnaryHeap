using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    [TestFixture]
    public class EnumValueTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(TrueBool.False, new EnumValue(TrueBool.False).Value);
            Assert.AreEqual(TrueBool.FileNotFound, new EnumValue(TrueBool.FileNotFound).Value);
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(new EnumValue(TrueBool.False),
                new EnumValue(TrueBool.False));
            Assert.AreNotEqual(new EnumValue(TrueBool.FileNotFound),
                new EnumValue(TrueBool.False));
        }

        [Test]
        public void Builder()
        {
            var start = new EnumValue(TrueBool.False);
            Assert.AreEqual(TrueBool.False, start.Value);
            var endBuilder = start.ToBuilder();
            Assert.AreEqual(TrueBool.False, endBuilder.Value);
            endBuilder.Value = TrueBool.FileNotFound;
            var end = endBuilder.Build();
            Assert.AreEqual(TrueBool.FileNotFound, end.Value);
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new EnumValue(TrueBool.FileNotFound),
                "beead77994cf573341ec17b58bbf7eb34d2711c993c1d976b128b3188dc1829a");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new EnumValue(TrueBool.False),
                @"{
                    Value = False
                }"
            }, {
                new EnumValue(TrueBool.FileNotFound),
                @"{
                    Value = FileNotFound
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new EnumValue(TrueBool.False),
                new EnumValue(TrueBool.FileNotFound)
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<EnumValue>(@"{
                ""Value"": ""True""
            }", @"{
                ""Value"": ""False""
            }", @"{
                ""Value"": ""FileNotFound""
            }");
        }
    }
}

using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    [TestFixture]
    class NullableStringArrayTests
    {
        [Test]
        public void Constructor()
        {
            var data = new[] { "A", null, "B" };
            var sut = new NullableStringArray(data);
            data[1] = "C"; // Ensures poco made a copy
            Assert.AreEqual(3, sut.Elements.Count);
            Assert.AreEqual("A", sut.Elements[0]);
            Assert.AreEqual(null, sut.Elements[1]);
            Assert.AreEqual("B", sut.Elements[2]);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new NullableStringArray(null));
        }

        [Test]
        [Ignore("Pending implementation")]
        public void Equality()
        {
        }

        [Test]
        [Ignore("Pending implementation")]
        public void Builder()
        {
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new NullableStringArray(new[] { null, null, "three", null }),
                "c18d189c10c7bf6fc1c55e36e46fc530346bc28fd7eea27d28fd98796c6e44d7");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new NullableStringArray(Enumerable.Empty<string>()),
                @"{
                    Elements = []
                }"
            }, {
                new NullableStringArray(new[] { "A", null, "B" }),
                @"{
                    Elements = ['A', null, 'B']
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new NullableStringArray(Enumerable.Empty<string>()),
                new NullableStringArray(new[] { "A", null, "D" })
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<NullableStringArray>(@"{
                ""Elements"": []
            }", @"{
                ""Elements"": [""a"",null,""c"",null]
            }");
        }
    }
}

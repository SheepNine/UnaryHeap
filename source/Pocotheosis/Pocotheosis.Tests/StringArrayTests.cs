using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Linq;

namespace Pocotheosis.Tests
{
    [TestFixture]
    class StringArrayTests
    {
        [Test]
        public void Constructor()
        {
            var data = new[] { "A", null, "B" };
            var sut = new StringArrayPoco(data);
            data[1] = "C"; // Ensures poco made a copy
            Assert.AreEqual(3, sut.Data.Count);
            Assert.AreEqual("A", sut.Data[0]);
            Assert.AreEqual(null, sut.Data[1]);
            Assert.AreEqual("B", sut.Data[2]);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StringArrayPoco(null));
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
                new StringArrayPoco(new[] { null, null, "three", null }),
                "c18d189c10c7bf6fc1c55e36e46fc530346bc28fd7eea27d28fd98796c6e44d7");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new StringArrayPoco(Enumerable.Empty<string>()),
                @"{
                    Data = []
                }"
            }, {
                new StringArrayPoco(new[] { "A", null, "B" }),
                @"{
                    Data = ['A', null, 'B']
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new StringArrayPoco(Enumerable.Empty<string>()),
                new StringArrayPoco(new[] { "A", null, "D" })
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<StringArrayPoco>(@"{
                ""Data"": []
            }", @"{
                ""Data"": [""a"",null,""c"",null]
            }");
        }
    }
}

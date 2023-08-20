using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    [TestFixture]
    class StringArrayTests
    {
        [Test]
        public void Constructor()
        {
            var data = new[] { "A", "B" };
            var sut = new StringArray(data);
            data[1] = "C"; // Ensures poco made a copy
            Assert.AreEqual(2, sut.Elements.Count);
            Assert.AreEqual("A", sut.Elements[0]);
            Assert.AreEqual("B", sut.Elements[1]);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new StringArray(null));
            Assert.Throws<ArgumentNullException>(() =>
                new StringArray(new string[] { null }));
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
        [Ignore("Pending implementation")]
        public void Builder_NullReference()
        {
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new StringArray(new[] { "three"}),
                "b5161981cb772868589cf4415af21d855458bdffabce0328765eefeb61af2699");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new StringArray(Enumerable.Empty<string>()),
                @"{
                    Elements = []
                }"
            }, {
                new StringArray(new[] { "A", "B" }),
                @"{
                    Elements = ['A', 'B']
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new StringArray(Enumerable.Empty<string>()),
                new StringArray(new[] { "A", "D" })
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<StringArray>(@"{
                ""Elements"": []
            }", @"{
                ""Elements"": [""a"",""c""]
            }");
        }
    }
}

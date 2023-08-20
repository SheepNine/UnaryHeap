using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;

namespace Pocotheosis.Tests.Arrays
{
    [TestFixture]
    public class ByteArrayPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0, new PrimitiveArray(Array.Empty<byte>()).Primitives.Count);
            var data = new byte[] { 17, 88 };
            var poco = new PrimitiveArray(data);
            Assert.AreEqual(2, poco.Primitives.Count);
            data[0] = 0; // Ensures poco made a copy
            Assert.AreEqual(17, poco.Primitives[0]);
            Assert.AreEqual(88, poco.Primitives[1]);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(() => new PrimitiveArray(null));
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(new PrimitiveArray(new byte[] { 1, 3 }),
                new PrimitiveArray(new byte[] { 1, 3 }));
            Assert.AreNotEqual(new PrimitiveArray(new byte[] { 1, 6 }),
                new PrimitiveArray(new byte[] { 1, 3 }));
            Assert.AreNotEqual(new PrimitiveArray(new byte[] { 1, 3, 4 }),
                new PrimitiveArray(new byte[] { 1, 3 }));
            Assert.AreNotEqual(new PrimitiveArray(new byte[] { 9 }),
                new PrimitiveArray(new byte[] { 1, 3 }));
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new PrimitiveArray(new byte[] { 42 }),
                "9bb9a2dc678bacc1ec2651d9afcb092320f4068ea9c8b55593574b3b70f9285f");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new PrimitiveArray(Array.Empty<byte>()),
                @"{
                    Primitives = []
                }"
            }, {
                new PrimitiveArray(new byte[] { 44 }),
                @"{
                    Primitives = [44]
                }"
            }, {
                new PrimitiveArray(new byte[] { 44, 88 }),
                @"{
                    Primitives = [44, 88]
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new PrimitiveArray(Array.Empty<byte>()),
                new PrimitiveArray(new byte[] { 44 }),
                new PrimitiveArray(new byte[] { 44, 88 })
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<PrimitiveArray>(@"{
                ""Primitives"": []
            }", @"{
                ""Primitives"": [44]
            }", @"{
                ""Primitives"": [44,88]
            }");
        }
    }
}

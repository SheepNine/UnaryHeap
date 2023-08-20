using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    [TestFixture]
    public class ClassArrayTests
    {
        static PrimitiveValue P(byte value) { return new PrimitiveValue(value); }

        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0, new ClassArray(Array.Empty<PrimitiveValue>()).Elements.Count);
            var data = new PrimitiveValue[] { P(1), P(2) };
            var poco = new ClassArray(data);
            Assert.AreEqual(2, poco.Elements.Count);
            data[0] = P(9); // Ensures poco made a copy
            Assert.AreEqual(1, poco.Elements[0].Value);
            Assert.AreEqual(2, poco.Elements[1].Value);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ClassArray(null));
            Assert.Throws<ArgumentNullException>(() =>
                new ClassArray(new PrimitiveValue[] { null }));
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(
                new ClassArray(new[] { P(1) }),
                new ClassArray(new[] { P(1) }));
            Assert.AreNotEqual(
                new ClassArray(new[] { P(1) }),
                new ClassArray(new[] { P(2) }));
            Assert.AreNotEqual(
                new ClassArray(new[] { P(1) }),
                new ClassArray(new[] { P(1), P(2) }));
            Assert.AreNotEqual(
                new ClassArray(new[] { P(1) }),
                new ClassArray(Enumerable.Empty<PrimitiveValue>()));
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new ClassArray(new[] { P(88), P(77) }),
                "dd4af20eaaf7edad8d6c08be993910d228bea226fc8cfe499ad210b33c06af98");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new ClassArray(new PrimitiveValue[] {
                    P(1)
                }),
                @"{
                    Elements = [{
                        Value = 1
                    }]
                }"
            }, {
                new ClassArray(new PrimitiveValue[] {
                    P(77),
                    P(80)
                }),
                @"{
                    Elements = [{
                        Value = 77
                    }, {
                        Value = 80
                    }]
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            var data = new PrimitiveValue[]
            {
                P(2),
                P(6),
                P(7)
            };

            PocoTest.Serialization(
                new ClassArray(data.Take(0)),
                new ClassArray(data.Take(1)),
                new ClassArray(data.Take(2))
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<ClassArray>(@"{
                ""Elements"": []
            }", @"{
                ""Elements"": [{
                    ""Value"": 3
                }]
            }", @"{
                ""Elements"": [{
                    ""Value"": 1
                },{
                    ""Value"": 2
                }]
            }");
        }
    }
}

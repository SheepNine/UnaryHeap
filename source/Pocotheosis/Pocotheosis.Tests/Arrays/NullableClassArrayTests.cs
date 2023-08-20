using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Linq;

namespace Pocotheosis.Tests.Arrays
{
    [TestFixture]
    public class NullableClassArrayTests
    {
        static PrimitiveValue P(byte value) { return new PrimitiveValue(value); }

        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0,
                new NullableClassArray(Array.Empty<PrimitiveValue>()).Elements.Count);
            var data = new PrimitiveValue[] { P(1), null, P(2) };
            var poco = new NullableClassArray(data);
            Assert.AreEqual(3, poco.Elements.Count);
            data[0] = P(9); // Ensures poco made a copy
            Assert.AreEqual(1, poco.Elements[0].Value);
            Assert.IsNull(poco.Elements[1]);
            Assert.AreEqual(2, poco.Elements[2].Value);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(() => new NullableClassArray(null));
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(
                new NullableClassArray(new[] { P(1) }),
                new NullableClassArray(new[] { P(1) }));
            Assert.AreNotEqual(
                new NullableClassArray(new[] { P(1) }),
                new NullableClassArray(new PrimitiveValue[] { null }));
            Assert.AreNotEqual(
                new NullableClassArray(new[] { P(1) }),
                new NullableClassArray(new[] { P(2) }));
            Assert.AreNotEqual(
                new NullableClassArray(new[] { P(1) }),
                new NullableClassArray(new[] { P(1), P(2) }));
            Assert.AreNotEqual(
                new NullableClassArray(new[] { P(1) }),
                new NullableClassArray(Enumerable.Empty<PrimitiveValue>()));
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new NullableClassArray(new[] { P(88), null, P(77) }),
                "b00f3f643b452203bc67a038c3e3cff63073a4680fca59a27a1733706edb2751");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new NullableClassArray(new PrimitiveValue[] {
                    P(1)
                }),
                @"{
                    Elements = [{
                        Value = 1
                    }]
                }"
            }, {
                new NullableClassArray(new PrimitiveValue[] {
                    P(77),
                    null,
                    P(80)
                }),
                @"{
                    Elements = [{
                        Value = 77
                    }, null, {
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
                null,
                P(6),
                P(7)
            };

            PocoTest.Serialization(
                new NullableClassArray(data.Take(0)),
                new NullableClassArray(data.Take(1)),
                new NullableClassArray(data.Take(2)),
                new NullableClassArray(data.Take(3))
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<NullableClassArray>(@"{
                ""Elements"": []
            }", @"{
                ""Elements"": [{
                    ""Value"": 3
                }]
            }", @"{
                ""Elements"": [{
                    ""Value"": 1
                },
                null,
                {
                    ""Value"": 2
                }]
            }");
        }
    }
}

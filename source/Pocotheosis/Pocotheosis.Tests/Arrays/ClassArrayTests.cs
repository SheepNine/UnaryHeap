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
            Assert.AreEqual(0, new ClassArray(Array.Empty<PrimitiveValue>()).Pocos.Count);
            var data = new PrimitiveValue[] { P(1), P(2) };
            var poco = new ClassArray(data);
            Assert.AreEqual(2, poco.Pocos.Count);
            data[0] = P(9); // Ensures poco made a copy
            Assert.AreEqual(1, poco.Pocos[0].Primitive);
            Assert.AreEqual(2, poco.Pocos[1].Primitive);
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
            PocoTest.Equality(new[]
            {
                new ClassArray(Enumerable.Empty<PrimitiveValue>()),
                new ClassArray(new[] { P(1) } ),
                new ClassArray(new[] { P(2) } ),
                new ClassArray(new[] { P(1), P(2) } ),
            });
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
                    Pocos = [{
                        Primitive = 1
                    }]
                }"
            }, {
                new ClassArray(new PrimitiveValue[] {
                    P(77),
                    P(80)
                }),
                @"{
                    Pocos = [{
                        Primitive = 77
                    }, {
                        Primitive = 80
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
                ""Pocos"": []
            }", @"{
                ""Pocos"": [{
                    ""Primitive"": 3
                }]
            }", @"{
                ""Pocos"": [{
                    ""Primitive"": 1
                },{
                    ""Primitive"": 2
                }]
            }");
        }
    }
}

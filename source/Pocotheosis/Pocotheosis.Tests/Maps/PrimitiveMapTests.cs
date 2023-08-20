using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using Dataset = System.Collections.Generic.Dictionary<bool, byte>;

namespace Pocotheosis.Tests.Maps
{
    [TestFixture]
    public class PrimitiveMapTests
    {
        [Test]
        public void Constructor()
        {
            var data = new Dataset()
            {
                { true, 1 },
                { false, 0 }
            };
            var sut = new PrimitiveMap(data);
            Assert.AreEqual(2, sut.Primitives.Count);
            Assert.AreEqual(1, sut.Primitives[true]);
            Assert.AreEqual(0, sut.Primitives[false]);

            sut = new PrimitiveMap(new Dataset());
            Assert.AreEqual(0, sut.Primitives.Count);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PrimitiveMap(null));
        }

        [Test]
        public void Equality()
        {
            var datasets = new[]
            {
                new Dataset(),
                new Dataset() { { true, 1 } },
                new Dataset() { { false, 1 } },
                new Dataset() { { true, 0 } },
                new Dataset() { { true, 1 }, { false, 1 } },
            };

            for (int i = 0; i < datasets.Length; i++)
                for (int j = 0; j < datasets.Length; j++)
                    Assert.AreEqual(i == j, new PrimitiveMap(datasets[i])
                        .Equals(new PrimitiveMap(datasets[j])));
        }

        [Test]
        [Ignore("TODO")]
        public void Builder()
        {

        }

        [Test]
        [Ignore("TODO")]
        public void BuilderNullReference()
        {

        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new PrimitiveMap(new Dataset() { { true, 255 } }),
                "34b5d0d734084832dea276360f994018f4e6eb7921334265e58217b360a2a79d");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new PrimitiveMap(new Dataset()
                {
                    { true, 8 },
                    { false, 0 }
                }),
                @"{
                    Primitives = (
                        False -> 0,
                        True -> 8
                    )
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new PrimitiveMap(new Dataset()),
                new PrimitiveMap(new Dataset()
                {
                    { true, 1 },
                    { false, 8 }
                })
            );
        }
    }
}

using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using Dataset = System.Collections.Generic.Dictionary
    <string, Pocotheosis.Tests.Pocos.PrimitiveValue>;

namespace Pocotheosis.Tests.Maps
{
    [TestFixture]
    public class NullableClassMapTests
    {
        static PrimitiveValue P(byte value) { return new PrimitiveValue(value); }

        [Test]
        public void Constructor()
        {
            var data = new Dataset()
            {
                { "3", P(1) },
                { "7", null },
                { "5", P(0) }
            };
            var sut = new NullableClassMap(data);
            data.Clear(); // Ensures that sut made a copy

            Assert.AreEqual(3, sut.MaybePocos.Count);
            Assert.AreEqual(1, sut.MaybePocos["3"].Primitive);
            Assert.IsNull(sut.MaybePocos["7"]);
            Assert.AreEqual(0, sut.MaybePocos["5"].Primitive);

            sut = new NullableClassMap(new Dataset());
            Assert.AreEqual(0, sut.MaybePocos.Count);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(
                () => new NullableClassMap(null));
        }

        [Test]
        public void Equality()
        {
            var datasets = new[]
            {
                new Dataset(),
                new Dataset() { { "1", P(0) } },
                new Dataset() { { "2", P(0) } },
                new Dataset() { { "1", P(1) } },
                new Dataset() { { "1", null } },
                new Dataset() { { "1", P(0) }, { "2", P(0) } },
            };

            for (int i = 0; i < datasets.Length; i++)
                for (int j = 0; j < datasets.Length; j++)
                    Assert.AreEqual(i == j, new NullableClassMap(datasets[i])
                        .Equals(new NullableClassMap(datasets[j])));
        }

        [Test]
        public void Builder()
        {
            var source = new NullableClassMap(new Dataset() {
                { "3", null },
                { "5", P(1) }
            });

            var target = new NullableClassMap(new Dataset() {
                { "3", P(1) },
                { "7", P(0) }
            });

            {
                var sut = source.ToBuilder();
                sut.SetMaybePoco("3", P(1));
                sut.RemoveMaybePoco("5");
                sut.SetMaybePoco("7", P(0));
                Assert.AreEqual(target, sut.Build());

                Assert.AreEqual(2, sut.CountMaybePocos);
                Assert.AreEqual(new[] { "3", "7" }, sut.MaybePocoKeys);
            }

            {
                var sut = target.ToBuilder();
                sut.ClearMaybePocos();
                sut.SetMaybePoco("3", null);
                sut.SetMaybePoco("5", P(1));
                Assert.AreEqual(source, sut.Build());

                Assert.AreEqual(2, sut.CountMaybePocos);
                Assert.AreEqual(new[] { "3", "5" }, sut.MaybePocoKeys);
            }
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new NullableClassMap(new Dataset() {
                    { "3", P(0) },
                    { "5", null }
                }),
                "9a8238c467801aa1e4a5b71cb568a9f833fb1c38badc95c746e860326f014db8");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new NullableClassMap(new Dataset()
                {
                    { "3", P(1) },
                    { "5", P(0) },
                    { "4", null }
                }),
                @"{
                    MaybePocos = (
                        '3' -> {
                            Primitive = 1
                        },
                        '4' -> null,
                        '5' -> {
                            Primitive = 0
                        }
                    )
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new NullableClassMap(new Dataset()
                {
                    { "-99", P(0) },
                    { "7", P(1) },
                    { "3", null },
                }),
                new NullableClassMap(new Dataset())
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<NullableClassMap>(@"{
                ""MaybePocos"": {}
            }", @"{
                ""MaybePocos"": {
                    ""3"": {
                        ""Primitive"": 0
                    }
                }
            }", @"{
                ""MaybePocos"": {
                    ""3"": {
                        ""Primitive"": 1
                    },
                    ""4"": null,
                    ""5"": {
                        ""Primitive"": 0
                    }
                }
            }");
        }
    }
}

using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using Dataset = System.Collections.Generic.Dictionary
    <string, Pocotheosis.Tests.Pocos.PrimitiveValue>;

namespace Pocotheosis.Tests.Maps
{
    [TestFixture]
    public class ClassMapTests
    {
        static PrimitiveValue P(byte value) { return new PrimitiveValue(value); }

        [Test]
        public void Constructor()
        {
            var data = new Dataset()
            {
                { "3", P(1) },
                { "5", P(0) }
            };
            var sut = new ClassMap(data);
            data.Clear(); // Ensures that sut made a copy

            Assert.AreEqual(2, sut.Pocos.Count);
            Assert.AreEqual(1, sut.Pocos["3"].Primitive);
            Assert.AreEqual(0, sut.Pocos["5"].Primitive);

            sut = new ClassMap(new Dataset());
            Assert.AreEqual(0, sut.Pocos.Count);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ClassMap(null));
            Assert.Throws<ArgumentNullException>(
                () => new ClassMap(new Dataset() { { "1", null } }));
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
                new Dataset() { { "1", P(0) }, { "2", P(0) } },
            };

            for (int i = 0; i < datasets.Length; i++)
                for (int j = 0; j < datasets.Length; j++)
                    Assert.AreEqual(i == j, new ClassMap(datasets[i])
                        .Equals(new ClassMap(datasets[j])));
        }

        [Test]
        public void Builder()
        {
            var source = new ClassMap(new Dataset() {
                { "5", P(1) }
            });

            var target = new ClassMap(new Dataset() {
                { "3", P(1) },
                { "7", P(0) }
            });

            {
                var sut = source.ToBuilder();
                sut.RemovePoco("5");
                sut.SetPoco("7", P(0));
                sut.SetPoco("3", P(1));
                Assert.AreEqual(target, sut.Build());

                Assert.AreEqual(2, sut.CountPocos);
                Assert.AreEqual(new[] { "3", "7" }, sut.PocoKeys);
            }

            {
                var sut = target.ToBuilder();
                sut.ClearPocos();
                sut.SetPoco("5", P(1));
                Assert.AreEqual(source, sut.Build());

                Assert.AreEqual(1, sut.CountPocos);
                Assert.AreEqual(new[] { "5" }, sut.PocoKeys);
            }
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new ClassMap(new Dataset() {
                    { "3", P(0) }
                }),
                "ba390572f657a894441aee73d4063315aa8dd89462d75123b077ebb409ab3cbf");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new ClassMap(new Dataset()
                {
                    { "3", P(1) },
                    { "5", P(0) }
                }),
                @"{
                    Pocos = (
                        '3' -> {
                            Primitive = 1
                        },
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
                new ClassMap(new Dataset()
                {
                    { "-99", P(0) },
                    { "7", P(1) }
                }),
                new ClassMap(new Dataset())
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<ClassMap>(@"{
                ""Pocos"": {}
            }", @"{
                ""Pocos"": {
                    ""3"": {
                        ""Primitive"": 0
                    }
                }
            }", @"{
                ""Pocos"": {
                    ""3"": {
                        ""Primitive"": 1
                    },
                    ""5"": {
                        ""Primitive"": 0
                    }
                }
            }");
        }
    }
}

using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using Dataset = System.Collections.Generic.Dictionary<int, Pocotheosis.Tests.Pocos.BoolPoco>;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class ClassDictionaryPocoTests
    {
        [Test]
        public void Constructor()
        {
            var data = new Dataset()
            {
                { 3, new BoolPoco(true) },
                { 7, null },
                { 5, new BoolPoco(false) }
            };
            var sut = new ClassDictionaryPoco(data);
            data.Clear(); // Ensures that sut made a copy

            Assert.AreEqual(3, sut.Geese.Count);
            Assert.AreEqual(true, sut.Geese[3].Value);
            Assert.IsNull(sut.Geese[7]);
            Assert.AreEqual(false, sut.Geese[5].Value);

            sut = new ClassDictionaryPoco(new Dataset());
            Assert.AreEqual(0, sut.Geese.Count);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ClassDictionaryPoco(null));
        }

        [Test]
        public void Equality()
        {
            var datasets = new[]
            {
                new Dataset(),
                new Dataset() { { 1, new BoolPoco(false) } },
                new Dataset() { { 2, new BoolPoco(false) } },
                new Dataset() { { 1, new BoolPoco(true) } },
                new Dataset() { { 1, null } },
                new Dataset() { { 1, new BoolPoco(false) }, { 2, new BoolPoco(false) } },
            };

            for (int i = 0; i < datasets.Length; i++)
                for (int j = 0; j < datasets.Length; j++)
                    Assert.AreEqual(i == j, new ClassDictionaryPoco(datasets[i])
                        .Equals(new ClassDictionaryPoco(datasets[j])));
        }

        [Test]
        public void Builder()
        {
            var source = new ClassDictionaryPoco(new Dataset() {
                { 3, null },
                { 5, new BoolPoco(true) }
            });

            var target = new ClassDictionaryPoco(new Dataset() {
                { 3, new BoolPoco(true) },
                { 7, new BoolPoco(false) }
            });

            {
                var sut = source.ToBuilder();
                sut.SetGoose(3, new BoolPoco(true));
                sut.RemoveGoose(5);
                sut.SetGoose(7, new BoolPoco(false));
                Assert.AreEqual(target, sut.Build());

                Assert.AreEqual(2, sut.CountGeese);
                Assert.AreEqual(new[] { 3, 7 }, sut.GooseKeys);
            }

            {
                var sut = target.ToBuilder();
                sut.ClearGeese();
                sut.SetGoose(3, null);
                sut.SetGoose(5, new BoolPoco(true));
                Assert.AreEqual(source, sut.Build());

                Assert.AreEqual(2, sut.CountGeese);
                Assert.AreEqual(new[] { 3, 5 }, sut.GooseKeys);
            }
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new ClassDictionaryPoco(new Dataset() {
                    { 3, new BoolPoco(false) },
                    { 5, null }
                }),
                "db383cf7f4e7deb4c8fbf0d80434824cac62125f07e3e6b981780de0b2214928");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new ClassDictionaryPoco(new Dataset()
                {
                    { 3, new BoolPoco(true) },
                    { 5, new BoolPoco(false) },
                    { 4, null }
                }),
                @"{
                    Geese = (
                        3 -> {
                            Value = True
                        },
                        4 -> null,
                        5 -> {
                            Value = False
                        }
                    )
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new ClassDictionaryPoco(new Dataset()
                {
                    { -99, new BoolPoco(false) },
                    { 7, new BoolPoco(true) },
                    { 6, new BoolPoco(false) },
                    { 3, null },
                }),
                new ClassDictionaryPoco(new Dataset())
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<ClassDictionaryPoco>(@"{
                ""Geese"": []
            }", @"{
                ""Geese"": [{
                    ""k"": 3,
                    ""v"": {
                        ""Value"": true
                    }
                }]
            }", @"{
                ""Geese"": [{
                    ""k"": 3,
                    ""v"": {
                        ""Value"": true
                    }
                },{
                    ""k"": 4,
                    ""v"": null
                },{
                    ""k"": 5,
                    ""v"": {
                        ""Value"": false
                    }
                }]
            }");
        }
    }
}

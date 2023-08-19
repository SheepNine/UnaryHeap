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
                { 5, new BoolPoco(false) }
            };
            var sut = new ClassDictionaryPoco(data);
            data.Clear(); // Ensures that sut made a copy

            Assert.AreEqual(2, sut.Geese.Count);
            Assert.AreEqual(true, sut.Geese[3].Value);
            Assert.AreEqual(false, sut.Geese[5].Value);

            sut = new ClassDictionaryPoco(new Dataset());
            Assert.AreEqual(0, sut.Geese.Count);
        }

        [Test]
        public void Checksum()
        {
            Assert.AreEqual("7820089a8b82f4397a2ec1af7bb52232b9e1a4b40f5a7ee5753c57ff098a9d12",
                new ClassDictionaryPoco(new Dataset() { { 3, new BoolPoco(false) } }).Checksum);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ClassDictionaryPoco(null));
            Assert.Throws<ArgumentNullException>(
                () => new ClassDictionaryPoco(new Dataset { { 1, null } }));
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
                new Dataset() { { 1, new BoolPoco(false) }, { 2, new BoolPoco(false) } },
            };

            for (int i = 0; i < datasets.Length; i++)
                for (int j = 0; j < datasets.Length; j++)
                    Assert.AreEqual(i == j, new ClassDictionaryPoco(datasets[i])
                        .Equals(new ClassDictionaryPoco(datasets[j])));
        }

        [Test]
        public void StringFormat()
        {
            TestUtils.TestToString(new ClassDictionaryPoco(new Dataset()
            {
                { 3, new BoolPoco(true) },
                { 5, new BoolPoco(false) }
            }),
@"{
    Geese = (
        3 -> {
            Value = True
        },
        5 -> {
            Value = False
        }
    )
}");
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(
                new ClassDictionaryPoco(new Dataset()
                {
                    { -99, new BoolPoco(false) },
                    { 7, new BoolPoco(true) },
                    { 6, new BoolPoco(false) },
                }),
                new ClassDictionaryPoco(new Dataset())
            );
        }

        [Test]
        public void Builder()
        {
            var source = new ClassDictionaryPoco(new Dataset()
            {
                { 3, new BoolPoco(true) },
                { 5, new BoolPoco(false) }
            });
            var builder = source.ToBuilder();
            builder.RemoveGoose(3);
            builder.SetGoose(5, new BoolPoco(true));
            builder.SetGoose(7, new BoolPoco(false));
            var modifiedSut = builder.Build();
            builder.ClearGeese();
            var emptySut = builder.Build();

            Assert.AreEqual(new ClassDictionaryPoco(new Dataset()
            {
                { 5, new BoolPoco(false) },
                { 3, new BoolPoco(true) }
            }), source);
            Assert.AreEqual(new ClassDictionaryPoco(new Dataset()),
                emptySut);
            Assert.AreEqual(new ClassDictionaryPoco(new Dataset()
            {
                { 7, new BoolPoco(false) },
                { 5, new BoolPoco(true) }
            }), modifiedSut);
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<ClassDictionaryPoco>(@"{
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
                    ""k"": 5,
                    ""v"": {
                        ""Value"": false
                    }
                }]
            }");
        }
    }
}

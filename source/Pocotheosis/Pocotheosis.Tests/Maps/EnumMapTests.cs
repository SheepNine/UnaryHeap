using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using Dataset = System.Collections.Generic.Dictionary
    <byte, Pocotheosis.Tests.Pocos.TrueBool>;

namespace Pocotheosis.Tests.Maps
{
    [TestFixture]
    public class EnumMapTests
    {
        [Test]
        public void Constructor()
        {
            var data = new Dataset()
            {
                { 3, TrueBool.True },
                { 9, TrueBool.False },
            };
            var sut = new EnumMap(data);
            Assert.AreEqual(2, sut.Enums.Count);
            Assert.AreEqual(TrueBool.True, sut.Enums[3]);
            Assert.AreEqual(TrueBool.False, sut.Enums[9]);

            sut = new EnumMap(new Dataset());
            Assert.AreEqual(0, sut.Enums.Count);
        }

        [Test]
        public void ConstructorNullReference()
        {
            Assert.Throws<ArgumentNullException>(
                () => new EnumMap(null));
        }

        [Test]
        public void Equality()
        {
            var datasets = new[]
            {
                new Dataset(),
                new Dataset() { { 1, TrueBool.True } },
                new Dataset() { { 2, TrueBool.True } },
                new Dataset() { { 1, TrueBool.False } },
                new Dataset() { { 1, TrueBool.True }, { 2, TrueBool.False } },
            };

            for (int i = 0; i < datasets.Length; i++)
                for (int j = 0; j < datasets.Length; j++)
                    Assert.AreEqual(i == j, new EnumMap(datasets[i])
                        .Equals(new EnumMap(datasets[j])));
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
                new EnumMap(new Dataset() { { 1, TrueBool.True } }),
                "a90d4563c6a0ae0417ab3110f1ba68592833465954774201b0a69e8c457dc6ad");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new EnumMap(new Dataset()
                {
                    { 3, TrueBool.True },
                    { 5, TrueBool.False }
                }),
                @"{
                    Enums = (
                        3 -> True,
                        5 -> False
                    )
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new EnumMap(new Dataset()),
                new EnumMap(new Dataset()
                {
                    { 3, TrueBool.True },
                    { 5, TrueBool.False }
                })
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<EnumMap>(@"{
                ""Enums"": [{
                    ""k"": 1,
                    ""v"": ""FileNotFound""
                },{
                    ""k"": 8,
                    ""v"": ""True""
                }]
            }");
        }
    }
}
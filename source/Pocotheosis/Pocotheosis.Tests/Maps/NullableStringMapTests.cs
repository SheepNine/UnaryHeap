using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using Dataset = System.Collections.Generic.Dictionary<int, string>;

namespace Pocotheosis.Tests.Maps
{
    [TestFixture]
    class NullableStringMapTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0,
                new NullableStringMap(new Dataset()).Mappings.Count);

            var data = new Dataset()
            {
                { 1, "ff0000" },
                { 3, "00ff00" }
            };

            var poco = new NullableStringMap(data);
            Assert.AreEqual(2, poco.Mappings.Count);
            data.Clear(); // Ensures poco made a copy
            Assert.AreEqual("ff0000", poco.Mappings[1]);
            Assert.AreEqual("00ff00", poco.Mappings[3]);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<ArgumentNullException>(() => new NullableStringMap(null));
        }

        [Test]
        public void Equality()
        {
            var data = new Dataset()
            {
                { 1, "one" },
                { 3, "three" }
            };
            var differentData = new Dataset()
            {
                { 1, "one" },
                { 3, "six" }
            };
            var longerData = new Dataset()
            {
                { 1, "one" },
                { 3, null },
                { 4, "four" }
            };
            var shorterData = new Dataset()
            {
                { 1, "one" }
            };

            Assert.AreEqual(new NullableStringMap(data), new NullableStringMap(data));
            Assert.AreNotEqual(new NullableStringMap(data), new NullableStringMap(differentData));
            Assert.AreNotEqual(new NullableStringMap(data), new NullableStringMap(longerData));
            Assert.AreNotEqual(new NullableStringMap(data), new NullableStringMap(shorterData));
        }

        [Test]
        public void Builder()
        {
            var source = new NullableStringMap(new Dataset()
            {
                { 1, "one" },
                { 2, "two" }
            });
            var destination = new NullableStringMap(new Dataset()
            {
                { 3, "three" },
                { 4, null }
            });

            {
                var sut = source.ToBuilder();
                sut.RemoveMapping(1);
                sut.SetMapping(3, "three");
                sut.SetMapping(4, null);
                sut.RemoveMapping(2);
                Assert.AreEqual(destination, sut.Build());
            }
            {
                var sut = destination.ToBuilder();
                sut.ClearMappings();
                sut.SetMapping(2, "two");
                sut.SetMapping(1, "one");
                Assert.AreEqual(source, sut.Build());
            }
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new NullableStringMap(new Dataset() { { 0, "val" }, { 1, null } }),
                "fc21e9be1bcad79c8ccd1b6898cfda190aed67d3c1b3af12084ea0d86b00bc16");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new NullableStringMap(new Dataset()
                {
                    { 0, "noughT" }
                }),
                @"{
                    Mappings = (
                        0 -> 'noughT'
                    )
                }"
            }, {
                new NullableStringMap(new Dataset()
                {
                    { 1, "Value1" },
                    { 2, "Value2" },
                    { 3, null },
                }),
                @"{
                    Mappings = (
                        1 -> 'Value1',
                        2 -> 'Value2',
                        3 -> null
                    )
                }"
            }, {
                new NullableStringMap(new Dataset()),
                @"{
                    Mappings = ()
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new NullableStringMap(new Dataset()
                {
                }),
                new NullableStringMap(new Dataset() {
                    { 44, "44" }
                }),
                new NullableStringMap(new Dataset() {
                    { 44, null },
                    { 88, "88" }
                })
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<NullableStringMap>(@"{
                ""Mappings"": []
            }", @"{
                ""Mappings"": [{
                    ""k"": 44,
                    ""v"": ""fortyfor""
                }]
            }", @"{
                ""Mappings"": [{
                    ""k"": 44,
                    ""v"": ""fortyfor""
                },{
                    ""k"": 88,
                    ""v"": ""eightyate""
                },{
                    ""k"": 100,
                    ""v"": null
                }]
            }");
        }
    }
}

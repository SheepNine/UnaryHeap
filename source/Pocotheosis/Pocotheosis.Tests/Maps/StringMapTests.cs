using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using Dataset = System.Collections.Generic.Dictionary<Pocotheosis.Tests.Pocos.TrueBool, string>;

namespace Pocotheosis.Tests.Maps
{
    [TestFixture]
    class StringMapTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0,
                new StringMap(new Dataset()).Strs.Count);

            var data = new Dataset()
            {
                { TrueBool.True, "ff0000" },
                { TrueBool.False, "00ff00" }
            };

            var poco = new StringMap(data);
            Assert.AreEqual(2, poco.Strs.Count);
            data.Clear(); // Ensures poco made a copy
            Assert.AreEqual("ff0000", poco.Strs[TrueBool.True]);
            Assert.AreEqual("00ff00", poco.Strs[TrueBool.False]);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<ArgumentNullException>(() => new StringMap(null));
        }

        [Test]
        public void Equality()
        {
            var data = new Dataset()
            {
                { TrueBool.True, "one" },
                { TrueBool.False, "three" }
            };
            var differentData = new Dataset()
            {
                { TrueBool.True, "one" },
                { TrueBool.False, "six" }
            };
            var longerData = new Dataset()
            {
                { TrueBool.True, "one" },
                { TrueBool.False, "three" },
                { TrueBool.FileNotFound, "four" }
            };
            var shorterData = new Dataset()
            {
                { TrueBool.True, "one" }
            };

            Assert.AreEqual(new StringMap(data), new StringMap(data));
            Assert.AreNotEqual(new StringMap(data), new StringMap(differentData));
            Assert.AreNotEqual(new StringMap(data), new StringMap(longerData));
            Assert.AreNotEqual(new StringMap(data), new StringMap(shorterData));
        }

        [Test]
        public void Builder()
        {
            var source = new StringMap(new Dataset()
            {
                { TrueBool.True, "one" },
                { TrueBool.False, "two" }
            });
            var destination = new StringMap(new Dataset()
            {
                { TrueBool.True, "three" },
            });

            {
                var sut = source.ToBuilder();
                sut.SetStr(TrueBool.True, "three");
                sut.RemoveStr(TrueBool.False);
                Assert.AreEqual(destination, sut.Build());
            }
            {
                var sut = destination.ToBuilder();
                sut.ClearStrs();
                sut.SetStr(TrueBool.False, "two");
                sut.SetStr(TrueBool.True, "one");
                Assert.AreEqual(source, sut.Build());
            }
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new StringMap(new Dataset() { { TrueBool.True, "val" } }),
                "792096dd64a5f6331bc75250035b43b57daf00a6d80277d6be3ff11ea4e633f2");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new StringMap(new Dataset()
                {
                    { TrueBool.True, "noughT" }
                }),
                @"{
                    Strs = (
                        True -> 'noughT'
                    )
                }"
            }, {
                new StringMap(new Dataset()
                {
                    { TrueBool.False, "Value1" },
                    { TrueBool.True, "Value2" },
                }),
                @"{
                    Strs = (
                        True -> 'Value2',
                        False -> 'Value1'
                    )
                }"
            }, {
                new StringMap(new Dataset()),
                @"{
                    Strs = ()
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new StringMap(new Dataset()
                {
                }),
                new StringMap(new Dataset() {
                    { TrueBool.FileNotFound, "44" }
                }),
                new StringMap(new Dataset() {
                    { TrueBool.True, "Foo" },
                    { TrueBool.False, "88" }
                })
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<StringMap>(@"{
                ""Strs"": {}
            }", @"{
                ""Strs"": {
                    ""True"": ""fortyfor""
                }
            }", @"{
                ""Strs"": {
                    ""False"": ""fortyfor"",
                    ""FileNotFound"": ""eightyate""
                }
            }");
        }
    }
}

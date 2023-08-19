using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using Dataset = System.Collections.Generic.Dictionary<string, string>;

namespace Pocotheosis.Tests
{
    [TestFixture]
    class StringMapTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0,
                new DictionaryPoco(new Dataset()).MappedStrings.Count);

            var data = new Dataset()
            {
                { "red", "ff0000" },
                { "green", "00ff00" }
            };

            var poco = new DictionaryPoco(data);
            Assert.AreEqual(2, poco.MappedStrings.Count);
            data.Clear(); // Ensures poco made a copy
            Assert.AreEqual("ff0000", poco.MappedStrings["red"]);
            Assert.AreEqual("00ff00", poco.MappedStrings["green"]);
        }

        [Test]
        public void Constructor_NullReference()
        {
            Assert.Throws<ArgumentNullException>(() => new DictionaryPoco(null));
        }

        [Test]
        public void Equality()
        {
            var data = new Dataset()
            {
                { "1", "one" },
                { "3", "three" }
            };
            var differentData = new Dataset()
            {
                { "1", "one" },
                { "3", "six" }
            };
            var longerData = new Dataset()
            {
                { "1", "one" },
                { "3", "three" },
                { "4", "four" }
            };
            var shorterData = new Dataset()
            {
                { "1", "one" }
            };

            Assert.AreEqual(new DictionaryPoco(data), new DictionaryPoco(data));
            Assert.AreNotEqual(new DictionaryPoco(data), new DictionaryPoco(differentData));
            Assert.AreNotEqual(new DictionaryPoco(data), new DictionaryPoco(longerData));
            Assert.AreNotEqual(new DictionaryPoco(data), new DictionaryPoco(shorterData));
        }

        [Test]
        public void Builder()
        {
            var source = new DictionaryPoco(new Dataset()
            {
                { "Alice", "one" },
                { "Bob", "two" }
            });
            var destination = new DictionaryPoco(new Dataset()
            {
                { "Charlie", "three" },
                { "Doug", null }
            });

            {
                var sut = source.ToBuilder();
                sut.RemoveMappedString("Alice");
                sut.SetMappedString("Charlie", "three");
                sut.SetMappedString("Doug", null);
                sut.RemoveMappedString("Bob");
                Assert.AreEqual(destination, sut.Build());
            }
            {
                var sut = destination.ToBuilder();
                sut.ClearMappedStrings();
                sut.SetMappedString("Bob", "two");
                sut.SetMappedString("Alice", "one");
                Assert.AreEqual(source, sut.Build());
            }
        }

        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new DictionaryPoco(new Dataset() { { "key", "val" }, { "nullval", null } }),
                "5f609b0170b3fcf8cd1e254e7b71d2a6e8ee5401cda7742a9c9c47ce82456331");
        }

        [Test]
        public void StringFormat()
        {
            PocoTest.StringFormat(new() { {
                new DictionaryPoco(new Dataset()
                {
                    { "Aleph", "noughT" }
                }),
                @"{
                    MappedStrings = (
                        'Aleph' -> 'noughT'
                    )
                }"
            }, {
                new DictionaryPoco(new Dataset()
                {
                    { "Key1", "Value1" },
                    { "Key2", "Value2" },
                    { "Key3", null },
                }),
                @"{
                    MappedStrings = (
                        'Key1' -> 'Value1',
                        'Key2' -> 'Value2',
                        'Key3' -> null
                    )
                }"
            }, {
                new DictionaryPoco(new Dataset()),
                @"{
                    MappedStrings = ()
                }"
            } });
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new DictionaryPoco(new Dataset() {
                }),
                new DictionaryPoco(new Dataset() {
                    { "fortyfor", "44" }
                }),
                new DictionaryPoco(new Dataset() {
                    { "fortyfor", null },
                    { "ateate", "88" }
                })
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<DictionaryPoco>(@"{
                ""MappedStrings"": {}
            }", @"{
                ""MappedStrings"": {
                    ""a"": ""1""
                }
            }", @"{
                ""MappedStrings"": {
                    ""a"": ""1"",
                    ""b"": ""2"",
                    ""c"": null
                }
            }");
        }
    }
}

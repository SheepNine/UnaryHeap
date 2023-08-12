using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Globalization;
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
        public void Checksum()
        {
            Assert.AreEqual("7a393bcc63828c5cfee3f708d59e44a2e36627545ea3b6c00e08c58b2638e686",
                new DictionaryPoco(new Dataset() { { "key", "val" } }).Checksum);
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
            Assert.AreNotEqual(null, new DictionaryPoco(data));
            Assert.AreEqual(new DictionaryPoco(data), new DictionaryPoco(data));
            Assert.AreNotEqual(new DictionaryPoco(data), new DictionaryPoco(differentData));
            Assert.AreNotEqual(new DictionaryPoco(data), new DictionaryPoco(longerData));
            Assert.AreNotEqual(new DictionaryPoco(data), new DictionaryPoco(shorterData));
        }

        [Test]
        public void StringFormat()
        {
            var data1 = new Dataset()
            {
                { "Aleph", "noughT" }
            };
            var data2 = new Dataset()
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" },
            };
            var data3 = new Dataset();

            Assert.AreEqual(
                "{\r\n\tMappedStrings = (\r\n\t\t'Aleph' -> 'noughT'\r\n\t)\r\n}",
                new DictionaryPoco(data1).ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(
                "{\r\n\tMappedStrings = (\r\n\t\t'Key1' -> 'Value1'," +
                "\r\n\t\t'Key2' -> 'Value2'\r\n\t)\r\n}",
                new DictionaryPoco(data2).ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(
                "{\r\n\tMappedStrings = ()\r\n}",
                new DictionaryPoco(data3).ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new DictionaryPoco(new Dataset() { }));
            TestUtils.TestRoundTrip(new DictionaryPoco(new Dataset() {
                { "fortyfor", "44" } }));
            TestUtils.TestRoundTrip(new DictionaryPoco(new Dataset() {
                { "fortyfor", "44" }, { "ateate", "88" } }));
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<DictionaryPoco>(
                @"{""MappedStrings"":{}}");
            TestUtils.TestJsonRoundTrip<DictionaryPoco>
                (@"{""MappedStrings"":{""a"":""1""}}");
            TestUtils.TestJsonRoundTrip<DictionaryPoco>(
                @"{""MappedStrings"":{""a"":""1"",""b"":""2""}}");
        }

        [Test]
        public void Builder()
        {
            var sut = new DictionaryPoco(new Dataset()
            {
                { "a", "alpha" },
                { "b", "beta" },
                { "c", "camma" }
            }).ToBuilder();

            sut.SetMappedString("d", "delta");
            sut.RemoveMappedString("b");
            Assert.AreEqual(3, sut.CountMappedStrings);
            Assert.False(sut.ContainsMappedStringKey("g"));
            Assert.True(sut.ContainsMappedStringKey("d"));
            Assert.AreEqual("acd", string.Join("", sut.MappedStringKeys));
            var built = sut.Build();
            Assert.AreEqual("alpha", built.MappedStrings["a"]);
            Assert.AreEqual("camma", built.MappedStrings["c"]);
            Assert.AreEqual("delta", built.MappedStrings["d"]);
        }
    }
}

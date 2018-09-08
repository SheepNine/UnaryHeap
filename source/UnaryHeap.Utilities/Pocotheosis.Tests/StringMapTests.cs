using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Collections.Generic;

namespace Pocotheosis.Tests
{
    [TestFixture]
    class StringMapTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(0,
                new DictionaryPoco(new Dictionary<string, string>()).MappedStrings.Count);

            var data = new Dictionary<string, string>()
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
            var data = new Dictionary<string, string>()
            {
                { "1", "one" },
                { "3", "three" }
            };
            var differentData = new Dictionary<string, string>()
            {
                { "1", "one" },
                { "3", "six" }
            };
            var longerData = new Dictionary<string, string>()
            {
                { "1", "one" },
                { "3", "three" },
                { "4", "four" }
            };
            var shorterData = new Dictionary<string, string>()
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
        [Ignore("NEEDS WORK")]
        public void StringFormat()
        {
            /*Assert.AreEqual("ByteArrayPoco\r\n\tOrrey: <empty>",
                new ByteArrayPoco(new byte[] { }).ToString());
            Assert.AreEqual("ByteArrayPoco\r\n\tOrrey: 44",
                new ByteArrayPoco(new byte[] { 44 }).ToString());
            Assert.AreEqual("ByteArrayPoco\r\n\tOrrey: 44, 88",
                new ByteArrayPoco(new byte[] { 44, 88 }).ToString());*/
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new DictionaryPoco(new Dictionary<string, string>() { }));
            TestUtils.TestRoundTrip(new DictionaryPoco(new Dictionary<string, string>() {
                { "fortyfor", "44" } }));
            TestUtils.TestRoundTrip(new DictionaryPoco(new Dictionary<string, string>() {
                { "fortyfor", "44" }, { "ateate", "88" } }));
        }

        [Test]
        public void Builder()
        {
            var sut = new DictionaryPoco(new Dictionary<string, string>()
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

using NUnit.Framework;
using Pocotheosis.Tests.Pocos;
using System;
using System.Collections.Generic;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class ClassDictionaryPocoTests
    {
        [Test]
        public void Constructor()
        {
            var data = new Dictionary<int, BoolPoco>()
            {
                { 3, new BoolPoco(true) },
                { 5, new BoolPoco(false) }
            };
            var sut = new ClassDictionaryPoco(data);
            data.Clear(); // Ensures that sut made a copy

            Assert.AreEqual(2, sut.Geese.Count);
            Assert.AreEqual(true, sut.Geese[3].Value);
            Assert.AreEqual(false, sut.Geese[5].Value);
        }

        [Test]
        [Ignore("TODO")]
        public void Constructor_NullReference()
        {
        }

        [Test]
        [Ignore("TODO")]
        public void Equality()
        {
        }

        [Test]
        public void StringFormat()
        {
            var sut = new ClassDictionaryPoco(new Dictionary<int, BoolPoco>()
            {
                { 3, new BoolPoco(true) },
                { 5, new BoolPoco(false) }
            });
            Assert.AreEqual("{\r\n\tGeese = (\r\n\t\t3 -> {\r\n\t\t\tValue = True\r\n\t\t},"
                + "\r\n\t\t5 -> {\r\n\t\t\tValue = False\r\n\t\t}\r\n\t)\r\n}", sut.ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new ClassDictionaryPoco(new Dictionary<int, BoolPoco>()
            {
                { -99, new BoolPoco(false) },
                { 7, new BoolPoco(false) },
                { 6, new BoolPoco(false) },
            }));
            TestUtils.TestRoundTrip(new ClassDictionaryPoco(new Dictionary<int, BoolPoco>()));
        }

        [Test]
        [Ignore("TODO")]
        public void Builder()
        {

        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<ClassDictionaryPoco>(
                @"{""Geese"":[]}");
            TestUtils.TestJsonRoundTrip<ClassDictionaryPoco>(
                @"{""Geese"":[{""k"":3,""v"":{""Value"":true}}]}");
            TestUtils.TestJsonRoundTrip<ClassDictionaryPoco>(
                @"{""Geese"":[{""k"":3,""v"":{""Value"":true}},"
                    + @"{""k"":5,""v"":{""Value"":false}}]}");
        }
    }
}

﻿using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class EnumPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(TestEnum.False, new EnumPoco(TestEnum.False).Albedo);
            Assert.AreEqual(TestEnum.FileNotFound, new EnumPoco(TestEnum.FileNotFound).Albedo);
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new EnumPoco(TestEnum.False));
            Assert.AreEqual(new EnumPoco(TestEnum.False),
                new EnumPoco(TestEnum.False));
            Assert.AreNotEqual(new EnumPoco(TestEnum.FileNotFound),
                new EnumPoco(TestEnum.False));
        }

        [Test]
        public void StringFormat()
        {
            Assert.AreEqual("{\r\n\t\"Albedo\": \"False\"\r\n}",
                new EnumPoco(TestEnum.False).ToString());
            Assert.AreEqual("{\r\n\t\"Albedo\": \"FileNotFound\"\r\n}",
                new EnumPoco(TestEnum.FileNotFound).ToString());
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new EnumPoco(TestEnum.False));
            TestUtils.TestRoundTrip(new EnumPoco(TestEnum.FileNotFound));
        }

        [Test]
        public void Builder()
        {
            var start = new EnumPoco(TestEnum.False);
            Assert.AreEqual(TestEnum.False, start.Albedo);
            var endBuilder = start.ToBuilder();
            Assert.AreEqual(TestEnum.False, endBuilder.Albedo);
            endBuilder.Albedo = TestEnum.FileNotFound;
            var end = endBuilder.Build();
            Assert.AreEqual(TestEnum.FileNotFound, end.Albedo);
        }
    }
}

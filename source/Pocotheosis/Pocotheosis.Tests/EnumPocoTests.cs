using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class EnumPocoTests
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(TrueBool.False, new EnumPoco(TrueBool.False).Albedo);
            Assert.AreEqual(TrueBool.FileNotFound, new EnumPoco(TrueBool.FileNotFound).Albedo);
        }

        [Test]
        public void Checksum()
        {
            Assert.AreEqual("beead77994cf573341ec17b58bbf7eb34d2711c993c1d976b128b3188dc1829a",
                new EnumPoco(TrueBool.FileNotFound).Checksum);
        }

        [Test]
        public void Equality()
        {
            Assert.AreNotEqual(null, new EnumPoco(TrueBool.False));
            Assert.AreEqual(new EnumPoco(TrueBool.False),
                new EnumPoco(TrueBool.False));
            Assert.AreNotEqual(new EnumPoco(TrueBool.FileNotFound),
                new EnumPoco(TrueBool.False));
        }

        [Test]
        public void StringFormat()
        {
            TestUtils.TestToString(new() { {
                new EnumPoco(TrueBool.False),
                @"{
                    Albedo = False
                }"
            }, {
                new EnumPoco(TrueBool.FileNotFound),
                @"{
                    Albedo = FileNotFound
                }"
            } });
        }

        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(
                new EnumPoco(TrueBool.False),
                new EnumPoco(TrueBool.FileNotFound)
            );
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<EnumPoco>(@"{
                ""Albedo"": ""True""
            }", @"{
                ""Albedo"": ""False""
            }", @"{
                ""Albedo"": ""FileNotFound""
            }");
        }

        [Test]
        public void Builder()
        {
            var start = new EnumPoco(TrueBool.False);
            Assert.AreEqual(TrueBool.False, start.Albedo);
            var endBuilder = start.ToBuilder();
            Assert.AreEqual(TrueBool.False, endBuilder.Albedo);
            endBuilder.Albedo = TrueBool.FileNotFound;
            var end = endBuilder.Build();
            Assert.AreEqual(TrueBool.FileNotFound, end.Albedo);
        }
    }
}

using NUnit.Framework;
using GeneratedTestPocos;

namespace Pocotheosis.Tests.Values
{
    internal class AllPrimitiveValuesTests : PocoTestFixture<AllPrimitiveValues>
    {
        public AllPrimitiveValuesTests()
        {
            AddSample(
                new AllPrimitiveValues(true,
                    ulong.MaxValue, uint.MaxValue, ushort.MaxValue, byte.MaxValue,
                    long.MaxValue, int.MaxValue, short.MaxValue, sbyte.MaxValue),
                "82b3562e08ff3148ffa7d8a9ce35e9989ab382b8471852b5b168f1075a9f9dc6",
                @"{
                    b = True
                    u8 = 18446744073709551615
                    u4 = 4294967295
                    u2 = 65535
                    u1 = 255
                    s8 = 9223372036854775807
                    s4 = 2147483647
                    s2 = 32767
                    s1 = 127
                }",
                @"{
                    ""b"": true,
                    ""u8"": ""18446744073709551615"",
                    ""u4"": 4294967295,
                    ""u2"": 65535,
                    ""u1"": 255,
                    ""s8"": 9223372036854775807,
                    ""s4"": 2147483647,
                    ""s2"": 32767,
                    ""s1"": 127
                }");
            AddSample(
                new AllPrimitiveValues(false,
                    ulong.MinValue, uint.MinValue, ushort.MinValue, byte.MinValue,
                    long.MinValue, int.MinValue, short.MinValue, sbyte.MinValue),
                "4d071a34b1e35e8c2e7bbae3e7b0950f5fa9ed7ee9c79dc7ed5793e623c4f883",
                @"{
                    b = False
                    u8 = 0
                    u4 = 0
                    u2 = 0
                    u1 = 0
                    s8 = -9223372036854775808
                    s4 = -2147483648
                    s2 = -32768
                    s1 = -128
                }",
                @"{
                    ""b"": false,
                    ""u8"": 0,
                    ""u4"": 0,
                    ""u2"": 0,
                    ""u1"": 0,
                    ""s8"": -9223372036854775808,
                    ""s4"": -2147483648,
                    ""s2"": -32768,
                    ""s1"": -128
                }");

            NoInvalidConstructions();
        }

        [Test]
        public override void Builder()
        {
            var sut = new AllPrimitiveValues(true, 1, 2, 3, 4, 5, 6, 7, 8).ToBuilder();
            Assert.IsTrue(sut.b);
            Assert.AreEqual(1, sut.u8);
            Assert.AreEqual(2, sut.u4);
            Assert.AreEqual(3, sut.u2);
            Assert.AreEqual(4, sut.u1);
            Assert.AreEqual(5, sut.s8);
            Assert.AreEqual(6, sut.s4);
            Assert.AreEqual(7, sut.s2);
            Assert.AreEqual(8, sut.s1);

            Assert.AreEqual(
                sut.Withb(false).Withu8(9).Withu4(10).Withu2(11).Withu1(12)
                    .Withs8(13).Withs4(14).Withs2(15).Withs1(16).Build(),
                new AllPrimitiveValues.Builder(false, 9, 10, 11, 12, 13, 14, 15, 16).Build());
        }

        [Test]
        public void JsonAcceptsStrings()
        {
            Assert.AreEqual(
                ReadFromJson(@"{
                    ""b"": true,
                    ""u8"": 8446744073709551615,
                    ""u4"": 4294967295,
                    ""u2"": 65535,
                    ""u1"": 255,
                    ""s8"": 9223372036854775807,
                    ""s4"": 2147483647,
                    ""s2"": 32767,
                    ""s1"": 127
                }", false),
                ReadFromJson(@"{
                    ""b"": true,
                    ""u8"": ""8446744073709551615"",
                    ""u4"": ""4294967295"",
                    ""u2"": ""65535"",
                    ""u1"": ""255"",
                    ""s8"": ""9223372036854775807"",
                    ""s4"": ""2147483647"",
                    ""s2"": ""32767"",
                    ""s1"": ""127""
                }", false));
        }
    }
}

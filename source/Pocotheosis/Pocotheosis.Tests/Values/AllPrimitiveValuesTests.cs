using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

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
                "ea482bd5ab01dc1005c33511db33235cf9f1e939a0953b50f29b632c0803c88d",
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
                "78e4bb6d6141e30141f2b024c6052a9ea6f5c067a2b905ebf8a93c0cb481cf0a",
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

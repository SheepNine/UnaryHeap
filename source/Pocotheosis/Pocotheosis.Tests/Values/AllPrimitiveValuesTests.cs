using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests.Values
{
    [TestFixture]
    public class AllPrimitiveValuesTests
    {
        [Test]
        public void Checksum()
        {
            PocoTest.Checksum(
                new AllPrimitiveValues(true,
                    ulong.MaxValue, uint.MaxValue, ushort.MaxValue, byte.MaxValue,
                    long.MaxValue, int.MaxValue, short.MaxValue, sbyte.MaxValue),
                "ea482bd5ab01dc1005c33511db33235cf9f1e939a0953b50f29b632c0803c88d");
        }

        [Test]
        public void Serialization()
        {
            PocoTest.Serialization(
                new AllPrimitiveValues(true,
                    ulong.MaxValue, uint.MaxValue, ushort.MaxValue, byte.MaxValue,
                    long.MaxValue, int.MaxValue, short.MaxValue, sbyte.MaxValue),
                new AllPrimitiveValues(false,
                    ulong.MinValue, uint.MinValue, ushort.MinValue, byte.MinValue,
                    long.MinValue, int.MinValue, short.MinValue, sbyte.MinValue),
                new AllPrimitiveValues(true, 0, 0, 0, 0, 0, 0, 0, 0)
            );
        }

        [Test]
        public void JsonSerialization()
        {
            PocoTest.JsonSerialization<AllPrimitiveValues>(@"{
                ""b"": true,
                ""u8"": ""18446744073709551615"",
                ""u4"": 4294967295,
                ""u2"": 65535,
                ""u1"": 255,
                ""s8"": 9223372036854775807,
                ""s4"": 2147483647,
                ""s2"": 32767,
                ""s1"": 127
            }", @"{
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
    }
}

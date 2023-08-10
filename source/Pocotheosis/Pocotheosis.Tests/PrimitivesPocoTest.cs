using NUnit.Framework;
using Pocotheosis.Tests.Pocos;

namespace Pocotheosis.Tests
{
    [TestFixture]
    public class PrimitivesPocoTest
    {
        [Test]
        public void RoundTrip()
        {
            TestUtils.TestRoundTrip(new PrimitivesPoco(
                ulong.MaxValue, uint.MaxValue, ushort.MaxValue, byte.MaxValue,
                long.MaxValue, int.MaxValue, short.MaxValue, sbyte.MaxValue));
            TestUtils.TestRoundTrip(new PrimitivesPoco(
                ulong.MinValue, uint.MinValue, ushort.MinValue, byte.MinValue,
                long.MinValue, int.MinValue, short.MinValue, sbyte.MinValue));
            TestUtils.TestRoundTrip(new PrimitivesPoco(
                0, 0, 0, 0, 0, 0, 0, 0));
        }

        [Test]
        public void JsonRoundTrip()
        {
            TestUtils.TestJsonRoundTrip<PrimitivesPoco>(
                @"{""u8"":""18446744073709551615"",""u4"":4294967295,""u2"":65535,""u1"":255," +
                @"""s8"":9223372036854775807,""s4"":2147483647,""s2"":32767,""s1"":127}");
            TestUtils.TestJsonRoundTrip<PrimitivesPoco>(
                @"{""u8"":0,""u4"":0,""u2"":0,""u1"":0," +
                @"""s8"":-9223372036854775808,""s4"":-2147483648,""s2"":-32768,""s1"":-128}");
        }
    }
}

using System;
using System.Linq;
using System.Numerics;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class RationalTests
    {
        [Theory]
        [InlineData(-5)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void Constructor_WholeNumber(int number)
        {
            var sut = new Rational(number);

            Assert.Equal((BigInteger)number, sut.Numerator);
            Assert.Equal((BigInteger)1, sut.Denominator);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(5)]
        public void Constructor_Harmonic(int denominator)
        {
            var sut = new Rational(1, denominator);

            Assert.Equal((BigInteger)Math.Sign(denominator), sut.Numerator);
            Assert.Equal(BigInteger.Abs(denominator), sut.Denominator);
        }

        [Fact]
        public void Constructor_ZeroDenominator()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>("denominator", () => { new Rational(1, 0); });
            Assert.StartsWith("Denominator cannot be zero.", ex.Message);
        }

        [Fact]
        public void Constructor_LowestTerms()
        {
            Constructor_LowestTerms_Case(006, 003, 02, 1);
            Constructor_LowestTerms_Case(-12, 008, -3, 2);
            Constructor_LowestTerms_Case(-21, -28, 03, 4);
            Constructor_LowestTerms_Case(088, -99, -8, 9);
        }

        void Constructor_LowestTerms_Case(BigInteger inputNumerator, BigInteger inputDenominator, BigInteger expectedNumerator, BigInteger expectedDenominator)
        {
            var sut = new Rational(inputNumerator, inputDenominator);

            Assert.Equal(expectedNumerator, sut.Numerator);
            Assert.Equal(expectedDenominator, sut.Denominator);
        }

        [Fact]
        public void ConversionOperators()
        {
            var suts = new Rational[] { (byte)4, (sbyte)4, (ushort)4, (short)4, (uint)4, (int)4, (ulong)4, (long)4, (BigInteger)4 };

            foreach (var sut in suts)
            {
                Assert.Equal((BigInteger)4, sut.Numerator);
                Assert.Equal((BigInteger)1, sut.Denominator);
            }
        }
    }
}

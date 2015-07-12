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

        [Fact]
        public void ComparisonAndEquality()
        {
            foreach (var i in Enumerable.Range(2, 21))
            {
                // --- Test against incorrect types ---

                var ex = Assert.Throws<ArgumentException>("obj", () => { new Rational(i).CompareTo("Hello"); });
                Assert.StartsWith("Object must be of type UnaryHeap.Utilities.Rational.", ex.Message);

                Assert.False(new Rational(i).Equals("Hello"));


                // --- Test against null ---

                Assert.False(null > new Rational(i));
                Assert.True(null <= new Rational(i));
                Assert.True(null < new Rational(i));
                Assert.False(null >= new Rational(i));
                Assert.True(null != new Rational(i));
                Assert.False(null == new Rational(i));

                Assert.True(new Rational(i) > null);
                Assert.False(new Rational(i) <= null);
                Assert.False(new Rational(i) < null);
                Assert.True(new Rational(i) >= null);
                Assert.True(new Rational(i) != null);
                Assert.False(new Rational(i) == null);
                Assert.False(new Rational(i).Equals((Rational)null));
                Assert.False(new Rational(i).Equals((object)null));
                Assert.Equal(1, new Rational(i).CompareTo((Rational)null));
                Assert.Equal(1, new Rational(i).CompareTo((object)null));


                // --- Test against inverse ---
                // --- NB: i >= 2 ---

                Assert.True(new Rational(i) > new Rational(1, i));
                Assert.False(new Rational(i) <= new Rational(1, i));
                Assert.False(new Rational(i) < new Rational(1, i));
                Assert.True(new Rational(i) >= new Rational(1, i));
                Assert.False(new Rational(i) == new Rational(1, i));
                Assert.True(new Rational(i) != new Rational(1, i));

                // --- Test against different number ---

                foreach (var j in Enumerable.Range(-10, 21))
                {
                    Assert.Equal(i > j, new Rational(i) > new Rational(j));
                    Assert.Equal(i <= j, new Rational(i) <= new Rational(j));
                    Assert.Equal(i < j, new Rational(i) < new Rational(j));
                    Assert.Equal(i >= j, new Rational(i) >= new Rational(j));
                    Assert.Equal(i != j, new Rational(i) != new Rational(j));
                    Assert.Equal(i == j, new Rational(i) == new Rational(j));

                    Assert.Equal(i > j, new Rational(i) > j);
                    Assert.Equal(i <= j, new Rational(i) <= j);
                    Assert.Equal(i < j, new Rational(i) < j);
                    Assert.Equal(i >= j, new Rational(i) >= j);
                    Assert.Equal(i != j, new Rational(i) != j);
                    Assert.Equal(i == j, new Rational(i) == j);

                    Assert.Equal(i > j, i > new Rational(j));
                    Assert.Equal(i <= j, i <= new Rational(j));
                    Assert.Equal(i < j, i < new Rational(j));
                    Assert.Equal(i >= j, i >= new Rational(j));
                    Assert.Equal(i != j, i != new Rational(j));
                    Assert.Equal(i == j, i == new Rational(j));

                    Assert.Equal(i.Equals(j), new Rational(i).Equals(new Rational(j)));
                    Assert.Equal(i.Equals(j), new Rational(i).Equals((object)new Rational(j)));
                    Assert.Equal(i.CompareTo(j), new Rational(i).CompareTo(new Rational(j)));
                    Assert.Equal(i.CompareTo(j), new Rational(i).CompareTo((object)new Rational(j)));
                }
            }
        }

        [Fact]
        public void Comparison_NullReferences()
        {
            Rational nullRational = null;
            Rational nullRational2 = null;

            Assert.False(nullRational > nullRational2);
            Assert.True(nullRational <= nullRational2);
            Assert.False(nullRational < nullRational2);
            Assert.True(nullRational >= nullRational2);
            Assert.False(nullRational != nullRational2);
            Assert.True(nullRational == nullRational2);
        }
    }
}

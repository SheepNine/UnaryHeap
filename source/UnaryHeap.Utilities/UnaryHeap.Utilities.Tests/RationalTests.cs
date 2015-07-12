using System;
using System.Collections.Generic;
using System.IO;
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

        [Theory]
        [MemberData("StringRepresentationData")]
        public void StringRepresentation(Rational input, string expected)
        {
            Assert.Equal(expected, input.ToString());
        }

        [Theory]
        [MemberData("StringRepresentationData")]
        public void Parse(Rational expected, string input)
        {
            Assert.Equal(expected, Rational.Parse(input));
        }

        public static IEnumerable<object[]> StringRepresentationData
        {
            get
            {
                return new[] {
                    new object[] { new Rational(-7, 5), "-7/5" },
                    new object[] { new Rational(-6, 5), "-6/5" },
                    new object[] { new Rational(-5, 5), "-1" },
                    new object[] { new Rational(-4, 5), "-4/5" },
                    new object[] { new Rational(-3, 5), "-3/5" },
                    new object[] { new Rational(-2, 5), "-2/5" },
                    new object[] { new Rational(-1, 5), "-1/5" },
                    new object[] { new Rational(0, 5), "0" },
                    new object[] { new Rational(1, 5), "1/5" },
                    new object[] { new Rational(2, 5), "2/5" },
                    new object[] { new Rational(3, 5), "3/5" },
                    new object[] { new Rational(4, 5), "4/5" },
                    new object[] { new Rational(5, 5), "1" },
                    new object[] { new Rational(6, 5), "6/5" },
                    new object[] { new Rational(7, 5), "7/5" }
                };
            }
        }

        [Theory]
        [MemberData("StringDecimalData")]
        public void ParseDecimal(Rational expected, string input)
        {
            Assert.Equal(expected, Rational.Parse(input));
        }

        public static IEnumerable<object[]> StringDecimalData
        {
            get
            {
                return new[] {
                    new object[] {new Rational(-1, 5), " -0.2" },
                    new object[] {new Rational(-3, 4), " -0.75 " },
                    new object[] {new Rational(-5, 8), "-0.625 " },
                    new object[] {new Rational(-16, 5), "-3.2" },
                    new object[] {new Rational(-15, 4), "-3.75" },
                    new object[] {new Rational(-29, 8), "-3.625" },
                    new object[] {new Rational(0), "0.0" },
                    new object[] {new Rational(9), " 9.0" },
                    new object[] {new Rational(-8), "-8.0 " },
                    new object[] {new Rational(-8), "-8.0 " },                    
                    new object[] {new Rational(100), "100" },
                    new object[] {new Rational(-600), "-600" },
                    new object[] {new Rational(0), "0" },
                    new object[] {new Rational(1, 5), "0.2" },
                    new object[] {new Rational(3, 4), "0.75" },
                    new object[] {new Rational(5, 8), "0.625" },
                    new object[] {new Rational(11, 5), " 2.2" },
                    new object[] {new Rational(11, 4), " 2.75 " },
                    new object[] {new Rational(21, 8), "2.625 " },
                };
            }
        }

        [Theory]
        [MemberData("InvalidlyFormattedStrings")]
        public void ParseInvalidData(string input)
        {
            var ex = Assert.Throws<FormatException>(() => { Rational.Parse(input); });
            Assert.StartsWith("Input string was not in a correct format.", ex.Message);
        }

        public static IEnumerable<object[]> InvalidlyFormattedStrings
        {
            get
            {
                return new[] {
                    new object[] { "" },
                    new object[] { " " },
                    new object[] { "-" },
                    new object[] { "-1/-3" },
                    new object[] { "1/-3" },
                    new object[] { "--1/3" },
                    new object[] { "2.-5" },
                    new object[] { "1/3/5" },
                    new object[] { ".25" },
                    new object[] { "7." },
                    new object[] { "1.2.3" },
                    new object[] { "1/0" },
                    new object[] { "2. 25" },
                    new object[] { "2 .25" },
                    new object[] { "1 /3" },
                    new object[] { "1/ 3" },
                };
            }
        }

        [Fact]
        public void ParseNull()
        {
            Assert.Throws<ArgumentNullException>("value", () => { Rational.Parse(null); });
        }

        [Theory]
        [MemberData("SerializationData")]
        public void Serialization(Rational value, byte[] expected)
        {
            using (var stream = new MemoryStream())
            {
                value.Serialize(stream);
                Assert.Equal(expected, stream.ToArray());
            }
        }

        [Theory]
        [MemberData("SerializationData")]
        public void Deserialization(Rational expected, byte[] value)
        {
            using (var stream = new MemoryStream(value))
            {
                Assert.Equal(expected, Rational.Deserialize(stream));
                Assert.True(stream.Position == stream.Length);
            }
        }

        public static IEnumerable<object[]> SerializationData
        {
            get
            {
                return new[] {
                    new object[] {new Rational(0), new byte[] {0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01} },
                    new object[] {new Rational(1), new byte[] {0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x01} },
                    new object[] {new Rational(-1), new byte[] {0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xFF, 0x01} },
                    new object[] {new Rational(255), new byte[] {0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x01} },
                    new object[] {new Rational(1, 255), new byte[] {0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0xFF, 0x00 } },
                    new object[] {new Rational(3351056, 3351057), new byte[] {0x03, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x10, 0x22, 0x33, 0x11, 0x22, 0x33 } },
                };
            }
        }

        [Fact]
        public void SerializationToNull()
        {
            Assert.Throws<ArgumentNullException>("output", () => { new Rational(0).Serialize(null); });
        }

        [Fact]
        public void DeserializationFromNull()
        {
            Assert.Throws<ArgumentNullException>("input", () => { Rational.Deserialize(null); });
        }

        [Fact]
        public void DeserializationZeroDenominator()
        {
            var ex = Assert.Throws<FormatException>(() =>
            {
                using (var stream = new MemoryStream(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00}))
                    Rational.Deserialize(stream);
            });

            Assert.Equal("Denominator corrupt.", ex.Message);
        }

        [Fact]
        public void DeserializationNegativeDenominator()
        {
            var ex = Assert.Throws<FormatException>(() =>
            {
                using (var stream = new MemoryStream(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0xFF }))
                    Rational.Deserialize(stream);
            });

            Assert.Equal("Denominator corrupt.", ex.Message);
        }

        [Fact]
        public void DeserializationInvalidNumeratorByteCount()
        {
            var ex = Assert.Throws<FormatException>(() =>
            {
                using (var stream = new MemoryStream(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20 }))
                    Rational.Deserialize(stream);
            });

            Assert.Equal("Numerator byte count corrupt.", ex.Message);
        }

        [Fact]
        public void DeserializationInvalidDenominatorByteCount()
        {
            var ex = Assert.Throws<FormatException>(() =>
            {
                using (var stream = new MemoryStream(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20 }))
                    Rational.Deserialize(stream);
            });

            Assert.Equal("Denominator byte count corrupt.", ex.Message);
        }
    }
}

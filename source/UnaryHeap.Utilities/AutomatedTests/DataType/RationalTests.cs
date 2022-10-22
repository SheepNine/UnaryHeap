using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class RationalTests
    {
        [Test]
        public void Constructor_WholeNumber()
        {
            foreach (int input in new[] { -5, -1, 0, 1, 5 })
                Constructor_WholeNumber(input);
        }

        void Constructor_WholeNumber(int number)
        {
            var sut = new Rational(number);

            Assert.AreEqual((BigInteger)number, sut.Numerator);
            Assert.AreEqual((BigInteger)1, sut.Denominator);
        }

        [Test]
        public void Constructor_Harmonic()
        {
            foreach (int input in new[] { -5, -1, 1, 5 })
                Constructor_Harmonic(input);
        }
        void Constructor_Harmonic(int denominator)
        {
            var sut = new Rational(1, denominator);

            Assert.AreEqual((BigInteger)Math.Sign(denominator), sut.Numerator);
            Assert.AreEqual(BigInteger.Abs(denominator), sut.Denominator);
        }

        [Test]
        public void Constructor_LowestTerms()
        {
            Constructor_LowestTerms_Case(006, 003, 02, 1);
            Constructor_LowestTerms_Case(-12, 008, -3, 2);
            Constructor_LowestTerms_Case(-21, -28, 03, 4);
            Constructor_LowestTerms_Case(088, -99, -8, 9);
        }

        void Constructor_LowestTerms_Case(
            BigInteger inputNumerator, BigInteger inputDenominator,
            BigInteger expectedNumerator, BigInteger expectedDenominator)
        {
            var sut = new Rational(inputNumerator, inputDenominator);

            Assert.AreEqual(expectedNumerator, sut.Numerator);
            Assert.AreEqual(expectedDenominator, sut.Denominator);
        }

        [Test]
        public void ConversionOperators()
        {
            var suts = new Rational[] {
                (byte)4, (sbyte)4, (ushort)4, (short)4,
                (uint)4, (int)4, (ulong)4, (long)4,
                (BigInteger)4
            };

            foreach (var sut in suts)
            {
                Assert.AreEqual((BigInteger)4, sut.Numerator);
                Assert.AreEqual((BigInteger)1, sut.Denominator);
            }
        }

        [Test]
        public void Constants()
        {
            Assert.AreEqual((BigInteger)0, Rational.Zero.Numerator);
            Assert.AreEqual((BigInteger)1, Rational.Zero.Denominator);

            Assert.AreEqual((BigInteger)1, Rational.One.Numerator);
            Assert.AreEqual((BigInteger)1, Rational.One.Denominator);

            Assert.AreEqual((BigInteger)(-1), Rational.MinusOne.Numerator);
            Assert.AreEqual((BigInteger)1, Rational.MinusOne.Denominator);
        }

        [Test]
        public void ComparisonAndEquality()
        {
            foreach (var i in Enumerable.Range(2, 21))
            {
                // --- Test against incorrect types ---

                Assert.That(
                    Assert.Throws<ArgumentException>(
                        () => { new Rational(i).CompareTo("Hello"); })
                    .Message.StartsWith(
                        "Object must be of type Rational."));

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
                Assert.AreEqual(1, new Rational(i).CompareTo((Rational)null));
                Assert.AreEqual(1, new Rational(i).CompareTo((object)null));


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
                    Assert.AreEqual(i > j, new Rational(i) > new Rational(j));
                    Assert.AreEqual(i <= j, new Rational(i) <= new Rational(j));
                    Assert.AreEqual(i < j, new Rational(i) < new Rational(j));
                    Assert.AreEqual(i >= j, new Rational(i) >= new Rational(j));
                    Assert.AreEqual(i != j, new Rational(i) != new Rational(j));
                    Assert.AreEqual(i == j, new Rational(i) == new Rational(j));

                    Assert.AreEqual(i > j, new Rational(i) > j);
                    Assert.AreEqual(i <= j, new Rational(i) <= j);
                    Assert.AreEqual(i < j, new Rational(i) < j);
                    Assert.AreEqual(i >= j, new Rational(i) >= j);
                    Assert.AreEqual(i != j, new Rational(i) != j);
                    Assert.AreEqual(i == j, new Rational(i) == j);

                    Assert.AreEqual(i > j, i > new Rational(j));
                    Assert.AreEqual(i <= j, i <= new Rational(j));
                    Assert.AreEqual(i < j, i < new Rational(j));
                    Assert.AreEqual(i >= j, i >= new Rational(j));
                    Assert.AreEqual(i != j, i != new Rational(j));
                    Assert.AreEqual(i == j, i == new Rational(j));

                    Assert.AreEqual(i.Equals(j), new Rational(i).Equals(new Rational(j)));
                    Assert.AreEqual(i.Equals(j), new Rational(i).Equals((object)new Rational(j)));
                    Assert.AreEqual(i.CompareTo(j),
                        new Rational(i).CompareTo(new Rational(j)));
                    Assert.AreEqual(i.CompareTo(j),
                        new Rational(i).CompareTo((object)new Rational(j)));
                }
            }
        }

        [Test]
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

        [Test, Sequential]
        public void StringRepresentation(
            [ValueSource("StringRepresentationResult")] Rational input,
            [ValueSource("StringRepresentationData")] string expected)
        {
            Assert.AreEqual(expected, input.ToString());
        }

        [Test, Sequential]
        public void Parse(
            [ValueSource("StringRepresentationData")] string input,
            [ValueSource("StringRepresentationResult")] Rational expected)
        {
            Assert.AreEqual(expected, Rational.Parse(input));
        }

        public static IEnumerable<string> StringRepresentationData
        {
            get
            {
                return new[] {
                    "-7/5",
                    "-6/5",
                    "-1",
                    "-4/5",
                    "-3/5",
                    "-2/5",
                    "-1/5",
                    "0",
                    "1/5",
                    "2/5",
                    "3/5",
                    "4/5",
                    "1",
                    "6/5",
                    "7/5"
                };
            }
        }

        public static IEnumerable<Rational> StringRepresentationResult
        {
            get
            {
                return new[] {
                    new Rational(-7, 5),
                    new Rational(-6, 5),
                    new Rational(-5, 5),
                    new Rational(-4, 5),
                    new Rational(-3, 5),
                    new Rational(-2, 5),
                    new Rational(-1, 5),
                    new Rational(0, 5),
                    new Rational(1, 5),
                    new Rational(2, 5),
                    new Rational(3, 5),
                    new Rational(4, 5),
                    new Rational(5, 5),
                    new Rational(6, 5),
                    new Rational(7, 5),
                };
            }
        }

        [Test, Sequential]
        public void ParseDecimal(
            [ValueSource("StringDecimalData")] string input,
            [ValueSource("StringDecimalResult")] Rational expected)
        {
            Assert.AreEqual(expected, Rational.Parse(input));
        }

        public static IEnumerable<string> StringDecimalData
        {
            get
            {
                return new[] {
                    " -0.2",
                    " -0.75 ",
                    "-0.625 ",
                    "-3.2",
                    "-3.75",
                    "-3.625",
                    "0.0",
                    " 9.0",
                    "-8.0 ",
                    "-8.0 ",
                    "100",
                    "-600",
                    "0",
                    "0.2",
                    "0.75",
                    "0.625",
                    " 2.2",
                    " 2.75 ",
                    "2.625 ",
                };
            }
        }

        public static IEnumerable<Rational> StringDecimalResult
        {
            get
            {
                return new[] {
                    new Rational(-1, 5),
                    new Rational(-3, 4),
                    new Rational(-5, 8),
                    new Rational(-16, 5),
                    new Rational(-15, 4),
                    new Rational(-29, 8),
                    new Rational(0),
                    new Rational(9),
                    new Rational(-8),
                    new Rational(-8),
                    new Rational(100),
                    new Rational(-600),
                    new Rational(0),
                    new Rational(1, 5),
                    new Rational(3, 4),
                    new Rational(5, 8),
                    new Rational(11, 5),
                    new Rational(11, 4),
                    new Rational(21, 8),
                };
            }
        }

        [Test]
        public void ParseInvalidData([ValueSource("InvalidlyFormattedStrings")] string input)
        {
            Assert.That(
                Assert.Throws<FormatException>(
                    () => { Rational.Parse(input); })
                .Message.StartsWith(
                    "Input string was not in a correct format."));
        }

        public static IEnumerable<string> InvalidlyFormattedStrings
        {
            get
            {
                return new[] {
                    "",
                    " ",
                    "-",
                    "-1/-3",
                    "1/-3",
                    "--1/3",
                    "2.-5",
                    "1/3/5",
                    ".25",
                    "7.",
                    "1.2.3",
                    "1/0",
                    "2. 25",
                    "2 .25",
                    "1 /3",
                    "1/ 3",
                };
            }
        }

        [Test, Sequential]
        public void Serialization(
            [ValueSource("SerializationResult")] Rational value,
            [ValueSource("SerializationData")] byte[] expected)
        {
            using (var stream = new MemoryStream())
            {
                value.Serialize(stream);
                Assert.AreEqual(expected, stream.ToArray());
            }
        }

        [Test, Sequential]
        public void Deserialization(
            [ValueSource("SerializationData")] byte[] value,
            [ValueSource("SerializationResult")] Rational expected)
        {
            using (var stream = new MemoryStream(value))
            {
                Assert.AreEqual(expected, Rational.Deserialize(stream));
                Assert.True(stream.Position == stream.Length);
            }
        }

        public static IEnumerable<byte[]> SerializationData
        {
            get
            {
                return new[] {
                    new byte[] {
                        0x01, 0x00, 0x00, 0x00,
                        0x01, 0x00, 0x00, 0x00,
                        0x00,
                        0x01
                    },
                    new byte[] {
                        0x01, 0x00, 0x00, 0x00,
                        0x01, 0x00, 0x00, 0x00,
                        0x01, 0x01
                    },
                    new byte[] {
                        0x01, 0x00, 0x00, 0x00,
                        0x01, 0x00, 0x00, 0x00,
                        0xFF, 0x01
                    },
                    new byte[] {
                        0x02, 0x00, 0x00, 0x00,
                        0x01, 0x00, 0x00, 0x00,
                        0xFF, 0x00,
                        0x01
                    },
                    new byte[] {
                        0x01, 0x00, 0x00, 0x00,
                        0x02, 0x00, 0x00, 0x00,
                        0x01,
                        0xFF, 0x00
                    } ,
                    new byte[] {
                        0x03, 0x00, 0x00, 0x00,
                        0x03, 0x00, 0x00, 0x00,
                        0x10, 0x22, 0x33,
                        0x11, 0x22, 0x33
                    }
                };
            }
        }

        public static IEnumerable<Rational> SerializationResult
        {
            get
            {
                return new[] {
                    new Rational(0),
                    new Rational(1),
                    new Rational(-1),
                    new Rational(255),
                    new Rational(1, 255),
                    new Rational(3351056, 3351057),
                };
            }
        }

        [Test]
        public void DeserializationZeroDenominator()
        {
            var data = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0x00 };
            var ex = Assert.Throws<FormatException>(() =>
            {
                using (var stream = new MemoryStream(data))
                    Rational.Deserialize(stream);
            });

            Assert.AreEqual("Denominator corrupt.", ex.Message);
        }

        [Test]
        public void DeserializationNegativeDenominator()
        {
            var data = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20, 0xFF };
            var ex = Assert.Throws<FormatException>(() =>
            {
                using (var stream = new MemoryStream(data))
                    Rational.Deserialize(stream);
            });

            Assert.AreEqual("Denominator corrupt.", ex.Message);
        }

        [Test]
        public void DeserializationInvalidNumeratorByteCount()
        {
            var data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x20 };
            var ex = Assert.Throws<FormatException>(() =>
            {
                using (var stream = new MemoryStream(data))
                    Rational.Deserialize(stream);
            });

            Assert.AreEqual("Numerator byte count corrupt.", ex.Message);
        }

        [Test]
        public void DeserializationInvalidDenominatorByteCount()
        {
            var data = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20 };
            var ex = Assert.Throws<FormatException>(() =>
            {
                using (var stream = new MemoryStream(data))
                    Rational.Deserialize(stream);
            });

            Assert.AreEqual("Denominator byte count corrupt.", ex.Message);
        }

        [Test, Sequential]
        public void Properties(
            [ValueSource("PropertiesInput")] Rational input,
            [ValueSource("PropertiesSign")] int expectedSign,
            [ValueSource("PropertiesFloor")] Rational expectedFloor,
            [ValueSource("PropertiesRounded")] Rational expectedRounded,
            [ValueSource("PropertiesCeiling")] Rational expectedCeiling,
            [ValueSource("PropertiesAbsolute")] Rational expectedAbsoluteValue,
            [ValueSource("PropertiesSquared")] Rational expectedSquared,
            [ValueSource("PropertiesInverse")] Rational expectedInverse)
        {
            Assert.AreEqual(expectedRounded, input.Rounded);
            Assert.AreEqual(expectedFloor, input.Floor);
            Assert.AreEqual(expectedCeiling, input.Ceiling);
            Assert.AreEqual(expectedAbsoluteValue, input.AbsoluteValue);
            Assert.AreEqual(expectedSquared, input.Squared);
            Assert.AreEqual(expectedSign, input.Sign);

            if (null == expectedInverse)
                Assert.Throws<InvalidOperationException>(() => { var a = input.Inverse; });
            else
                Assert.AreEqual(expectedInverse, input.Inverse);
        }

        public static IEnumerable<Rational> PropertiesInput
        {
            get
            {
                return new[] {
                        new Rational(-9, 4),
                        new Rational(-8, 4),
                        new Rational(-7, 4),
                        new Rational(-6, 4),
                        new Rational(-5, 4),
                        new Rational(-4, 4),
                        new Rational(-3, 4),
                        new Rational(-2, 4),
                        new Rational(-1, 4),
                        new Rational(00, 4),
                        new Rational(01, 4),
                        new Rational(02, 4),
                        new Rational(03, 4),
                        new Rational(04, 4),
                        new Rational(05, 4),
                        new Rational(06, 4),
                        new Rational(07, 4),
                        new Rational(08, 4),
                        new Rational(09, 4),
                };
            }
        }

        public static IEnumerable<int> PropertiesSign
        {
            get
            {
                return new[] {
                        -1,
                        -1,
                        -1,
                        -1,
                        -1,
                        -1,
                        -1,
                        -1,
                        -1,
                        0,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                };
            }
        }

        public static IEnumerable<Rational> PropertiesFloor
        {
            get
            {
                return new[] {
                        new Rational(-3),
                        new Rational(-2),
                        new Rational(-2),
                        new Rational(-2),
                        new Rational(-2),
                        new Rational(-1),
                        new Rational(-1),
                        new Rational(-1),
                        new Rational(-1),
                        new Rational(00),
                        new Rational(00),
                        new Rational(00),
                        new Rational(00),
                        new Rational(01),
                        new Rational(01),
                        new Rational(01),
                        new Rational(01),
                        new Rational(02),
                        new Rational(02),
                };
            }
        }

        public static IEnumerable<Rational> PropertiesRounded
        {
            get
            {
                return new[] {
                        new Rational(-2),
                        new Rational(-2),
                        new Rational(-2),
                        new Rational(-2),
                        new Rational(-1),
                        new Rational(-1),
                        new Rational(-1),
                        new Rational(00),
                        new Rational(00),
                        new Rational(00),
                        new Rational(00),
                        new Rational(00),
                        new Rational(01),
                        new Rational(01),
                        new Rational(01),
                        new Rational(02),
                        new Rational(02),
                        new Rational(02),
                        new Rational(02),
                };
            }
        }

        public static IEnumerable<Rational> PropertiesCeiling
        {
            get
            {
                return new[] {
                        new Rational(-2),
                        new Rational(-2),
                        new Rational(-1),
                        new Rational(-1),
                        new Rational(-1),
                        new Rational(-1),
                        new Rational(00),
                        new Rational(00),
                        new Rational(00),
                        new Rational(00),
                        new Rational(01),
                        new Rational(01),
                        new Rational(01),
                        new Rational(01),
                        new Rational(02),
                        new Rational(02),
                        new Rational(02),
                        new Rational(02),
                        new Rational(03),
                };
            }
        }

        public static IEnumerable<Rational> PropertiesAbsolute
        {
            get
            {
                return new[] {
                        new Rational(09, 4),
                        new Rational(08, 4),
                        new Rational(07, 4),
                        new Rational(06, 4),
                        new Rational(05, 4),
                        new Rational(04, 4),
                        new Rational(03, 4),
                        new Rational(02, 4),
                        new Rational(01, 4),
                        new Rational(00, 4),
                        new Rational(01, 4),
                        new Rational(02, 4),
                        new Rational(03, 4),
                        new Rational(04, 4),
                        new Rational(05, 4),
                        new Rational(06, 4),
                        new Rational(07, 4),
                        new Rational(08, 4),
                        new Rational(09, 4),
                };
            }
        }

        public static IEnumerable<Rational> PropertiesSquared
        {
            get
            {
                return new[] {
                        new Rational(81, 16),
                        new Rational(64, 16),
                        new Rational(49, 16),
                        new Rational(36, 16),
                        new Rational(25, 16),
                        new Rational(16, 16),
                        new Rational(09, 16),
                        new Rational(04, 16),
                        new Rational(01, 16),
                        new Rational(00, 16),
                        new Rational(01, 16),
                        new Rational(04, 16),
                        new Rational(09, 16),
                        new Rational(16, 16),
                        new Rational(25, 16),
                        new Rational(36, 16),
                        new Rational(49, 16),
                        new Rational(64, 16),
                        new Rational(81, 16),
                };
            }
        }

        public static IEnumerable<Rational> PropertiesInverse
        {
            get
            {
                return new[] {
                        new Rational(4, -9),
                        new Rational(4, -8),
                        new Rational(4, -7),
                        new Rational(4, -6),
                        new Rational(4, -5),
                        new Rational(4, -4),
                        new Rational(4, -3),
                        new Rational(4, -2),
                        new Rational(4, -1),
                        null,
                        new Rational(4, 01),
                        new Rational(4, 02),
                        new Rational(4, 03),
                        new Rational(4, 04),
                        new Rational(4, 05),
                        new Rational(4, 06),
                        new Rational(4, 07),
                        new Rational(4, 08),
                        new Rational(4, 09),
                };
            }
        }

        [Test]
        public void HashCode()
        {
            var data = TwentyFiveHundredRationals();

            foreach (var i in data)
                foreach (var j in data)
                {
                    if (i == j)
                    {
                        // --- This is the requirement for a hash code to be correct ---
                        Assert.True(i.GetHashCode() == j.GetHashCode());
                    }
                    else
                    {
                        // --- This is not required for a hash code to be correct,
                        // --- but the more often it is correct, the less often hash
                        // --- collisions occur.
                        Assert.False(i.GetHashCode() == j.GetHashCode());
                    }
                }
        }

        [Test]
        public void AdditionWithZero()
        {
            var data = TwentyFiveHundredRationals();

            foreach (var datum in data)
            {
                Assert.AreEqual(datum, datum + Rational.Zero);
                Assert.AreEqual(datum, Rational.Zero + datum);
            }
        }

        [Test]
        public void SubtractionWithZero()
        {
            var data = TwentyFiveHundredRationals();

            foreach (var datum in data)
            {
                Assert.AreEqual(datum, datum - Rational.Zero);
                Assert.AreEqual(-datum, Rational.Zero - datum);
                Assert.AreEqual(Rational.Zero, datum - datum);
            }
        }

        [Test]
        public void AdditionSubtraction()
        {
            var data = OneHundredRationals();

            foreach (var a in data)
                foreach (var b in data)
                {
                    var c = a + b;

                    Assert.AreEqual(c, b + a);
                    Assert.AreEqual(a, c - b);
                    Assert.AreEqual(b, c - a);
                    Assert.AreEqual(-a, b - c);
                    Assert.AreEqual(-b, a - c);
                }
        }

        [Test]
        public void MultiplicationWithZeroAndOne()
        {
            var data = TwentyFiveHundredRationals();

            foreach (var datum in data)
            {
                Assert.AreEqual(Rational.Zero, datum * Rational.Zero);
                Assert.AreEqual(Rational.Zero, Rational.Zero * datum);

                Assert.AreEqual(datum, datum * Rational.One);
                Assert.AreEqual(datum, Rational.One * datum);

                Assert.AreEqual(-datum, datum * Rational.MinusOne);
                Assert.AreEqual(-datum, Rational.MinusOne * datum);
            }
        }

        [Test]
        public void DivisionWithZeroAndOne()
        {
            var data = TwentyFiveHundredRationals();

            foreach (var datum in data)
            {
                Assert.AreEqual(datum, datum / Rational.One);

                if (0 != datum.Numerator)
                {
                    Assert.AreEqual(Rational.Zero, Rational.Zero / datum);
                    Assert.AreEqual(datum.Inverse, Rational.One / datum);
                }
            }
        }

        [Test]
        public void DivisionByZero()
        {
            Assert.Throws<DivideByZeroException>(() => { var a = Rational.One / Rational.Zero; });
        }

        [Test]
        public void MultiplicationAndDivision()
        {
            var data = OneHundredRationals();

            foreach (var a in data.Where(r => false == r.Numerator.IsZero))
                foreach (var b in data.Where(r => false == r.Numerator.IsZero))
                {
                    var c = a * b;

                    Assert.AreEqual(c, b * a);
                    Assert.AreEqual(a, c / b);
                    Assert.AreEqual(b, c / a);
                    Assert.AreEqual(a.Inverse, b / c);
                    Assert.AreEqual(b.Inverse, a / c);
                }
        }

        [Test]
        public void MinMax()
        {
            var data = TwentyFiveHundredRationals();
            Array.Sort(data);

            for (int i = 0; i < data.Length; i++)
                for (int j = i; j < data.Length; j++)
                {
                    Assert.AreEqual(data[i], Rational.Min(data[i], data[j]));
                    Assert.AreEqual(data[j], Rational.Max(data[i], data[j]));
                }
        }

        static Rational[] OneHundredRationals()
        {
            return Enumerable.Range(-5, 11).Where(denom => denom != 0).SelectMany(denom =>
                Enumerable.Range(-5, 11).Select(num =>
                    new Rational(5 * num, 3 * denom))).ToArray();
        }

        static Rational[] TwentyFiveHundredRationals()
        {
            return Enumerable.Range(-25, 51).Where(denom => denom != 0).SelectMany(denom =>
                Enumerable.Range(-25, 51).Select(num =>
                    new Rational(num, denom))).ToArray();
        }

        [Test]
        public void DoubleConversionOfIntegralValues()
        {
            foreach (var i in Enumerable.Range(-2000, 4001))
                Assert.AreEqual((double)i, (double)new Rational(i));
        }

        [Test]
        public void DoubleConversionOfFractionalValues()
        {
            foreach (var denominator in Enumerable.Range(2, 254))
                foreach (var numerator in Enumerable.Range(1, denominator - 1))
                {
                    Assert.AreEqual((double)numerator / (double)denominator,
                        (double)new Rational(numerator, denominator));
                    Assert.AreEqual((double)-numerator / (double)denominator,
                        (double)new Rational(-numerator, denominator));
                }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Rational Null = null;

            Assert.Throws<ArgumentNullException>(
                () => { var a = Null + Rational.One; });
            Assert.Throws<ArgumentNullException>(
                () => { var a = Rational.One + Null; });
            Assert.Throws<ArgumentNullException>(
                () => { var a = Null - Rational.One; });
            Assert.Throws<ArgumentNullException>(
                () => { var a = Rational.One - Null; });
            Assert.Throws<ArgumentNullException>(
                () => { var a = Null * Rational.One; });
            Assert.Throws<ArgumentNullException>(
                () => { var a = Rational.One * Null; });
            Assert.Throws<ArgumentNullException>(
                () => { var a = Null / Rational.One; });
            Assert.Throws<ArgumentNullException>(
                () => { var a = Rational.One / Null; });
            Assert.Throws<ArgumentNullException>(
                () => { var a = -Null; });
            Assert.Throws<ArgumentNullException>(
                () => { Rational.Min(Null, Rational.One); });
            Assert.Throws<ArgumentNullException>(
                () => { Rational.Min(Rational.One, Null); });
            Assert.Throws<ArgumentNullException>(
                () => { Rational.Max(Null, Rational.One); });
            Assert.Throws<ArgumentNullException>(
                () => { Rational.Max(Rational.One, Null); });
            Assert.Throws<ArgumentNullException>(
                () => { new Rational(0).Serialize(null); });
            Assert.Throws<ArgumentNullException>(
                () => { Rational.Deserialize(null); });
            Assert.Throws<ArgumentNullException>(
                () => { Rational.Parse(null); });
            Assert.Throws<ArgumentNullException>(
                () => { var a = (double)Null; });
            Assert.That(
                Assert.Throws<ArgumentOutOfRangeException>(
                    () => { new Rational(1, 0); })
                .Message.StartsWith(
                    "Denominator cannot be zero."));
        }
    }
}
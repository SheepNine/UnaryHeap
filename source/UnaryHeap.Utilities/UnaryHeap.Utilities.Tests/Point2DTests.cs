using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class Point2DTests
    {
        [Test]
        public void Constructor()
        {
            var sut = new Point2D(1, 3);

            Assert.AreEqual((Rational)1, sut.X);
            Assert.AreEqual((Rational)3, sut.Y);
        }

        [Test]
        public void Origin()
        {
            var sut = Point2D.Origin;

            Assert.AreEqual((Rational)0, sut.X);
            Assert.AreEqual((Rational)0, sut.Y);
        }

        [Test]
        public void Equality()
        {
            var sut = new Point2D(1, 2);

            Assert.True(sut.Equals(new Point2D(1, 2)));
            Assert.False(sut.Equals(new Point2D(1, 1)));
            Assert.False(sut.Equals(new Point2D(2, 2)));
            Assert.False(sut.Equals(null));
            Assert.False(sut.Equals("a string"));
        }

        [Test, Sequential]
        public void ToString(
            [ValueSource("StringFormatResult")]Point2D value,
            [ValueSource("StringFormatData")]string expected)
        {
            Assert.AreEqual(expected, value.ToString());
        }

        [Test, Sequential]
        public void Parse(
            [ValueSource("StringFormatData")]string value,
            [ValueSource("StringFormatResult")]Point2D expected)
        {
            Assert.AreEqual(expected, Point2D.Parse(value));
        }

        public static IEnumerable<string> StringFormatData
        {
            get
            {
                return new[] {
                    "1/2,-3/4",
                    "-9,-2"
                };
            }
        }

        public static IEnumerable<Point2D> StringFormatResult
        {
            get
            {
                return new[] {
                    new Point2D(new Rational(1, 2), new Rational(-3, 4)),
                    new Point2D(new Rational(-9), new Rational(-2)),
                };
            }
        }

        [Test]
        public void ParseDecimalRepresentation()
        {
            Assert.AreEqual(new Point2D(5, new Rational(-3, 7)), Point2D.Parse("5.000,-3/7"));
        }

        [Test]
        public void ParseInvalidData([ValueSource("InvalidlyFormattedStrings")]string input)
        {
            Assert.That(
                Assert.Throws<FormatException>(
                    () => { Point2D.Parse(input); })
                .Message.StartsWith(
                    "Input string was not in a correct format."));
        }

        public static IEnumerable<string> InvalidlyFormattedStrings
        {
            get
            {
                return new[] {
                    "",
                    ",",
                    "1,",
                    ",3",                    
                    "2 ,2",
                    "2, 2",
                };
            }
        }

        [Test, Sequential]
        public void Serialize(
            [ValueSource("BinaryFormatResult")]Point2D value,
            [ValueSource("BinaryFormatData")]byte[] expected)
        {
            using (var buffer = new MemoryStream())
            {
                value.Serialize(buffer);
                Assert.AreEqual(expected, buffer.ToArray());
            }
        }

        [Test, Sequential]
        public void Deserialize(
            [ValueSource("BinaryFormatData")]byte[] value,
            [ValueSource("BinaryFormatResult")]Point2D expected)
        {
            using (var buffer = new MemoryStream(value))
            {
                Assert.AreEqual(expected, Point2D.Deserialize(buffer));
                Assert.AreEqual(buffer.Length, buffer.Position);
            }
        }

        public static IEnumerable<byte[]> BinaryFormatData
        {
            get
            {
                return new[] {
                    new byte[] {
                        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01,
                        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x03, 0x01, },
                    new byte[] {
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x2C, 0x01, 0x01,
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x01, },
                };
            }
        }
        public static IEnumerable<Point2D> BinaryFormatResult
        {
            get
            {
                return new[] {
                    new Point2D(0, 3),
                    new Point2D(300, 200),
                };
            }
        }

        [Test]
        public void HashCode()
        {
            var data = SomeRandomPoints();

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

        private List<Point2D> SomeRandomPoints()
        {
            var random = new Random(19830630);
            var result = new List<Point2D>();

            foreach (var i in Enumerable.Range(0, 500))
                result.Add(new Point2D(
                    new Rational(random.Next(), random.Next()),
                    new Rational(random.Next(), random.Next())));

            return result;
        }

        [Test]
        public void Quadrance()
        {
            Assert.AreEqual(Rational.Zero,
                Point2D.Quadrance(Point2D.Origin, Point2D.Origin));

            for (int x = 0; x < 10; x++)
            {
                Assert.AreEqual(new Rational(x * x),
                    Point2D.Quadrance(new Point2D(0, 0), new Point2D(x, 0)));

                for (int y = 0; y < 10; y++)
                {
                    Assert.AreEqual(new Rational(25),
                        Point2D.Quadrance(new Point2D(x, y), new Point2D(x + 3, y + 4)));
                }
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { new Point2D(null, 1); });
            Assert.Throws<ArgumentNullException>(
                () => { new Point2D(1, null); });
            Assert.Throws<ArgumentNullException>(
                () => { Point2D.Deserialize(null); });
            Assert.Throws<ArgumentNullException>(
                () => { Point2D.Parse(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new Point2D(1, 1).Serialize(null); });

            Assert.Throws<ArgumentNullException>(
                () => { Point2D.Quadrance(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { Point2D.Quadrance(Point2D.Origin, null); });
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D3;
using NUnit.Framework;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class Point3DTests
    {
        [Test]
        public void Constructor()
        {
            var sut = new Point3D(1, 3, 2);

            Assert.AreEqual((Rational)1, sut.X);
            Assert.AreEqual((Rational)3, sut.Y);
            Assert.AreEqual((Rational)2, sut.Z);
        }

        [Test]
        public void Origin()
        {
            var sut = Point3D.Origin;

            Assert.AreEqual((Rational)0, sut.X);
            Assert.AreEqual((Rational)0, sut.Y);
            Assert.AreEqual((Rational)0, sut.Z);
        }

        [Test]
        public void Equality()
        {
            var sut = new Point3D(1, 2, 3);

            Assert.True(sut.Equals(new Point3D(1, 2, 3)));
            Assert.False(sut.Equals(new Point3D(1, 1, 3)));
            Assert.False(sut.Equals(new Point3D(2, 2, 3)));
            Assert.False(sut.Equals(new Point3D(1, 2, 4)));
            Assert.False(sut.Equals(null));
            Assert.False(sut.Equals("a string"));
        }

        [Test, Sequential]
        public void ToString(
            [ValueSource("StringFormatResult")]Point3D value,
            [ValueSource("StringFormatData")]string expected)
        {
            Assert.AreEqual(expected, value.ToString());
        }

        [Test, Sequential]
        public void Parse(
            [ValueSource("StringFormatData")]string value,
            [ValueSource("StringFormatResult")]Point3D expected)
        {
            Assert.AreEqual(expected, Point3D.Parse(value));
        }

        public static IEnumerable<string> StringFormatData
        {
            get
            {
                return new[] {
                    "1/2,-3/4,5/6",
                    "-9,-2,6"
                };
            }
        }

        public static IEnumerable<Point3D> StringFormatResult
        {
            get
            {
                return new[] {
                    new Point3D(new Rational(1, 2), new Rational(-3, 4), new Rational(5,6)),
                    new Point3D(new Rational(-9), new Rational(-2), new Rational(6)),
                };
            }
        }

        [Test]
        public void ParseDecimalRepresentation()
        {
            Assert.AreEqual(new Point3D(5, new Rational(-3, 7), 1),
                Point3D.Parse("5.000,-3/7,1.0"));
        }

        [Test]
        public void ParseInvalidData([ValueSource("InvalidlyFormattedStrings")]string input)
        {
            Assert.That(
                Assert.Throws<FormatException>(
                    () => { Point3D.Parse(input); })
                .Message.StartsWith(
                    "Input string was not in a correct format."));
        }

        public static IEnumerable<string> InvalidlyFormattedStrings
        {
            get
            {
                return new[]
                {
                    "",
                    "1,2,",
                    "1,,3",
                    "1,,",
                    ",2,3",
                    ",2,",
                    ",,3",
                    ",,",                    
                    "1 ,2,3",
                    "1, 2,3",
                    "1,2 ,3",
                    "1,2, 3",
                };
            }
        }

        [Test, Sequential]
        public void Serialize(
            [ValueSource("BinaryFormatResult")]Point3D value,
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
            [ValueSource("BinaryFormatResult")]Point3D expected)
        {
            using (var buffer = new MemoryStream(value))
            {
                Assert.AreEqual(expected, Point3D.Deserialize(buffer));
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
                        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x03, 0x01,
                        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x01, },
                    new byte[] {
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x2C, 0x01, 0x01,
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x01,
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC9, 0x00, 0x01, },
                };
            }
        }

        public static IEnumerable<Point3D> BinaryFormatResult
        {
            get
            {
                return new[] {
                    new Point3D(0, 3, 4),
                    new Point3D(300, 200, 201),
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
                    if (i == j || i.Equals(j))
                    {
                        // --- This is the requirement for a hash code to be correct ---
                        Assert.True(i.GetHashCode() == j.GetHashCode());
                    }
                    else
                    {
                        // --- This is not required for a hash code to be correct,
                        // --- but the more often it is correct, the less often hash
                        // --- collisions occur.
                        Assert.False(i.GetHashCode() == j.GetHashCode(),
                            string.Format("Hash collision between {0} and {1}", i, j));
                    }
                }
        }

        List<Point3D> SomeRandomPoints()
        {
            var random = new Random(19830630);
            var result = new List<Point3D>();

            foreach (var i in Enumerable.Range(0, 500))
                result.Add(new Point3D(
                    new Rational(random.Next(), random.Next()),
                    new Rational(random.Next(), random.Next()),
                    new Rational(random.Next(), random.Next())));

            return result;
        }

        [Test]
        public void Quadrance()
        {
            Assert.AreEqual(Rational.Zero,
                Point3D.Quadrance(Point3D.Origin, Point3D.Origin));

            for (int x = 0; x < 10; x++)
            {
                Assert.AreEqual(new Rational(x * x),
                    Point3D.Quadrance(new Point3D(0, 0, 0), new Point3D(x, 0, 0)));
                Assert.AreEqual(new Rational(x * x),
                    Point3D.Quadrance(new Point3D(0, 0, 0), new Point3D(0, x, 0)));
                Assert.AreEqual(new Rational(x * x),
                    Point3D.Quadrance(new Point3D(0, 0, 0), new Point3D(0, 0, x)));

                for (int y = 0; y < 10; y++)
                {
                    Assert.AreEqual(new Rational(25),
                        Point3D.Quadrance(new Point3D(x, y, 0), new Point3D(x + 3, y + 4, 0)));
                    Assert.AreEqual(new Rational(25),
                        Point3D.Quadrance(new Point3D(x, 0, y), new Point3D(x + 3, 0, y + 4)));
                    Assert.AreEqual(new Rational(25),
                        Point3D.Quadrance(new Point3D(0, x, y), new Point3D(0, x + 3, y + 4)));
                }
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { new Point3D(null, 1, 1); });
            Assert.Throws<ArgumentNullException>(
                () => { new Point3D(1, null, 1); });
            Assert.Throws<ArgumentNullException>(
                () => { new Point3D(1, 1, null); });
            Assert.Throws<ArgumentNullException>(
                () => { Point3D.Parse(null); });
            Assert.Throws<ArgumentNullException>(
                () => { Point3D.Deserialize(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new Point3D(1, 1, 1).Serialize(null); });

            Assert.Throws<ArgumentNullException>(
                () => { Point3D.Quadrance(null, Point3D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { Point3D.Quadrance(Point3D.Origin, null); });
        }
    }
}

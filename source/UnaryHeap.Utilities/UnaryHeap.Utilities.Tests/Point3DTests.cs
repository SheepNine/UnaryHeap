using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D3;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Point3DTests
    {
        [Fact]
        public void Constructor()
        {
            var sut = new Point3D(1, 3, 2);

            Assert.Equal(1, sut.X);
            Assert.Equal(3, sut.Y);
            Assert.Equal(2, sut.Z);
        }

        [Fact]
        public void Origin()
        {
            var sut = Point3D.Origin;

            Assert.Equal(0, sut.X);
            Assert.Equal(0, sut.Y);
            Assert.Equal(0, sut.Z);
        }

        [Fact]
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

        [Theory]
        [MemberData("StringFormatData")]
        public void ToString(Point3D value, string expected)
        {
            Assert.Equal(expected, value.ToString());
        }

        [Theory]
        [MemberData("StringFormatData")]
        public void Parse(Point3D expected, string value)
        {
            Assert.Equal(expected, Point3D.Parse(value));
        }

        [Fact]
        public void ParseDecimalRepresentation()
        {
            Assert.Equal(new Point3D(5, new Rational(-3, 7), 1), Point3D.Parse("5.000,-3/7,1.0"));
        }

        public static IEnumerable<object[]> StringFormatData
        {
            get
            {
                return new[] {
                    new object [] {
                        new Point3D(new Rational(1, 2), new Rational(-3, 4), new Rational(5,6)),
                        "1/2,-3/4,5/6"
                    },
                    new object [] {
                        new Point3D(new Rational(-9), new Rational(-2), new Rational(6)),
                        "-9,-2,6"
                    },
                };
            }
        }

        [Theory]
        [MemberData("InvalidlyFormattedStrings")]
        public void ParseInvalidData(string input)
        {
            var ex = Assert.Throws<FormatException>(() => { Point3D.Parse(input); });
            Assert.StartsWith("Input string was not in a correct format.", ex.Message);
        }

        public static IEnumerable<object[]> InvalidlyFormattedStrings
        {
            get
            {
                return new[]
                {
                    new object[] { "" },
                    new object[] { "1,2," },
                    new object[] { "1,,3" },
                    new object[] { "1,," },
                    new object[] { ",2,3" },
                    new object[] { ",2," },
                    new object[] { ",,3" },
                    new object[] { ",," },                    
                    new object[] { "1 ,2,3" },
                    new object[] { "1, 2,3" },
                    new object[] { "1,2 ,3" },
                    new object[] { "1,2, 3" },
                };
            }
        }

        [Theory]
        [MemberData("BinaryFormatData")]
        public void Serialize(Point3D value, byte[] expected)
        {
            using (var buffer = new MemoryStream())
            {
                value.Serialize(buffer);
                Assert.Equal(expected, buffer.ToArray());
            }
        }

        [Theory]
        [MemberData("BinaryFormatData")]
        public void Deserialize(Point3D expected, byte[] value)
        {
            using (var buffer = new MemoryStream(value))
            {
                Assert.Equal(expected, Point3D.Deserialize(buffer));
                Assert.Equal(buffer.Length, buffer.Position);
            }
        }

        public static IEnumerable<object[]> BinaryFormatData
        {
            get
            {
                return new[] {
                    new object [] { new Point3D(0, 3, 4), new byte[] {
                        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01,
                        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x03, 0x01,
                        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x01,} },
                    new object [] { new Point3D(300, 200, 201), new byte[] {
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x2C, 0x01, 0x01,
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x01,
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC9, 0x00, 0x01,} },
                };
            }
        }

        [Fact]
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

        [Fact]
        public void Quadrance()
        {
            Assert.Equal(Rational.Zero,
                Point3D.Quadrance(Point3D.Origin, Point3D.Origin));

            for (int x = 0; x < 10; x++)
            {
                Assert.Equal(new Rational(x * x),
                    Point3D.Quadrance(new Point3D(0, 0, 0), new Point3D(x, 0, 0)));
                Assert.Equal(new Rational(x * x),
                    Point3D.Quadrance(new Point3D(0, 0, 0), new Point3D(0, x, 0)));
                Assert.Equal(new Rational(x * x),
                    Point3D.Quadrance(new Point3D(0, 0, 0), new Point3D(0, 0, x)));

                for (int y = 0; y < 10; y++)
                {
                    Assert.Equal(new Rational(25),
                        Point3D.Quadrance(new Point3D(x, y, 0), new Point3D(x + 3, y + 4, 0)));
                    Assert.Equal(new Rational(25),
                        Point3D.Quadrance(new Point3D(x, 0, y), new Point3D(x + 3, 0, y + 4)));
                    Assert.Equal(new Rational(25),
                        Point3D.Quadrance(new Point3D(0, x, y), new Point3D(0, x + 3, y + 4)));
                }
            }
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("x",
                () => { new Point3D(null, 1, 1); });
            Assert.Throws<ArgumentNullException>("y",
                () => { new Point3D(1, null, 1); });
            Assert.Throws<ArgumentNullException>("z",
                () => { new Point3D(1, 1, null); });
            Assert.Throws<ArgumentNullException>("value",
                () => { Point3D.Parse(null); });
            Assert.Throws<ArgumentNullException>("input",
                () => { Point3D.Deserialize(null); });
            Assert.Throws<ArgumentNullException>("output",
                () => { new Point3D(1, 1, 1).Serialize(null); });

            Assert.Throws<ArgumentNullException>("p1",
                () => { Point3D.Quadrance(null, Point3D.Origin); });
            Assert.Throws<ArgumentNullException>("p2",
                () => { Point3D.Quadrance(Point3D.Origin, null); });
        }
    }
}

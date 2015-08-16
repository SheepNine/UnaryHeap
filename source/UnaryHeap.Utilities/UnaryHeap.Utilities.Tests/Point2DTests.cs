using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Point2DTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Constructor()
        {
            var sut = new Point2D(1, 3);

            Assert.Equal(1, sut.X);
            Assert.Equal(3, sut.Y);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Origin()
        {
            var sut = Point2D.Origin;

            Assert.Equal(0, sut.X);
            Assert.Equal(0, sut.Y);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Equality()
        {
            var sut = new Point2D(1, 2);

            Assert.True(sut.Equals(new Point2D(1, 2)));
            Assert.False(sut.Equals(new Point2D(1, 1)));
            Assert.False(sut.Equals(new Point2D(2, 2)));
            Assert.False(sut.Equals(null));
            Assert.False(sut.Equals("a string"));
        }

        [Theory]
        [MemberData("StringFormatData")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void ToString(Point2D value, string expected)
        {
            Assert.Equal(expected, value.ToString());
        }

        [Theory]
        [MemberData("StringFormatData")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Parse(Point2D expected, string value)
        {
            Assert.Equal(expected, Point2D.Parse(value));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void ParseDecimalRepresentation()
        {
            Assert.Equal(new Point2D(5, new Rational(-3, 7)), Point2D.Parse("5.000,-3/7"));
        }

        public static IEnumerable<object[]> StringFormatData
        {
            get
            {
                return new[] {
                    new object [] { new Point2D(new Rational(1, 2), new Rational(-3, 4)), "1/2,-3/4" },
                    new object [] { new Point2D(new Rational(-9), new Rational(-2)), "-9,-2" },
                };
            }
        }

        [Theory]
        [MemberData("InvalidlyFormattedStrings")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void ParseInvalidData(string input)
        {
            var ex = Assert.Throws<FormatException>(() => { Point2D.Parse(input); });
            Assert.StartsWith("Input string was not in a correct format.", ex.Message);
        }

        public static IEnumerable<object[]> InvalidlyFormattedStrings
        {
            get
            {
                return new[] {
                    new object [] { "" },
                    new object [] { "," },
                    new object [] { "1," },
                    new object [] { ",3" },                    
                    new object [] { "2 ,2" },
                    new object [] { "2, 2" },
                };
            }
        }

        [Theory]
        [MemberData("BinaryFormatData")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Serialize(Point2D value, byte[] expected)
        {
            using (var buffer = new MemoryStream())
            {
                value.Serialize(buffer);
                Assert.Equal(expected, buffer.ToArray());
            }
        }

        [Theory]
        [MemberData("BinaryFormatData")]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Deserialize(Point2D expected, byte[] value)
        {
            using (var buffer = new MemoryStream(value))
            {
                Assert.Equal(expected, Point2D.Deserialize(buffer));
                Assert.Equal(buffer.Length, buffer.Position);
            }
        }

        public static IEnumerable<object[]> BinaryFormatData
        {
            get
            {
                return new[] {
                    new object [] { new Point2D(0, 3), new byte[] {
                        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01,
                        0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x03, 0x01,} },
                    new object [] { new Point2D(300, 200), new byte[] {
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x2C, 0x01, 0x01,
                        0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC8, 0x00, 0x01,} },
                };
            }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("x", () => { new Point2D(null, 1); });
            Assert.Throws<ArgumentNullException>("y", () => { new Point2D(1, null); });
            Assert.Throws<ArgumentNullException>("input", () => { Point2D.Deserialize(null); });
            Assert.Throws<ArgumentNullException>("value", () => { Point2D.Parse(null); });
            Assert.Throws<ArgumentNullException>("output", () => { new Point2D(1, 1).Serialize(null); });
        }
    }
}

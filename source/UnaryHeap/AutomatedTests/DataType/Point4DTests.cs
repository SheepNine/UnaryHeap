using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Point4DTests
    {
        [Test]
        public void Constructor()
        {
            var sut = new Point4D(1, 3, 5, 7);
            Assert.AreEqual((Rational)1, sut.X);
            Assert.AreEqual((Rational)3, sut.Y);
            Assert.AreEqual((Rational)5, sut.Z);
            Assert.AreEqual((Rational)7, sut.W);
        }

        [Test]
        public void Origin()
        {
            var sut = Point4D.Origin;
            Assert.AreEqual((Rational)0, sut.X);
            Assert.AreEqual((Rational)0, sut.Y);
            Assert.AreEqual((Rational)0, sut.Z);
            Assert.AreEqual((Rational)0, sut.W);
        }

        [Test]
        public void Equality()
        {
            var points = new[]
            {
                new Point4D(1, 2, 3, 4),
                new Point4D(-1, 2, 3, 4),
                new Point4D(1, -2, 3, 4),
                new Point4D(1, 2, -3, 4),
                new Point4D(1, 2, 3, -4),
            };

            for (var i = 0; i < points.Length; i++)
            {
                Assert.IsFalse(points[i].Equals(null));
                Assert.IsFalse(points[i].Equals("a string"));

                for (var j = 0; j < points.Length; j++)
                    Assert.AreEqual(i == j, points[i].Equals(points[j]));
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

        static List<Point4D> SomeRandomPoints()
        {
            var random = new Random(19830630);
            var result = new List<Point4D>();

            foreach (var i in Enumerable.Range(0, 500))
                result.Add(new Point4D(
                    new Rational(random.Next(), random.Next()),
                    new Rational(random.Next(), random.Next()),
                    new Rational(random.Next(), random.Next()),
                    new Rational(random.Next(), random.Next())));

            return result;
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            TestUtils.NullChecks(new()
            {
                { typeof(ArgumentNullException), new TestDelegate[] {
                    () => { _ = new Point4D(null, 0, 0, 0); },
                    () => { _ = new Point4D(0, null, 0, 0); },
                    () => { _ = new Point4D(0, 0, null, 0); },
                    () => { _ = new Point4D(0, 0, 0, null); },
                }}
            });
        }
    }
}

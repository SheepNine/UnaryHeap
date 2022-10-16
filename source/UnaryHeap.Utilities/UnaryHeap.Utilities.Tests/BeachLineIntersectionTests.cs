using System;
using System.Linq;
using UnaryHeap.Algorithms;
using NUnit.Framework;
using UnaryHeap.DataType;

namespace UnaryHeap.Utilities.Tests
{
    [TestFixture]
    public class BeachLineIntersectionTests
    {
        [Test]
        public void EqualElevation()
        {
            var arcA = new Point2D(-2, 10);
            var arcB = new Point2D(4, 10);

            var center = 1;

            foreach (var y in Enumerable.Range(0, 10))
            {
                foreach (var x in Enumerable.Range(-10, 21))
                {
                    Assert.AreEqual(x.CompareTo(center),
                        FortunesAlgorithm.DetermineBeachLineArcIntersected(
                        new Point2D(x, y), arcA, arcB));

                    Assert.Throws<ArgumentException>(() =>
                        {
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(
                              new Point2D(x, y), arcB, arcA);
                        });
                }
            }
        }

        [Test]
        public void UnequalElevation()
        {
            foreach (var dx in Enumerable.Range(-10, 21))
                foreach (var dy in Enumerable.Range(-10, 21))
                {
                    var arcA = new Point2D(-8 + dx, 8 + dy);
                    var arcB = new Point2D(-1 + dx, 1 + dy);
                    var ABIntersection = -4 + dx;
                    var BAIntersection = 4 + dx;

                    // Check intersection points are as expected
                    var parabolaA = Parabola.FromFocusDirectrix(arcA, dy);
                    var parabolaB = Parabola.FromFocusDirectrix(arcB, dy);
                    Assert.AreEqual(parabolaA.Evaulate(ABIntersection),
                        parabolaB.Evaulate(ABIntersection));
                    Assert.AreEqual(parabolaA.Evaulate(BAIntersection),
                        parabolaB.Evaulate(BAIntersection));

                    foreach (var x in Enumerable.Range(-20, 41))
                    {
                        var point = new Point2D(x + dx, dy);

                        Assert.AreEqual(point.X.CompareTo(ABIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(
                            point, arcA, arcB));

                        Assert.AreEqual(point.X.CompareTo(BAIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(
                            point, arcB, arcA));
                    }
                }
        }

        [Test]
        public void UnequalElevation2()
        {
            foreach (var dx in Enumerable.Range(-10, 21))
                foreach (var dy in Enumerable.Range(-10, 21))
                {
                    var arcA = new Point2D(-2 + dx, 2 + dy);
                    var arcB = new Point2D(-1 + dx, 1 + dy);
                    var ABIntersection = -2 + dx;
                    var BAIntersection = 2 + dx;

                    // Check intersection points are as expected
                    var parabolaA = Parabola.FromFocusDirectrix(arcA, dy);
                    var parabolaB = Parabola.FromFocusDirectrix(arcB, dy);
                    Assert.AreEqual(parabolaA.Evaulate(ABIntersection),
                        parabolaB.Evaulate(ABIntersection));
                    Assert.AreEqual(parabolaA.Evaulate(BAIntersection),
                        parabolaB.Evaulate(BAIntersection));

                    foreach (var x in Enumerable.Range(-20, 41))
                    {
                        var point = new Point2D(x + dx, dy);

                        Assert.AreEqual(point.X.CompareTo(ABIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(
                            point, arcA, arcB));

                        Assert.AreEqual(point.X.CompareTo(BAIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(
                            point, arcB, arcA));
                    }
                }
        }

        [Test]
        public void UnequalElevation3()
        {
            foreach (var dx in Enumerable.Range(-10, 21))
                foreach (var dy in Enumerable.Range(-10, 21))
                {
                    var arcA = new Point2D(0 + dx, 9 + dy);
                    var arcB = new Point2D(0 + dx, 1 + dy);
                    var ABIntersection = -3 + dx;
                    var BAIntersection = 3 + dx;

                    // Check intersection points are as expected
                    var parabolaA = Parabola.FromFocusDirectrix(arcA, dy);
                    var parabolaB = Parabola.FromFocusDirectrix(arcB, dy);
                    Assert.AreEqual(parabolaA.Evaulate(ABIntersection),
                        parabolaB.Evaulate(ABIntersection));
                    Assert.AreEqual(parabolaA.Evaulate(BAIntersection),
                        parabolaB.Evaulate(BAIntersection));

                    foreach (var x in Enumerable.Range(-20, 41))
                    {
                        var point = new Point2D(x + dx, dy);

                        Assert.AreEqual(point.X.CompareTo(ABIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(
                            point, arcA, arcB));

                        Assert.AreEqual(point.X.CompareTo(BAIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(
                            point, arcB, arcA));
                    }
                }
        }

        [Test]
        public void OneSiteOnDirectrix()
        {
            var dx = 0;
            var dy = 0;
            {
                var intersection = 3 + dx;
                var arcA = new Point2D(intersection, dy);
                var arcB = new Point2D(intersection + 4, dy + 3);

                foreach (var x in Enumerable.Range(-20, 41))
                {
                    var point = new Point2D(x + dx, dy);

                    Assert.AreEqual(point.X.CompareTo(intersection),
                        FortunesAlgorithm.DetermineBeachLineArcIntersected(
                        point, arcA, arcB));
                    Assert.AreEqual(point.X.CompareTo(intersection),
                        FortunesAlgorithm.DetermineBeachLineArcIntersected(
                        point, arcB, arcA));
                }
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FortunesAlgorithm.DetermineBeachLineArcIntersected(
                null, Point2D.Origin, Point2D.Origin));
            Assert.Throws<ArgumentNullException>(() =>
                FortunesAlgorithm.DetermineBeachLineArcIntersected(
                Point2D.Origin, null, Point2D.Origin));
            Assert.Throws<ArgumentNullException>(() =>
                FortunesAlgorithm.DetermineBeachLineArcIntersected(
                Point2D.Origin, Point2D.Origin, null));
        }


        [Test]
        public void RandomSamples()
        {
            var suts = Enumerable.Range(1, 20).Select(
                i => Point2D.GenerateRandomPoints(11, i)).ToList();

            foreach (var sut in suts)
            {
                var range = Orthotope2D.FromPoints(sut);

                Assert.AreEqual((Rational)0, range.X.Min);
                Assert.AreEqual((Rational)0, range.Y.Min);
                Assert.AreEqual((Rational)10, range.X.Max);
                Assert.AreEqual((Rational)10, range.Y.Max);
            }
        }

        [Test]
        public void BoundarySites()
        {
            var originalSites = Point2D.GenerateRandomPoints(9, 19830630);
            var augmentSites = FortunesAlgorithm.AddBoundarySites(originalSites);

            var range = Orthotope2D.FromPoints(augmentSites);
            Assert.AreEqual((Rational)(-1), range.X.Min);
            Assert.AreEqual((Rational)(-1), range.Y.Min);
            Assert.AreEqual((Rational)9, range.X.Max);
            Assert.AreEqual((Rational)9, range.Y.Max);

            Assert.Contains(new Point2D(00, -1), augmentSites);
            Assert.Contains(new Point2D(02, -1), augmentSites);
            Assert.Contains(new Point2D(04, -1), augmentSites);
            Assert.Contains(new Point2D(06, -1), augmentSites);
            Assert.Contains(new Point2D(08, -1), augmentSites);
            Assert.Contains(new Point2D(00, 09), augmentSites);
            Assert.Contains(new Point2D(02, 09), augmentSites);
            Assert.Contains(new Point2D(04, 09), augmentSites);
            Assert.Contains(new Point2D(06, 09), augmentSites);
            Assert.Contains(new Point2D(08, 09), augmentSites);
            Assert.Contains(new Point2D(-1, 00), augmentSites);
            Assert.Contains(new Point2D(-1, 02), augmentSites);
            Assert.Contains(new Point2D(-1, 04), augmentSites);
            Assert.Contains(new Point2D(-1, 06), augmentSites);
            Assert.Contains(new Point2D(-1, 08), augmentSites);
            Assert.Contains(new Point2D(09, 00), augmentSites);
            Assert.Contains(new Point2D(09, 02), augmentSites);
            Assert.Contains(new Point2D(09, 04), augmentSites);
            Assert.Contains(new Point2D(09, 06), augmentSites);
            Assert.Contains(new Point2D(09, 08), augmentSites);
        }
    }
}

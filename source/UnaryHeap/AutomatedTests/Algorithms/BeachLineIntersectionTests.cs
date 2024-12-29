using NUnit.Framework;
using System;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.Algorithms.Tests
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
                    var p = new Point2D(x, y);
                    Assert.That(
                        FortunesAlgorithm.DetermineBeachLineArcIntersected(p, arcA, arcB),
                        Is.EqualTo(x.CompareTo(center))
                    );
                    Assert.That(() =>
                    {
                        FortunesAlgorithm.DetermineBeachLineArcIntersected(p, arcB, arcA);
                    }, Throws.InstanceOf<ArgumentException>());
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
                    Assert.That(
                        parabolaA.Evaulate(ABIntersection),
                        Is.EqualTo(parabolaB.Evaulate(ABIntersection)));
                    Assert.That(
                        parabolaA.Evaulate(BAIntersection),
                        Is.EqualTo(parabolaB.Evaulate(BAIntersection)));

                    foreach (var x in Enumerable.Range(-20, 41))
                    {
                        var p = new Point2D(x + dx, dy);
                        Assert.That(
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(p, arcA, arcB),
                                Is.EqualTo(p.X.CompareTo(ABIntersection)));
                        Assert.That(
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(p, arcB, arcA),
                                Is.EqualTo(p.X.CompareTo(BAIntersection)));
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
                    Assert.That(
                        parabolaA.Evaulate(ABIntersection),
                        Is.EqualTo(parabolaB.Evaulate(ABIntersection)));
                    Assert.That(
                        parabolaA.Evaulate(BAIntersection),
                        Is.EqualTo(parabolaB.Evaulate(BAIntersection)));

                    foreach (var x in Enumerable.Range(-20, 41))
                    {
                        var p = new Point2D(x + dx, dy);
                        Assert.That(
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(p, arcA, arcB),
                                Is.EqualTo(p.X.CompareTo(ABIntersection)));
                        Assert.That(
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(p, arcB, arcA),
                                Is.EqualTo(p.X.CompareTo(BAIntersection)));
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
                    Assert.That(parabolaA.Evaulate(ABIntersection),
                        Is.EqualTo(parabolaB.Evaulate(ABIntersection)));
                    Assert.That(parabolaA.Evaulate(BAIntersection),
                        Is.EqualTo(parabolaB.Evaulate(BAIntersection)));

                    foreach (var x in Enumerable.Range(-20, 41))
                    {
                        var p = new Point2D(x + dx, dy);
                        Assert.That(
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(p, arcA, arcB),
                                Is.EqualTo(p.X.CompareTo(ABIntersection)));
                        Assert.That(
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(p, arcB, arcA),
                                Is.EqualTo(p.X.CompareTo(BAIntersection)));
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
                    Assert.That(
                        FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcA, arcB),
                            Is.EqualTo(point.X.CompareTo(intersection)));
                    Assert.That(
                        FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcB, arcA),
                            Is.EqualTo(point.X.CompareTo(intersection)));
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

                Assert.That(range.X.Min,
                    Is.EqualTo((Rational)0));
                Assert.That(range.Y.Min,
                    Is.EqualTo((Rational)0));
                Assert.That(range.X.Max,
                    Is.EqualTo((Rational)10));
                Assert.That(range.Y.Max,
                    Is.EqualTo((Rational)10));
            }
        }

        [Test]
        public void BoundarySites()
        {
            var originalSites = Point2D.GenerateRandomPoints(9, 19830630);
            var augmentSites = FortunesAlgorithm.AddBoundarySites(originalSites);

            var range = Orthotope2D.FromPoints(augmentSites);

            Assert.That(range.X.Min,
                Is.EqualTo((Rational)(-1)));
            Assert.That(range.Y.Min,
                Is.EqualTo((Rational)(-1)));
            Assert.That(range.X.Max,
                Is.EqualTo((Rational)9));
            Assert.That(range.Y.Max,
                Is.EqualTo((Rational)9));

            Assert.That(augmentSites, Contains.Item(new Point2D(00, -1)));
            Assert.That(augmentSites, Contains.Item(new Point2D(02, -1)));
            Assert.That(augmentSites, Contains.Item(new Point2D(04, -1)));
            Assert.That(augmentSites, Contains.Item(new Point2D(06, -1)));
            Assert.That(augmentSites, Contains.Item(new Point2D(08, -1)));
            Assert.That(augmentSites, Contains.Item(new Point2D(00, 09)));
            Assert.That(augmentSites, Contains.Item(new Point2D(02, 09)));
            Assert.That(augmentSites, Contains.Item(new Point2D(04, 09)));
            Assert.That(augmentSites, Contains.Item(new Point2D(06, 09)));
            Assert.That(augmentSites, Contains.Item(new Point2D(08, 09)));
            Assert.That(augmentSites, Contains.Item(new Point2D(-1, 00)));
            Assert.That(augmentSites, Contains.Item(new Point2D(-1, 02)));
            Assert.That(augmentSites, Contains.Item(new Point2D(-1, 04)));
            Assert.That(augmentSites, Contains.Item(new Point2D(-1, 06)));
            Assert.That(augmentSites, Contains.Item(new Point2D(-1, 08)));
            Assert.That(augmentSites, Contains.Item(new Point2D(09, 00)));
            Assert.That(augmentSites, Contains.Item(new Point2D(09, 02)));
            Assert.That(augmentSites, Contains.Item(new Point2D(09, 04)));
            Assert.That(augmentSites, Contains.Item(new Point2D(09, 06)));
            Assert.That(augmentSites, Contains.Item(new Point2D(09, 08)));
        }
    }
}

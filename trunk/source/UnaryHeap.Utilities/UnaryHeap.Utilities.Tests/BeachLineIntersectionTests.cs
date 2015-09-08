using System;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class BeachLineIntersectionTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void EqualElevation()
        {
            var arcA = new Point2D(-2, 10);
            var arcB = new Point2D(4, 10);

            var center = 1;

            foreach (var y in Enumerable.Range(0, 10))
            {
                foreach (var x in Enumerable.Range(-10, 21))
                {
                    Assert.Equal(x.CompareTo(center),
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

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
                    Assert.Equal(parabolaA.Evaulate(ABIntersection),
                        parabolaB.Evaulate(ABIntersection));
                    Assert.Equal(parabolaA.Evaulate(BAIntersection),
                        parabolaB.Evaulate(BAIntersection));

                    foreach (var x in Enumerable.Range(-20, 41))
                    {
                        var point = new Point2D(x + dx, dy);

                        Assert.Equal(point.X.CompareTo(ABIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcA, arcB));

                        Assert.Equal(point.X.CompareTo(BAIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcB, arcA));
                    }
                }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
                    Assert.Equal(parabolaA.Evaulate(ABIntersection),
                        parabolaB.Evaulate(ABIntersection));
                    Assert.Equal(parabolaA.Evaulate(BAIntersection),
                        parabolaB.Evaulate(BAIntersection));

                    foreach (var x in Enumerable.Range(-20, 41))
                    {
                        var point = new Point2D(x + dx, dy);

                        Assert.Equal(point.X.CompareTo(ABIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcA, arcB));

                        Assert.Equal(point.X.CompareTo(BAIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcB, arcA));
                    }
                }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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
                    Assert.Equal(parabolaA.Evaulate(ABIntersection),
                        parabolaB.Evaulate(ABIntersection));
                    Assert.Equal(parabolaA.Evaulate(BAIntersection),
                        parabolaB.Evaulate(BAIntersection));

                    foreach (var x in Enumerable.Range(-20, 41))
                    {
                        var point = new Point2D(x + dx, dy);

                        Assert.Equal(point.X.CompareTo(ABIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcA, arcB));

                        Assert.Equal(point.X.CompareTo(BAIntersection),
                            FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcB, arcA));
                    }
                }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
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

                    Assert.Equal(point.X.CompareTo(intersection),
                        FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcA, arcB));
                    Assert.Equal(point.X.CompareTo(intersection),
                        FortunesAlgorithm.DetermineBeachLineArcIntersected(point, arcB, arcA));
                }
            }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("site", () =>
                FortunesAlgorithm.DetermineBeachLineArcIntersected(null, Point2D.Origin, Point2D.Origin));
            Assert.Throws<ArgumentNullException>("arcAFocus", () =>
                FortunesAlgorithm.DetermineBeachLineArcIntersected(Point2D.Origin, null, Point2D.Origin));
            Assert.Throws<ArgumentNullException>("arcBFocus", () =>
                FortunesAlgorithm.DetermineBeachLineArcIntersected(Point2D.Origin, Point2D.Origin, null));
        }


        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void RandomSamples()
        {
            var suts = Enumerable.Range(1, 20).Select(
                i => Point2D.GenerateRandomPoints(11, i)).ToList();

            foreach (var sut in suts)
            {
                var range = Orthotope2D.FromPoints(sut);

                Assert.Equal(0, range.X.Min);
                Assert.Equal(0, range.Y.Min);
                Assert.Equal(10, range.X.Max);
                Assert.Equal(10, range.Y.Max);
            }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void BoundarySites()
        {
            var originalSites = Point2D.GenerateRandomPoints(9, 19830630);
            var augmentSites = FortunesAlgorithm.AddBoundarySites(originalSites);

            var range = Orthotope2D.FromPoints(augmentSites);
            Assert.Equal(-1, range.X.Min);
            Assert.Equal(-1, range.Y.Min);
            Assert.Equal(9, range.X.Max);
            Assert.Equal(9, range.Y.Max);

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

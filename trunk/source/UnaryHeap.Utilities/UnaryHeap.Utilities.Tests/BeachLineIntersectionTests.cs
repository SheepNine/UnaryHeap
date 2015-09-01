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
                        { FortunesAlgorithm.DetermineBeachLineArcIntersected(
                            new Point2D(x, y), arcB, arcA); });
                }
            }
        }

        [Fact]
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
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("site", () =>
                FortunesAlgorithm.DetermineBeachLineArcIntersected(null, Point2D.Origin, Point2D.Origin));
            Assert.Throws<ArgumentNullException>("arcAFocus", () =>
                FortunesAlgorithm.DetermineBeachLineArcIntersected(Point2D.Origin, null, Point2D.Origin));
            Assert.Throws<ArgumentNullException>("arcBFocus", () =>
                FortunesAlgorithm.DetermineBeachLineArcIntersected(Point2D.Origin, Point2D.Origin, null));
        }
    }
}

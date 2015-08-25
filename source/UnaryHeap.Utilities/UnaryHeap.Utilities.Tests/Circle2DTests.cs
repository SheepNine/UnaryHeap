using System;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Circle2DTests
    {
        [Fact]
        public void Constructor()
        {
            var sut = new Circle2D(new Point2D(1, 2));

            Assert.Equal(new Point2D(1, 2), sut.Center);
            Assert.Equal(0, sut.Quadrance);

            sut = new Circle2D(new Point2D(2, 3), 4);
            Assert.Equal(new Point2D(2, 3), sut.Center);
            Assert.Equal(4, sut.Quadrance);
        }

        [Fact]
        public void Circumcenter()
        {
            var points = new[] {
                new Point2D(5, 0),
                new Point2D(4, 3),
                new Point2D(3, 4),
                
                new Point2D( 0, 5),
                new Point2D(-3, 4),
                new Point2D(-4, 3),
                
                new Point2D(-5,  0),
                new Point2D(-4, -3),
                new Point2D(-3, -4),
                
                new Point2D(0, -5),
                new Point2D(3, -4),
                new Point2D(4, -3),
            };

            for (int i = 0; i < points.Length; i++)
                for (int j = i + 1; j < points.Length; j++)
                {
                    Assert.Null(Circle2D.Circumcircle(points[i], points[j], points[j]));
                    Assert.Null(Circle2D.Circumcircle(points[j], points[i], points[j]));
                    Assert.Null(Circle2D.Circumcircle(points[j], points[j], points[i]));

                    for (int k = j + 1; k < points.Length; k++)
                    {
                        var circumcircle = Circle2D.Circumcircle(points[i], points[j], points[k]);

                        Assert.Equal(Point2D.Origin, circumcircle.Center);
                        Assert.Equal(25, circumcircle.Quadrance);

                        var dx = 4;
                        var dy = -2;
                        var circumcircle2 = Circle2D.Circumcircle(
                            new Point2D(points[i].X + dx, points[i].Y + dy),
                            new Point2D(points[j].X + dx, points[j].Y + dy),
                            new Point2D(points[k].X + dx, points[k].Y + dy));

                        Assert.Equal(new Point2D(dx, dy), circumcircle2.Center);
                        Assert.Equal(25, circumcircle.Quadrance);
                    }
                }
        }

        [Fact]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("center",
                () => { new Circle2D(null); });
            Assert.Throws<ArgumentNullException>("center",
                () => { new Circle2D(null, 1); });
            Assert.Throws<ArgumentNullException>("quadrance",
                () => { new Circle2D(Point2D.Origin, null); });
            Assert.Throws<ArgumentOutOfRangeException>("quadrance",
                () => { new Circle2D(Point2D.Origin, -1); });

            Assert.Throws<ArgumentNullException>("a",
                () => { Circle2D.Circumcircle(null, Point2D.Origin, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("b",
                () => { Circle2D.Circumcircle(Point2D.Origin, null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("c",
                () => { Circle2D.Circumcircle(Point2D.Origin, Point2D.Origin, null); });
        }
    }
}

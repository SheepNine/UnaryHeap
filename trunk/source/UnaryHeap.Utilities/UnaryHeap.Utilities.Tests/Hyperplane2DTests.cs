using System;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using Xunit;

namespace UnaryHeap.Utilities.Tests
{
    public class Hyperplane2DTests
    {
        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void ConstructorFromPoints()
        {
            var sut = new Hyperplane2D(Point2D.Origin, new Point2D(1, 0));

            Assert.Equal(0, sut.A);
            Assert.Equal(1, sut.B);
            Assert.Equal(0, sut.C);

            for (int x = -10; x <= 10; x++)
            {
                Assert.Equal(01, sut.DetermineHalfspaceOf(new Point2D(x, 02)));
                Assert.Equal(01, sut.DetermineHalfspaceOf(new Point2D(x, 01)));
                Assert.Equal(00, sut.DetermineHalfspaceOf(new Point2D(x, 00)));
                Assert.Equal(-1, sut.DetermineHalfspaceOf(new Point2D(x, -1)));
                Assert.Equal(-1, sut.DetermineHalfspaceOf(new Point2D(x, -2)));

                Assert.Equal(-1, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, 02)));
                Assert.Equal(-1, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, 01)));
                Assert.Equal(00, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, 00)));
                Assert.Equal(01, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, -1)));
                Assert.Equal(01, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, -2)));
            }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void ConstructorFromPoints2()
        {
            var sut = new Hyperplane2D(Point2D.Origin, new Point2D(0, 1));

            Assert.Equal(-1, sut.A);
            Assert.Equal(0, sut.B);
            Assert.Equal(0, sut.C);

            for (int y = -10; y <= 10; y++)
            {
                Assert.Equal(-1, sut.DetermineHalfspaceOf(new Point2D(02, y)));
                Assert.Equal(-1, sut.DetermineHalfspaceOf(new Point2D(01, y)));
                Assert.Equal(00, sut.DetermineHalfspaceOf(new Point2D(00, y)));
                Assert.Equal(01, sut.DetermineHalfspaceOf(new Point2D(-1, y)));
                Assert.Equal(01, sut.DetermineHalfspaceOf(new Point2D(-2, y)));

                Assert.Equal(01, sut.Coplane.DetermineHalfspaceOf(new Point2D(02, y)));
                Assert.Equal(01, sut.Coplane.DetermineHalfspaceOf(new Point2D(01, y)));
                Assert.Equal(00, sut.Coplane.DetermineHalfspaceOf(new Point2D(00, y)));
                Assert.Equal(-1, sut.Coplane.DetermineHalfspaceOf(new Point2D(-1, y)));
                Assert.Equal(-1, sut.Coplane.DetermineHalfspaceOf(new Point2D(-2, y)));
            }
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void ConstructorFromCoefficients()
        {
            var sut = new Hyperplane2D(1, 2, 3);

            Assert.Equal(new Rational(1, 2), sut.A);
            Assert.Equal(1, sut.B);
            Assert.Equal(new Rational(3, 2), sut.C);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Equality()
        {
            var p1 = new Point2D(1, 3);
            var p2 = new Point2D(2, 3);
            var p3 = new Point2D(3, 3);

            var h1 = new Hyperplane2D(p1, p2);
            var h2 = new Hyperplane2D(p2, p3);
            var h3 = new Hyperplane2D(p1, p3);
            Assert.Equal(h1, h2);
            Assert.Equal(h2, h3);
            Assert.Equal(h3, h1);
            Assert.False(h1.Equals(null));
            Assert.False(h2.Equals(null));
            Assert.False(h3.Equals(null));

            var nh1 = new Hyperplane2D(p2, p1);
            var nh2 = new Hyperplane2D(p3, p2);
            var nh3 = new Hyperplane2D(p3, p1);
            Assert.Equal(nh1, nh2);
            Assert.Equal(nh2, nh3);
            Assert.Equal(nh3, nh1);
            Assert.False(nh1.Equals(null));
            Assert.False(nh2.Equals(null));
            Assert.False(nh3.Equals(null));

            Assert.NotEqual(h1, nh1);
            Assert.NotEqual(h2, nh2);
            Assert.NotEqual(h3, nh3);
            Assert.Equal(h1, nh1.Coplane);
            Assert.Equal(h2, nh2.Coplane);
            Assert.Equal(h3, nh3.Coplane);
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void Intersection()
        {
            var answer = new Point2D(1, 4);

            for (int dx = 1; dx < 5; dx++)
                for (int dy = 1; dy < 5; dy++)
                {
                    var h1 = new Hyperplane2D(
                        answer, new Point2D(answer.X + 5, answer.Y + dy));
                    var h2 = new Hyperplane2D(
                        answer, new Point2D(answer.X - dx, answer.Y + 5));

                    Assert.Equal(answer, h1.FindIntersection(h2));
                    Assert.Equal(answer, h2.FindIntersection(h1));
                    Assert.Null(h1.FindIntersection(h1));
                    Assert.Null(h2.FindIntersection(h2));
                }
        }

        [Fact]
        public void Quadrance_XAxis()
        {
            var xAxis = new Hyperplane2D(Point2D.Origin, new Point2D(1, 0));

            Assert.Equal(0, xAxis.Quadrance(new Point2D(2, 0)));
            Assert.Equal(0, xAxis.Quadrance(new Point2D(-2, 0)));
            Assert.Equal(1, xAxis.Quadrance(new Point2D(0, 1)));
            Assert.Equal(1, xAxis.Quadrance(new Point2D(4, -1)));
            Assert.Equal(4, xAxis.Quadrance(new Point2D(-3, 2)));
            Assert.Equal(4, xAxis.Quadrance(new Point2D(8, -2)));
            Assert.Equal(9, xAxis.Quadrance(new Point2D(-9, -3)));
            Assert.Equal(9, xAxis.Quadrance(new Point2D(15, 3)));
        }

        [Fact]
        public void Quadrance_OffXAxis()
        {
            var xAxis = new Hyperplane2D(new Point2D(0, 4), new Point2D(1, 4));
            
            Assert.Equal(0, xAxis.Quadrance(new Point2D(2, 4)));
            Assert.Equal(0, xAxis.Quadrance(new Point2D(-2, 4)));
            Assert.Equal(1, xAxis.Quadrance(new Point2D(0, 5)));
            Assert.Equal(1, xAxis.Quadrance(new Point2D(4, 3)));
            Assert.Equal(4, xAxis.Quadrance(new Point2D(-3, 6)));
            Assert.Equal(4, xAxis.Quadrance(new Point2D(8, 2)));
            Assert.Equal(9, xAxis.Quadrance(new Point2D(-9, 7)));
            Assert.Equal(9, xAxis.Quadrance(new Point2D(15, 1)));
        }

        [Fact]
        public void Quadrance_YAxis()
        {
            var xAxis = new Hyperplane2D(Point2D.Origin, new Point2D(0, 1));

            Assert.Equal(0, xAxis.Quadrance(new Point2D(0, 2)));
            Assert.Equal(0, xAxis.Quadrance(new Point2D(0, -2)));
            Assert.Equal(1, xAxis.Quadrance(new Point2D(1, 0)));
            Assert.Equal(1, xAxis.Quadrance(new Point2D(-1, 4)));
            Assert.Equal(4, xAxis.Quadrance(new Point2D(2, -3)));
            Assert.Equal(4, xAxis.Quadrance(new Point2D(-2, 8)));
            Assert.Equal(9, xAxis.Quadrance(new Point2D(-3, -9)));
            Assert.Equal(9, xAxis.Quadrance(new Point2D(3, 15)));
        }

        [Fact]
        public void Quadrance_XY()
        {
            var xAxis = new Hyperplane2D(new Point2D(-1, -1), new Point2D(1, 1));

            Assert.Equal(new Rational(1, 2), xAxis.Quadrance(new Point2D(0, 1)));
            Assert.Equal(new Rational(1, 2), xAxis.Quadrance(new Point2D(0, -1)));
            Assert.Equal(new Rational(1, 2), xAxis.Quadrance(new Point2D(1, 0)));
            Assert.Equal(new Rational(1, 2), xAxis.Quadrance(new Point2D(-1, 0)));
            Assert.Equal(2, xAxis.Quadrance(new Point2D(0, 2)));
            Assert.Equal(2, xAxis.Quadrance(new Point2D(0, -2)));
            Assert.Equal(2, xAxis.Quadrance(new Point2D(2, 0)));
            Assert.Equal(2, xAxis.Quadrance(new Point2D(-2, 0)));
            Assert.Equal(new Rational(9, 2), xAxis.Quadrance(new Point2D(0, 3)));
            Assert.Equal(new Rational(9, 2), xAxis.Quadrance(new Point2D(0, -3)));
            Assert.Equal(new Rational(9, 2), xAxis.Quadrance(new Point2D(3, 0)));
            Assert.Equal(new Rational(9, 2), xAxis.Quadrance(new Point2D(-3, 0)));
        }

        [Fact]
        public void Quadrance_Diagonal()
        {
            var sut = new Hyperplane2D(new Point2D(3, 0), new Point2D(0, 4));

            Console.WriteLine(sut.ToString());
            Assert.Equal(25, sut.Quadrance(new Point2D(-1, -3)));
            Assert.Equal(25, sut.Quadrance(new Point2D(-4, 1)));
        }

        [Fact]
        [Trait(Traits.Status.Name, Traits.Status.Stable)]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>("a",
                () => { new Hyperplane2D(null, 0, 0); });
            Assert.Throws<ArgumentNullException>("b",
                () => { new Hyperplane2D(0, null, 0); });
            Assert.Throws<ArgumentNullException>("c",
                () => { new Hyperplane2D(0, 0, null); });
            Assert.Throws<ArgumentException>(
                () => { new Hyperplane2D(0, 0, 1); });
            Assert.Throws<ArgumentNullException>("p1",
                () => { new Hyperplane2D(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>("p2",
                () => { new Hyperplane2D(Point2D.Origin, null); });
            Assert.Throws<ArgumentException>(
                () => { new Hyperplane2D(Point2D.Origin, Point2D.Origin); });

            var sut = new Hyperplane2D(Point2D.Origin, new Point2D(1, 0));

            Assert.Throws<ArgumentNullException>("p",
                () => { sut.DetermineHalfspaceOf(null); });
            Assert.Throws<ArgumentNullException>("other",
                () => { sut.FindIntersection(null); });
            Assert.Throws<ArgumentNullException>("p",
                () => { sut.Quadrance(null); });
        }
    }
}

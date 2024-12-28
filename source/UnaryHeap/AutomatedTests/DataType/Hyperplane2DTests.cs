using NUnit.Framework;
using System;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Hyperplane2DTests
    {
        [Test]
        public void ConstructorFromPoints()
        {
            var sut = new Hyperplane2D(Point2D.Origin, new Point2D(1, 0));

            Assert.AreEqual((Rational)0, sut.A);
            Assert.AreEqual((Rational)1, sut.B);
            Assert.AreEqual((Rational)0, sut.C);

            for (int x = -10; x <= 10; x++)
            {
                Assert.AreEqual(01, sut.DetermineHalfspaceOf(new Point2D(x, 02)));
                Assert.AreEqual(01, sut.DetermineHalfspaceOf(new Point2D(x, 01)));
                Assert.AreEqual(00, sut.DetermineHalfspaceOf(new Point2D(x, 00)));
                Assert.AreEqual(-1, sut.DetermineHalfspaceOf(new Point2D(x, -1)));
                Assert.AreEqual(-1, sut.DetermineHalfspaceOf(new Point2D(x, -2)));

                Assert.AreEqual(-1, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, 02)));
                Assert.AreEqual(-1, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, 01)));
                Assert.AreEqual(00, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, 00)));
                Assert.AreEqual(01, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, -1)));
                Assert.AreEqual(01, sut.Coplane.DetermineHalfspaceOf(new Point2D(x, -2)));
            }
        }

        [Test]
        public void ConstructorFromPoints2()
        {
            var sut = new Hyperplane2D(Point2D.Origin, new Point2D(0, 1));

            Assert.AreEqual((Rational)(-1), sut.A);
            Assert.AreEqual((Rational)0, sut.B);
            Assert.AreEqual((Rational)0, sut.C);

            for (int y = -10; y <= 10; y++)
            {
                Assert.AreEqual(-1, sut.DetermineHalfspaceOf(new Point2D(02, y)));
                Assert.AreEqual(-1, sut.DetermineHalfspaceOf(new Point2D(01, y)));
                Assert.AreEqual(00, sut.DetermineHalfspaceOf(new Point2D(00, y)));
                Assert.AreEqual(01, sut.DetermineHalfspaceOf(new Point2D(-1, y)));
                Assert.AreEqual(01, sut.DetermineHalfspaceOf(new Point2D(-2, y)));

                Assert.AreEqual(01, sut.Coplane.DetermineHalfspaceOf(new Point2D(02, y)));
                Assert.AreEqual(01, sut.Coplane.DetermineHalfspaceOf(new Point2D(01, y)));
                Assert.AreEqual(00, sut.Coplane.DetermineHalfspaceOf(new Point2D(00, y)));
                Assert.AreEqual(-1, sut.Coplane.DetermineHalfspaceOf(new Point2D(-1, y)));
                Assert.AreEqual(-1, sut.Coplane.DetermineHalfspaceOf(new Point2D(-2, y)));
            }
        }

        [Test]
        public void ConstructorFromCoefficients()
        {
            var sut = new Hyperplane2D(1, 2, 3);

            Assert.AreEqual(new Rational(1, 2), sut.A);
            Assert.AreEqual((Rational)1, sut.B);
            Assert.AreEqual(new Rational(3, 2), sut.C);
        }

        [Test]
        public void Equality()
        {
            var p1 = new Point2D(1, 3);
            var p2 = new Point2D(2, 3);
            var p3 = new Point2D(3, 3);

            var h1 = new Hyperplane2D(p1, p2);
            var h2 = new Hyperplane2D(p2, p3);
            var h3 = new Hyperplane2D(p1, p3);
            Assert.AreEqual(h1, h2);
            Assert.AreEqual(h2, h3);
            Assert.AreEqual(h3, h1);
            Assert.False(h1.Equals(null));
            Assert.False(h2.Equals(null));
            Assert.False(h3.Equals(null));

            var nh1 = new Hyperplane2D(p2, p1);
            var nh2 = new Hyperplane2D(p3, p2);
            var nh3 = new Hyperplane2D(p3, p1);
            Assert.AreEqual(nh1, nh2);
            Assert.AreEqual(nh2, nh3);
            Assert.AreEqual(nh3, nh1);
            Assert.False(nh1.Equals(null));
            Assert.False(nh2.Equals(null));
            Assert.False(nh3.Equals(null));

            Assert.AreNotEqual(h1, nh1);
            Assert.AreNotEqual(h2, nh2);
            Assert.AreNotEqual(h3, nh3);
            Assert.AreEqual(h1, nh1.Coplane);
            Assert.AreEqual(h2, nh2.Coplane);
            Assert.AreEqual(h3, nh3.Coplane);
        }

        [Test]
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

                    Assert.AreEqual(answer, h1.FindIntersection(h2));
                    Assert.AreEqual(answer, h2.FindIntersection(h1));
                    Assert.Null(h1.FindIntersection(h1));
                    Assert.Null(h2.FindIntersection(h2));
                }
        }

        [Test]
        public void Quadrance_XAxis()
        {
            var xAxis = new Hyperplane2D(Point2D.Origin, new Point2D(1, 0));

            Assert.AreEqual((Rational)0, xAxis.Quadrance(new Point2D(2, 0)));
            Assert.AreEqual((Rational)0, xAxis.Quadrance(new Point2D(-2, 0)));
            Assert.AreEqual((Rational)1, xAxis.Quadrance(new Point2D(0, 1)));
            Assert.AreEqual((Rational)1, xAxis.Quadrance(new Point2D(4, -1)));
            Assert.AreEqual((Rational)4, xAxis.Quadrance(new Point2D(-3, 2)));
            Assert.AreEqual((Rational)4, xAxis.Quadrance(new Point2D(8, -2)));
            Assert.AreEqual((Rational)9, xAxis.Quadrance(new Point2D(-9, -3)));
            Assert.AreEqual((Rational)9, xAxis.Quadrance(new Point2D(15, 3)));
        }

        [Test]
        public void Quadrance_OffXAxis()
        {
            var xAxis = new Hyperplane2D(new Point2D(0, 4), new Point2D(1, 4));

            Assert.AreEqual((Rational)0, xAxis.Quadrance(new Point2D(2, 4)));
            Assert.AreEqual((Rational)0, xAxis.Quadrance(new Point2D(-2, 4)));
            Assert.AreEqual((Rational)1, xAxis.Quadrance(new Point2D(0, 5)));
            Assert.AreEqual((Rational)1, xAxis.Quadrance(new Point2D(4, 3)));
            Assert.AreEqual((Rational)4, xAxis.Quadrance(new Point2D(-3, 6)));
            Assert.AreEqual((Rational)4, xAxis.Quadrance(new Point2D(8, 2)));
            Assert.AreEqual((Rational)9, xAxis.Quadrance(new Point2D(-9, 7)));
            Assert.AreEqual((Rational)9, xAxis.Quadrance(new Point2D(15, 1)));
        }

        [Test]
        public void Quadrance_YAxis()
        {
            var xAxis = new Hyperplane2D(Point2D.Origin, new Point2D(0, 1));

            Assert.AreEqual((Rational)0, xAxis.Quadrance(new Point2D(0, 2)));
            Assert.AreEqual((Rational)0, xAxis.Quadrance(new Point2D(0, -2)));
            Assert.AreEqual((Rational)1, xAxis.Quadrance(new Point2D(1, 0)));
            Assert.AreEqual((Rational)1, xAxis.Quadrance(new Point2D(-1, 4)));
            Assert.AreEqual((Rational)4, xAxis.Quadrance(new Point2D(2, -3)));
            Assert.AreEqual((Rational)4, xAxis.Quadrance(new Point2D(-2, 8)));
            Assert.AreEqual((Rational)9, xAxis.Quadrance(new Point2D(-3, -9)));
            Assert.AreEqual((Rational)9, xAxis.Quadrance(new Point2D(3, 15)));
        }

        [Test]
        public void Quadrance_XY()
        {
            var xAxis = new Hyperplane2D(new Point2D(-1, -1), new Point2D(1, 1));

            Assert.AreEqual(new Rational(1, 2), xAxis.Quadrance(new Point2D(0, 1)));
            Assert.AreEqual(new Rational(1, 2), xAxis.Quadrance(new Point2D(0, -1)));
            Assert.AreEqual(new Rational(1, 2), xAxis.Quadrance(new Point2D(1, 0)));
            Assert.AreEqual(new Rational(1, 2), xAxis.Quadrance(new Point2D(-1, 0)));
            Assert.AreEqual((Rational)2, xAxis.Quadrance(new Point2D(0, 2)));
            Assert.AreEqual((Rational)2, xAxis.Quadrance(new Point2D(0, -2)));
            Assert.AreEqual((Rational)2, xAxis.Quadrance(new Point2D(2, 0)));
            Assert.AreEqual((Rational)2, xAxis.Quadrance(new Point2D(-2, 0)));
            Assert.AreEqual(new Rational(9, 2), xAxis.Quadrance(new Point2D(0, 3)));
            Assert.AreEqual(new Rational(9, 2), xAxis.Quadrance(new Point2D(0, -3)));
            Assert.AreEqual(new Rational(9, 2), xAxis.Quadrance(new Point2D(3, 0)));
            Assert.AreEqual(new Rational(9, 2), xAxis.Quadrance(new Point2D(-3, 0)));
        }

        [Test]
        public void Quadrance_Diagonal()
        {
            var sut = new Hyperplane2D(new Point2D(3, 0), new Point2D(0, 4));

            Assert.AreEqual((Rational)25, sut.Quadrance(new Point2D(-1, -3)));
            Assert.AreEqual((Rational)25, sut.Quadrance(new Point2D(-4, 1)));
        }

        [Test]
        public void EqualityAndHashCode()
        {
            TestUtils.TestEqualityAndHashCode(
                () => new Hyperplane2D(1, 2, 3),
                () => new Hyperplane2D(-1, 2, 3),
                () => new Hyperplane2D(1, -2, 3),
                () => new Hyperplane2D(1, 2, -3)
            );
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { new Hyperplane2D(null, 0, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Hyperplane2D(0, null, 0); });
            Assert.Throws<ArgumentNullException>(
                () => { new Hyperplane2D(0, 0, null); });
            Assert.Throws<ArgumentException>(
                () => { new Hyperplane2D(0, 0, 1); });
            Assert.Throws<ArgumentNullException>(
                () => { new Hyperplane2D(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { new Hyperplane2D(Point2D.Origin, null); });
            Assert.Throws<ArgumentException>(
                () => { new Hyperplane2D(Point2D.Origin, Point2D.Origin); });

            var sut = new Hyperplane2D(Point2D.Origin, new Point2D(1, 0));

            Assert.Throws<ArgumentNullException>(
                () => { sut.DetermineHalfspaceOf(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.FindIntersection(null); });
            Assert.Throws<ArgumentNullException>(
                () => { sut.Quadrance(null); });
        }
    }
}

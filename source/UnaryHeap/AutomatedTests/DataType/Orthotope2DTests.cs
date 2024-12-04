using NUnit.Framework;
using System;
using System.Linq;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Orthotope2DTests
    {
        [Test]
        public void Constructor_Values()
        {
            var sut = new Orthotope2D(1, 2, 3, 4);

            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)3, sut.X.Max);
            Assert.AreEqual((Rational)4, sut.Y.Max);
        }

        [Test]
        public void Constructor_Ranges()
        {
            var sut = new Orthotope2D(new Range(1, 2), new Range(3, 4));

            Assert.AreEqual((Rational)1, sut.X.Min);
            Assert.AreEqual((Rational)2, sut.X.Max);
            Assert.AreEqual((Rational)3, sut.Y.Min);
            Assert.AreEqual((Rational)4, sut.Y.Max);
        }

        [Test]
        public void FromPoints()
        {
            var sut = Orthotope2D.FromPoints(new[] {
                new Point2D(-7, 2),
                new Point2D(-4, 3),
                new Point2D(-1, 4),
                new Point2D(02, 5),
                new Point2D(05, 6),
            });

            Assert.AreEqual((Rational)(-7), sut.X.Min);
            Assert.AreEqual((Rational)5, sut.X.Max);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)6, sut.Y.Max);

            Assert.AreEqual(new Point2D(-1, 4), sut.Center);
        }

        [Test]
        public void FromOnePoint()
        {
            var sut = Orthotope2D.FromPoints(new[] {
                new Point2D(-7, 2),
            });

            Assert.AreEqual((Rational)(-7), sut.X.Min);
            Assert.AreEqual((Rational)(-7), sut.X.Max);
            Assert.AreEqual((Rational)2, sut.Y.Min);
            Assert.AreEqual((Rational)2, sut.Y.Max);
        }

        [Test]
        public void Contains()
        {
            var sut = new Orthotope2D(0, 10, 10, 20);

            Assert.True(sut.Contains(new Point2D(5, 15)));
            Assert.False(sut.Contains(new Point2D(15, 15)));
            Assert.False(sut.Contains(new Point2D(-5, 15)));
            Assert.False(sut.Contains(new Point2D(5, 25)));
            Assert.False(sut.Contains(new Point2D(5, 5)));
        }

        [Test]
        public void GetPadded()
        {
            var sut = new Orthotope2D(0, 10, 20, 30).GetPadded(3);

            Assert.AreEqual((Rational)(-3), sut.X.Min);
            Assert.AreEqual((Rational)7, sut.Y.Min);
            Assert.AreEqual((Rational)23, sut.X.Max);
            Assert.AreEqual((Rational)33, sut.Y.Max);
        }

        [Test]
        public void GetScaled()
        {
            var sut = new Orthotope2D(-1, -3, 1, 3).GetScaled(2);

            Assert.AreEqual((Rational)(-2), sut.X.Min);
            Assert.AreEqual((Rational)(-6), sut.Y.Min);
            Assert.AreEqual((Rational)2, sut.X.Max);
            Assert.AreEqual((Rational)6, sut.Y.Max);
        }

        [Test]
        public void CenteredAt()
        {
            var sut = new Orthotope2D(-1, -3, 1, 3)
                .CenteredAt(new Point2D(1, 2));

            Assert.AreEqual((Rational)0, sut.X.Min);
            Assert.AreEqual((Rational)(-1), sut.Y.Min);
            Assert.AreEqual((Rational)2, sut.X.Max);
            Assert.AreEqual((Rational)5, sut.Y.Max);
        }

        [Test]
        public void Intersects()
        {
            var sut = new Orthotope2D(2, 3, 4, 5);

            Assert.IsTrue(sut.Intersects(new Orthotope2D(0, 1, 6, 7)));
            Assert.IsFalse(sut.Intersects(new Orthotope2D(0, 1, 1, 7)));
            Assert.IsFalse(sut.Intersects(new Orthotope2D(5, 1, 6, 7)));
            Assert.IsFalse(sut.Intersects(new Orthotope2D(0, 1, 6, 2)));
            Assert.IsFalse(sut.Intersects(new Orthotope2D(0, 6, 6, 7)));
        }

        [Test]
        public void MakeFacets()
        {
            foreach (var sut in new[]
            {
                new Orthotope2D(-1, -1, 0, 0),
                new Orthotope2D(0, 0, 1, 1),
                new Orthotope2D(-1, -1, 1, 1),
                new Orthotope2D(-3, -2, -1, 0)
            })
                TestMakeFacets(sut);
        }

        static void TestMakeFacets(Orthotope2D sut)
        {
            var facets = sut.MakeFacets().ToList();

            // There are six different planes
            Assert.AreEqual(4, facets.Select(facet => facet.Plane).Distinct().Count());

            foreach (var facet in facets)
            {
                // Facet points lie on its plane
                Assert.AreEqual(0, facet.Plane.DetermineHalfspaceOf(facet.Start));
                Assert.AreEqual(0, facet.Plane.DetermineHalfspaceOf(facet.End));
                // Facet plane is derived from its points
                Assert.AreEqual(facet.Plane, new Hyperplane2D(facet.Start, facet.End));
                // Facet faces towards the orhtotope center
                Assert.AreEqual(1, facet.Plane.DetermineHalfspaceOf(sut.Center));
            }

            foreach (var corner in new[]
            {
                new Point2D(sut.X.Min, sut.Y.Min),
                new Point2D(sut.X.Max, sut.Y.Min),
                new Point2D(sut.X.Min, sut.Y.Max),
                new Point2D(sut.X.Max, sut.Y.Max),
            })
            {
                // The corner lies on three faces
                Assert.AreEqual(2, facets.Count(
                    facet => facet.Plane.DetermineHalfspaceOf(corner) == 0));
                // The corner is in front of the other three faces
                Assert.AreEqual(2, facets.Count(
                    facet => facet.Plane.DetermineHalfspaceOf(corner) == 1));
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(null, new DataType.Range(-1, 1)); });
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(new DataType.Range(-1, 1), null); });

            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(0, 0, 0, 0).Contains(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(0, 0, 0, 0).GetPadded(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(0, 0, 0, 0).GetScaled(null); });
            Assert.Throws<ArgumentNullException>(
                () => { new Orthotope2D(0, 0, 0, 0).CenteredAt(null); });

            Assert.Throws<ArgumentNullException>(
                () => { Orthotope2D.FromPoints(null); });
            Assert.Throws<ArgumentNullException>(
                () => { Orthotope2D.FromPoints(new Point2D[] { null }); });
            Assert.Throws<ArgumentException>(
                () => { Orthotope2D.FromPoints(new Point2D[] { }); });
        }
    }
}

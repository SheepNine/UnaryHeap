using NUnit.Framework;
using System;
using System.Linq;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Facet2DTests
    {
        [Test]
        public void Constructor_PlaneSize()
        {
            var sut = new Facet2D(new Hyperplane2D(1, -1, 4), 25);
            Assert.AreEqual(new Point2D(21, 25), sut.Start);
            Assert.AreEqual(new Point2D(-29, -25), sut.End);
            Assert.AreEqual(new Hyperplane2D(1, -1, 4), sut.Plane);
            var cosut = sut.Cofacet;
            Assert.AreEqual(new Point2D(-29, -25), cosut.Start);
            Assert.AreEqual(new Point2D(21, 25), cosut.End);
            Assert.AreEqual(new Hyperplane2D(-1, 1, -4), cosut.Plane);
        }

        [Test]
        public void Constructor_Points()
        {
            var sut = new Facet2D(new Hyperplane2D(1, -1, 4),
                new Point2D(5, 1), new Point2D(4, 0));
            Assert.AreEqual(new Point2D(5, 1), sut.Start);
            Assert.AreEqual(new Point2D(4, 0), sut.End);
            Assert.AreEqual(new Hyperplane2D(1, -1, 4), sut.Plane);
            var cosut = sut.Cofacet;
            Assert.AreEqual(new Point2D(4, 0), cosut.Start);
            Assert.AreEqual(new Point2D(5, 1), cosut.End);
            Assert.AreEqual(new Hyperplane2D(-1, 1, -4), cosut.Plane);
        }

        [Test]
        public void XPlaneWinding()
        {
            TestFacetFromHyperplane(new Hyperplane2D(1, 0, 0));
            TestFacetFromHyperplane(new Hyperplane2D(-1, 0, 0));
        }

        [Test]
        public void YPlaneWinding()
        {
            TestFacetFromHyperplane(new Hyperplane2D(0, 1, 0));
            TestFacetFromHyperplane(new Hyperplane2D(0, -1, 0));
        }

        static void TestFacetFromHyperplane(Hyperplane2D sut)
        {
            var facet = new Facet2D(sut, 100);
            Assert.AreEqual(0, sut.DetermineHalfspaceOf(facet.Start));
            Assert.AreEqual(0, sut.DetermineHalfspaceOf(facet.End));
            Assert.AreEqual(sut, new Hyperplane2D(facet.Start, facet.End));
        }

        [Test]
        public void Winding()
        {
            var facet = new Facet2D(new Hyperplane2D(1, 1, 0), 10);
            Assert.AreEqual(new Point2D(-10, 10), facet.Start);
            Assert.AreEqual(new Point2D(10, -10), facet.End);
        }


        [Test]
        public void Split()
        {
            var start = new Point2D(1, 2);
            var end = new Point2D(7, 5);
            var mid = new Point2D(5, 4);

            var sut = new Facet2D(new Hyperplane2D(start, end), start, end);

            // Coplanar
            sut.Split(new Hyperplane2D(start, end), out Facet2D front, out Facet2D back);
            Assert.IsNull(back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(end, front.End);

            // Counterplanar
            sut.Split(new Hyperplane2D(end, start), out front, out back);
            Assert.IsNull(front);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(end, back.End);

            // Edge touch
            sut.Split(new Hyperplane2D(start, new Point2D(start.X, start.Y - 1)),
                out front, out back);
            Assert.IsNull(back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(end, front.End);

            sut.Split(new Hyperplane2D(start, new Point2D(start.X, start.Y + 1)),
                out front, out back);
            Assert.IsNull(front);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(end, back.End);

            sut.Split(new Hyperplane2D(end, new Point2D(end.X, end.Y + 1)),
                out front, out back);
            Assert.IsNull(back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(end, front.End);

            sut.Split(new Hyperplane2D(end, new Point2D(end.X, end.Y - 1)),
                out front, out back);
            Assert.IsNull(front);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(end, back.End);

            // Actual split
            sut.Split(new Hyperplane2D(mid, new Point2D(mid.X, mid.Y + 1)),
                out front, out back);
            Assert.AreEqual(start, front.Start);
            Assert.AreEqual(mid, front.End);
            Assert.AreEqual(mid, back.Start);
            Assert.AreEqual(end, back.End);

            sut.Split(new Hyperplane2D(mid, new Point2D(mid.X, mid.Y - 1)),
                out front, out back);
            Assert.AreEqual(start, back.Start);
            Assert.AreEqual(mid, back.End);
            Assert.AreEqual(mid, front.Start);
            Assert.AreEqual(end, front.End);
        }

        [Test]
        public void Facetize2D()
        {
            foreach (var sut in new[]
            {
                new Orthotope2D(0, 0, 1, 1),
                new Orthotope2D(-1, -1, 0, 0),
                new Orthotope2D(-1, -1, 1, 1),
                new Orthotope2D(-2, -1, 4, 8),
            })
                TestMakeFacets(sut);
        }

        private static void TestMakeFacets(Orthotope2D sut)
        {
            var facets = sut.MakeFacets().ToList();

            // There are four different planes
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
                // The corner lies on two faces
                Assert.AreEqual(2, facets.Count(
                    facet => facet.Plane.DetermineHalfspaceOf(corner) == 0));
                // The corner is in front of the other two faces
                Assert.AreEqual(2, facets.Count(
                    facet => facet.Plane.DetermineHalfspaceOf(corner) == 1));
            }
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var plane = new Hyperplane2D(1, 0, -1);
            var p1 = new Point2D(1, 2);
            var p2 = new Point2D(1, 4);
            var sut = new Facet2D(plane, p1, p2);

            TestUtils.NullChecks(new() {
                { typeof(ArgumentNullException), new TestDelegate[] {
                    () => { _ = new Facet2D(null, 1); },
                    () => { _ = new Facet2D(plane, null); },
                    () => { _ = new Facet2D(null, p1, p2); },
                    () => { _ = new Facet2D(plane, null, p2); },
                    () => { _ = new Facet2D(plane, p1, null); },
                    () => { sut.Split(null, out _, out _); }
                }},
                { typeof(ArgumentOutOfRangeException), new TestDelegate[]
                {
                    () => { _ = new Facet2D(plane, 0); },
                    () => { _ = new Facet2D(plane, -1); },
                }},
                { typeof(ArgumentException), new TestDelegate[]
                {
                    () => { _ = new Facet2D(plane, p1, p1); }
                }}
            });
        }
    }
}

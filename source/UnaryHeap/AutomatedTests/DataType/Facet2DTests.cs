using NUnit.Framework;
using System.Linq;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Facet2DTests
    {
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
    }
}

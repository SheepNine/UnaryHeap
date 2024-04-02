using NUnit.Framework;
using System.Linq;

namespace UnaryHeap.DataType.Tests
{
    [TestFixture]
    public class Facet3DTests
    {
        [Test]
        public void XPlaneWinding()
        {
            TestFacetFromHyperplane(new Hyperplane3D(1, 0, 0, 0));
            TestFacetFromHyperplane(new Hyperplane3D(-1, 0, 0, 0));
        }

        [Test]
        public void YPlaneWinding()
        {
            TestFacetFromHyperplane(new Hyperplane3D(0, 1, 0, 0));
            TestFacetFromHyperplane(new Hyperplane3D(0, -1, 0, 0));
        }

        [Test]
        public void ZPlaneWinding()
        {
            TestFacetFromHyperplane(new Hyperplane3D(0, 0, 1, 0));
            TestFacetFromHyperplane(new Hyperplane3D(0, 0, -1, 0));
        }

        static void TestFacetFromHyperplane(Hyperplane3D sut)
        {
            var points = new Facet3D(sut, 100).Points.ToList();
            foreach (var point in points)
                Assert.AreEqual(0, sut.DetermineHalfspaceOf(point));

            var A = points[0];
            var B = points[1];
            var C = points[2];
            var D = points[3];

            Assert.AreEqual(sut, new Hyperplane3D(A, B, C));
            Assert.AreEqual(sut, new Hyperplane3D(B, C, A));
            Assert.AreEqual(sut, new Hyperplane3D(C, A, B));
            Assert.AreEqual(sut, new Hyperplane3D(A, B, D));
            Assert.AreEqual(sut, new Hyperplane3D(B, D, A));
            Assert.AreEqual(sut, new Hyperplane3D(D, A, B));
            Assert.AreEqual(sut, new Hyperplane3D(A, C, D));
            Assert.AreEqual(sut, new Hyperplane3D(C, D, A));
            Assert.AreEqual(sut, new Hyperplane3D(D, A, C));
            Assert.AreEqual(sut, new Hyperplane3D(B, C, D));
            Assert.AreEqual(sut, new Hyperplane3D(C, D, B));
            Assert.AreEqual(sut, new Hyperplane3D(D, B, C));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(C, B, A));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(B, A, C));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(A, C, B));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(D, B, A));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(B, A, D));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(A, D, B));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(D, C, A));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(C, A, D));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(A, D, C));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(D, C, B));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(C, B, D));
            Assert.AreEqual(sut.Coplane, new Hyperplane3D(B, D, C));
        }

        [Test]
        public void Winding()
        {
            var points = new Facet3D(new Hyperplane3D(1, 1, 1, 0), 10).Points.ToList();
            Assert.AreEqual(points[0], new Point3D(-20, 10, 10));
            Assert.AreEqual(points[1], new Point3D(0, -10, 10));
            Assert.AreEqual(points[2], new Point3D(20, -10, -10));
            Assert.AreEqual(points[3], new Point3D(0, 10, -10));
        }

        [Test]
        public void Facetize3D()
        {
            foreach (var sut in new[]
            {
                new Orthotope3D(-1, -1, -1, 0, 0, 0),
                new Orthotope3D(0, 0, 0, 1, 1, 1),
                new Orthotope3D(-1, -1, -1, 1, 1, 1),
                new Orthotope3D(-3, -2, 1, -1, 0, 6)
            })
                TestMakeFacets(sut);
        }

        private static void TestMakeFacets(Orthotope3D sut)
        {
            var facets = sut.MakeFacets().ToList();

            // There are six different planes
            Assert.AreEqual(6, facets.Select(facet => facet.Plane).Distinct().Count());

            foreach (var facet in facets)
            {
                // Facet points lie on its plane
                foreach (var point in facet.Points)
                    Assert.AreEqual(0, facet.Plane.DetermineHalfspaceOf(point));
                // Facet plane is derived from its points
                Assert.AreEqual(facet.Plane, new Hyperplane3D(facet.Points.ElementAt(0),
                    facet.Points.ElementAt(1), facet.Points.ElementAt(2)));
                Assert.AreEqual(facet.Plane, new Hyperplane3D(facet.Points.ElementAt(1),
                    facet.Points.ElementAt(2), facet.Points.ElementAt(3)));
                Assert.AreEqual(facet.Plane, new Hyperplane3D(facet.Points.ElementAt(2),
                    facet.Points.ElementAt(3), facet.Points.ElementAt(0)));
                Assert.AreEqual(facet.Plane, new Hyperplane3D(facet.Points.ElementAt(3),
                    facet.Points.ElementAt(0), facet.Points.ElementAt(1)));
                // Facet faces towards the orhtotope center
                Assert.AreEqual(1, facet.Plane.DetermineHalfspaceOf(sut.Center));
            }

            foreach (var corner in new[]
            {
                new Point3D(sut.X.Min, sut.Y.Min, sut.Z.Min),
                new Point3D(sut.X.Max, sut.Y.Min, sut.Z.Min),
                new Point3D(sut.X.Min, sut.Y.Max, sut.Z.Min),
                new Point3D(sut.X.Max, sut.Y.Max, sut.Z.Min),
                new Point3D(sut.X.Min, sut.Y.Min, sut.Z.Max),
                new Point3D(sut.X.Max, sut.Y.Min, sut.Z.Max),
                new Point3D(sut.X.Min, sut.Y.Max, sut.Z.Max),
                new Point3D(sut.X.Max, sut.Y.Max, sut.Z.Max),
            })
            {
                // The corner lies on three faces
                Assert.AreEqual(3, facets.Count(
                    facet => facet.Plane.DetermineHalfspaceOf(corner) == 0));
                // The corner is in front of the other three faces
                Assert.AreEqual(3, facets.Count(
                    facet => facet.Plane.DetermineHalfspaceOf(corner) == 1));
            }
        }
    }
}

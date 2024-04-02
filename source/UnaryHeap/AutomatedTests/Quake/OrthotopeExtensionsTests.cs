using NUnit.Framework;
using Quake;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace AutomatedTests.Quake
{
    static class OrthotopeExtensions
    {
        public static IEnumerable<Facet2D> Facetize(this Orthotope2D bounds)
        {
            var points = new[]
            {
                new Point2D(bounds.X.Min, bounds.Y.Min),
                new Point2D(bounds.X.Min, bounds.Y.Max),
                new Point2D(bounds.X.Max, bounds.Y.Max),
                new Point2D(bounds.X.Max, bounds.Y.Min),
            };

            var result = new[]
            {
                new Facet2D(new Hyperplane2D(1, 0, -bounds.X.Min), points[1], points[0]),
                new Facet2D(new Hyperplane2D(0, -1, bounds.Y.Max), points[2], points[1]),
                new Facet2D(new Hyperplane2D(-1, 0, bounds.X.Max), points[3], points[2]),
                new Facet2D(new Hyperplane2D(0, 1, -bounds.Y.Min), points[0], points[3]),
            };

            return result;
        }

        public static IEnumerable<Facet3D> Facetize(this Orthotope3D bounds)
        {
            var points = new[]
            {
                new Point3D(bounds.X.Min, bounds.Y.Min, bounds.Z.Min),
                new Point3D(bounds.X.Min, bounds.Y.Max, bounds.Z.Min),
                new Point3D(bounds.X.Min, bounds.Y.Min, bounds.Z.Max),
                new Point3D(bounds.X.Min, bounds.Y.Max, bounds.Z.Max),
                new Point3D(bounds.X.Max, bounds.Y.Min, bounds.Z.Max),
                new Point3D(bounds.X.Max, bounds.Y.Max, bounds.Z.Max),
                new Point3D(bounds.X.Max, bounds.Y.Min, bounds.Z.Min),
                new Point3D(bounds.X.Max, bounds.Y.Max, bounds.Z.Min),
            };

            return new[]
            {
                new Facet3D(new Hyperplane3D(1, 0, 0, -bounds.X.Min),
                    new[] { points[0], points[1], points[3], points[2] }),
                new Facet3D(new Hyperplane3D(0, 0, -1, bounds.Z.Max),
                    new[] { points[2], points[3], points[5], points[4] }),
                new Facet3D(new Hyperplane3D(-1, 0, 0, bounds.X.Max),
                    new[] { points[4], points[5], points[7], points[6] }),
                new Facet3D(new Hyperplane3D(0, 0, 1, -bounds.Z.Min),
                    new[] { points[6], points[7], points[1], points[0] }),
                new Facet3D(new Hyperplane3D(0, 1, 0, -bounds.Y.Min),
                    new[] { points[0], points[2], points[4], points[6] }),
                new Facet3D(new Hyperplane3D(0, -1, 0, bounds.Y.Max),
                    new[] { points[7], points[5], points[3], points[1] })
            };
        }
    }

    [TestFixture]
    public class OrthotopeExtensionsTests
    {
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
                CheckFacetization(sut);
        }

        private static void CheckFacetization(Orthotope2D sut)
        {
            var facets = sut.Facetize().ToList();

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
        public void Facetize3D()
        {
            foreach (var sut in new[]
            {
                new Orthotope3D(-1, -1, -1, 0, 0, 0),
                new Orthotope3D(0, 0, 0, 1, 1, 1),
                new Orthotope3D(-1, -1, -1, 1, 1, 1),
                new Orthotope3D(-3, -2, 1, -1, 0, 6)
            })
                CheckFacetization(sut);
        }

        private static void CheckFacetization(Orthotope3D sut)
        {
            var facets = sut.Facetize().ToList();

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

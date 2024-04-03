using NUnit.Framework;
using System;
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

        [Test]
        public void Split()
        {
            // Through two vertices
            TestSplit(
                "0,0,0  1,1,0  0,2,0  -1,1,0",
                "0,0,0  0,1,0  0,0,1",
                "1,1,0  0,2,0  0,0,0",
                "-1,1,0  0,0,0  0,2,0");

            // Through one vertex
            TestSplit(
                "0,0,0  1,1,0  0,2,0  -1,1,0",
                "0,0,0  1,2,1  1,2,0",
                "0,0,0  2/3,4/3,0  0,2,0  -1,1,0",
                "0,0,0  1,1,0  2/3,4/3,0");

            // Through no vertices
            TestSplit(
                "0,0,0  1,1,0  0,2,0  -1,1,0",
                "-1,0,0  1,2,1  1,2,0",
                "0,2,0  -1,1,0  -1/2,1/2,0  1/2,3/2,0",
                "0,0,0  1,1,0  1/2,3/2,0  -1/2,1/2,0");

            // Through an edge
            TestSplit(
                "0,0,0  1,1,0  0,2,0  -1,1,0",
                "0,0,0  1,1,1  1,1,0",
                "0,0,0  1,1,0  0,2,0  -1,1,0",
                null);

            // Through a point
            TestSplit(
                "0,0,0  1,1,0  0,2,0  -1,1,0",
                "1,0,0  1,1,1  1,1,0",
                "0,0,0  1,1,0  0,2,0  -1,1,0",
                null);

            // Coplanar
            TestSplit(
                "0,0,0  1,1,0  0,2,0  -1,1,0",
                "0,0,0  1,1,0  0,2,0",
                "0,0,0  1,1,0  0,2,0  -1,1,0",
                null);
        }

        private void TestSplit(string facetPointDef, string splitterPointDef,
            string frontResultPointsDef, string backResultPointsDef)
        {
            // TODO: multiplex each test case by applying a set of different affine
            // transformations to the points to get unique results
            var facetPoints = DecodeDef(facetPointDef);
            var splitterPoints = DecodeDef(splitterPointDef);
            var frontResultPoints = DecodeDef(frontResultPointsDef);
            var backResultPoints = DecodeDef(backResultPointsDef);

            Assert.AreEqual(3, splitterPoints.Length);
            var splitter = new Hyperplane3D(splitterPoints[0], splitterPoints[1],
                splitterPoints[2]);

            var facetPlane = new Hyperplane3D(facetPoints[0], facetPoints[1], facetPoints[2]);
            // Check the test data
            for (var i = 3; i < facetPoints.Length; i++)
                Assert.AreEqual(0, facetPlane.DetermineHalfspaceOf(facetPoints[i]));

            var facet = new Facet3D(facetPlane, facetPoints);
            facet.Split(splitter, out Facet3D front, out Facet3D back);
            VerifyResult(frontResultPoints, front);
            VerifyResult(backResultPoints, back);

            var cosplitter = new Hyperplane3D(splitterPoints[2], splitterPoints[1],
                splitterPoints[0]);
            facet.Split(cosplitter, out front, out back);
            VerifyResult(backResultPoints, front);
            VerifyResult(frontResultPoints, back);
        }

        void VerifyResult(Point3D[] expectedPoints, Facet3D actual)
        {
            if (expectedPoints.Length == 0)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                var actualPoints = actual.Points.ToList();
                Assert.That(actualPoints.Contains(expectedPoints[0]));
                while (!actualPoints[0].Equals(expectedPoints[0]))
                {
                    var swap = actualPoints[0];
                    actualPoints.Add(swap);
                    actualPoints.RemoveAt(0);
                }
                Assert.AreEqual(expectedPoints, actualPoints.ToArray());
                foreach (var point in expectedPoints)
                    Assert.AreEqual(0, actual.Plane.DetermineHalfspaceOf(point));
            }
        }

        static Point3D[] DecodeDef(string def)
        {
            if (def == null)
                return Array.Empty<Point3D>();

            return def.Split("  ").Select(Point3D.Parse).ToArray();
        }
    }
}

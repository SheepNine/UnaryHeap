using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;
using UnaryHeap.DataType.Tests;
using UnaryHeap.Utilities;

namespace Quake
{
    class Facet
    {
        List<Point3D> points;
        public IEnumerable<Point3D> Points { get { return points; } }
        public Hyperplane3D Plane { get; private set; }

        public static List<Facet> Facetize(MapBrush brush)
        {
            var result = brush.Planes.Select((plane, i) =>
            {
                var facet = new Facet(GetHyperplane(plane));
                foreach (var j in Enumerable.Range(0, brush.Planes.Count))
                {
                    if (facet == null)
                        break;
                    if (i == j)
                        continue;
                    facet.Split(GetHyperplane(brush.Planes[j]), out Facet front, out Facet back);
                    facet = back;
                }
                return facet;
            }).Where(plane => plane != null).ToList();

            if (result.Count < 4)
            {
                throw new Exception("Degenerate brush");
            }

            return result;
        }


        public static List<Point3D> ComputeWinding(Hyperplane3D plane, Rational size)
        {
            if (plane.A != 0)
            {
                var sign = plane.A.Sign;
                return new List<Point3D>()
                {
                    SolveForX(plane, size, size),
                    SolveForX(plane, -size * sign, size * sign),
                    SolveForX(plane, -size, -size),
                    SolveForX(plane, size * sign, -size * sign),
                };
            }
            else if (plane.B != 0)
            {
                var sign = plane.B.Sign;
                return new List<Point3D>()
                {
                    SolveForY(plane, size, size),
                    SolveForY(plane, size * sign, -size * sign),
                    SolveForY(plane, -size, -size),
                    SolveForY(plane, -size * sign, size * sign),
                };
            }
            else // Constructor affirms that C != 0
            {
                var sign = plane.C.Sign;
                return new List<Point3D>()
                {
                    SolveForZ(plane, size, size),
                    SolveForZ(plane, -size * sign, size * sign),
                    SolveForZ(plane, -size, -size),
                    SolveForZ(plane, size * sign, -size * sign),
                };
            }
        }

        static Point3D SolveForX(Hyperplane3D plane, Rational y, Rational z)
        {
            return new Point3D(-(plane.B * y + plane.C * z + plane.D) / plane.A, y, z);
        }

        static Point3D SolveForY(Hyperplane3D plane, Rational x, Rational z)
        {
            return new Point3D(x, -(plane.A * x + plane.C * z + plane.D) / plane.B, z);
        }

        static Point3D SolveForZ(Hyperplane3D plane, Rational x, Rational y)
        {
            return new Point3D(x, y, -(plane.A * x + plane.B * y + plane.D) / plane.C);
        }

        public static Hyperplane3D GetHyperplane(MapPlane plane)
        {
            return new Hyperplane3D(
                new Point3D(plane.P3X, plane.P3Y, plane.P3Z),
                new Point3D(plane.P2X, plane.P2Y, plane.P2Z),
                new Point3D(plane.P1X, plane.P1Y, plane.P1Z)
            );
        }

        public Facet(Hyperplane3D plane) : this(plane, ComputeWinding(plane, 100000))
        {
        }

        public Facet(Hyperplane3D plane, IEnumerable<Point3D> winding)
        {
            Plane = plane;
            this.points = new List<Point3D>(winding);
        }

        public void Split(Hyperplane3D partitioningPlane, out Facet frontSurface,
            out Facet backSurface)
        {
            if (partitioningPlane.Equals(Plane))
            {
                frontSurface = this;
                backSurface = null;
                return;
            }
            if (partitioningPlane.Coplane.Equals(Plane))
            {
                frontSurface = null;
                backSurface = this;
                return;
            }

            var windingPointHalfspaces = points.Select(point =>
                partitioningPlane.DetermineHalfspaceOf(point)).ToList();
            var windingPoints = new List<Point3D>(points);

            if (windingPointHalfspaces.All(hs => hs >= 0))
            {
                frontSurface = this;
                backSurface = null;
                return;
            }
            if (windingPointHalfspaces.All(hs => hs <= 0))
            {
                frontSurface = null;
                backSurface = this;
                return;
            }

            for (var i = windingPoints.Count - 1; i >= 0; i--)
            {
                var j = (i + 1) % windingPoints.Count;

                if (windingPointHalfspaces[i] * windingPointHalfspaces[j] >= 0)
                    continue;

                var detI = partitioningPlane.Determinant(windingPoints[i]);
                var detJ = partitioningPlane.Determinant(windingPoints[j]);

                var tJ = detI / (detI - detJ);
                var tI = 1 - tJ;

                if (tI < 0 || tI > 1)
                    throw new Exception("Coefficient not right");

                var intersectionPoint = new Point3D(
                    windingPoints[i].X * tI + windingPoints[j].X * tJ,
                    windingPoints[i].Y * tI + windingPoints[j].Y * tJ,
                    windingPoints[i].Z * tI + windingPoints[j].Z * tJ);

                if (partitioningPlane.DetermineHalfspaceOf(intersectionPoint) != 0)
                {
                    throw new Exception("Point calculation isn't right");
                }

                windingPointHalfspaces.Insert(i + 1, 0);
                windingPoints.Insert(i + 1, intersectionPoint);
            }

            frontSurface = new Facet(Plane,
                Enumerable.Range(0, windingPointHalfspaces.Count)
                .Where(i => windingPointHalfspaces[i] >= 0)
                .Select(i => windingPoints[i]));
            backSurface = new Facet(Plane,
                Enumerable.Range(0, windingPointHalfspaces.Count)
                .Where(i => windingPointHalfspaces[i] <= 0)
                .Select(i => windingPoints[i]));
        }
    }

    class QuakeExhaustivePartitioner : ExhaustivePartitioner<Facet, Hyperplane3D>
    {
        public QuakeExhaustivePartitioner(int imbalanceWeight, int splitWeight)
            : base(imbalanceWeight, splitWeight)
        {
        }

        public override void ClassifySurface(Facet surface, Hyperplane3D plane,
            out int minDeterminant, out int maxDeterminant)
        {
            if (surface.Plane == plane)
            {
                minDeterminant = 1;
                maxDeterminant = 1;
                return;
            }
            if (surface.Plane == plane.Coplane)
            {
                minDeterminant = -1;
                maxDeterminant = -1;
                return;
            }
            var determinants = surface.Points.Select(p => plane.DetermineHalfspaceOf(p));

            minDeterminant = determinants.Min();
            maxDeterminant = determinants.Max();
        }

        public override Hyperplane3D GetPlane(Facet surface)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            return surface.Plane;
        }
    }

    class QuakeBSP : BinarySpacePartitioner<Facet, Hyperplane3D>
    {
        public QuakeBSP(IPartitioner<Facet, Hyperplane3D> partitioner) : base(partitioner)
        {
        }

        /// <summary>
        /// Checks if a surface is a 'hint surface' used to speed up the first few levels
        /// of BSP partitioning by avoiding an exhaustive search for a balanced plane.
        /// </summary>
        /// <param name="surface">The surface to check.</param>
        /// <param name="depth">The current depth of the BSP tree.</param>
        /// <returns>True of this surface should be used for a partitioning plane
        /// (and discarded from the final BSP tree), false otherwise.</returns>
        protected override bool IsHintSurface(Facet surface, int depth)
        {
            // TODO: implement some hint surfaces
            return false;
        }

        /// <summary>
        /// Splits a surface into two subsurfaces lying on either side of a
        /// partitioning plane.
        /// If surface lies on the partitioningPlane, it should be considered in the
        /// front halfspace of partitioningPlane if its front halfspace is identical
        /// to that of partitioningPlane. Otherwise, it should be considered in the 
        /// back halfspace of partitioningPlane.
        /// </summary>
        /// <param name="surface">The surface to split.</param>
        /// <param name="partitioningPlane">The plane used to split surface.</param>
        /// <param name="frontSurface">The subsurface of surface lying in the front
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// back halfspace of partitioningPlane.</param>
        /// <param name="backSurface">The subsurface of surface lying in the back
        /// halfspace of partitioningPlane, or null, if surface is entirely in the
        /// front halfspace of partitioningPlane.</param>
        protected override void Split(Facet surface, Hyperplane3D partitioningPlane,
            out Facet frontSurface, out Facet backSurface)
        {
            surface.Split(partitioningPlane, out frontSurface, out backSurface);
        }
    }

    [TestFixture]
    public class Quake
    {
        private static void CheckPolytope(Hyperplane3D sut)
        {
            var points = Facet.ComputeWinding(sut, 100);
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
        public void XPlaneWinding()
        {
            CheckPolytope(new Hyperplane3D(1, 0, 0, 0));
            CheckPolytope(new Hyperplane3D(-1, 0, 0, 0));
        }

        [Test]
        public void YPlaneWinding()
        {
            CheckPolytope(new Hyperplane3D(0, 1, 0, 0));
            CheckPolytope(new Hyperplane3D(0, -1, 0, 0));
        }

        [Test]
        public void ZPlaneWinding()
        {
            CheckPolytope(new Hyperplane3D(0, 0, 1, 0));
            CheckPolytope(new Hyperplane3D(0, 0, -1, 0));
        }

        [Test]
        public void Winding()
        {
            var points = Facet.ComputeWinding(new Hyperplane3D(1, 1, 1, 0), 10);
            Assert.AreEqual(points[0], new Point3D(-20, 10, 10));
            Assert.AreEqual(points[1], new Point3D(0, -10, 10));
            Assert.AreEqual(points[2], new Point3D(20, -10, -10));
            Assert.AreEqual(points[3], new Point3D(0, 10, -10));
        }


        const string Dir = @"..\..\..\..\..\quakemaps";

        [Test]
        public void DM2()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            var output = MapFileFormat.Load(Path.Combine(Dir, "DM2.MAP"));
            Assert.AreEqual(271, output.Length);
            var worldSpawn = output.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var facets = worldSpawn.Brushes.SelectMany(Facet.Facetize).ToList();
            Assert.AreEqual(7239, facets.Count);
            var tree = new QuakeBSP(new QuakeExhaustivePartitioner(1, 10))
                .ConstructBspTree(facets);
            Assert.AreEqual(8667, tree.NodeCount);
        }

        [Test]
        public void All()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            foreach (var file in Directory.GetFiles(Dir, "*.MAP"))
            {
                var entities = MapFileFormat.Load(file);
                var worldSpawn = entities.Single(
                    entity => entity.Attributes["classname"] == "worldspawn");
                var facets = worldSpawn.Brushes.SelectMany(Facet.Facetize).ToList();
            }
        }
    }
}

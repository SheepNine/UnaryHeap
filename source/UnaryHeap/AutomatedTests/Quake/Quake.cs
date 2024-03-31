using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;
using UnaryHeap.DataType.Tests;

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

        public static Hyperplane3D GetHyperplane(MapPlane plane)
        {
            return new Hyperplane3D(plane.P3, plane.P2, plane.P1);
        }

        public Facet(Hyperplane3D plane) : this(plane, plane.MakePolytope(100000))
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
            Assert.NotNull(tree); // Not much else to assert on
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

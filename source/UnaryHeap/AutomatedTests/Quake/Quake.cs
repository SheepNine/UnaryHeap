using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;
using UnaryHeap.Utilities;

namespace Quake
{
    static class ExtensionMethods
    {
        public static List<QuakeSurface> Facetize(this MapBrush brush, Rational radius)
        {
            var result = brush.Planes.Select((plane, i) =>
            {
                var facet = new Facet3D(plane.GetHyperplane(), radius);
                foreach (var j in Enumerable.Range(0, brush.Planes.Count))
                {
                    if (facet == null)
                        break;
                    if (i == j)
                        continue;
                    facet.Split(GetHyperplane(brush.Planes[j]),
                        out Facet3D front, out Facet3D back);
                    facet = back;
                }
                return facet == null ? null : new QuakeSurface(facet, plane);
            }).Where(plane => plane != null).ToList();

            if (result.Count < 4)
            {
                throw new Exception("Degenerate brush");
            }

            return result;
        }

        public static Hyperplane3D GetHyperplane(this MapPlane plane)
        {
            return new Hyperplane3D(
                new Point3D(plane.P3X, plane.P3Y, plane.P3Z),
                new Point3D(plane.P2X, plane.P2Y, plane.P2Z),
                new Point3D(plane.P1X, plane.P1Y, plane.P1Z)
            );
        }
    }

    class QuakeExhaustivePartitioner : ExhaustivePartitioner<QuakeSurface, Hyperplane3D>
    {
        public QuakeExhaustivePartitioner(int imbalanceWeight, int splitWeight)
            : base(imbalanceWeight, splitWeight)
        {
        }

        public override void ClassifySurface(QuakeSurface surface, Hyperplane3D plane,
            out int minDeterminant, out int maxDeterminant)
        {
            if (surface.Facet.Plane == plane)
            {
                minDeterminant = 1;
                maxDeterminant = 1;
                return;
            }
            if (surface.Facet.Plane == plane.Coplane)
            {
                minDeterminant = -1;
                maxDeterminant = -1;
                return;
            }
            var determinants = surface.Facet.Points.Select(p => plane.DetermineHalfspaceOf(p));

            minDeterminant = determinants.Min();
            maxDeterminant = determinants.Max();
        }

        public override Hyperplane3D GetPlane(QuakeSurface surface)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            return surface.Facet.Plane;
        }
    }

    class QuakeBSP : BinarySpacePartitioner<QuakeSurface, Hyperplane3D>
    {
        public QuakeBSP(IPartitioner<QuakeSurface, Hyperplane3D> partitioner) : base(partitioner)
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
        protected override bool IsHintSurface(QuakeSurface surface, int depth)
        {
            return surface.TextureData.TextureName == $"HINT{depth}";
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
        protected override void Split(QuakeSurface surface, Hyperplane3D partitioningPlane,
            out QuakeSurface frontSurface, out QuakeSurface backSurface)
        {
            if (null == surface)
                throw new ArgumentNullException(nameof(surface));
            if (null == partitioningPlane)
                throw new ArgumentNullException(nameof(partitioningPlane));

            frontSurface = null;
            backSurface = null;
            surface.Facet.Split(partitioningPlane, out Facet3D frontFacet, out Facet3D backFacet);
            if (frontFacet != null)
                frontSurface = new QuakeSurface(frontFacet, surface.TextureData);
            if (backFacet != null)
                backSurface = new QuakeSurface(backFacet, surface.TextureData);
        }
    }

    class QuakeSurface
    {
        public Facet3D Facet { get; private set; }
        public MapPlane TextureData { get; private set; }

        public QuakeSurface(Facet3D facet, MapPlane textureData)
        {
            Facet = facet;
            TextureData = textureData;
        }
    }

    [TestFixture]
    public class Quake
    {
        const string Dir = @"..\..\..\..\..\quakemaps";

        [Test]
        public void DM2()
        {
            /*
                Root: (-1)x + (0)y + (0)z + (2208)
                FrontChild: (0)x + (1)y + (0)z + (832)
                BackChild: (0)x + (-1)y + (0)z + (-896)
             */
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            var output = MapFileFormat.Load(Path.Combine(Dir, "DM2.MAP"));
            Assert.AreEqual(271, output.Length);
            var worldSpawn = output.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var facets = worldSpawn.Brushes.SelectMany(brush => brush.Facetize(100000)).ToList();
            Assert.AreEqual(7239, facets.Count);
            var rootPlane = new Hyperplane3D(-1, 0, 0, 2208);
            var frontChildPlane = new Hyperplane3D(0, 1, 0, 832);
            var backChildPlane = new Hyperplane3D(0, -1, 0, -896);
            facets = facets
                .Append(new QuakeSurface(new Facet3D(rootPlane, 1),
                    new MapPlane(0, 0, 0, 0, 0, 0, 0, 0, 0, "HINT0", 0, 0, 0, 0, 0)))
                .ToList();
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
                var facets = worldSpawn.Brushes.SelectMany(brush => brush.Facetize(100000))
                    .ToList();
            }
        }
    }
}

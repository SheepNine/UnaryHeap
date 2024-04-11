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
                return facet == null ? null : new QuakeSurface(facet, plane.Texture);
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

    interface CsgBrush
    {
        IEnumerable<QuakeSurface> Surfaces { get; }
        IEnumerable<Hyperplane3D> Planes { get; }
        Orthotope3D Bounds { get; }
        int Material { get; }
    }

    static class QuakeCSG
    {
        const int AIR = 0;
        const int WATER = 1;
        const int SLIME = 2;
        const int LAVA = 3;
        const int SKY = 4;
        const int SOLID = 5;

        public static IEnumerable<QuakeSurface> ConstructSolidGeometry(
            this IList<CsgBrush> brushes)
        {
            var result = new List<QuakeSurface>();

            foreach (var i in Enumerable.Range(0, brushes.Count))
            {
                var overwrite = false;
                var sourceBrush = brushes[i];
                List<QuakeSurface> surfaces = sourceBrush.Surfaces.ToList();

                foreach (var j in Enumerable.Range(0, brushes.Count))
                {
                    if (i == j)
                    {
                        // Brushes don't clip with themselves
                        // but note that we have reached the source brush in the clip iteration
                        // to toggle the behaviour of facets coplanar with a clip brush surface
                        overwrite = true;
                        continue;
                    }

                    var clipBrush = brushes[j];

                    if (!BoundsOverlap(sourceBrush, clipBrush))
                    {
                        // All facets must lie outside of the clip brush so we can skip it
                        continue;
                    }

                    surfaces = ClipSurfaces(surfaces, clipBrush, overwrite);
                }

                result.AddRange(surfaces);
            }

            result.AddRange(result.ToList().Select(GetCosurface));

            return result;
        }

        private static QuakeSurface GetCosurface(QuakeSurface surface)
        {
            return new QuakeSurface(GetCofacet(surface.Facet), surface.Texture);
        }

        private static Facet3D GetCofacet(Facet3D facet)
        {
            throw new NotImplementedException("Already defined");
        }

        private static List<QuakeSurface> ClipSurfaces(List<QuakeSurface> surfaces,
            CsgBrush clipBrush, bool overwrite)
        {
            var result = new List<QuakeSurface>();

            foreach (var surface in surfaces)
            {
                ClipSurface(surface, clipBrush, overwrite,
                    out List<QuakeSurface> inside, out List<QuakeSurface> outside);

                result.AddRange(outside);
                result.AddRange(inside.Where(surface => surface.BackMaterial > clipBrush.Material)
                    .Select(surface => FillFront(surface, clipBrush.Material)));
            }

            return result;
        }

        private static void ClipSurface(QuakeSurface surface, CsgBrush clipBrush, bool overwrite,
            out List<QuakeSurface> inside, out List<QuakeSurface> outside)
        {
            inside = new List<QuakeSurface>();
            outside = new List<QuakeSurface>();
            var coplanar = false;

            foreach (var plane in clipBrush.Planes)
            {
                if (plane.Equals(surface.Facet.Plane))
                {
                    coplanar = true;
                    continue;
                }

                ClipSurface(surface, plane, out QuakeSurface outsidePiece, out surface);

                if (outsidePiece != null)
                    outside.Add(outsidePiece);
                if (surface == null)
                    break;
            }

            if (surface != null)
                (coplanar && !overwrite ? outside : inside).Add(surface);
        }

        private static void ClipSurface(QuakeSurface surface, Hyperplane3D plane,
            out QuakeSurface frontSurface, out QuakeSurface backSurface)
        {
            throw new NotImplementedException();
        }

        private static QuakeSurface FillFront(QuakeSurface surface, int material)
        {
            // Make a copy of surface, with a new front material
            throw new NotImplementedException();
        }

        private static bool BoundsOverlap(CsgBrush a, CsgBrush b)
        {
            return a.Bounds.Intersects(b.Bounds);
        }
    }

    class QuakeBSP : Spatial3D<QuakeSurface>
    {
        public static readonly QuakeBSP Instance = new();
        private QuakeBSP() : base(new QuakeDimension()) { }

        class QuakeDimension : Dimension
        {
            public override Orthotope3D CalculateBounds(IEnumerable<QuakeSurface> surfaces)
            {
                return Orthotope3D.FromPoints(
                    surfaces.SelectMany(surface => surface.Facet.Points));
            }

            public override Facet3D GetFacet(QuakeSurface surface)
            {
                return surface.Facet;
            }

            public override bool IsHintSurface(QuakeSurface surface, int depth)
            {
                return surface.Texture.Name == $"HINT{depth}";
            }

            public override void Split(QuakeSurface surface, Hyperplane3D partitioningPlane,
                out QuakeSurface frontSurface, out QuakeSurface backSurface)
            {
                if (null == surface)
                    throw new ArgumentNullException(nameof(surface));
                if (null == partitioningPlane)
                    throw new ArgumentNullException(nameof(partitioningPlane));

                frontSurface = null;
                backSurface = null;
                surface.Facet.Split(partitioningPlane,
                    out Facet3D frontFacet, out Facet3D backFacet);
                if (frontFacet != null)
                    frontSurface = new QuakeSurface(frontFacet, surface.Texture);
                if (backFacet != null)
                    backSurface = new QuakeSurface(backFacet, surface.Texture);
            }
        }
    }

    class QuakeSurface
    {
        public Facet3D Facet { get; private set; }
        public PlaneTexture Texture { get; private set; }
        public int FrontMaterial { get; private set; }
        public int BackMaterial { get; private set; }

        public QuakeSurface(Facet3D facet, PlaneTexture texture)
        {
            Facet = facet;
            Texture = texture;
        }
    }

    [TestFixture]
    public class Quake
    {
        const string Dir = @"..\..\..\..\..\quakemaps";

        [Test]
        public void E1M1()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            var output = MapFileFormat.Load(Path.Combine(Dir, "E1M1.MAP"));
            var worldSpawn = output.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var facets = worldSpawn.Brushes
                .SelectMany(brush => brush.Facetize(100000)).ToList();
            var tree = QuakeBSP.Instance.ConstructBspTree(
                QuakeBSP.Instance.ExhaustivePartitionStrategy(1, 10), facets);

            var vertsAndnormals = new List<float>();
            var indices = new List<int>();
            var i = 0;
            tree.InOrderTraverse((node) =>
            {
                if (!node.IsLeaf)
                    return;

                foreach (var surface in node.Surfaces)
                {
                    var facet = surface.Facet;
                    var plane = facet.Plane;
                    var normalLength = Math.Sqrt(
                        (double)(plane.A.Squared + plane.B.Squared + plane.C.Squared));

                    foreach(var point in facet.Points)
                    {
                        vertsAndnormals.Add(Convert.ToSingle((double)point.X / 10.0));
                        vertsAndnormals.Add(Convert.ToSingle((double)point.Y / 10.0));
                        vertsAndnormals.Add(Convert.ToSingle((double)point.Z / 10.0));
                        vertsAndnormals.Add(Convert.ToSingle((double)plane.A / normalLength));
                        vertsAndnormals.Add(Convert.ToSingle((double)plane.B / normalLength));
                        vertsAndnormals.Add(Convert.ToSingle((double)plane.C / normalLength));
                    }
                    for (int j = facet.Points.Count() - 1; j > 1; j--)
                    {
                        indices.Add(i + j);
                        indices.Add(i + j - 1);
                        indices.Add(i);
                    }
                    i += facet.Points.Count();
                }
            });

            using (var writer = new BinaryWriter(File.Create("e1m1.raw")))
            {
                writer.Write(vertsAndnormals.Count / 6);
                foreach (var coord in vertsAndnormals)
                    writer.Write(coord);
                writer.Write(indices.Count);
                foreach (var index in indices)
                    writer.Write(index);
            }
        }

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
                    new PlaneTexture("HINT0", 0, 0, 0, 0, 0)))
                .ToList();
            var tree = QuakeBSP.Instance.ConstructBspTree(
                QuakeBSP.Instance.ExhaustivePartitionStrategy(1, 10), facets);
            Assert.AreEqual(8667, tree.NodeCount);
            var portalSet = QuakeBSP.Instance.Portalize(tree).ToList();
            Assert.AreEqual(18265, portalSet.Count);
            var interiorPoints = output.Where(e => e.NumBrushes == 0
                    && e.Attributes.ContainsKey("origin"))
                .Select(e =>
                {
                    var tokens = e.Attributes["origin"].Split();
                    return new Point3D(
                        int.Parse(tokens[0]),
                        int.Parse(tokens[1]),
                        int.Parse(tokens[2])
                    );
                }).ToList();
            var culledTree = QuakeBSP.Instance.CullOutside(tree, portalSet, interiorPoints);
            // TODO: 8667 - 8623 = 44; kind of expected more nodes to be exterior. Is
            // the portalization giving wrong answers?
            Assert.AreEqual(8623, culledTree.NodeCount);
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

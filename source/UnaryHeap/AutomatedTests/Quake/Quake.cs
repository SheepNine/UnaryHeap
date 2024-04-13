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
        public static QuakeBSP.Brush Chungo(MapBrush brush)
        {
            var firstMapPlane = brush.Planes[0];
            var brushMaterial = QuakeBSP.SOLID;

            if (firstMapPlane.Texture.Name.StartsWith("*lava"))
                brushMaterial = QuakeBSP.LAVA;
            else if (firstMapPlane.Texture.Name.StartsWith("*slime"))
                brushMaterial = QuakeBSP.SLIME;
            else if (firstMapPlane.Texture.Name.StartsWith("*"))
                brushMaterial = QuakeBSP.WATER;
            else if (firstMapPlane.Texture.Name.StartsWith("sky"))
                brushMaterial = QuakeBSP.SKY;

            var surfaces = brush.Planes.Select((plane, i) =>
            {
                var facet = new Facet3D(plane.GetHyperplane(), 100000);
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
                return facet == null ? null : new QuakeSurface(facet, plane.Texture,
                    QuakeBSP.AIR, brushMaterial);
            }).Where(plane => plane != null).ToList();

            if (surfaces.Count < 4)
            {
                throw new Exception("Degenerate brush");
            }

            return QuakeBSP.Instance.MakeBrush(surfaces, brushMaterial);
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

    class QuakeBSP : Spatial3D<QuakeSurface>
    {
        public static readonly int AIR = 0;
        public static readonly int WATER = 1;
        public static readonly int SLIME = 2;
        public static readonly int LAVA = 3;
        public static readonly int SKY = 4;
        public static readonly int SOLID = 5;

        public static readonly QuakeBSP Instance = new();
        private QuakeBSP() : base(new QuakeDimension()) { }

        class QuakeDimension : Dimension
        {
            public override Orthotope3D CalculateBounds(IEnumerable<QuakeSurface> surfaces)
            {
                return Orthotope3D.FromPoints(
                    surfaces.SelectMany(surface => surface.Facet.Points));
            }

            public override QuakeSurface FillFront(QuakeSurface surface, int newFrontMaterial)
            {
                return new QuakeSurface(surface.Facet, surface.Texture,
                    newFrontMaterial, surface.BackMaterial);
            }

            public override int GetBackMaterial(QuakeSurface surface)
            {
                return surface.BackMaterial;
            }

            public override int GetFrontMaterial(QuakeSurface surface)
            {
                return surface.FrontMaterial;
            }

            public override QuakeSurface GetCosurface(QuakeSurface surface)
            {
                return new QuakeSurface(GetCofacet(surface.Facet), surface.Texture,
                    surface.BackMaterial, surface.FrontMaterial);
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
                    frontSurface = new QuakeSurface(frontFacet, surface.Texture,
                        surface.FrontMaterial, surface.BackMaterial);
                if (backFacet != null)
                    backSurface = new QuakeSurface(backFacet, surface.Texture,
                        surface.FrontMaterial, surface.BackMaterial);
            }
        }
    }

    class QuakeSurface
    {
        public Facet3D Facet { get; private set; }
        public PlaneTexture Texture { get; private set; }
        public int FrontMaterial { get; private set; }
        public int BackMaterial { get; private set; }

        public QuakeSurface(Facet3D facet, PlaneTexture texture,
            int frontMaterial, int backMaterial)
        {
            Facet = facet;
            Texture = texture;
            FrontMaterial = frontMaterial;
            BackMaterial = backMaterial;
        }
    }

    [TestFixture]
    public class Quake
    {
        const string Dir = @"..\..\..\..\..\quakemaps";
        const string RawOut = @"C:\Users\marsh\Documents\FirstGoLang";

        [Test]
        public void DM7()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            var entities = MapFileFormat.Load(Path.Combine(Dir, "DM7.MAP"));
            var worldSpawn = entities.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var brushes = worldSpawn.Brushes
                .Select(ExtensionMethods.Chungo).ToList();
            var surfaces = QuakeBSP.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeBSP.SOLID);
            var rawTree = QuakeBSP.Instance.ConstructBspTree(
                QuakeBSP.Instance.ExhaustivePartitionStrategy(1, 10), surfaces);
            var portals = QuakeBSP.Instance.Portalize(rawTree);
            var interiorPoints = entities.Where(e => e.NumBrushes == 0
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
            var culledTree = QuakeBSP.Instance.CullOutside(rawTree, portals, interiorPoints);

            if (Directory.Exists(RawOut))
            {
                SaveRawFile(rawTree, Path.Combine(RawOut, "dm7_nocull.raw"));
                SaveRawFile(culledTree, Path.Combine(RawOut, "dm7.raw"));
            }
        }

        [Test]
        public void Rockets()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            var entities = MapFileFormat.Load(Path.Combine(Dir, "B_ROCK0.MAP"));
            var worldSpawn = entities.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var brushes = worldSpawn.Brushes
                .Select(ExtensionMethods.Chungo).ToList();
            var surfaces = QuakeBSP.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeBSP.SOLID);
            var rawTree = QuakeBSP.Instance.ConstructBspTree(
                QuakeBSP.Instance.ExhaustivePartitionStrategy(1, 10), surfaces);

            if (Directory.Exists(RawOut))
                SaveRawFile(rawTree, Path.Combine(RawOut, "b_rock0.raw"));
        }

        static void SaveRawFile(QuakeBSP.BspNode culledTree, string filename)
        {
            var vertsAndnormals = new List<float>();
            var indices = new List<int>();
            var i = 0;
            culledTree.InOrderTraverse((node) =>
            {
                if (!node.IsLeaf)
                    return;

                foreach (var surface in node.Surfaces)
                {
                    var facet = surface.Facet;
                    var plane = facet.Plane;
                    var normalLength = Math.Sqrt(
                        (double)(plane.A.Squared + plane.B.Squared + plane.C.Squared));

                    foreach (var point in facet.Points)
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

            using (var writer = new BinaryWriter(File.Create(filename)))
            {
                writer.Write(vertsAndnormals.Count / 6);
                foreach (var coord in vertsAndnormals)
                    writer.Write(coord);
                writer.Write(indices.Count);
                foreach (var index in indices)
                    writer.Write(index);
            }
        }

        static QuakeBSP.Brush AABB(int material, Rational minX, Rational minY, Rational minZ,
            Rational maxX, Rational maxY, Rational maxZ)
        {
            var texture = new PlaneTexture("bork", 0, 0, 0, 0, 0);

            return QuakeBSP.Instance.MakeBrush(
                new Orthotope3D(minX, minY, minZ, maxX, maxY, maxZ).MakeFacets().Select(f =>
                    new QuakeSurface(f.Cofacet, texture, QuakeBSP.AIR, material)
                ), material);
        }

        [Test]
        public void IntersectingBox()
        {
            var brushes = new QuakeBSP.Brush[]
            {
                AABB(QuakeBSP.SOLID,  -9, -10, -10, -8, 10, 10),
                AABB(QuakeBSP.SOLID,   8, -10, -10,  9, 10, 10),
                AABB(QuakeBSP.SOLID, -10,  -9, -10, 10, -8, 10),
                AABB(QuakeBSP.SOLID, -10,   8, -10, 10,  9, 10),
                AABB(QuakeBSP.SOLID, -10, -10,  -9, 10, 10, -8),
                AABB(QuakeBSP.SOLID, -10, -10,   8, 10, 10,  9),
                AABB(QuakeBSP.WATER, -10, -10, -10, 10, 10, -1),
            };
            var interiorPoints = new Point3D[]
            {
                new(0, 0, 0)
            };

            var surfaces = QuakeBSP.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeBSP.SOLID);

            var fullTree = QuakeBSP.Instance.ConstructBspTree(
                QuakeBSP.Instance.ExhaustivePartitionStrategy(1, 10),
                surfaces
            );

            var portals = QuakeBSP.Instance.Portalize(fullTree);

            var culledTree = QuakeBSP.Instance.CullOutside(fullTree, portals, interiorPoints);
            Assert.AreEqual(3, culledTree.NodeCount);
            Assert.AreEqual(6, culledTree.FrontChild.SurfaceCount);
            Assert.AreEqual(6, culledTree.BackChild.SurfaceCount);

            if (Directory.Exists(RawOut))
                SaveRawFile(culledTree, Path.Combine(RawOut, "intersectingBox.raw"));
        }

        [Test]
        public void EdgeBox()
        {
            var brushes = new QuakeBSP.Brush[]
            {
                AABB(QuakeBSP.SOLID, -10,  -9,  -9, -9,  9,  9),
                AABB(QuakeBSP.SOLID,   9,  -9,  -9, 10,  9,  9),
                AABB(QuakeBSP.SOLID,  -9, -10,  -9,  9, -9,  9),
                AABB(QuakeBSP.SOLID,  -9,   9,  -9,  9, 10,  9),
                AABB(QuakeBSP.SOLID,  -9,  -9, -10,  9,  9, -9),
                AABB(QuakeBSP.SOLID,  -9,  -9,   9,  9,  9, 10),
                AABB(QuakeBSP.LAVA,  -11, -11, -11, 11, 11, -6),
            };
            var interiorPoints = new Point3D[]
            {
                new(0, 0, 0)
            };

            var surfaces = QuakeBSP.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeBSP.SOLID);

            var fullTree = QuakeBSP.Instance.ConstructBspTree(
                QuakeBSP.Instance.ExhaustivePartitionStrategy(1, 10),
                surfaces
            );

            var portals = QuakeBSP.Instance.Portalize(fullTree);

            var culledTree = QuakeBSP.Instance.CullOutside(fullTree, portals, interiorPoints);
            Assert.AreEqual(3, culledTree.NodeCount);
            Assert.AreEqual(6, culledTree.FrontChild.SurfaceCount);
            Assert.AreEqual(6, culledTree.BackChild.SurfaceCount);

            if (Directory.Exists(RawOut))
                SaveRawFile(culledTree, Path.Combine(RawOut, "edgeBox.raw"));
        }

        [Test]
        [Ignore("Needs conversion to CSG")]
        public void DM2()
        {
            /*
                Root: (-1)x + (0)y + (0)z + (2208)
                FrontChild: (0)x + (1)y + (0)z + (832)
                BackChild: (0)x + (-1)y + (0)z + (-896)
             */
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            /*var output = MapFileFormat.Load(Path.Combine(Dir, "DM2.MAP"));
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
                    new PlaneTexture("HINT0", 0, 0, 0, 0, 0), QuakeBSP.AIR, QuakeBSP.SOLID))
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
            Assert.AreEqual(8623, culledTree.NodeCount);*/
        }

        [Test]
        [Ignore("Needs conversion to CSG")]
        public void All()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            /*foreach (var file in Directory.GetFiles(Dir, "*.MAP"))
            {
                var entities = MapFileFormat.Load(file);
                var worldSpawn = entities.Single(
                    entity => entity.Attributes["classname"] == "worldspawn");
                var facets = worldSpawn.Brushes.SelectMany(brush => brush.Facetize(100000))
                    .ToList();
            }*/
        }
    }
}

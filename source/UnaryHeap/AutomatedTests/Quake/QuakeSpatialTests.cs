using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnaryHeap.DataType;
using UnaryHeap.Quake;

namespace Quake
{
    [TestFixture]
    public class QuakeSpatialTests
    {
        const string Dir = @"..\..\..\..\quake_map_source";

        static QuakeSpatial.Brush AABB(int material, Rational minX, Rational minY, Rational minZ,
            Rational maxX, Rational maxY, Rational maxZ)
        {
            var texture = new PlaneTexture("bork", 0, 0, 0, 0, 0);

            return QuakeSpatial.Instance.MakeBrush(
                new Orthotope3D(minX, minY, minZ, maxX, maxY, maxZ).MakeFacets().Select(f =>
                    new QuakeSurface(f.Cofacet, texture, QuakeSpatial.AIR, material)
                ), material);
        }

        [Test]
        public void IntersectingBox()
        {
            var brushes = new QuakeSpatial.Brush[]
            {
                AABB(QuakeSpatial.SOLID,  -9, -10, -10, -8, 10, 10),
                AABB(QuakeSpatial.SOLID,   8, -10, -10,  9, 10, 10),
                AABB(QuakeSpatial.SOLID, -10,  -9, -10, 10, -8, 10),
                AABB(QuakeSpatial.SOLID, -10,   8, -10, 10,  9, 10),
                AABB(QuakeSpatial.SOLID, -10, -10,  -9, 10, 10, -8),
                AABB(QuakeSpatial.SOLID, -10, -10,   8, 10, 10,  9),
                AABB(QuakeSpatial.WATER, -10, -10, -10, 10, 10, -1),
            };
            var interiorPoints = new Point3D[]
            {
                new(0, 0, 0)
            };

            var surfaces = QuakeSpatial.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeSpatial.SOLID);

            var fullTree = QuakeSpatial.Instance.ConstructBspTree(
                QuakeSpatial.Instance.ExhaustivePartitionStrategy(1, 10),
                surfaces
            );

            QuakeSpatial.Instance.Portalize(fullTree,
                out IEnumerable<QuakeSpatial.Portal> portals,
                out _);

            var culledTree = QuakeSpatial.Instance.CullOutside(fullTree, portals, interiorPoints);
            Assert.AreEqual(3, culledTree.NodeCount);
            Assert.AreEqual(6, culledTree.SurfaceCount(1));
            Assert.AreEqual(6, culledTree.SurfaceCount(2));
        }

        [Test]
        public void EdgeBox()
        {
            var brushes = new QuakeSpatial.Brush[]
            {
                AABB(QuakeSpatial.SOLID, -10,  -9,  -9, -9,  9,  9),
                AABB(QuakeSpatial.SOLID,   9,  -9,  -9, 10,  9,  9),
                AABB(QuakeSpatial.SOLID,  -9, -10,  -9,  9, -9,  9),
                AABB(QuakeSpatial.SOLID,  -9,   9,  -9,  9, 10,  9),
                AABB(QuakeSpatial.SOLID,  -9,  -9, -10,  9,  9, -9),
                AABB(QuakeSpatial.SOLID,  -9,  -9,   9,  9,  9, 10),
                AABB(QuakeSpatial.LAVA,  -11, -11, -11, 11, 11, -6),
            };
            var interiorPoints = new Point3D[]
            {
                new(0, 0, 0)
            };

            var surfaces = QuakeSpatial.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeSpatial.SOLID);

            var fullTree = QuakeSpatial.Instance.ConstructBspTree(
                QuakeSpatial.Instance.ExhaustivePartitionStrategy(1, 10),
                surfaces
            );

            QuakeSpatial.Instance.Portalize(fullTree,
                out IEnumerable<QuakeSpatial.Portal> portals,
                out _);

            var culledTree = QuakeSpatial.Instance.CullOutside(fullTree, portals, interiorPoints);
            Assert.AreEqual(3, culledTree.NodeCount);
            Assert.AreEqual(6, culledTree.SurfaceCount(1));
            Assert.AreEqual(6, culledTree.SurfaceCount(2));
        }

        [Test]
        public void TJoinSample()
        {
            var brushes = new[]
            {
                AABB(QuakeSpatial.SOLID, -10, -10, -10, -9, 10, 10),
                AABB(QuakeSpatial.SOLID, -10, -10, -10, 10, -9, 10),
                AABB(QuakeSpatial.SOLID, -10, -10, -10, 10, 10, -9),
                AABB(QuakeSpatial.SOLID,   9, -10, -10, 10, 10, 10),
                AABB(QuakeSpatial.SOLID, -10,   9, -10, 10, 10, 10),
                AABB(QuakeSpatial.SOLID, -10, -10,   9, 10, 10, 10),
                AABB(QuakeSpatial.SOLID, -10, -10, -10, -5, -4, -3),
            };
            var interiorPoints = new Point3D[]
            {
                new(0, 0, 0)
            };

            var surfaces = QuakeSpatial.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeSpatial.SOLID);

            var fullTree = QuakeSpatial.Instance.ConstructBspTree(
                QuakeSpatial.Instance.ExhaustivePartitionStrategy(1, 10),
                surfaces
            );

            QuakeSpatial.Instance.Portalize(fullTree,
                out IEnumerable<QuakeSpatial.Portal> portals, out _);

            var culledTree = QuakeSpatial.Instance.CullOutside(fullTree, portals, interiorPoints);

            QuakeSpatial.Instance.Portalize(culledTree,
                out portals, out _);

            var finalPortals = portals.ToList();

            Assert.AreEqual(3, finalPortals.Count);
        }

        [Test]
        public void SBlock3D()
        {
            var brushes = new[]
            {
                AABB(QuakeSpatial.SOLID, -1000, -1000, -1000, -900, 1000, 1000),
                AABB(QuakeSpatial.SOLID, -1000, -1000, -1000, 1000, -900, 1000),
                AABB(QuakeSpatial.SOLID, -1000, -1000, -1000, 1000, 1000, -900),
                AABB(QuakeSpatial.SOLID,   900, -1000, -1000, 1000, 1000, 1000),
                AABB(QuakeSpatial.SOLID, -1000,   900, -1000, 1000, 1000, 1000),
                AABB(QuakeSpatial.SOLID, -1000, -1000,   900, 1000, 1000, 1000),

                AABB(QuakeSpatial.SOLID, -1000, -1000, -1000, -500, -400, -300),
                AABB(QuakeSpatial.SOLID,  -500,  -400,   800, 1000, 1000, 1000),
                AABB(QuakeSpatial.SOLID,   700,  -400,  -300, 1000, 1000, 1000),
                AABB(QuakeSpatial.SOLID,  -500,   600,  -300, 1000, 1000, 1000),
            };
            var interiorPoints = new Point3D[]
            {
                new(0, 0, 0)
            };

            var surfaces = QuakeSpatial.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeSpatial.SOLID);

            var fullTree = QuakeSpatial.Instance.ConstructBspTree(
                QuakeSpatial.Instance.ExhaustivePartitionStrategy(1, 10),
                surfaces
            );

            QuakeSpatial.Instance.Portalize(fullTree,
                out IEnumerable<QuakeSpatial.Portal> portals, out _);

            var culledTree = QuakeSpatial.Instance.CullOutside(fullTree, portals, interiorPoints);

            QuakeSpatial.Instance.Portalize(culledTree,
                out portals, out _);

            var finalPortals = portals.ToList();
            // TODO: verify something IDK
        }

        [Test]
        public void VerifyInputBrushes()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            foreach (var file in Directory.GetFiles(Dir, "*.MAP"))
                VerifyMapBrushes(file);
        }

        private static void VerifyMapBrushes(string mapFileName)
        {
            Console.WriteLine($"Checking {mapFileName}");
            var entities = MapFileFormat.Load(mapFileName);
            var worldSpawn = entities.Single(
                entity => entity.Attributes["classname"] == "worldspawn");

            foreach (var mapBrush in worldSpawn.Brushes)
            {
                try
                {
                    VerifyBrush(mapBrush);
                }
                catch (Exception)
                {
                    Console.WriteLine("File: " + Path.GetFileNameWithoutExtension(mapFileName));
                    throw;
                }
            }
        }

        private static void VerifyBrush(MapBrush mapBrush)
        {
            var spatialBrush = QuakeExtensions.CreateSpatialBrush(mapBrush);
            if (spatialBrush.Surfaces.Count() == mapBrush.Planes.Count)
                return;

            foreach (var plane in mapBrush.Planes)
            {
                var hp = new Hyperplane3D(
                    new Point3D(plane.P3X, plane.P3Y, plane.P3Z),
                    new Point3D(plane.P2X, plane.P2Y, plane.P2Z),
                    new Point3D(plane.P1X, plane.P1Y, plane.P1Z));

                Console.WriteLine(plane);
                Console.WriteLine($"\t{hp}");
            }
            Assert.Fail();
        }
    }
}

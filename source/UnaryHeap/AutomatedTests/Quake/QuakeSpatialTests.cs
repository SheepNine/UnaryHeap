﻿using NUnit.Framework;
using System;
using System.Collections;
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
        const string Dir = @"..\..\..\..\..\quakemaps";
        const string RawOut = @"C:\Users\marsh\Documents\FirstGoLang";

        [Test]
        [Ignore("Use Qtwols for this now")]
        public void DM7()
        {
            //X: -608:1520   Y: -432:3072   Z: -608:288

            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            var entities = MapFileFormat.Load(Path.Combine(Dir, "E1M1.MAP"));
            var worldSpawn = entities.Single(
                entity => entity.Attributes["classname"] == "worldspawn");
            var brushes = worldSpawn.Brushes
                .Select(QuakeExtensions.CreateSpatialBrush).ToList();
            var surfaces = QuakeSpatial.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeSpatial.SOLID);
            var rawTree = QuakeSpatial.Instance.ConstructBspTree(
                QuakeSpatial.Instance.AxialPartitionStrategy(), surfaces);
            Console.WriteLine(QuakeSpatial.Instance.CalculateBoundingBox(rawTree));
            QuakeSpatial.Instance.Portalize(rawTree,
                out IEnumerable<QuakeSpatial.Portal> portals,
                out _);
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
            var culledTree = QuakeSpatial.Instance.CullOutside(rawTree, portals, interiorPoints);

            if (Directory.Exists(RawOut))
            {
                rawTree.SaveRawFile(Path.Combine(RawOut, "e1m1_nocull.raw"));
                culledTree.SaveRawFile(Path.Combine(RawOut, "e1m1.raw"));
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
                .Select(QuakeExtensions.CreateSpatialBrush).ToList();
            var surfaces = QuakeSpatial.Instance.ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != QuakeSpatial.SOLID);
            var rawTree = QuakeSpatial.Instance.ConstructBspTree(
                QuakeSpatial.Instance.ExhaustivePartitionStrategy(1, 10), surfaces);

            if (Directory.Exists(RawOut))
                rawTree.SaveRawFile(Path.Combine(RawOut, "b_rock0.raw"));
        }

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
            Assert.AreEqual(6, culledTree.FrontChild.SurfaceCount);
            Assert.AreEqual(6, culledTree.BackChild.SurfaceCount);

            if (Directory.Exists(RawOut))
                culledTree.SaveRawFile(Path.Combine(RawOut, "intersectingBox.raw"));
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
            Assert.AreEqual(6, culledTree.FrontChild.SurfaceCount);
            Assert.AreEqual(6, culledTree.BackChild.SurfaceCount);

            if (Directory.Exists(RawOut))
                culledTree.SaveRawFile(Path.Combine(RawOut, "edgeBox.raw"));
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
        public void All()
        {
            if (!Directory.Exists(Dir))
                throw new InconclusiveException("No maps to test");

            foreach (var file in Directory.GetFiles(Dir, "*.MAP"))
            {
                Console.WriteLine($"---{Path.GetFileNameWithoutExtension(file)}---");
                var entities = MapFileFormat.Load(file);
                var worldSpawn = entities.Single(
                    entity => entity.Attributes["classname"] == "worldspawn");
                
                foreach (var mapBrush in worldSpawn.Brushes)
                {
                    var spatialBrush = QuakeExtensions.CreateSpatialBrush(mapBrush);
                    if (spatialBrush.Surfaces.Count() != mapBrush.Planes.Count)
                    {
                        Console.WriteLine("Degenerate brush!");
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
        }
    }
}

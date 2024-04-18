using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;
using UnaryHeap.DataType.Tests;
using UnaryHeap.Graph;

namespace UnaryHeap.GraphAlgorithms.Tests
{
    [TestFixture]
    public class Graph2DBinarySpacePartitionerTests
    {
        [Test]
        public void ConvexBox()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    -1, 1,
                    -1, -1,
                    1, -1
                ).WithPolygon(
                    0, 1, 2, 3
                );

            var tree = builder.ConstructBspTree();
            Assert.IsTrue(tree.IsLeaf);
            Assert.AreEqual(4, tree.SurfaceCount);
            var portalSet = Portalize(tree, s => true);
            Assert.AreEqual(0, portalSet.Count);
        }

        [Test]
        public void ConvexBoxInverted()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    1, -1,
                    -1, -1,
                    -1, 1
                ).WithPolygon(
                    0, 1, 2, 3
                );

            var bspTree = builder.ConstructBspTree();
            Assert.AreEqual(7, bspTree.NodeCount);
            var portalSet = Portalize(bspTree, s => true);
            Assert.AreEqual(4, portalSet.Count);
        }

        [Test]
        public void LShape()
        {
            var builder = new GraphBuilder().WithPoints(
                    2, 2,
                    -2, 2,
                    -2, 0,
                    0, 0,
                    0, -2,
                    2, -2
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5
                );

            var tree = builder.ConstructBspTree();
            Assert.AreEqual(3, tree.NodeCount);
            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(4, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.IsLeaf);
            Assert.AreEqual(3, tree.BackChild.SurfaceCount);

            // Single split should have one portal
            var portalSet = Portalize(tree, s => true);
            CheckPortals(tree, portalSet, @"
                (F.) [0,0] -> [2,0] (B.)
            ");

            // All leaves are interior so culling should not remove anything
            var culledTree = tree.CullOutside(portalSet,
                new[] { new Point2D(1, 1) });
            Assert.AreEqual(3, culledTree.NodeCount);
        }

        [Test]
        public void LShapeReverseSplittingPlane()
        {
            var builder = new GraphBuilder().WithPoints(
                    2, 2,
                    -2, 2,
                    -2, 0,
                    0, 0,
                    0, -2,
                    2, -2
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5
                ).WithHint(
                    0, 4, 3
                );

            var tree = builder.ConstructBspTree();
            Assert.AreEqual(3, tree.NodeCount);
            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(3, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.IsLeaf);
            Assert.AreEqual(4, tree.BackChild.SurfaceCount);

            var portalSet = Portalize(tree, s => true);
            CheckPortals(tree, portalSet, @"
                (F.) [0,0] -> [0,2] (B.)
            ");
        }

        [Test]
        public void SBlock()
        {
            var builder = new GraphBuilder().WithPoints(
                1, 0,
                2, 0,
                2, 2,
                1, 2,
                1, 3,
                0, 3,
                0, 1,
                1, 1
            ).WithPolygon(
                0, 1, 2, 3, 4, 5, 6, 7
            );

            var tree = builder.ConstructBspTree();

            var portalSet = Portalize(tree, s => true);
            CheckPortals(tree, portalSet, @"
                (F.) [1,1] -> [1,2] (B.)
            ");
        }

        [Test]
        [Ignore("HERE is the bug: interior points lying on splitting planes that end up" +
            "placed in the wrong leaf")]
        public void OOfDestruction()
        {
            var builder = new GraphBuilder().WithPoints(
                3, 3,
                3, 0,
                0, 0,
                0, 3,

                2, 2,
                1, 2,
                1, 1,
                2, 1
            ).WithPolygon(
                0, 1, 2, 3
            ).WithPolygon(
                4, 5, 6, 7
            ).WithHint(
                0, 6, 5
            );

            var unculledTree = builder.ConstructBspTree();
            var portals = Portalize(unculledTree, s => true);
            var culledTree = CullOutside(unculledTree, portals, new[] { new Point2D(1, 1) });
            Assert.AreEqual(1, culledTree.NodeCount);
        }

        [Test]
        public void LShapeInverted()
        {
            var builder = new GraphBuilder().WithPoints(
                    2, 2,
                    2, -2,
                    0, -2,
                    0, 0,
                    -1, 0,
                    -2, 2
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5
                );

            var bspTree = builder.ConstructBspTree();
            Assert.AreEqual(9, bspTree.NodeCount);
        }

        [Test]
        public void LShapeHinted()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    -1, 1,
                    -1, 0,
                    0, 0,
                    0, -1,
                    1, -1
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5
                ).WithHint(
                    0, 0, 3
                );

            var tree = builder.ConstructBspTree();

            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(3, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.IsLeaf);
            Assert.AreEqual(3, tree.BackChild.SurfaceCount);

            // Single split should have one portal
            var portalSet = Portalize(tree, s => true);
            CheckPortals(tree, portalSet, @"
                (F.) [1,1] -> [0,0] (B.)
            ");
        }

        [Test]
        public void TwoRooms()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    3, 1,
                    3, 3,
                    1, 3,

                    4, 1,
                    5, 1,
                    5, 3,
                    4, 3
                ).WithPolygon(
                    0, 1, 2, 3
                ).WithPolygon(
                    4, 5, 6, 7
                );

            var tree = builder.ConstructBspTree();

            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(4, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.IsLeaf);
            Assert.AreEqual(4, tree.BackChild.SurfaceCount);

            // Separated leaves should have no portals between them
            var portalSet = Portalize(tree, s => true);
            Assert.AreEqual(0, portalSet.Count);

            var culledTree = tree.CullOutside(portalSet, new[] { new Point2D(2, 2) });
            Assert.IsTrue(culledTree.IsLeaf);
        }

        [Test]
        public void UShape()
        {
            var builder = new GraphBuilder().WithPoints(
                    2, -2,
                    2, 1,
                    0, 2,
                    -2, 1,
                    -2, -2,

                    -1, -2,
                    -1, 1,
                    1, 1,
                    1, -2
                ).WithPolygon(
                    0, 1, 2, 3, 4, 5, 6, 7, 8
                );

            var tree = builder.ConstructBspTree();

            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(3, tree.FrontChild.SurfaceCount);
            Assert.IsFalse(tree.BackChild.IsLeaf);
            Assert.IsTrue(tree.BackChild.FrontChild.IsLeaf);
            Assert.AreEqual(3, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.BackChild.IsLeaf);
            Assert.AreEqual(3, tree.FrontChild.SurfaceCount);

            // Two portals between center and edges
            var portalSet = Portalize(tree, s => true);
            CheckPortals(tree, portalSet, @"
                (F.) [-2,1] -> [-1,1] (BF.)
                (F.) [1,1] -> [2,1] (BB.)
            ");
        }

        [Test]
        public void RingRoom()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    -1, 1,
                    -1, -1,
                    1, -1,

                    2, 2,
                    2, -2,
                    -2, -2,
                    -2, 2,

                    4, 4,
                    -4, 4,
                    -4, -4,
                    4, -4
                ).WithPolygon(
                    0, 1, 2, 3
                ).WithPolygon(
                    4, 5, 6, 7
                ).WithPolygon(
                    8, 9, 10, 11
                );

            var tree = builder.ConstructBspTree();
            Assert.AreEqual(9, tree.NodeCount);

            var portalSet = Portalize(tree, s => true);
            Assert.AreEqual(4, portalSet.Count);

            var middleRoomTree = tree.CullOutside(portalSet, new[] { new Point2D(0, 0) });
            Assert.IsTrue(middleRoomTree.IsLeaf);

            var outerRingRoomBsp = tree.CullOutside(portalSet, new[] { new Point2D(3, 3) });
            Assert.AreEqual(7, outerRingRoomBsp.NodeCount);
        }

        [Test]
        public void FourCubeRoom()
        {
            var builder = new GraphBuilder().WithPoints(
                    1, 1,
                    2, 1,
                    2, -1,

                    1, -1,
                    1, -2,
                    -1, -2,

                    -1, -1,
                    -2, -1,
                    -2, 1,

                    -1, 1,
                    -1, 2,
                    1, 2
                ).WithPolygon(
                    0, 1, 2, 3
                ).WithPolygon(
                    3, 4, 5, 6
                ).WithPolygon(
                    6, 7, 8, 9
                ).WithPolygon(
                    9, 10, 11, 0
                );

            var tree = builder.ConstructBspTree();
            Assert.AreEqual(17, tree.NodeCount);

            var portalSet = Portalize(tree, s => true);
            var middleRoomTree = tree.CullOutside(portalSet, new[] { new Point2D(0, 0) });
            Assert.IsTrue(middleRoomTree.IsLeaf);
        }

        [Test]
        public void NonSolidSurfaces()
        {
            var points = new[]
            {
                new Point2D(0, 1),
                new Point2D(-1, 1),
                new Point2D(-1, -1),
                new Point2D(0, -1),
                new Point2D(1, -1),
                new Point2D(1, 1),
            };

            var sut = new Graph2D(true);
            foreach (var point in points)
                sut.AddVertex(point);
            sut.AddEdge(points[0], points[1]);
            sut.AddEdge(points[1], points[2]);
            sut.AddEdge(points[2], points[3]);

            sut.AddEdge(points[3], points[4]);
            sut.AddEdge(points[4], points[5]);
            sut.AddEdge(points[5], points[0]);

            sut.AddEdge(points[3], points[0]);
            sut.SetEdgeMetadatum(points[3], points[0], "frontsector", "1");
            sut.SetEdgeMetadatum(points[3], points[0], "backsector", "2");

            var tree = sut.ConstructBspTree();
            Assert.AreEqual(3, tree.NodeCount);
            Assert.AreEqual(4, tree.FrontChild.SurfaceCount);
            Assert.AreEqual(4, tree.BackChild.SurfaceCount);

            var portals = Portalize(tree, s => s.BackMaterial == 3).ToList();
            // TODO: this ended up working first try, but is there a geometry configuration
            // which causes the two-sided surfaces to close portals?
            Assert.AreEqual(1, portals.Count);
        }

        [Test]
        public void CsgMaterialSuperiority()
        {
            // Coplanar facets of brushes with different materials
            var start = new Point2D(1, 2);
            var end = new Point2D(3, 4);

            CheckCsgOutput(ConstructSolidGeometry(new[]
            {
                Monofacet(0, 0, 1, start, end),
                Monofacet(1, 0, 2, start, end),
            }), @"
                B1 (0) [1,2] -> [3,4] (2)
                B1 (2) [3,4] -> [1,2] (0)
            ");

            CheckCsgOutput(ConstructSolidGeometry(new[]
            {
                Monofacet(0, 0, 2, start, end),
                Monofacet(1, 0, 1, start, end),
            }), @"
                B0 (0) [1,2] -> [3,4] (2)
                B0 (2) [3,4] -> [1,2] (0)
            ");
        }

        [Test]
        public void CsgOneBrush()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(-1, -1),
                new(-1, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, points[0], points[1], points[2], points[3]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [1,1] -> [1,-1] (1)
                B0 (1) [1,-1] -> [1,1] (0)

                B0 (0) [1,-1] -> [-1,-1] (1)
                B0 (1) [-1,-1] -> [1,-1] (0)

                B0 (0) [-1,-1] -> [-1,1] (1)
                B0 (1) [-1,1] -> [-1,-1] (0)

                B0 (0) [-1,1] -> [1,1] (1)
                B0 (1) [1,1] -> [-1,1] (0)
            ");
        }

        [Test]
        public void CsgDisjointBrushes()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(-1, -1),
                new(-1, 1),

                new(4, 1),
                new(4, -1),
                new(3, -1),
                new(3, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, points[0], points[1], points[2], points[3]),
                MakeBrush(1, 1, points[4], points[5], points[6], points[7]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [1,1] -> [1,-1] (1)
                B0 (1) [1,-1] -> [1,1] (0)

                B0 (0) [1,-1] -> [-1,-1] (1)
                B0 (1) [-1,-1] -> [1,-1] (0)

                B0 (0) [-1,-1] -> [-1,1] (1)
                B0 (1) [-1,1] -> [-1,-1] (0)

                B0 (0) [-1,1] -> [1,1] (1)
                B0 (1) [1,1] -> [-1,1] (0)

                B1 (0) [4,1] -> [4,-1] (1)
                B1 (1) [4,-1] -> [4,1] (0)

                B1 (0) [4,-1] -> [3,-1] (1)
                B1 (1) [3,-1] -> [4,-1] (0)

                B1 (0) [3,-1] -> [3,1] (1)
                B1 (1) [3,1] -> [3,-1] (0)

                B1 (0) [3,1] -> [4,1] (1)
                B1 (1) [4,1] -> [3,1] (0)
            ");
        }

        [Test]
        public void CsgButteBrushes()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(0, -1),
                new(-1, -1),
                new(-1, 1),
                new(0, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, points[0], points[1], points[2], points[5]),
                MakeBrush(1, 1, points[2], points[3], points[4], points[5]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [0,1] -> [1,1] (1)
                B0 (1) [1,1] -> [0,1] (0)

                B0 (0) [1,1] -> [1,-1] (1)
                B0 (1) [1,-1] -> [1,1] (0)

                B0 (0) [1,-1] -> [0,-1] (1)
                B0 (1) [0,-1] -> [1,-1] (0)

                B1 (0) [0,-1] -> [-1,-1] (1)
                B1 (1) [-1,-1] -> [0,-1] (0)

                B1 (0) [-1,-1] -> [-1,1] (1)
                B1 (1) [-1,1] -> [-1,-1] (0)

                B1 (0) [-1,1] -> [0,1] (1)
                B1 (1) [0,1] -> [-1,1] (0)
             ");
        }

        [Test]
        public void CsgButteBrushesDifferentMaterial()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(0, -1),
                new(-1, -1),
                new(-1, 1),
                new(0, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, points[0], points[1], points[2], points[5]),
                MakeBrush(1, 2, points[2], points[3], points[4], points[5]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [0,1] -> [1,1] (1)
                B0 (1) [1,1] -> [0,1] (0)

                B0 (0) [1,1] -> [1,-1] (1)
                B0 (1) [1,-1] -> [1,1] (0)

                B0 (0) [1,-1] -> [0,-1] (1)
                B0 (1) [0,-1] -> [1,-1] (0)

                B1 (2) [0,-1] -> [0,1] (1)
                B1 (1) [0,1] -> [0,-1] (2)

                B1 (0) [0,-1] -> [-1,-1] (2)
                B1 (2) [-1,-1] -> [0,-1] (0)

                B1 (0) [-1,-1] -> [-1,1] (2)
                B1 (2) [-1,1] -> [-1,-1] (0)

                B1 (0) [-1,1] -> [0,1] (2)
                B1 (2) [0,1] -> [-1,1] (0)
            ");
        }

        [Test]
        public void CsgCoplanarFaces()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(-1, -1),
                new(-1, 1),
                new(0, 1),
            };

            var brushes = new[]
            {
                MakeBrush(0, 1, points[0], points[1], points[2], points[4]),
                MakeBrush(1, 1, points[4], points[1], points[2], points[3]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [0,1] -> [1,1] (1)
                B0 (1) [1,1] -> [0,1] (0)

                B0 (0) [1,1] -> [1,-1] (1)
                B0 (1) [1,-1] -> [1,1] (0)

                B1 (0) [1,-1] -> [-1,-1] (1)
                B1 (1) [-1,-1] -> [1,-1] (0)

                B1 (0) [-1,-1] -> [-1,1] (1)
                B1 (1) [-1,1] -> [-1,-1] (0)

                B1 (0) [-1,1] -> [0,1] (1)
                B1 (1) [0,1] -> [-1,1] (0)
            ");
        }

        [Test]
        public void AABB_DslTest()
        {
            var brushes = new[]
            {
                AABB(0, 1, 1, 2, 3, 4),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [1,4] -> [3,4] (1)
                B0 (0) [3,4] -> [3,2] (1)
                B0 (0) [3,2] -> [1,2] (1)
                B0 (0) [1,2] -> [1,4] (1)

                B0 (1) [3,4] -> [1,4] (0)
                B0 (1) [3,2] -> [3,4] (0)
                B0 (1) [1,2] -> [3,2] (0)
                B0 (1) [1,4] -> [1,2] (0)
            ");
        }

        [Test]
        public void MaterialPrecedence_HigherOverrides()
        {
            var brushes = new[]
            {
                AABB(0, 1, 0, 0, 10, 10),
                AABB(1, 2, 5, 0, 15, 10),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B1 (0) [15,0] -> [10,0] (2)
                B1 (0) [10,0] -> [5,0] (2)
                B0 (0) [5,0] -> [0,0] (1)
                B0 (0) [0,0] -> [0,10] (1)
                B0 (0) [0,10] -> [5,10] (1)
                B1 (0) [5,10] -> [10,10] (2)
                B1 (0) [10,10] -> [15,10] (2)
                B1 (0) [15,10] -> [15,0] (2)

                B0 (1) [0,0] -> [5,0] (0)
                B1 (1) [5,0] -> [5,10] (2)
                B0 (1) [5,10] -> [0,10] (0)
                B0 (1) [0,10] -> [0,0] (0)

                B1 (2) [5,0] -> [10,0] (0)
                B1 (2) [10,0] -> [15,0] (0)
                B1 (2) [15,0] -> [15,10] (0)
                B1 (2) [15,10] -> [10,10] (0)
                B1 (2) [10,10] -> [5,10] (0)
                B1 (2) [5,10] -> [5,0] (1)
            ");
        }

        [Test]
        public void MaterialPrecedence_LowerOverrides()
        {
            var brushes = new[]
            {
                AABB(0, 2, 5, 0, 15, 10),
                AABB(1, 1, 0, 0, 10, 10),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B0 (0) [15,0] -> [10,0] (2)
                B0 (0) [10,0] -> [5,0] (2)
                B1 (0) [5,0] -> [0,0] (1)
                B1 (0) [0,0] -> [0,10] (1)
                B1 (0) [0,10] -> [5,10] (1)
                B0 (0) [5,10] -> [10,10] (2)
                B0 (0) [10,10] -> [15,10] (2)
                B0 (0) [15,10] -> [15,0] (2)

                B1 (1) [0,0] -> [5,0] (0)
                B0 (1) [5,0] -> [5,10] (2)
                B1 (1) [5,10] -> [0,10] (0)
                B1 (1) [0,10] -> [0,0] (0)

                B0 (2) [5,0] -> [10,0] (0)
                B0 (2) [10,0] -> [15,0] (0)
                B0 (2) [15,0] -> [15,10] (0)
                B0 (2) [15,10] -> [10,10] (0)
                B0 (2) [10,10] -> [5,10] (0)
                B0 (2) [5,10] -> [5,0] (1)
            ");
        }

        [Test]
        public void ButteJoins()
        {
            const int SOLID = 10;
            var points = new Point2D[]
            {
                new(2, 1),
                new(-2, 1),
                new(-2, 2),
                new(2, 2),

                new(2, -2),
                new(-2, -2),
                new(-2, -1),
                new(2, -1),

                new(2, 3),
                new(3, 3),
                new(3, -3),
                new(2, -3),

                new(-3, 3),
                new(-2, 3),
                new(-2, -3),
                new(-3, -3),
            };

            var brushes = new[]
            {
                MakeBrush(0, SOLID, points[0], points[1], points[2], points[3]),
                MakeBrush(1, SOLID, points[4], points[5], points[6], points[7]),
                MakeBrush(2, SOLID, points[8], points[9], points[10], points[11]),
                MakeBrush(3, SOLID, points[12], points[13], points[14], points[15]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B2 (0) [2,2] -> [2,3] (10)
                B2 (0) [2,3] -> [3,3] (10)
                B2 (0) [3,3] -> [3,2] (10)
                B2 (0) [3,2] -> [3,1] (10)
                B2 (0) [3,1] -> [3,-1] (10)
                B2 (0) [3,-1] -> [3,-2] (10)
                B2 (0) [3,-2] -> [3,-3] (10)
                B2 (0) [3,-3] -> [2,-3] (10)
                B2 (0) [2,-3] -> [2,-2] (10)
                B1 (0) [2,-2] -> [-2,-2] (10)
                B3 (0) [-2,-2] -> [-2,-3] (10)
                B3 (0) [-2,-3] -> [-3,-3] (10)
                B3 (0) [-3,-3] -> [-3,-2] (10)
                B3 (0) [-3,-2] -> [-3,1] (10)
                B3 (0) [-3,1] -> [-3,3] (10)
                B3 (0) [-3,3] -> [-2,3] (10)
                B3 (0) [-2,3] -> [-2,2] (10)
                B0 (0) [-2,2] -> [2,2] (10)

                B0 (0) [2,1] -> [-2,1] (10)
                B3 (0) [-2,1] -> [-2,-1] (10)
                B1 (0) [-2,-1] -> [2,-1] (10)
                B2 (0) [2,-1] -> [2,1] (10)

                B0 (10) [-2,1] -> [2,1] (0)
                B3 (10) [-2,-1] -> [-2,1] (0)
                B1 (10) [2,-1] -> [-2,-1] (0)
                B2 (10) [2,1] -> [2,-1] (0)

                B2 (10) [2,3] -> [2,2] (0)
                B0 (10) [2,2] -> [-2,2] (0)
                B3 (10) [-2,2] -> [-2,3] (0)
                B3 (10) [-2,3] -> [-3,3] (0)
                B3 (10) [-3,3] -> [-3,1] (0)
                B3 (10) [-3,1] -> [-3,-2] (0)
                B3 (10) [-3,-2] -> [-3,-3] (0)
                B3 (10) [-3,-3] -> [-2,-3] (0)
                B3 (10) [-2,-3] -> [-2,-2] (0)
                B1 (10) [-2,-2] -> [2,-2] (0)
                B2 (10) [2,-2] -> [2,-3] (0)
                B2 (10) [2,-3] -> [3,-3] (0)
                B2 (10) [3,-3] -> [3,-2] (0)
                B2 (10) [3,-2] -> [3,-1] (0)
                B2 (10) [3,-1] -> [3,1] (0)
                B2 (10) [3,1] -> [3,2] (0)
                B2 (10) [3,2] -> [3,3] (0)
                B2 (10) [3,3] -> [2,3] (0)
            ");

            var tree = ConstructBspTree(
                surfaces.Where(s => s.FrontMaterial != SOLID));

            var portalSet = Portalize(tree, s => s.BackMaterial == SOLID);
            var middleRoomTree = tree.CullOutside(portalSet, new[] { new Point2D(0, 0) });
            Assert.IsTrue(middleRoomTree.IsLeaf);
        }

        [Test]
        public void PortalWaterBlob()
        {
            const int SOLID = 10;
            const int WATER = 5;

            var brushes = new[]
            {
                AABB(0, SOLID, -10, -10, -9, 10),
                AABB(1, SOLID,   9, -10, 10, 10),
                AABB(2, SOLID, -10, -10, 10, -9),
                AABB(3, SOLID, -10,   9, 10, 10),
                AABB(4, WATER, -10, -10,  5,  5),
            };

            var interiorPoints = new Point2D[]
            {
                new(0, 0),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            var rawTree = ConstructBspTree(
                surfaces.Where(s => s.FrontMaterial != SOLID));
            var rawPortals = Portalize(rawTree, (s) => s.BackMaterial == SOLID).ToList();
            var culledTree = CullOutside(
                rawTree, rawPortals, interiorPoints);
            var cullPortals = Portalize(culledTree, (s) => s.BackMaterial == SOLID).ToList();
            CheckPortals(culledTree, cullPortals, @"
                (F.) [5,5] -> [9,5] (BF.)
                (F.) [-9,5] -> [5,5] (BB.)
                (BF.) [5,5] -> [5,-9] (BB.)
            ");
        }

        [Test]
        public void PortalWaterBlob_NonAABB()
        {
            const int SOLID = 10;
            const int WATER = 5;

            var brushes = new[]
            {
                AABB(0, SOLID, -10, -10, -9, 10),
                AABB(1, SOLID,   9, -10, 10, 10),
                AABB(2, SOLID, -10, -10, 10, -9),
                AABB(3, SOLID, -10,   9, 10, 10),
                AABB(4, WATER,  -9,  -9,  5,  5),
            };

            var interiorPoints = new Point2D[]
            {
                new(0, 0),
            };

            var transform = AffineMapping
                .From(Point2D.Origin, new Point2D(1, 0), new Point2D(0, 1))
                .Onto(Point2D.Origin, new Point2D(1, 1), new Point2D(-1, 1));

            var surfaces = Transform(transform,
                ConstructSolidGeometry(brushes));
            interiorPoints = Tranfsform(transform, interiorPoints);

            var rawTree = ConstructBspTree(
                surfaces.Where(s => s.FrontMaterial != SOLID));
            var rawPortals = Portalize(rawTree, (s) => s.BackMaterial == SOLID).ToList();
            var culledTree = CullOutside(
                rawTree, rawPortals, interiorPoints);
            var cullPortals = Portalize(culledTree, (s) => s.BackMaterial == SOLID).ToList();
            CheckPortals(culledTree, cullPortals, @"
                (F.) [0,10] -> [4,14] (BF.)
                (F.) [-14,-4] -> [0,10] (BB.)
                (BF.) [0,10] -> [14,-4] (BB.)
            ");
        }

        #region Test DSL

        static List<GraphSegment> ConstructSolidGeometry(IList<GraphSpatial.Brush> brushes)
        {
            return GraphSpatial.Instance.ConstructSolidGeometry(brushes).ToList();
        }

        static void CheckCsgOutput(IEnumerable<GraphSegment> segments, string expected)
        {
            var actualLines = segments.Select(segment => $"{segment.Source.Metadata["brush"]} "
                + $"({segment.FrontMaterial}) [{segment.Facet.Start}] -> [{segment.Facet.End}] "
                + $"({segment.BackMaterial})").ToList();
            var expectedLines = expected.Split(Environment.NewLine)
                .Select(s => s.Trim())
                .Where(s => !s.StartsWith("//"))
                .Where(s => s.Length > 0).ToList();
            CollectionAssert.AreEquivalent(expectedLines, actualLines);
        }

        static GraphSpatial.BspNode ConstructBspTree(IEnumerable<GraphSegment> surfaces)
        {
            return GraphSpatial.Instance.ConstructBspTree(
                GraphSpatial.Instance.ExhaustivePartitionStrategy(1, 10),
                surfaces);
        }

        static List<GraphSpatial.Portal> Portalize(GraphSpatial.BspNode tree,
            Func<GraphSegment, bool> solidPredicate)
        {
            GraphSpatial.Instance.Portalize(tree, solidPredicate,
                out IEnumerable<GraphSpatial.Portal> portals,
                out _);
            return portals.ToList();
        }

        static void CheckPortals(GraphSpatial.BspNode tree,
            IEnumerable<GraphSpatial.Portal> portals, string expected)
        {
            var nodeNames = NameNodes(tree);

            var actualLines = portals.Select(portal =>
            {
                var frontNodeName = nodeNames[portal.Front];
                var backNodeName = nodeNames[portal.Back];
                var startPoint = portal.Facet.Start;
                var endPoint = portal.Facet.End;

                if (string.CompareOrdinal(frontNodeName, backNodeName) >= 0)
                    return $"({frontNodeName}) [{startPoint}] -> [{endPoint}] ({backNodeName})";
                else
                    return $"({backNodeName}) [{endPoint}] -> [{startPoint}] ({frontNodeName})";

            }).ToList();

            if (expected == null)
            {
                Console.WriteLine(string.Join(Environment.NewLine, actualLines));
                Assert.Fail("Set up expectation");
            }

            var expectedLines = expected.Split(Environment.NewLine)
                .Select(s => s.Trim())
                .Where(s => !s.StartsWith("//"))
                .Where(s => s.Length > 0).ToList();
            CollectionAssert.AreEquivalent(expectedLines, actualLines);
        }

        static IDictionary<GraphSpatial.BspNode, string> NameNodes(
            GraphSpatial.BspNode root)
        {
            var result = new Dictionary<GraphSpatial.BspNode, string>();
            NameNodes(root, string.Empty, result);
            return result;
        }

        static void NameNodes(GraphSpatial.BspNode node, string name,
            IDictionary<GraphSpatial.BspNode, string> result)
        {
            if (node.IsLeaf)
            {
                result.Add(node, name + ".");
            }
            else
            {
                result.Add(node, name + "-");
                NameNodes(node.FrontChild, name + "F", result);
                NameNodes(node.BackChild, name + "B", result);
            }
        }


        static GraphSpatial.BspNode CullOutside(GraphSpatial.BspNode rawTree,
            List<GraphSpatial.Portal> rawPortals, IEnumerable<Point2D> interiorPoints)
        {
            return GraphSpatial.Instance.CullOutside(rawTree, rawPortals, interiorPoints);
        }

        static GraphSegment MakeHintSurface(Point2D start, Point2D end, int level)
        {
            var metadata = new Dictionary<string, string>() { { "hint", $"{level}" } };
            return new GraphSegment(new GraphLine(start, end, metadata), 0, 0);
        }

        static GraphSpatial.Brush MakeBrush(int index, int material, params Point2D[] points)
        {
            var metadata = new Dictionary<string, string>() { { "brush", $"B{index}" } };

            return GraphSpatial.Instance.MakeBrush(
                Enumerable.Range(0, points.Length).Select(i =>
                new GraphSegment(new GraphLine(points[i], points[(i + 1) % points.Length],
                    metadata), 0, material)), material);
        }

        static GraphSpatial.Brush AABB(int index, int material,
            Rational minX, Rational minY, Rational maxX, Rational maxY)
        {
            var metadata = new Dictionary<string, string>() { { "brush", $"B{index}" } };

            var facets = new Orthotope2D(minX, minY, maxX, maxY).MakeFacets();
            var lines = facets.Select(f => f.Cofacet)
                .Select(f => new GraphLine(f.Start, f.End, metadata));

            return GraphSpatial.Instance.MakeBrush(
                lines.Select(line => new GraphSegment(line, 0, material)), material);
        }

        static GraphSpatial.Brush Monofacet(int index, int frontMaterial, int backMaterial,
            Point2D from, Point2D to)
        {
            var metadata = new Dictionary<string, string>() { { "brush", $"B{index}" } };
            var facet = new Facet2D(new Hyperplane2D(from, to), from, to);
            var line = new GraphLine(facet.Start, facet.End, metadata);
            var segment = new GraphSegment(line, frontMaterial, backMaterial);

            return GraphSpatial.Instance.MakeBrush(
                new[] { segment }, backMaterial);
        }

        #region Affine transformation of points

        static IEnumerable<GraphSegment> Transform(Matrix3D transform,
            IEnumerable<GraphSegment> segments)
        {
            return segments.Select(s => new GraphSegment(Transform(transform, s.Facet),
                s.Source, s.FrontMaterial, s.BackMaterial));
        }

        static Facet2D Transform(Matrix3D transform, Facet2D facet)
        {
            var tStart = AffineTransform(transform, facet.Start);
            var tEnd = AffineTransform(transform, facet.End);
            return new Facet2D(new Hyperplane2D(tStart, tEnd), tStart, tEnd);
        }

        static Point2D[] Tranfsform(Matrix3D transform, IEnumerable<Point2D> points)
        {
            return points.Select(p => AffineTransform(transform, p)).ToArray();
        }

        static Point2D AffineTransform(Matrix3D transform, Point2D point)
        {
            return (transform * point.Homogenized()).Dehomogenized();
        }

        #endregion

        class GraphBuilder
        {
            readonly Graph2D graph = new Graph2D(true);
            readonly List<Point2D> points = new List<Point2D>();

            public GraphBuilder WithPoints(params Rational[] pointXYs)
            {
                if (pointXYs.Length % 2 == 1)
                    throw new ArgumentException("Missing Y value for last point!");

                for (var i = 0; i < pointXYs.Length; i += 2)
                {
                    var point = new Point2D(pointXYs[i], pointXYs[i + 1]);
                    graph.AddVertex(point);
                    points.Add(point);
                }

                return this;
            }

            public GraphBuilder WithPolygon(params int[] indices)
            {
                foreach (var i in Enumerable.Range(0, indices.Length))
                    graph.AddEdge(points[indices[i]], points[indices[(i + 1) % indices.Length]]);

                return this;
            }

            public GraphBuilder WithHint(int depth, int p1index, int p2index)
            {
                graph.AddEdge(points[p1index], points[p2index]);
                graph.SetEdgeMetadatum(points[p1index], points[p2index], "hint", $"{depth}");

                return this;
            }

            public GraphSpatial.BspNode ConstructBspTree()
            {
                return graph.ConstructBspTree();
            }
        }

        #endregion
    }
}

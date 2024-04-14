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
        public void JustTwoFacetsNbd()
        {
            var points = new Point2D[]
            {
                new (1, -1),
                new (-1, 0),
                new (-1, 1),
                new (-1, 2),
            };
            var graph = new Graph2D(true);
            foreach (var point in points)
                graph.AddVertex(point);
            graph.AddEdge(points[0], points[1]);
            graph.AddEdge(points[1], points[2]);
            graph.AddEdge(points[2], points[3]);
            graph.SetEdgeMetadatum(points[2], points[3], "hint", "0");


            var tree = graph.ConstructBspTree();
            Assert.AreEqual(3, tree.NodeCount);
            Assert.AreEqual(new Hyperplane2D(-1, 0, -1), tree.PartitionPlane);

            var portals = Portalize(tree).ToList();
            Assert.AreEqual(0, portals.Count);
        }

        [Test]
        public void ConvexBox()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(-1, 1),
                new(-1, -1),
                new(1, -1),
            };
            var graph = new Graph2D(true);
            foreach (var point in points)
                graph.AddVertex(point);
            graph.AddEdge(points[0], points[1]);
            graph.AddEdge(points[1], points[2]);
            graph.AddEdge(points[2], points[3]);
            graph.AddEdge(points[3], points[0]);

            var tree = graph.ConstructBspTree();
            Assert.IsTrue(tree.IsLeaf);
            Assert.AreEqual(4, tree.SurfaceCount);
            var portalSet = tree.Portalize().ToList();
            Assert.AreEqual(0, portalSet.Count);
        }

        [Test]
        public void ConvexBoxInverted()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(1, -1),
                new(-1, -1),
                new(-1, 1),
            };
            var graph = new Graph2D(true);
            foreach (var point in points)
                graph.AddVertex(point);
            graph.AddEdge(points[0], points[1]);
            graph.AddEdge(points[1], points[2]);
            graph.AddEdge(points[2], points[3]);
            graph.AddEdge(points[3], points[0]);

            var bspTree = graph.ConstructBspTree();
            Assert.AreEqual(7, bspTree.NodeCount);
            var portalSet = bspTree.Portalize().ToList();
            Assert.AreEqual(0, portalSet.Count);
        }

        [Test]
        public void LShape()
        {
            var points = new Point2D[]
            {
                new(2, 2),
                new(-2, 2),
                new(-2, 0),
                new(0, 0),
                new(0, -2),
                new(2, -2),
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

            var tree = sut.ConstructBspTree();
            Assert.AreEqual(3, tree.NodeCount);
            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(4, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.IsLeaf);
            Assert.AreEqual(3, tree.BackChild.SurfaceCount);

            // Single split should have one portal
            var portalSet = tree.Portalize().ToList();
            Assert.AreEqual(1, portalSet.Count);
            Assert.AreEqual(new Point2D(2, 0), portalSet[0].Facet.Start);
            Assert.AreEqual(new Point2D(0, 0), portalSet[0].Facet.End);

            // All leaves are interior so culling should not remove anything
            var culledTree = tree.CullOutside(portalSet,
                new[] { new Point2D(1, 1) });
            Assert.AreEqual(3, culledTree.NodeCount);
        }

        [Test]
        public void LShapeInverted()
        {
            var points = new Point2D[]
            {
                new(2, 2),
                new(2, -2),
                new(0, -2),
                new(0, 0),
                new(-1, 0),
                new(-2, 2),
            };
            var graph = new Graph2D(true);
            foreach (var point in points)
                graph.AddVertex(point);
            graph.AddEdge(points[0], points[1]);
            graph.AddEdge(points[1], points[2]);
            graph.AddEdge(points[2], points[3]);
            graph.AddEdge(points[3], points[4]);
            graph.AddEdge(points[4], points[5]);
            graph.AddEdge(points[5], points[0]);

            var bspTree = graph.ConstructBspTree();
            Assert.AreEqual(9, bspTree.NodeCount);
            var portalSet = bspTree.Portalize().ToList();
            Assert.AreEqual(1, portalSet.Count);
            Assert.AreEqual(new Point2D(-2, 0), portalSet[0].Facet.Start);
            Assert.AreEqual(new Point2D(-1, 0), portalSet[0].Facet.End);
        }

        [Test]
        public void LShapeHinted()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(-1, 1),
                new(-1, 0),
                new(0, 0),
                new(0, -1),
                new(1, -1),
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
            sut.AddEdge(points[0], points[3]);
            sut.SetEdgeMetadatum(points[0], points[3], "hint", "0");

            var tree = sut.ConstructBspTree();

            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(3, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.IsLeaf);
            Assert.AreEqual(3, tree.BackChild.SurfaceCount);

            // Single split should have one portal
            var portalSet = tree.Portalize().ToList();
            Assert.AreEqual(1, portalSet.Count);
            // Expectations are brittle; are order and facing sensitive
            // Expectations don't check leaves
            Assert.AreEqual(new Point2D(0, 0), portalSet[0].Facet.Start);
            Assert.AreEqual(new Point2D(1, 1), portalSet[0].Facet.End);
        }

        [Test]
        public void TwoRooms()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(3, 1),
                new(3, 3),
                new(1, 3),

                new(4, 1),
                new(5, 1),
                new(5, 3),
                new(4, 3),
            };

            var sut = new Graph2D(true);
            foreach (var point in points)
                sut.AddVertex(point);
            sut.AddEdge(points[0], points[1]);
            sut.AddEdge(points[1], points[2]);
            sut.AddEdge(points[2], points[3]);
            sut.AddEdge(points[3], points[0]);
            sut.AddEdge(points[4], points[5]);
            sut.AddEdge(points[5], points[6]);
            sut.AddEdge(points[6], points[7]);
            sut.AddEdge(points[7], points[4]);

            var tree = sut.ConstructBspTree();

            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(4, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.IsLeaf);
            Assert.AreEqual(4, tree.BackChild.SurfaceCount);

            // Separated leaves should have no portals between them
            var portalSet = tree.Portalize().ToList();
            Assert.AreEqual(0, portalSet.Count);

            var culledTree = tree.CullOutside(portalSet, new[] { new Point2D(2, 2) });
            Assert.IsTrue(culledTree.IsLeaf);
        }

        [Test]
        public void UShape()
        {
            var points = new Point2D[]
            {
                new(2, -2),
                new(2, 1),
                new(0, 2),
                new(-2, 1),
                new(-2, -2),

                new(-1, -2),
                new(-1, 1),
                new(1, 1),
                new(1, -2),
            };

            var sut = new Graph2D(true);
            foreach (var point in points)
                sut.AddVertex(point);
            sut.AddEdge(points[0], points[1]);
            sut.AddEdge(points[1], points[2]);
            sut.AddEdge(points[2], points[3]);
            sut.AddEdge(points[3], points[4]);
            sut.AddEdge(points[4], points[5]);
            sut.AddEdge(points[5], points[6]);
            sut.AddEdge(points[6], points[7]);
            sut.AddEdge(points[7], points[8]);
            sut.AddEdge(points[8], points[0]);

            var tree = sut.ConstructBspTree();

            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(3, tree.FrontChild.SurfaceCount);
            Assert.IsFalse(tree.BackChild.IsLeaf);
            Assert.IsTrue(tree.BackChild.FrontChild.IsLeaf);
            Assert.AreEqual(3, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.BackChild.IsLeaf);
            Assert.AreEqual(3, tree.FrontChild.SurfaceCount);

            // Two portals between center and edges
            var portalSet = tree.Portalize().ToList();
            Assert.AreEqual(2, portalSet.Count);
            // Expectations are brittle; are order and facing sensitive
            // Expectations don't check leaves
            Assert.AreEqual(new Point2D(-1, 1), portalSet[0].Facet.Start);
            Assert.AreEqual(new Point2D(-2, 1), portalSet[0].Facet.End);
            Assert.AreEqual(new Point2D(2, 1), portalSet[1].Facet.Start);
            Assert.AreEqual(new Point2D(1, 1), portalSet[1].Facet.End);
        }

        [Test]
        public void RingRoom()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(-1, 1),
                new(-1, -1),
                new(1, -1),
                new(2, 2),
                new(2, -2),
                new(-2, -2),
                new(-2, 2),
                new(4, 4),
                new(-4, 4),
                new(-4, -4),
                new(4, -4),
            };

            var sut = new Graph2D(true);
            foreach (var point in points)
                sut.AddVertex(point);
            sut.AddEdge(points[0], points[1]);
            sut.AddEdge(points[1], points[2]);
            sut.AddEdge(points[2], points[3]);
            sut.AddEdge(points[3], points[0]);
            sut.AddEdge(points[4], points[5]);
            sut.AddEdge(points[5], points[6]);
            sut.AddEdge(points[6], points[7]);
            sut.AddEdge(points[7], points[4]);
            sut.AddEdge(points[8], points[9]);
            sut.AddEdge(points[9], points[10]);
            sut.AddEdge(points[10], points[11]);
            sut.AddEdge(points[11], points[8]);

            var tree = sut.ConstructBspTree();
            Assert.AreEqual(9, tree.NodeCount);

            var portalSet = tree.Portalize().ToList();
            Assert.AreEqual(4, portalSet.Count);

            var middleRoomTree = tree.CullOutside(portalSet, new[] { new Point2D(0, 0) });
            Assert.IsTrue(middleRoomTree.IsLeaf);

            var outerRingRoomBsp = tree.CullOutside(portalSet, new[] { new Point2D(3, 3) });
            Assert.AreEqual(7, outerRingRoomBsp.NodeCount);
        }

        [Test]
        public void FourCubeRoom()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(2, 1),
                new(2, -1),

                new(1, -1),
                new(1, -2),
                new(-1, -2),

                new(-1, -1),
                new(-2, -1),
                new(-2, 1),

                new(-1, 1),
                new(-1, 2),
                new(1, 2),
            };

            var sut = new Graph2D(true);
            foreach (var point in points)
                sut.AddVertex(point);
            sut.AddEdge(points[0], points[1]);
            sut.AddEdge(points[1], points[2]);
            sut.AddEdge(points[2], points[3]);
            sut.AddEdge(points[3], points[0]);

            sut.AddEdge(points[3], points[4]);
            sut.AddEdge(points[4], points[5]);
            sut.AddEdge(points[5], points[6]);
            sut.AddEdge(points[6], points[3]);

            sut.AddEdge(points[6], points[7]);
            sut.AddEdge(points[7], points[8]);
            sut.AddEdge(points[8], points[9]);
            sut.AddEdge(points[9], points[6]);

            sut.AddEdge(points[9], points[10]);
            sut.AddEdge(points[10], points[11]);
            sut.AddEdge(points[11], points[0]);
            sut.AddEdge(points[0], points[9]);

            var tree = sut.ConstructBspTree();
            Assert.AreEqual(17, tree.NodeCount);

            var portalSet = tree.Portalize().ToList();
            Assert.AreEqual(0, portalSet.Count);

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
            sut.SetEdgeMetadatum(points[0], points[1], "frontsector", "1");
            sut.SetEdgeMetadatum(points[1], points[2], "frontsector", "1");
            sut.SetEdgeMetadatum(points[2], points[3], "frontsector", "1");

            sut.AddEdge(points[3], points[4]);
            sut.AddEdge(points[4], points[5]);
            sut.AddEdge(points[5], points[0]);
            sut.SetEdgeMetadatum(points[3], points[4], "frontsector", "2");
            sut.SetEdgeMetadatum(points[4], points[5], "frontsector", "2");
            sut.SetEdgeMetadatum(points[5], points[0], "frontsector", "2");

            sut.AddEdge(points[3], points[0]);
            sut.SetEdgeMetadatum(points[3], points[0], "frontsector", "1");
            sut.SetEdgeMetadatum(points[3], points[0], "backsector", "2");

            var tree = sut.ConstructBspTree();
            Assert.AreEqual(3, tree.NodeCount);
            Assert.AreEqual(4, tree.FrontChild.SurfaceCount);
            Assert.AreEqual(4, tree.BackChild.SurfaceCount);

            var portals = Portalize(tree).ToList();
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
            CheckCsgOutput(surfaces,@"
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
                MakeBrush(0, 1, points[0], points[1], points[2], points[3]),
                MakeBrush(1, 1, points[4], points[5], points[6], points[7]),
                MakeBrush(2, 1, points[8], points[9], points[10], points[11]),
                MakeBrush(3, 1, points[12], points[13], points[14], points[15]),
            };

            var surfaces = ConstructSolidGeometry(brushes);
            CheckCsgOutput(surfaces, @"
                B2 (0) [2,2] -> [2,3] (1)
                B2 (0) [2,3] -> [3,3] (1)
                B2 (0) [3,3] -> [3,2] (1)
                B2 (0) [3,2] -> [3,1] (1)
                B2 (0) [3,1] -> [3,-1] (1)
                B2 (0) [3,-1] -> [3,-2] (1)
                B2 (0) [3,-2] -> [3,-3] (1)
                B2 (0) [3,-3] -> [2,-3] (1)
                B2 (0) [2,-3] -> [2,-2] (1)
                B1 (0) [2,-2] -> [-2,-2] (1)
                B3 (0) [-2,-2] -> [-2,-3] (1)
                B3 (0) [-2,-3] -> [-3,-3] (1)
                B3 (0) [-3,-3] -> [-3,-2] (1)
                B3 (0) [-3,-2] -> [-3,1] (1)
                B3 (0) [-3,1] -> [-3,3] (1)
                B3 (0) [-3,3] -> [-2,3] (1)
                B3 (0) [-2,3] -> [-2,2] (1)
                B0 (0) [-2,2] -> [2,2] (1)

                B0 (0) [2,1] -> [-2,1] (1)
                B3 (0) [-2,1] -> [-2,-1] (1)
                B1 (0) [-2,-1] -> [2,-1] (1)
                B2 (0) [2,-1] -> [2,1] (1)

                B0 (1) [-2,1] -> [2,1] (0)
                B3 (1) [-2,-1] -> [-2,1] (0)
                B1 (1) [2,-1] -> [-2,-1] (0)
                B2 (1) [2,1] -> [2,-1] (0)

                B2 (1) [2,3] -> [2,2] (0)
                B0 (1) [2,2] -> [-2,2] (0)
                B3 (1) [-2,2] -> [-2,3] (0)
                B3 (1) [-2,3] -> [-3,3] (0)
                B3 (1) [-3,3] -> [-3,1] (0)
                B3 (1) [-3,1] -> [-3,-2] (0)
                B3 (1) [-3,-2] -> [-3,-3] (0)
                B3 (1) [-3,-3] -> [-2,-3] (0)
                B3 (1) [-2,-3] -> [-2,-2] (0)
                B1 (1) [-2,-2] -> [2,-2] (0)
                B2 (1) [2,-2] -> [2,-3] (0)
                B2 (1) [2,-3] -> [3,-3] (0)
                B2 (1) [3,-3] -> [3,-2] (0)
                B2 (1) [3,-2] -> [3,-1] (0)
                B2 (1) [3,-1] -> [3,1] (0)
                B2 (1) [3,1] -> [3,2] (0)
                B2 (1) [3,2] -> [3,3] (0)
                B2 (1) [3,3] -> [2,3] (0)
            ");

            var tree = ConstructBspTree(
                surfaces.Where(s => s.FrontMaterial != 1));

            var portalSet = tree.Portalize().ToList();
            Assert.AreEqual(0, portalSet.Count);

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
            var rawPortals = Portalize(rawTree).ToList();
            CheckPortals(rawTree, rawPortals, @"
                (FFFBBB.) [5,-9] -> [5,5] (FFFBBF.)
                (FFFBBB.) [5,5] -> [-9,5] (FFFBF.)
                (FFFBBF.) [9,5] -> [5,5] (FFFBF.)
            ");
            var culledTree = CullOutside(
                rawTree, rawPortals, interiorPoints);
            var cullPortals = Portalize(culledTree).ToList();
            CheckPortals(culledTree, cullPortals, @"
                (BF.) [9,5] -> [5,5] (F.)
                (BB.) [5,5] -> [-9,5] (F.)
                (BB.) [5,-9] -> [5,5] (BF.)
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
            var rawPortals = Portalize(rawTree).ToList();
            CheckPortals(rawTree, rawPortals, @"
                (FFFFBF.) [4,14] -> [0,10] (FFFFF.)
                (FFFFBB.) [0,10] -> [-14,-4] (FFFFF.)
                (FFFFBB.) [14,-4] -> [0,10] (FFFFBF.)
                // These ones should not be here?
                (FBBF.) [-20,-2] -> [-19,-1] (FFB.)
                (BBF.) [-1,-19] -> [-2,-20] (FFB.)
                (FBBB.) [1,19] -> [2,20] (FFFB.)
                (BBB.) [20,2] -> [19,1] (FFFB.)
            ");
            var culledTree = CullOutside(
                rawTree, rawPortals, interiorPoints);
            var cullPortals = Portalize(culledTree).ToList();
            CheckPortals(culledTree, cullPortals, @"
                (BF.) [4,14] -> [0,10] (F.)
                (BB.) [0,10] -> [-14,-4] (F.)
                (BB.) [14,-4] -> [0,10] (BF.)
            ");
        }

        [Test]
        public void CShape()
        {
            const int SOLID = 1;
            var brushes = new[]
            {
                MakeBrush(0, SOLID, new Point2D[] { new(1, 0), new(0, 1), new(3, 2) }),
                MakeBrush(1, SOLID, new Point2D[] { new(0, 3), new(1, 4), new(3, 2) }),
            };
            var surfaces = ConstructSolidGeometry(brushes)
                .Where(s => s.FrontMaterial != SOLID);
            var tree = ConstructBspTree(surfaces);
            Assert.AreEqual(9, tree.NodeCount);
            var portals = Portalize(tree).ToList();
            CheckPortals(tree, portals, @"");
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

        static List<GraphSpatial.Portal> Portalize(GraphSpatial.BspNode tree)
        {
            return GraphSpatial.Instance.Portalize(tree).ToList();
        }

        static void CheckPortals(GraphSpatial.BspNode tree,
            IEnumerable<GraphSpatial.Portal> portals, string expected)
        {
            var nodeNames = NameNodes(tree);

            var actualLines = portals.Select(portal =>
                $"({nodeNames[portal.Front]}) "
                + $"[{portal.Facet.Start}] -> [{portal.Facet.End}] "
                + $"({nodeNames[portal.Back]})").ToList();

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

        #endregion
    }
}

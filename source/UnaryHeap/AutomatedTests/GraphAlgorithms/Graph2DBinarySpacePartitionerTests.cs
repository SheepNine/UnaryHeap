﻿using NUnit.Framework;
using System.Linq;
using UnaryHeap.DataType;
using UnaryHeap.Graph;

namespace UnaryHeap.GraphAlgorithms.Tests
{
    [TestFixture]
    public class Graph2DBinarySpacePartitionerTests
    {
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
        [Ignore("Correct results require first that a CSG pass be done")]
        // From QTools: See surface_t *CSGFaces (brushset_t *bs)
        // for CSG from a set of brushes (null-terminated list + extent info)
        // to a null-terminated list of surfaces
        public void ButteJoins()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(-1, 1),
                new(-1, 2),
                new(1, 2),

                new(-1, -1),
                new(1, -1),
                new(1, -2),
                new(-1, -2),

                new(2, 3),
                new(3, 3),
                new(3, -3),
                new(2, -3),

                new(-3, 3),
                new(-2, 3),
                new(-2, -3),
                new(-3, -3),
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

            sut.AddEdge(points[12], points[13]);
            sut.AddEdge(points[13], points[14]);
            sut.AddEdge(points[14], points[15]);
            sut.AddEdge(points[15], points[12]);

            var tree = sut.ConstructBspTree();
            Assert.AreEqual(17, tree.NodeCount);

            var portalSet = tree.Portalize().ToList();
            Assert.AreEqual(0, portalSet.Count);

            var middleRoomTree = tree.CullOutside(portalSet, new[] { new Point2D(0, 0) });
            Assert.IsTrue(middleRoomTree.IsLeaf);
        }
    }
}

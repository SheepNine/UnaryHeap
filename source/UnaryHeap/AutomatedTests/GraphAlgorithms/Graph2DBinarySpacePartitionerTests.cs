﻿using NUnit.Framework;
using Quake;
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
            var sut = new Graph2D(true);
            foreach (var point in points)
                sut.AddVertex(point);
            sut.AddEdge(points[0], points[1]);
            sut.AddEdge(points[1], points[2]);
            sut.AddEdge(points[2], points[3]);
            sut.AddEdge(points[3], points[0]);

            var tree = sut.ConstructBspTree();

            Assert.IsTrue(tree.IsLeaf);
            Assert.AreEqual(4, tree.SurfaceCount);

            // Single leaf should have no portals
            var portalSet = new GraphPortalizer().Portalize(tree).ToList();
            Assert.AreEqual(0, portalSet.Count);
        }

        [Test]
        public void LShape()
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

            var tree = sut.ConstructBspTree();

            Assert.IsFalse(tree.IsLeaf);
            Assert.IsTrue(tree.FrontChild.IsLeaf);
            Assert.AreEqual(4, tree.FrontChild.SurfaceCount);
            Assert.IsTrue(tree.BackChild.IsLeaf);
            Assert.AreEqual(3, tree.BackChild.SurfaceCount);

            // Single split should have one portal
            var portalSet = new GraphPortalizer().Portalize(tree).ToList();
            Assert.AreEqual(1, portalSet.Count);
            Assert.AreEqual(new Point2D(1, 0), portalSet[0].Facet.Start);
            Assert.AreEqual(new Point2D(0, 0), portalSet[0].Facet.End);
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
            var portalSet = new GraphPortalizer().Portalize(tree).ToList();
            Assert.AreEqual(1, portalSet.Count);
            Assert.AreEqual(new Point2D(0, 0), portalSet[0].Facet.Start);
            Assert.AreEqual(new Point2D(1, 1), portalSet[0].Facet.End);
        }

        [Test]
        public void TwoRooms()
        {
            var points = new Point2D[]
            {
                new(1, 1),
                new(2, 1),
                new(2, 2),
                new(1, 2),

                new(3, 1),
                new(4, 1),
                new(4, 2),
                new(3, 2),
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
            var portalSet = new GraphPortalizer().Portalize(tree).ToList();
            Assert.AreEqual(0, portalSet.Count);
        }
    }
}

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
            sut.AddVertex(points[0]);
            sut.AddVertex(points[1]);
            sut.AddVertex(points[2]);
            sut.AddVertex(points[3]);
            sut.AddEdge(points[0], points[1]);
            sut.AddEdge(points[1], points[2]);
            sut.AddEdge(points[2], points[3]);
            sut.AddEdge(points[3], points[0]);

            var result = sut.ConstructBspTree();

            Assert.IsTrue(result.IsLeaf);
            Assert.AreEqual(4, result.SurfaceCount);

            // Single leaf should have no portals
            var portalSet = new GraphPortalizer().Portalize(result);
            Assert.AreEqual(0, portalSet.Count());
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
            sut.AddVertex(points[0]);
            sut.AddVertex(points[1]);
            sut.AddVertex(points[2]);
            sut.AddVertex(points[3]);
            sut.AddVertex(points[4]);
            sut.AddVertex(points[5]);
            sut.AddEdge(points[0], points[1]);
            sut.AddEdge(points[1], points[2]);
            sut.AddEdge(points[2], points[3]);
            sut.AddEdge(points[3], points[4]);
            sut.AddEdge(points[4], points[5]);
            sut.AddEdge(points[5], points[0]);

            var result = sut.ConstructBspTree();

            Assert.IsFalse(result.IsLeaf);
            Assert.IsTrue(result.FrontChild.IsLeaf);
            Assert.AreEqual(4, result.FrontChild.SurfaceCount);
            Assert.IsTrue(result.BackChild.IsLeaf);
            Assert.AreEqual(3, result.BackChild.SurfaceCount);

            // Single split should have one portal
            var portalSet = new GraphPortalizer().Portalize(result).ToList();
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
            sut.AddVertex(points[0]);
            sut.AddVertex(points[1]);
            sut.AddVertex(points[2]);
            sut.AddVertex(points[3]);
            sut.AddVertex(points[4]);
            sut.AddVertex(points[5]);
            sut.AddEdge(points[0], points[1]);
            sut.AddEdge(points[1], points[2]);
            sut.AddEdge(points[2], points[3]);
            sut.AddEdge(points[3], points[4]);
            sut.AddEdge(points[4], points[5]);
            sut.AddEdge(points[5], points[0]);
            sut.AddEdge(points[0], points[3]);
            sut.SetEdgeMetadatum(points[0], points[3], "hint", "0");

            var result = sut.ConstructBspTree();

            Assert.IsFalse(result.IsLeaf);
            Assert.IsTrue(result.FrontChild.IsLeaf);
            Assert.AreEqual(3, result.FrontChild.SurfaceCount);
            Assert.IsTrue(result.BackChild.IsLeaf);
            Assert.AreEqual(3, result.BackChild.SurfaceCount);

            // Single split should have one portal
            var portalSet = new GraphPortalizer().Portalize(result).ToList();
            Assert.AreEqual(1, portalSet.Count);
            Assert.AreEqual(new Point2D(0, 0), portalSet[0].Facet.Start);
            Assert.AreEqual(new Point2D(1, 1), portalSet[0].Facet.End);
        }
    }
}

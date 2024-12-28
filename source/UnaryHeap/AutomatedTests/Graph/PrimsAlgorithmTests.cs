using NUnit.Framework;
using System;
using System.Linq;
using UnaryHeap.DataType;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace UnaryHeap.Graph.Tests
{
    [TestFixture]
    public class PrimsAlgorithmTests
    {
        [Test]
        public void NoEdges()
        {
            var pointA = Point2D.Origin;

            var sut = new Graph2D(false);
            sut.AddVertex(pointA);

            var result = PrimsAlgorithm.FindMinimumSpanningTree(sut, pointA);

            Assert.AreEqual(1, result.NumVertices);
            Assert.True(result.HasVertex(pointA));
        }

        [Test]
        public void TwoVertices()
        {
            var pointA = Point2D.Origin;
            var pointB = new Point2D(1, 1);

            var sut = new Graph2D(false);
            sut.AddVertex(pointA);
            sut.AddVertex(pointB);

            var resultA = PrimsAlgorithm.FindMinimumSpanningTree(sut, pointA);
            Assert.AreEqual(1, resultA.NumVertices);
            Assert.True(resultA.HasVertex(pointA));

            var resultB = PrimsAlgorithm.FindMinimumSpanningTree(sut, pointB);
            Assert.AreEqual(1, resultB.NumVertices);
            Assert.True(resultB.HasVertex(pointB));
        }

        [Test]
        public void K2Graph()
        {
            var pointA = Point2D.Origin;
            var pointB = new Point2D(1, 1);

            var sut = new Graph2D(false);
            sut.AddVertex(pointA);
            sut.AddVertex(pointB);
            sut.AddEdge(pointA, pointB);

            var result = PrimsAlgorithm.FindMinimumSpanningTree(sut, pointA);

            Assert.AreEqual(2, result.NumVertices);
            Assert.True(result.HasVertex(pointA));
            Assert.True(result.HasVertex(pointB));

            Assert.AreEqual(1, result.Edges.Count());
            Assert.True(result.HasEdge(pointA, pointB));
        }

        [Test]
        public void Triangle()
        {
            var pointA = Point2D.Origin;
            var pointB = new Point2D(1, 0);
            var pointC = new Point2D(1, 2);

            var sut = new Graph2D(false);
            sut.AddVertex(pointA);
            sut.AddVertex(pointB);
            sut.AddVertex(pointC);
            sut.AddEdge(pointA, pointB);
            sut.AddEdge(pointA, pointC);
            sut.AddEdge(pointC, pointB);

            var result = PrimsAlgorithm.FindMinimumSpanningTree(sut, pointA);

            Assert.AreEqual(2, result.Edges.Count());
            Assert.True(result.HasEdge(pointA, pointB));
            Assert.True(result.HasEdge(pointB, pointC));
        }

        [Test]
        public void TriangleWithExplicitWeights()
        {
            var pointA = Point2D.Origin;
            var pointB = new Point2D(1, 0);
            var pointC = new Point2D(1, 2);

            var sut = new Graph2D(false);
            sut.AddVertex(pointA);
            sut.AddVertex(pointB);
            sut.AddVertex(pointC);
            sut.AddEdge(pointA, pointB);
            sut.SetEdgeMetadatum(pointA, pointB, "weight", "2");
            sut.AddEdge(pointA, pointC);
            sut.SetEdgeMetadatum(pointA, pointC, "weight", "1");
            sut.AddEdge(pointC, pointB);
            sut.SetEdgeMetadatum(pointB, pointC, "weight", "3");

            var result = PrimsAlgorithm.FindMinimumSpanningTree(sut, pointA);

            Assert.AreEqual(2, result.Edges.Count());
            Assert.True(result.HasEdge(pointA, pointB));
            Assert.True(result.HasEdge(pointA, pointC));
        }

        [Test]
        public void SimpleArgumentExceptions()
        {
            var directedGraph = new Graph2D(true);
            directedGraph.AddVertex(Point2D.Origin);
            var undirectedGraph = new Graph2D(false);
            undirectedGraph.AddVertex(Point2D.Origin);

            Assert.Throws<ArgumentNullException>(
                () => { PrimsAlgorithm.FindMinimumSpanningTree(null, Point2D.Origin); });
            Assert.Throws<ArgumentNullException>(
                () => { PrimsAlgorithm.FindMinimumSpanningTree(new Graph2D(false), null); });
            Assert.Throws<ArgumentException>(
                () =>
                {
                    PrimsAlgorithm.FindMinimumSpanningTree(
                        directedGraph, Point2D.Origin);
                });
            Assert.Throws<ArgumentException>(
                () =>
                {
                    PrimsAlgorithm.FindMinimumSpanningTree(
                        undirectedGraph, new Point2D(1, 1));
                });
        }
    }
}

using System;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace MazeGenerator
{
    static class HeightMapMazeConnector
    {
        public static void ConnectRooms(
            Graph2D logicalGraph, Graph2D physicalGraph,
            IHeightMap heightMap, bool mergeDeadEnds)
        {
            AssignLogicalGraphEdgeWeights(logicalGraph, heightMap);

            var mst = PrimsAlgorithm.FindMinimumSpanningTree(
                logicalGraph, logicalGraph.Vertices.First());

            if (mergeDeadEnds)
                MergeDeadEnds(logicalGraph, mst);

            RemoveSpanningTreeDuals(physicalGraph, mst);
        }

        static void AssignLogicalGraphEdgeWeights(Graph2D logicalGraph, IHeightMap heightMap)
        {
            foreach (var edge in logicalGraph.Edges)
            {
                var dual = DualValue(
                    logicalGraph.GetEdgeMetadatum(edge.Item1, edge.Item2, "dual"));

                if (PhysicalEdgeTooShort(dual))
                    logicalGraph.SetEdgeMetadatum(edge.Item1, edge.Item2, "weight", "100000");
                else
                    logicalGraph.SetEdgeMetadatum(edge.Item1, edge.Item2, "weight",
                        HeightDifference(heightMap, edge.Item1, edge.Item2));
            }
        }

        static bool PhysicalEdgeTooShort(Tuple<Point2D, Point2D> physicalVertices)
        {
            return SS(physicalVertices.Item1, physicalVertices.Item2) < 100;
        }

        static Rational SS(Point2D p1, Point2D p2)
        {
            return (p1.X - p2.X).Squared + (p1.Y - p2.Y).Squared;
        }

        static string HeightDifference(IHeightMap heightMap, Point2D v1, Point2D v2)
        {
            var w1 = heightMap.Height(v1);
            var w2 = heightMap.Height(v2);
            var delta = (w1 - w2).AbsoluteValue;

            return delta.ToString();
        }

        static Tuple<Point2D, Point2D> DualValue(string p)
        {
            var tokens = p.Split(';');
            return Tuple.Create(Point2D.Parse(tokens[0]), Point2D.Parse(tokens[1]));
        }

        static void MergeDeadEnds(Graph2D logicalGraph, Graph2D spanningTree)
        {
            bool found = true;
            while (found)
            {
                found = false;
                var deadEnds = spanningTree.Vertices.Where(v => IsDeadEnd(spanningTree, v))
                    .OrderBy(v => v, new Point2DComparer()).ToArray();

                foreach (var source in deadEnds)
                {
                    if (false == IsSource(spanningTree, source))
                        continue;

                    var sinks = logicalGraph.GetNeighbours(source)
                        .Where(v => IsSink(spanningTree, v)).ToArray();

                    if (sinks.Length > 0)
                    {
                        found = true;
                        spanningTree.RemoveEdge(
                            sinks[0], spanningTree.GetNeighbours(sinks[0]).First());

                        spanningTree.AddEdge(source, sinks[0]);
                        spanningTree.SetEdgeMetadatum(source, sinks[0], "dual",
                            logicalGraph.GetEdgeMetadatum(source, sinks[0], "dual"));
                    }
                }
            }
        }

        static bool IsDeadEnd(Graph2D graph, Point2D vertex)
        {
            var neighbors = graph.GetNeighbours(vertex).ToArray();

            return (1 == neighbors.Length);
        }

        static bool IsSource(Graph2D graph, Point2D vertex)
        {
            var neighbors = graph.GetNeighbours(vertex).ToArray();

            if (1 != neighbors.Length)
                return false;

            return graph.GetNeighbours(neighbors[0]).Count() >= 2;
        }

        static bool IsSink(Graph2D graph, Point2D v)
        {
            var neighbors = graph.GetNeighbours(v).ToArray();

            if (1 != neighbors.Length)
                return false;

            return graph.GetNeighbours(neighbors[0]).Count() > 2;
        }

        static void RemoveSpanningTreeDuals(Graph2D physicalGraph, Graph2D spanningTree)
        {
            foreach (var edge in spanningTree.Edges)
            {
                var dual = DualValue(
                    spanningTree.GetEdgeMetadatum(edge.Item1, edge.Item2, "dual"));

                physicalGraph.RemoveEdge(dual.Item1, dual.Item2);
            }
        }
    }
}

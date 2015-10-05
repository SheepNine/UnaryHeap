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
            IEdgeWeightAssignment edgeWeights, bool mergeDeadEnds)
        {
            MakeShortWallsImpassable(logicalGraph);

            AssignLogicalGraphEdgeWeights(logicalGraph, edgeWeights);

            var mst = PrimsAlgorithm.FindMinimumSpanningTree(
                logicalGraph, logicalGraph.Vertices.First());

            if (mergeDeadEnds)
                MergeDeadEnds(logicalGraph, mst);

            RemoveSpanningTreeDuals(physicalGraph, mst);
        }

        static void AssignLogicalGraphEdgeWeights(
            Graph2D logicalGraph, IEdgeWeightAssignment edgeWeights)
        {
            foreach (var edge in logicalGraph.Edges)
            {
                var dual = logicalGraph.GetDualEdge(edge.Item1, edge.Item2);

                logicalGraph.SetEdgeMetadatum(edge.Item1, edge.Item2, "weight",
                    edgeWeights.AssignEdgeWeight(
                    edge.Item1, edge.Item2, dual.Item1, dual.Item2).ToString());
            }
        }


        static void MakeShortWallsImpassable(Graph2D logicalGraph)
        {
            foreach (var edge in logicalGraph.Edges.ToArray())
            {
                var dual = logicalGraph.GetDualEdge(edge.Item1, edge.Item2);

                if (PhysicalEdgeTooShort(dual))
                    logicalGraph.RemoveEdge(edge.Item1, edge.Item2);
            }
        }

        static bool PhysicalEdgeTooShort(Tuple<Point2D, Point2D> physicalVertices)
        {
            return Point2D.Quadrance(physicalVertices.Item1, physicalVertices.Item2) < 100;
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
                        spanningTree.SetEdgeMetadatum(
                            source, sinks[0], Graph2DExtensions.DualMetadataKey,
                            logicalGraph.GetEdgeMetadatum(
                            source, sinks[0], Graph2DExtensions.DualMetadataKey));
                    }
                }
            }
        }

        static bool IsDeadEnd(Graph2D graph, Point2D vertex)
        {
            var neighbours = graph.GetNeighbours(vertex).ToArray();

            return (1 == neighbours.Length);
        }

        static bool IsSource(Graph2D graph, Point2D vertex)
        {
            var neighbours = graph.GetNeighbours(vertex).ToArray();

            if (1 != neighbours.Length)
                return false;

            return graph.GetNeighbours(neighbours[0]).Count() >= 2;
        }

        static bool IsSink(Graph2D graph, Point2D v)
        {
            var neighbours = graph.GetNeighbours(v).ToArray();

            if (1 != neighbours.Length)
                return false;

            return graph.GetNeighbours(neighbours[0]).Count() > 2;
        }

        static void RemoveSpanningTreeDuals(Graph2D physicalGraph, Graph2D spanningTree)
        {
            foreach (var edge in spanningTree.Edges)
            {
                var dual = spanningTree.GetDualEdge(edge.Item1, edge.Item2);
                physicalGraph.RemoveEdge(dual.Item1, dual.Item2);
            }
        }
    }
}

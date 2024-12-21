using System.Linq;
using UnaryHeap.DataType;
using UnaryHeap.Graph;

namespace MazeGenerator
{
    sealed class MazeConnector
    {
        bool mergeDeadEnds;
        bool changeColor;

        public MazeConnector(bool mergeDeadEnds, bool changeColor)
        {
            this.mergeDeadEnds = mergeDeadEnds;
            this.changeColor = changeColor;
        }

        public void ConnectRooms(
            Graph2D logicalGraph, Graph2D physicalGraph, IEdgeWeightAssignment edgeWeights)
        {
            AssignLogicalGraphEdgeWeights(logicalGraph, edgeWeights);

            var mst = PrimsAlgorithm.FindMinimumSpanningTree(
                logicalGraph, logicalGraph.Vertices.First());

            if (mergeDeadEnds)
                MergeDeadEnds(logicalGraph, mst);

            RemoveSpanningTreeDuals(physicalGraph, mst);
        }


        #region Edge Weighting

        static void AssignLogicalGraphEdgeWeights(
            Graph2D logicalGraph, IEdgeWeightAssignment edgeWeights)
        {
            foreach (var edge in logicalGraph.Edges)
            {
                var dual = logicalGraph.GetDualEdge(edge.Item1, edge.Item2);

                logicalGraph.SetEdgeMetadatum(edge.Item1, edge.Item2, "weight",
                    edgeWeights.GetEdgeWeight(
                    edge.Item1, edge.Item2, dual.Item1, dual.Item2).ToString());
            }
        }

        #endregion


        #region Dead End Elimination

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

            return graph.GetNeighbours(neighbours[0]).Length >= 2;
        }

        static bool IsSink(Graph2D graph, Point2D v)
        {
            var neighbours = graph.GetNeighbours(v).ToArray();

            if (1 != neighbours.Length)
                return false;

            return graph.GetNeighbours(neighbours[0]).Length > 2;
        }

        #endregion


        #region Dual Removal

        void RemoveSpanningTreeDuals(
            Graph2D physicalGraph, Graph2D spanningTree)
        {
            foreach (var edge in spanningTree.Edges)
            {
                var dual = spanningTree.GetDualEdge(edge.Item1, edge.Item2);

                if (changeColor)
                {
                    physicalGraph.SetEdgeMetadatum(
                        dual.Item1, dual.Item2, "color", "#B8B8B8");
                    physicalGraph.SetEdgeMetadatum(
                        dual.Item1, dual.Item2, "order", "-1");
                }
                else
                {
                    physicalGraph.RemoveEdge(dual.Item1, dual.Item2);
                }
            }
        }

        #endregion
    }
}

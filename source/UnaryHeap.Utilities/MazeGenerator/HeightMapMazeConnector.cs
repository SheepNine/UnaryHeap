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
            Graph2D logicalGraph, Graph2D physicalGraph, IHeightMap heightMap)
        {
            AssignLogicalGraphEdgeWeights(logicalGraph, heightMap);

            var mst = PrimsAlgorithm.FindMinimumSpanningTree(
                logicalGraph, logicalGraph.Vertices.First());

            foreach (var edge in mst.Edges)
            {
                var dual = DualValue(mst.GetEdgeMetadatum(edge.Item1, edge.Item2, "dual"));
                physicalGraph.RemoveEdge(dual.Item1, dual.Item2);
            }
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

        static bool PhysicalEdgeTooShort(Tuple<Point2D, Point2D> phycialVertices)
        {
            return SS(phycialVertices.Item1, phycialVertices.Item2) < 100;
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
    }
}

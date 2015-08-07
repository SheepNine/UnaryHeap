#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Misc;

namespace UnaryHeap.Algorithms
{
    /// <summary>
    /// Provides an implementation of Prim's Algorithm for finindg a minimal spanning tree for
    /// a connected component in a Graph2D object.
    /// </summary>
    public static class PrimsAlgorithm
    {
        /// <summary>
        /// Calculates a minimum spanning tree of the specified Graph2D instance.
        /// </summary>
        /// <param name="inputGraph">The graph for which to compute the spanning tree.</param>
        /// <param name="startingVertex">The starting vertex for Prim's Algorithm.</param>
        /// <returns>A copy of inputGraph, with all edges removed, except for those that are part of a minimum spanning tree.</returns>
        public static Graph2D FindMinimumSpanningTree(Graph2D inputGraph, Point2D startingVertex)
        {
            if (null == inputGraph)
                throw new ArgumentNullException("inputGraph");
            if (null == startingVertex)
                throw new ArgumentNullException("startingVertex");

            return FindMinimumSpanningTree(inputGraph, startingVertex, CancellationToken.None);
        }

        /// <summary>
        /// Calculates a minimum spanning tree of the specified Graph2D instance, allowing for cancellation.
        /// </summary>
        /// <param name="inputGraph">The graph for which to compute the spanning tree.</param>
        /// <param name="startingVertex">The starting vertex for Prim's Algorithm.</param>
        /// <param name="token">The cancellation token to stop the operation.</param>
        /// <returns>A copy of inputGraph, with all edges removed, except for those that are part of a minimum spanning tree.</returns>
        public static Graph2D FindMinimumSpanningTree(Graph2D inputGraph, Point2D startingVertex, CancellationToken token)
        {
            if (null == inputGraph)
                throw new ArgumentNullException("inputGraph");
            if (null == startingVertex)
                throw new ArgumentNullException("startingVertex");

            if (inputGraph.IsDirected)
                throw new ArgumentException("Input graph cannot be directed.");


            // --- Init ---

            var result = inputGraph.Clone();
            var visitedVertices = new SortedSet<Point2D>(new Point2DComparer());
            var candidateEdges = new PriorityQueue<WeightedEdge>();

            AddNewEdges(result, startingVertex, visitedVertices, candidateEdges);


            // --- Loop ---

            while (false == candidateEdges.IsEmpty)
            {
                if (token.IsCancellationRequested)
                    return null;

                var edge = candidateEdges.Dequeue();

                if (false == visitedVertices.Contains(edge.P1))
                    AddNewEdges(result, edge.P1, visitedVertices, candidateEdges);
                else if (false == visitedVertices.Contains(edge.P2))
                    AddNewEdges(result, edge.P2, visitedVertices, candidateEdges);
                else
                    result.RemoveEdge(edge.P1, edge.P2);
            }


            // --- Remove unvisited vertices; they are from disconnected components ---

            if (visitedVertices.Count < result.NumVertices)
            {
                var unvisitedVertices = new SortedSet<Point2D>(result.Vertices, new Point2DComparer());
                unvisitedVertices.ExceptWith(visitedVertices);

                foreach (var vertex in unvisitedVertices)
                    result.RemoveVertex(vertex);
            }

            // --- Done ---

            return result;
        }

        static void AddNewEdges(Graph2D graph, Point2D vertex, SortedSet<Point2D> visitedVertices, PriorityQueue<WeightedEdge> candidateEdges)
        {
            visitedVertices.Add(vertex);

            foreach (var neighbor in graph.GetNeighbours(vertex).Where(n => false == visitedVertices.Contains(n)))
                candidateEdges.Enqueue(new WeightedEdge(vertex, neighbor));
        }

        class WeightedEdge : IComparable<WeightedEdge>
        {
            public Point2D P1 { get; private set; }
            public Point2D P2 { get; private set; }
            Rational LengthSquared;

            public WeightedEdge(Point2D p1, Point2D p2)
            {
                P1 = p1;
                P2 = p2;
                LengthSquared = (p2.X - p1.X).Squared + (p2.Y - p1.Y).Squared;
            }

            public int CompareTo(WeightedEdge other)
            {
                if (null == other)
                    return 1;

                return this.LengthSquared.CompareTo(other.LengthSquared);
            }
        }
    }
}

#endif
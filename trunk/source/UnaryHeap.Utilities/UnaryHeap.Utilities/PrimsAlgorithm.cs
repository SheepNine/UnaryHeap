using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.Misc;

namespace UnaryHeap.Algorithms
{
    /// <summary>
    /// Provides an implementation of Prim's Algorithm for finindg a minimal spanning tree for
    /// a connected component in a Graph2D object.
    /// </summary>
    /// <remarks>Each edge is assigned a weight equal to the square of the distance between its two vertices.
    /// The edge weight can be overridden by assigning a Rational value to the edge's 'weight' metadata entry.
    /// </remarks>
    public static class PrimsAlgorithm
    {
        /// <summary>
        /// Calculates a minimum spanning tree of the specified Graph2D instance, allowing for cancellation.
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
            if (inputGraph.IsDirected)
                throw new ArgumentException("Input graph cannot be directed.");
            if (false == inputGraph.HasVertex(startingVertex))
                throw new ArgumentException("Input graph does not contain starting vertex.");


            // --- Init ---
            
            var result = inputGraph.Clone();
            var visitedVertices = new SortedSet<Point2D>(new Point2DComparer());
            var candidateEdges = new PriorityQueue<WeightedEdge>();
            
            AddNewEdges(result, startingVertex, visitedVertices, candidateEdges);
            
            
            // --- Loop ---
            
            while (false == candidateEdges.IsEmpty)
            {
                var edge = candidateEdges.Dequeue();
            
                if (false == visitedVertices.Contains(edge.V1))
                    AddNewEdges(result, edge.V1, visitedVertices, candidateEdges);
                else if (false == visitedVertices.Contains(edge.V2))
                    AddNewEdges(result, edge.V2, visitedVertices, candidateEdges);
                else
                    result.RemoveEdge(edge.V1, edge.V2);
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
                candidateEdges.Enqueue(MakeEdge(graph, vertex, neighbor));
        }

        static WeightedEdge MakeEdge(Graph2D graph, Point2D v1, Point2D v2)
        {
            var explicitWeight = graph.GetEdgeMetadatum(v1, v2, "weight");

            if (null != explicitWeight)
                return new WeightedEdge(v1, v2, Rational.Parse(explicitWeight));
            else
                return new WeightedEdge(v1, v2, (v2.X - v1.X).Squared + (v2.Y - v1.Y).Squared);
        }

        class WeightedEdge : IComparable<WeightedEdge>
        {
            public Point2D V1 { get; private set; }
            public Point2D V2 { get; private set; }
            Rational Weight;

            public WeightedEdge(Point2D v1, Point2D v2, Rational weight)
            {
                V1 = v1;
                V2 = v2;
                Weight = weight;
            }

            public int CompareTo(WeightedEdge other)
            {
                if (null == other)
                    return 1;

                return this.Weight.CompareTo(other.Weight);
            }
        }
    }
}
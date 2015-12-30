﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace GraphPaper
{
    class GraphObjectSelection
    {
        public event EventHandler SelectionChanged;

        protected void OnSelectionChanged()
        {
            if (null != SelectionChanged)
                SelectionChanged(this, EventArgs.Empty);
        }

        IComparer<Point2D> comparer;
        SortedSet<Point2D> selectedVertices;
        SortedDictionary<Point2D, SortedSet<Point2D>> selectedEdges;

        public GraphObjectSelection()
        {
            comparer = new Point2DComparer();
            selectedVertices = new SortedSet<Point2D>(comparer);
            selectedEdges = new SortedDictionary<Point2D, SortedSet<Point2D>>(comparer);
        }

        public IEnumerable<Point2D> Vertices
        {
            get { return selectedVertices; }
        }

        public IEnumerable<Tuple<Point2D, Point2D>> Edges
        {
            get
            {
                return selectedEdges.SelectMany(
                    start => start.Value.Select(
                        end => Tuple.Create(start.Key, end)));
            }
        }

        void SelectVertex(Point2D vertex)
        {
            selectedVertices.Add(vertex);
            OnSelectionChanged();
        }

        void DeselectVertex(Point2D vertex)
        {
            selectedVertices.Remove(vertex);
            OnSelectionChanged();
        }

        void ToggleVertexSelection(Point2D vertex)
        {
            if (IsVertexSelected(vertex))
                DeselectVertex(vertex);
            else
                SelectVertex(vertex);
        }

        void SelectEdge(Point2D start, Point2D end)
        {
            if (false == selectedEdges.ContainsKey(start))
                selectedEdges.Add(start, new SortedSet<Point2D>(new Point2DComparer()));

            selectedEdges[start].Add(end);
            OnSelectionChanged();
        }

        void DeselectEdge(Point2D start, Point2D end)
        {
            selectedEdges[start].Remove(end);
            if (0 == selectedEdges[start].Count)
                selectedEdges.Remove(start);

            OnSelectionChanged();
        }

        void ToggleEdgeSelection(Point2D start, Point2D end)
        {
            if (IsEdgeSelected(start, end))
                DeselectEdge(start, end);
            else
                SelectEdge(start, end);
        }

        public bool IsEdgeSelected(Point2D start, Point2D end)
        {
            return selectedEdges.ContainsKey(start)
                && selectedEdges[start].Contains(end);
        }

        public bool IsVertexSelected(Point2D vertex)
        {
            return selectedVertices.Contains(vertex);
        }

        public void ClearSelection()
        {
            selectedVertices.Clear();
            selectedEdges.Clear();
            OnSelectionChanged();
        }

        public void SelectAll(ReadOnlyGraph2D currentModelState)
        {
            foreach (var vertex in currentModelState.Vertices)
                SelectVertex(vertex);
            foreach (var edge in currentModelState.Edges)
                SelectEdge(edge.Item1, edge.Item2);

            OnSelectionChanged();
        }

        public void SelectNearestObject(
            ReadOnlyGraph2D g, Point2D p, Rational quadranceCutoff)
        {
            ClearSelection();
            DoWithNearest(g, p, quadranceCutoff, SelectVertex, SelectEdge);
        }

        public void ToggleSelectionOfNearestObject(
            ReadOnlyGraph2D g, Point2D p, Rational quadranceCutoff)
        {
            DoWithNearest(g, p, quadranceCutoff, ToggleVertexSelection, ToggleEdgeSelection);
        }

        void DoWithNearest(ReadOnlyGraph2D g, Point2D p, Rational quadranceCutoff,
            Action<Point2D> vertexIsClosest, Action<Point2D, Point2D> edgeIsClosest)
        {
            var nearestVertex = FindNearestVertex(g.Vertices, p, quadranceCutoff);
            var nearestEdge = FindNearestEdge(g.Edges, p, quadranceCutoff);

            if (null != nearestVertex)
                vertexIsClosest(nearestVertex);
            else if (null != nearestEdge)
                edgeIsClosest(nearestEdge.Item1, nearestEdge.Item2);
        }

        Point2D FindNearestVertex(IEnumerable<Point2D> vertices,
            Point2D p, Rational quadranceCutoff)
        {
            Point2D result = null;
            Rational resultQuadrance = null;

            foreach (var vertex in vertices)
            {
                var vertexQuadrance = Point2D.Quadrance(vertex, p);

                if (vertexQuadrance <= quadranceCutoff &&
                    (resultQuadrance == null || vertexQuadrance < resultQuadrance ))
                {
                    result = vertex;
                    resultQuadrance = vertexQuadrance;
                }
            }

            return result;
        }

        Tuple<Point2D, Point2D> FindNearestEdge(IEnumerable<Tuple<Point2D, Point2D>> edges,
            Point2D p, Rational quadranceCutoff)
        {
            Tuple<Point2D, Point2D> result = null;
            Rational resultQuadrance = null;

            foreach (var edge in edges)
            {
                if (false == CanSelectEdge(edge, p))
                    continue;

                var edgeQuadrance = EdgeQuadrance(edge, p);

                if (edgeQuadrance <= quadranceCutoff &&
                    (resultQuadrance == null || edgeQuadrance < resultQuadrance))
                {
                    result = edge;
                    resultQuadrance = edgeQuadrance;
                }
            }

            return result;
        }

        bool CanSelectEdge(Tuple<Point2D, Point2D> edge, Point2D point)
        {
            var vX = edge.Item2.X - edge.Item1.X;
            var vY = edge.Item2.Y - edge.Item1.Y;

            var s1 = vX * edge.Item1.X + vY * edge.Item1.Y;
            var sPoint = vX * point.X + vY * point.Y;
            var s2 = vX * edge.Item2.X + vY * edge.Item2.Y;

            var result = s2 >= sPoint && sPoint >= s1;

            return result;
        }

        Rational EdgeQuadrance(Tuple<Point2D, Point2D> e, Point2D p)
        {
            return new Hyperplane2D(e.Item1, e.Item2).Quadrance(p);
        }
    }
}
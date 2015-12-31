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
            g.DoWithNearest(p, quadranceCutoff, SelectVertex, SelectEdge);
        }

        public void ToggleSelectionOfNearestObject(
            ReadOnlyGraph2D g, Point2D p, Rational quadranceCutoff)
        {
            g.DoWithNearest(p, quadranceCutoff, ToggleVertexSelection, ToggleEdgeSelection);
        }
    }
}
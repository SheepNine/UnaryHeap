using System;
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

        public void SelectVertex(Point2D vertex)
        {
            selectedVertices.Add(vertex);
            OnSelectionChanged();
        }

        public void DeselectVertex(Point2D vertex)
        {
            selectedVertices.Remove(vertex);
            OnSelectionChanged();
        }

        public void ToggleVertexSelection(Point2D vertex)
        {
            if (selectedVertices.Contains(vertex))
                DeselectVertex(vertex);
            else
                SelectVertex(vertex);
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

        public void SelectEdge(Point2D start, Point2D end)
        {
            if (false == selectedEdges.ContainsKey(start))
                selectedEdges.Add(start, new SortedSet<Point2D>(new Point2DComparer()));

            selectedEdges[start].Add(end);
            OnSelectionChanged();
        }

        public void ClearSelection()
        {
            selectedVertices.Clear();
            selectedEdges.Clear();
            OnSelectionChanged();
        }

        public Tuple<Point2D, Point2D> FindNearestEdge(
            IEnumerable<Tuple<Point2D, Point2D>> edges, Point2D p)
        {
            return edges
                .Where(e => CanSelectEdge(e, p))
                .OrderBy(e => EdgeQuadrance(e, p))
                .FirstOrDefault();
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

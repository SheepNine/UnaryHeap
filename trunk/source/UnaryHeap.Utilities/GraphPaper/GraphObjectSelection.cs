using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private SortedSet<Point2D> selectedVertices =
            new SortedSet<Point2D>(new Point2DComparer());

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

        public void ClearSelection()
        {
            selectedVertices.Clear();
            OnSelectionChanged();
        }
    }
}

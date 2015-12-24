using System.Drawing;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace GraphPaper
{
    class Screen
    {
        ModelViewTransform mvTransform;

        public Screen(ModelViewTransform mvTransform)
        {
            this.mvTransform = mvTransform;
        }

        public void RenderGrid(Graphics g, Rational gridSize)
        {
            var extents = mvTransform.ModelExtents;

            bool drawYAxis = false;
            bool drawXAxis = false;

            using (var pen = new Pen(GraphPaperColors.GridLines))
            {
                for (var x = (extents.X.Min / gridSize).Floor;
                    x <= (extents.X.Max / gridSize).Ceiling; x += 1)
                {
                    if (0 == x)
                        drawYAxis = true;
                    else
                        DrawLine(g, pen, new Point2D(gridSize * x, extents.Y.Min),
                            new Point2D(gridSize * x, extents.Y.Max));
                }

                for (var y = (extents.Y.Min / gridSize).Floor;
                    y <= (extents.Y.Max / gridSize).Ceiling; y += 1)
                {
                    if (0 == y)
                        drawXAxis = true;
                    else
                        DrawLine(g, pen, new Point2D(extents.X.Min, gridSize * y),
                            new Point2D(extents.X.Max, gridSize * y));
                }
            }

            using (var pen = new Pen(GraphPaperColors.AxisLines))
            {
                if (drawYAxis)
                    DrawLine(g, pen, new Point2D(0, extents.Y.Min), new Point2D(0, extents.Y.Max));
                if (drawXAxis)
                    DrawLine(g, pen, new Point2D(extents.X.Min, 0), new Point2D(extents.X.Max, 0));
            }
        }

        public void Render(Graphics g, GraphObjectSelection selection)
        {
            using (var brush = new SolidBrush(GraphPaperColors.SelectionHighlight))
                foreach (var vertex in selection.Vertices)
                    FillCircle(g, brush, vertex, 5.0f);
        }

        public void Render(Graphics g, ReadOnlyGraph2D graph)
        {
            using (var pen = new Pen(GraphPaperColors.BluePen, 3.0f))
                foreach (var edge in graph.Edges)
                    DrawLine(g, pen, edge.Item1, edge.Item2);

            using (var brush = new SolidBrush(GraphPaperColors.RedPen))
                foreach (var vertex in graph.Vertices)
                    FillCircle(g, brush, vertex, 4.0f);
        }

        void FillCircle(Graphics g, Brush b, Point2D modelCoords, float radius)
        {
            var viewCoords = mvTransform.ViewFromModel(modelCoords);

            g.FillEllipse(b,
                (float)viewCoords.X - radius, (float)viewCoords.Y - radius,
                radius * 2.0f, radius * 2.0f);
        }

        void DrawLine(Graphics g, Pen p, Point2D modelStart, Point2D modelEnd)
        {
            var viewStart = mvTransform.ViewFromModel(modelStart);
            var viewEnd = mvTransform.ViewFromModel(modelEnd);

            g.DrawLine(p,
                (float)viewStart.X, (float)viewStart.Y,
                (float)viewEnd.X, (float)viewEnd.Y);
        }
    }
}

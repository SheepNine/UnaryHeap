using System.Drawing;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace GraphPaper
{
    class Screen
    {
        Graphics g;
        ModelViewTransform mvTransform;

        public Screen(Graphics g, ModelViewTransform mvTransform)
        {
            this.g = g;
            this.mvTransform = mvTransform;
        }

        public void RenderGrid(Rectangle viewExtents, Rational gridSize)
        {
            var min = mvTransform.ModelFromView(
                new Point2D(viewExtents.Left, viewExtents.Bottom));
            var max = mvTransform.ModelFromView(
                new Point2D(viewExtents.Right, viewExtents.Top));

            bool drawYAxis = false;
            bool drawXAxis = false;

            using (var pen = new Pen(GraphPaperColors.GridLines))
            {
                for (var x = (min.X / gridSize).Floor;
                    x <= (max.X / gridSize).Ceiling; x += 1)
                {
                    if (0 == x)
                        drawYAxis = true;
                    else
                        DrawLine(pen, new Point2D(gridSize * x, min.Y),
                            new Point2D(gridSize * x, max.Y));
                }

                for (var y = (min.Y / gridSize).Floor;
                    y <= (max.Y / gridSize).Ceiling; y += 1)
                {
                    if (0 == y)
                        drawXAxis = true;
                    else
                        DrawLine(pen, new Point2D(min.X, gridSize * y),
                            new Point2D(max.X, gridSize * y));
                }
            }

            using (var pen = new Pen(GraphPaperColors.AxisLines))
            {
                if (drawYAxis)
                    DrawLine(pen, new Point2D(0, min.Y), new Point2D(0, max.Y));
                if (drawXAxis)
                    DrawLine(pen, new Point2D(min.X, 0), new Point2D(max.X, 0));
            }
        }

        public void Render(GraphObjectSelection selection)
        {
            using (var brush = new SolidBrush(GraphPaperColors.SelectionHighlight))
                foreach (var vertex in selection.Vertices)
                    FillCircle(brush, vertex, 5.0f);
        }

        public void Render(ReadOnlyGraph2D graph)
        {
            using (var pen = new Pen(GraphPaperColors.BluePen, 3.0f))
                foreach (var edge in graph.Edges)
                    DrawLine(pen, edge.Item1, edge.Item2);

            using (var brush = new SolidBrush(GraphPaperColors.RedPen))
                foreach (var vertex in graph.Vertices)
                    FillCircle(brush, vertex, 4.0f);
        }

        void FillCircle(Brush b, Point2D modelCoords, float radius)
        {
            var viewCoords = mvTransform.ViewFromModel(modelCoords);

            g.FillEllipse(b,
                (float)viewCoords.X - radius, (float)viewCoords.Y - radius,
                radius * 2.0f, radius * 2.0f);
        }

        void DrawLine(Pen p, Point2D modelStart, Point2D modelEnd)
        {
            var viewStart = mvTransform.ViewFromModel(modelStart);
            var viewEnd = mvTransform.ViewFromModel(modelEnd);

            g.DrawLine(p,
                (float)viewStart.X, (float)viewStart.Y,
                (float)viewEnd.X, (float)viewEnd.Y);
        }
    }
}

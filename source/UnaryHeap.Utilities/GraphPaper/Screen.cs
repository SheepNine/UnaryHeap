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
                        DrawLine(pen,
                            mvTransform.ViewFromModel(new Point2D(gridSize * x, min.Y)),
                            mvTransform.ViewFromModel(new Point2D(gridSize * x, max.Y)));
                }

                for (var y = (min.Y / gridSize).Floor;
                    y <= (max.Y / gridSize).Ceiling; y += 1)
                {
                    if (0 == y)
                        drawXAxis = true;
                    else
                        DrawLine(pen,
                            mvTransform.ViewFromModel(new Point2D(min.X, gridSize * y)),
                            mvTransform.ViewFromModel(new Point2D(max.X, gridSize * y)));
                }
            }

            using (var pen = new Pen(GraphPaperColors.AxisLines))
            {
                if (drawYAxis)
                    DrawLine(pen,
                        mvTransform.ViewFromModel(new Point2D(0, min.Y)),
                        mvTransform.ViewFromModel(new Point2D(0, max.Y)));
                if (drawXAxis)
                    DrawLine(pen,
                        mvTransform.ViewFromModel(new Point2D(min.X, 0)),
                        mvTransform.ViewFromModel(new Point2D(max.X, 0)));
            }
        }

        public void Render(ReadOnlyGraph2D graph)
        {
            using (var brush = new SolidBrush(GraphPaperColors.BluePen))
            using (var pen = new Pen(brush, 3.0f))
                foreach (var edge in graph.Edges)
                    DrawLine(pen,
                        mvTransform.ViewFromModel(edge.Item1),
                        mvTransform.ViewFromModel(edge.Item2));

            using (var brush = new SolidBrush(GraphPaperColors.RedPen))
                foreach (var vertex in graph.Vertices)
                    DrawPoint(brush, mvTransform.ViewFromModel(vertex));
        }

        void DrawPoint(Brush b, Point2D point2D)
        {
            var radius = 4.0f;

            var x = (float)point2D.X;
            var y = (float)point2D.Y;
            g.FillEllipse(b, x - radius, y - radius, radius * 2, radius * 2);
        }

        void DrawLine(Pen p, Point2D start, Point2D end)
        {
            g.DrawLine(p, (float)start.X, (float)start.Y, (float)end.X, (float)end.Y);
        }
    }
}

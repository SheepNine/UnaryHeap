using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace GraphPaper
{
    class Screen : IDisposable
    {
        Graphics g;
        GraphicsState gState;
        ModelViewTransform mvTransform;

        public Screen(Graphics g, ModelViewTransform mvTransform)
        {
            this.g = g;
            this.mvTransform = mvTransform;
            PushGraphicsState(g);
        }

        private void PushGraphicsState(Graphics g)
        {
            gState = g.Save();
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        public void Dispose()
        {
            g.Restore(gState);
        }

        public void RenderGrid(Rational gridSize)
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
                        DrawLine(pen, new Point2D(gridSize * x, extents.Y.Min),
                            new Point2D(gridSize * x, extents.Y.Max));
                }

                for (var y = (extents.Y.Min / gridSize).Floor;
                    y <= (extents.Y.Max / gridSize).Ceiling; y += 1)
                {
                    if (0 == y)
                        drawXAxis = true;
                    else
                        DrawLine(pen, new Point2D(extents.X.Min, gridSize * y),
                            new Point2D(extents.X.Max, gridSize * y));
                }
            }

            using (var pen = new Pen(GraphPaperColors.AxisLines))
            {
                if (drawYAxis)
                    DrawLine(pen, new Point2D(0, extents.Y.Min), new Point2D(0, extents.Y.Max));
                if (drawXAxis)
                    DrawLine(pen, new Point2D(extents.X.Min, 0), new Point2D(extents.X.Max, 0));
            }
        }

        public void Render(GraphObjectSelection selection)
        {
            using (var pen = new Pen(GraphPaperColors.SelectionHighlight, 3.0f))
                foreach (var edge in selection.Edges)
                    DrawLine(pen, edge.Item1, edge.Item2);

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

        public void DrawCircle(Pen pen, Point2D modelCoords, float radius)
        {
            g.DrawEllipse(pen, CircleBounds(
                mvTransform.ViewFromModel(modelCoords), radius));
        }

        public void FillCircle(Brush b, Point2D modelCoords, float radius)
        {
            var viewCoords = mvTransform.ViewFromModel(modelCoords);

            g.FillEllipse(b,
                (float)viewCoords.X - radius, (float)viewCoords.Y - radius,
                radius * 2.0f, radius * 2.0f);
        }

        public void DrawLine(Pen p, Point2D modelStart, Point2D modelEnd)
        {
            var viewStart = mvTransform.ViewFromModel(modelStart);
            var viewEnd = mvTransform.ViewFromModel(modelEnd);

            g.DrawLine(p,
                (float)viewStart.X, (float)viewStart.Y,
                (float)viewEnd.X, (float)viewEnd.Y);
        }

        static RectangleF CircleBounds(Point2D p, float radius)
        {
            var pX = (float)p.X;
            var pY = (float)p.Y;

            return new RectangleF(
                pX - radius, pY - radius, radius * 2.0f, radius * 2.0f);
        }

        public void DrawString(string s, Font font, Brush brush,
            Point2D modelCoords, TextOffset offset)
        {
            var p = mvTransform.ViewFromModel(modelCoords);

            g.DrawString(s, font, brush, (float)p.X, (float)p.Y,
                StringFormatFromTextOffset(offset));
        }

        static StringFormat StringFormatFromTextOffset(TextOffset offset)
        {
            var format = new StringFormat();

            switch (offset)
            {
                case TextOffset.NorthEast:
                    format.LineAlignment = StringAlignment.Far;
                    format.Alignment = StringAlignment.Near;
                    break;
                case TextOffset.NorthWest:
                    format.LineAlignment = StringAlignment.Far;
                    format.Alignment = StringAlignment.Far;
                    break;
                case TextOffset.SouthEast:
                    format.LineAlignment = StringAlignment.Near;
                    format.Alignment = StringAlignment.Near;
                    break;
                case TextOffset.SouthWest:
                    format.LineAlignment = StringAlignment.Near;
                    format.Alignment = StringAlignment.Far;
                    break;
            }

            return format;
        }
    }

    enum TextOffset
    {
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
    }
}

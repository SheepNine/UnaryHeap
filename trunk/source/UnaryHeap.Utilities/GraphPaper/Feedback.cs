using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using UnaryHeap.Utilities.D2;

namespace GraphPaper
{
    interface IFeedback : IEquatable<IFeedback>
    {
        void Render(Screen screen);
    }

    class NullFeedback : IFeedback
    {
        public bool Equals(IFeedback other)
        {
            var castOther = other as NullFeedback;
            return castOther != null;
        }

        public void Render(Screen screen)
        {
        }
    }

    class UnsupportedFeedback : IFeedback
    {
        public bool Equals(IFeedback other)
        {
            var castOther = other as UnsupportedFeedback;
            return castOther != null;
        }

        public void Render(Screen screen)
        {
            using (var font = new Font(FontFamily.GenericSansSerif, 16.0f))
                screen.DrawStatusText("Unsupported Operation", font, Brushes.Red);
        }
    }

    class HoverFeedback : IFeedback
    {
        Point2D feedbackPoint;

        public HoverFeedback(Point2D feedbackPoint)
        {
            this.feedbackPoint = feedbackPoint;
        }

        public bool Equals(IFeedback other)
        {
            var castOther = other as HoverFeedback;

            if (null == castOther)
                return false;

            return this.feedbackPoint.Equals(castOther.feedbackPoint);
        }

        public void Render(Screen screen)
        {
            using (var pen = new Pen(GraphPaperColors.HotTrackingPen, 2.0f))
                screen.DrawCircle(pen, feedbackPoint, 4.0f);

            var display = string.Format("\r\nX: {0}\r\nY: {1}",
                (double)feedbackPoint.X, (double)feedbackPoint.Y);

            using (var font = new Font(FontFamily.GenericSansSerif, 16.0f))
                screen.DrawStatusText(display, font, Brushes.Black);
        }
    }

    class AddVertexFeedback : IFeedback
    {
        Point2D vertexPoint;

        public AddVertexFeedback(Point2D vertexPoint)
        {
            this.vertexPoint = vertexPoint;
        }

        public bool Equals(IFeedback other)
        {
            var castOther = other as AddVertexFeedback;

            if (null == castOther)
                return false;

            return this.vertexPoint.Equals(castOther.vertexPoint);
        }

        public void Render(Screen screen)
        {
            using (var brush = new SolidBrush(Color.CornflowerBlue))
                screen.FillCircle(brush, vertexPoint, 5.0f);

            var display = string.Format("Add Vertex\r\nX: {0}\r\nY: {1}\r\n",
                (double)vertexPoint.X, (double)vertexPoint.Y);

            using (var font = new Font(FontFamily.GenericSansSerif, 16.0f))
                screen.DrawStatusText(display, font, Brushes.Black);
        }
    }

    class AddEdgeFeedback : IFeedback
    {
        Point2D startPoint;
        Point2D endPoint;

        public AddEdgeFeedback(Point2D startPoint, Point2D endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public bool Equals(IFeedback other)
        {
            var castObj = other as AddEdgeFeedback;

            if (null == castObj)
                return false;

            return this.startPoint.Equals(castObj.startPoint) &&
                this.endPoint.Equals(castObj.endPoint);
        }

        public void Render(Screen screen)
        {
            using (var brush = new SolidBrush(Color.CornflowerBlue))
            {
                screen.FillCircle(brush, startPoint, 5.0f);
                screen.FillCircle(brush, endPoint, 5.0f);
            }

            using (var pen = new Pen(Color.CornflowerBlue, 4.0f))
            {
                screen.DrawLine(pen, startPoint, endPoint);
                screen.DrawTick(pen, startPoint, endPoint);
            }

            var display = string.Format(
                "Add Edge\r\nX1: {0}\r\nY1: {1}\r\nX2: {2}\r\nY2: {3}",
                (double)startPoint.X, (double)startPoint.Y,
                (double)endPoint.X, (double)endPoint.Y);

            using (var font = new Font(FontFamily.GenericSansSerif, 16.0f))
                screen.DrawStatusText(display, font, Brushes.Black);
        }
    }

    class AdjustViewExtentsFeedback : IFeedback
    {
        Orthotope2D bounds;

        public AdjustViewExtentsFeedback(Orthotope2D bounds)
        {
            this.bounds = bounds;
        }

        public bool Equals(IFeedback other)
        {
            var castObj = other as AdjustViewExtentsFeedback;

            if (null == castObj)
                return false;

            return
                this.bounds.X.Min.Equals(castObj.bounds.X.Min) &&
                this.bounds.Y.Min.Equals(castObj.bounds.Y.Min) &&
                this.bounds.X.Max.Equals(castObj.bounds.X.Max) &&
                this.bounds.Y.Max.Equals(castObj.bounds.Y.Max);
        }

        public void Render(Screen screen)
        {
            using (var brush = new SolidBrush(Color.FromArgb(32, Color.Black)))
                screen.FillRectangle(brush, bounds);
            using (var pen = new Pen(Color.Silver, 2.0f) { DashStyle = DashStyle.Dash })
                screen.DrawRectangle(pen, bounds);

            var display = string.Format(
                "Adjust View Extents\r\nX: {0:F2} to {1:F2}\r\nY: {2:F2} to {3:F2}",
                (double)bounds.X.Min, (double)bounds.X.Max,
                (double)bounds.Y.Min, (double)bounds.Y.Max);

            using (var font = new Font(FontFamily.GenericSansSerif, 16.0f))
                screen.DrawStatusText(display, font, Brushes.Black);
        }
    }

    class SelectObjectsFeedback : IFeedback
    {
        Orthotope2D bounds;

        public SelectObjectsFeedback(Orthotope2D bounds)
        {
            this.bounds = bounds;
        }

        public bool Equals(IFeedback other)
        {
            var castObj = other as SelectObjectsFeedback;

            if (null == castObj)
                return false;

            return
                this.bounds.X.Min.Equals(castObj.bounds.X.Min) &&
                this.bounds.Y.Min.Equals(castObj.bounds.Y.Min) &&
                this.bounds.X.Max.Equals(castObj.bounds.X.Max) &&
                this.bounds.Y.Max.Equals(castObj.bounds.Y.Max);
        }

        public void Render(Screen screen)
        {
            using (var brush = new SolidBrush(Color.FromArgb(32, Color.CornflowerBlue)))
                screen.FillRectangle(brush, bounds);

            using (var pen = new Pen(Color.White, 2.0f))
                screen.DrawRectangle(pen, bounds);

            var display = string.Format(
                "Select Objects\r\nX: {0:F2} to {1:F2}\r\nY: {2:F2} to {3:F2}",
                (double)bounds.X.Min, (double)bounds.X.Max,
                (double)bounds.Y.Min, (double)bounds.Y.Max);

            using (var font = new Font(FontFamily.GenericSansSerif, 16.0f))
                screen.DrawStatusText(display, font, Brushes.Black);
        }
    }

    class AppendSelectionFeedback : IFeedback
    {
        Orthotope2D bounds;

        public AppendSelectionFeedback(Orthotope2D bounds)
        {
            this.bounds = bounds;
        }

        public bool Equals(IFeedback other)
        {
            var castObj = other as AppendSelectionFeedback;

            if (null == castObj)
                return false;

            return
                this.bounds.X.Min.Equals(castObj.bounds.X.Min) &&
                this.bounds.Y.Min.Equals(castObj.bounds.Y.Min) &&
                this.bounds.X.Max.Equals(castObj.bounds.X.Max) &&
                this.bounds.Y.Max.Equals(castObj.bounds.Y.Max);
        }

        public void Render(Screen screen)
        {
            using (var brush = new SolidBrush(Color.FromArgb(32, Color.DarkRed)))
                screen.FillRectangle(brush, bounds);

            using (var pen = new Pen(Color.White, 2.0f))
                screen.DrawRectangle(pen, bounds);

            var display = string.Format(
                "Append Objects to Selection\r\nX: {0:F2} to {1:F2}\r\nY: {2:F2} to {3:F2}",
                (double)bounds.X.Min, (double)bounds.X.Max,
                (double)bounds.Y.Min, (double)bounds.Y.Max);

            using (var font = new Font(FontFamily.GenericSansSerif, 16.0f))
                screen.DrawStatusText(display, font, Brushes.Black);
        }
    }

    class CenterViewFeedback : IFeedback
    {
        Point2D newCenterPoint;

        public CenterViewFeedback(Point2D newCenterPoint)
        {
            this.newCenterPoint = newCenterPoint;
        }

        public bool Equals(IFeedback other)
        {
            var castObj = other as CenterViewFeedback;

            if (null == castObj)
                return false;

            return this.newCenterPoint.Equals(castObj.newCenterPoint);
        }

        public void Render(Screen screen)
        {
            var extents = screen.ModelExtents.CenteredAt(newCenterPoint);

            using (var brush = new SolidBrush(Color.FromArgb(32, Color.Black)))
                screen.FillRectangle(brush, extents);
            using (var pen = new Pen(Color.Silver, 2.0f) { DashStyle = DashStyle.Dash })
                screen.DrawRectangle(pen, extents);

            var display = string.Format("Center View\r\nX: {0:F2}\r\nY: {1:F2}\r\n",
                (double)newCenterPoint.X, (double)newCenterPoint.Y);

            using (var font = new Font(FontFamily.GenericSansSerif, 16.0f))
                screen.DrawStatusText(display, font, Brushes.Black);
        }
    }
}

using System;
using System.Drawing;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.UI;

namespace GraphPaper
{
    static class RefactorThis
    {
        public static RectangleF CircleBounds(Point2D p, float radius)
        {
            var pX = (float)p.X;
            var pY = (float)p.Y;

            return new RectangleF(
                pX - radius, pY - radius, radius * 2.0f, radius * 2.0f);
        }

        public static PointF Nearest(Point2D p)
        {
            return new PointF(
                (float)p.X,
                (float)p.Y);
        }
    }

    class HoverFeedback : IWysiwygFeedbackStrategy
    {
        Point2D feedbackPoint;
        ModelViewTransform transform;

        public HoverFeedback(Point2D feedbackPoint, ModelViewTransform transform)
        {
            this.feedbackPoint = feedbackPoint;
            this.transform = transform;
        }

        public bool Equals(IWysiwygFeedbackStrategy other)
        {
            var castOther = other as HoverFeedback;

            if (null == castOther)
                return false;

            return object.ReferenceEquals(this.transform, castOther.transform) &&
                this.feedbackPoint.Equals(castOther.feedbackPoint);
        }

        public void Render(Graphics g, Rectangle clipRectangle)
        {
            var gstate = g.Save();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            DrawCircle(g, transform.ViewFromModel(feedbackPoint));
            g.Restore(gstate);
        }

        private static void DrawCircle(Graphics g, Point2D p)
        {
            using (var pen = new Pen(GraphPaperColors.HotTrackingPen, 2.0f))
                g.DrawEllipse(pen, RefactorThis.CircleBounds(p, 4.0f));
        }
    }

    class AddEdgeFeedback : IWysiwygFeedbackStrategy
    {
        Point2D startPoint;
        Point2D endPoint;
        ModelViewTransform transform;

        public AddEdgeFeedback(
            Point2D startPoint, Point2D endPoint, ModelViewTransform transform)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.transform = transform;
        }

        public bool Equals(IWysiwygFeedbackStrategy other)
        {
            var castObj = other as AddEdgeFeedback;

            if (null == castObj)
                return false;

            return this.startPoint.Equals(castObj.startPoint) &&
                this.endPoint.Equals(castObj.endPoint) &&
                object.ReferenceEquals(this.transform, castObj.transform);
        }

        public void Render(Graphics g, Rectangle clipRectangle)
        {
            var gstate = g.Save();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            var start = transform.ViewFromModel(startPoint);
            var end = transform.ViewFromModel(endPoint);

            using (var brush = new SolidBrush(Color.CornflowerBlue))
            {
                g.FillEllipse(brush, RefactorThis.CircleBounds(start, 5.0f));
                g.FillEllipse(brush, RefactorThis.CircleBounds(end, 5.0f));
            }

            using (var pen = new Pen(Color.CornflowerBlue, 4.0f))
                g.DrawLine(pen, RefactorThis.Nearest(start), RefactorThis.Nearest(end));

            g.Restore(gstate);
        }
    }
}

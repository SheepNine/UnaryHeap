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

    interface IFeedback : IEquatable<IFeedback>
    {
        void Render(Graphics g, Screen screen);
    }

    class NullFeedback : IFeedback
    {
        public bool Equals(IFeedback other)
        {
            var castOther = other as NullFeedback;
            return castOther != null;
        }

        public void Render(Graphics g, Screen screen)
        {
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

        public void Render(Graphics g, Screen screen)
        {
            using (var pen = new Pen(GraphPaperColors.HotTrackingPen, 2.0f))
                screen.DrawCircle(g, pen, feedbackPoint, 4.0f);
        }

        private static void DrawCircle(Graphics g, Point2D p)
        {
            using (var pen = new Pen(GraphPaperColors.HotTrackingPen, 2.0f))
                g.DrawEllipse(pen, RefactorThis.CircleBounds(p, 4.0f));
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

        public void Render(Graphics g, Screen screen)
        {
            using (var brush = new SolidBrush(Color.CornflowerBlue))
            {
                screen.FillCircle(g, brush, startPoint, 5.0f);
                screen.FillCircle(g, brush, endPoint, 5.0f);
            }

            using (var pen = new Pen(Color.CornflowerBlue, 4.0f))
                screen.DrawLine(g, pen, startPoint, endPoint);
        }
    }
}

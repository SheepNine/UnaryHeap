using System;
using System.Drawing;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.UI;

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

            var display = string.Format("X: {0}\r\nY: {1}",
                (double)feedbackPoint.X, (double)feedbackPoint.Y);

            using (var font = new Font(FontFamily.GenericSansSerif, 16.0f))
                screen.DrawString(display, font, Brushes.Black,
                    feedbackPoint, TextOffset.NorthEast);
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
                screen.DrawLine(pen, startPoint, endPoint);
        }
    }
}

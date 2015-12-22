using System;
using System.Drawing;
using UnaryHeap.Utilities.D2;
using UnaryHeap.Utilities.UI;

namespace GraphPaper
{
    class ModelPointFeedback : IWysiwygFeedbackStrategy
    {
        Point2D feedbackPoint;
        ModelViewTransform transform;

        public ModelPointFeedback(Point2D feedbackPoint, ModelViewTransform transform)
        {
            this.feedbackPoint = feedbackPoint;
            this.transform = transform;
        }

        public bool Equals(IWysiwygFeedbackStrategy other)
        {
            var castOther = other as ModelPointFeedback;

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
            var pX = (float)p.X;
            var pY = (float)p.Y;

            using (var pen = new Pen(GraphPaperColors.HotTrackingPen, 2.0f))
                g.DrawEllipse(pen, new RectangleF(pX - 4.0f, pY - 4.0f, 8.0f, 8.0f));
        }
    }
}

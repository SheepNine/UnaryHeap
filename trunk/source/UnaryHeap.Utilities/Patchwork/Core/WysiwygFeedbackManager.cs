using System;
using System.Drawing;
using System.Windows.Forms;

namespace UnaryHeap.Utilities.UI
{
    interface IWysiwygFeedback : IEquatable<IWysiwygFeedback>
    {
        void Render(Graphics g, Rectangle clipRectangle);
    }

    class NullWysiwygFeedback : IWysiwygFeedback
    {
        public void Render(Graphics g, Rectangle clipRectangle)
        {
        }

        public bool Equals(IWysiwygFeedback other)
        {
            return (null != (other as NullWysiwygFeedback));
        }
    }

    class WysiwygFeedbackManager
    {
        WysiwygPanel target;
        IWysiwygFeedback currentFeedback = new NullWysiwygFeedback();

        public WysiwygFeedbackManager(WysiwygPanel panel)
        {
            target = panel;
            target.PaintFeedback += target_PaintFeedback;
        }

        void target_PaintFeedback(object sender, PaintEventArgs e)
        {
            currentFeedback.Render(e.Graphics, e.ClipRectangle);
        }

        public void ClearFeedback()
        {
            SetFeedback(new NullWysiwygFeedback());
        }

        public void SetFeedback(IWysiwygFeedback newFeedback)
        {
            if (null == newFeedback)
                newFeedback = new NullWysiwygFeedback();

            if (false == currentFeedback.Equals(newFeedback))
            {
                currentFeedback = newFeedback;
                target.InvalidateFeedback();
            }
        }
    }
}

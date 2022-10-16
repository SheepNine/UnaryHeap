using System;
using System.Drawing;
using System.Windows.Forms;

namespace UnaryHeap.GUI
{
    /// <summary>
    /// Provides the ability to interchange WYSIWIG feedback using the strategy pattern.
    /// </summary>
    public class WysiwygFeedbackStrategyContext
    {
        #region Member Variables

        WysiwygPanel target;
        IWysiwygFeedbackStrategy currentFeedback = new NullWysiwygFeedbackStrategy();

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the WysiwygFeedbackStrategyContext class.
        /// </summary>
        /// <param name="panel">The WysiwygPanel that will be displaying the feedback.</param>
        /// <exception cref="System.ArgumentNullException">
        /// panel is null.</exception>
        public WysiwygFeedbackStrategyContext(WysiwygPanel panel)
        {
            if (null == panel)
                throw new ArgumentNullException("panel");

            target = panel;
            target.PaintFeedback += target_PaintFeedback;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Removes any feedback currently being displayed.
        /// </summary>
        public void ClearFeedback()
        {
            SetFeedback(new NullWysiwygFeedbackStrategy());
        }

        /// <summary>
        /// Sets the feedback to be displayed.
        /// </summary>
        /// <param name="newFeedback">The feedback to display, or null
        /// to display no feedback.</param>
        public void SetFeedback(IWysiwygFeedbackStrategy newFeedback)
        {
            if (null == newFeedback)
                newFeedback = new NullWysiwygFeedbackStrategy();

            if (false == currentFeedback.Equals(newFeedback))
            {
                currentFeedback = newFeedback;
                target.InvalidateFeedback();
            }
        }

        #endregion


        #region Helper Methods

        void target_PaintFeedback(object sender, PaintEventArgs e)
        {
            currentFeedback.Render(e.Graphics, e.ClipRectangle);
        }

        #endregion
    }

    /// <summary>
    /// Represents a feedback strategy for use in the WysiwygFeedbackStrategyContext class.
    /// </summary>
    public interface IWysiwygFeedbackStrategy : IEquatable<IWysiwygFeedbackStrategy>
    {
        /// <summary>
        /// Renders this feedback.
        /// </summary>
        /// <param name="g">The graphics context to which to render the feedback.</param>
        /// <param name="clipRectangle">The clipping bounds to use while rendering.</param>
        void Render(Graphics g, Rectangle clipRectangle);
    }

    /// <summary>
    /// Represents blank feedback for use in the WysiwygFeedbackStrategyContext class.
    /// </summary>
    public class NullWysiwygFeedbackStrategy : IWysiwygFeedbackStrategy
    {
        /// <summary>
        /// Renders this feedback.
        /// </summary>
        /// <param name="g">The graphics context to which to render the feedback.</param>
        /// <param name="clipRectangle">The clipping bounds to use while rendering.</param>
        public void Render(Graphics g, Rectangle clipRectangle)
        {
        }

        /// <summary>Indicates whether the current object is equal to another object
        /// of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter;
        /// otherwise, false.</returns>
        public bool Equals(IWysiwygFeedbackStrategy other)
        {
            return (null != (other as NullWysiwygFeedbackStrategy));
        }
    }
}

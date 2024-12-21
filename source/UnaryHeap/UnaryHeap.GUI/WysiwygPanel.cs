using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;

namespace UnaryHeap.GUI
{
    /// <summary>
    /// Represents a display window that is optimized for efficient rendering of content
    /// (potentially expensive to render but infrequently changed) and feedback (inexpensive
    /// to render but frequently changed due to user input).
    /// </summary>
    public class WysiwygPanel : Control
    {
        #region Member Variables

        Bitmap content;
        bool contentStale = true;
        int debugFrameCounter;
        bool debugFrameCounterVisible;

        #endregion


        #region Events

        /// <summary>
        /// Occurs when the content of the control is redrawn.
        /// </summary>
        [Category("Appearance")]
        [Description("Occurs when the content of the control needs repainting.")]
        public event EventHandler<PaintEventArgs> PaintContent;

        /// <summary>
        /// Raises the PaintContent event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected void OnPaintContent(PaintEventArgs e)
        {
            if (null != PaintContent)
                PaintContent(this, e);
            else
                PaintDebugMessage(e, Color.Pink, Pens.Red,
                    "No event handlers have been added to the PaintContent event.");
        }

        /// <summary>
        /// Occurs when the feedback of the control is redrawn.
        /// </summary>
        [Category("Appearance")]
        [Description("Occurs when the feedback of the control needs repainting.")]
        public event EventHandler<PaintEventArgs> PaintFeedback;

        /// <summary>
        /// Raises the PaintFeedback event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected void OnPaintFeedback(PaintEventArgs e)
        {
            if (null != PaintFeedback)
                PaintFeedback(this, e);
        }

        /// <summary>
        /// Occurrs when the panel renders itself.
        /// </summary>
        public event EventHandler<RenderPerformanceEventArgs> RenderOccurred;

        /// <summary>
        /// Raises the RenderOccurred event.
        /// </summary>
        /// <param name="contentPaintTime">How long it took to paint the content.</param>
        /// <param name="contentCopyTime">
        /// How long it took to copy the content to the frame buffer.
        /// </param>
        /// <param name="feedbackPaintTime">How long it took to paint the feedback.</param>
        protected void OnRenderOccurred(
            long contentPaintTime, long contentCopyTime, long feedbackPaintTime)
        {
            if (null != RenderOccurred)
                RenderOccurred(this, new RenderPerformanceEventArgs(
                    contentPaintTime, contentCopyTime, feedbackPaintTime));
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the WysiwigPanel class.
        /// </summary>
        public WysiwygPanel()
        {
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
        }

        #endregion


        #region Properties

        /// <summary>
        /// When this property is true, the current frame count will be rendered 
        /// in the top-left corner of the control so that inefficient multiple renderings 
        /// can be detected.
        /// </summary>
        [Category("Appearance")]
        [Description("When this property is true, the current frame count will be rendered " +
            "in the top-left corner of the control so that inefficient multiple renderings " +
            "can be detected.")]
        [DefaultValue(false)]
        public bool DebugFrameCounterVisible
        {
            get
            {
                return debugFrameCounterVisible;
            }
            set
            {
                if (value == debugFrameCounterVisible)
                    return;

                debugFrameCounterVisible = value;
                InvalidateContent();
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Invalidates the entire surface of the control and causes only the
        /// feedback to be redrawn. The content will not be redrawn.
        /// </summary>
        /// <seealso cref="InvalidateContent"/>
        public void InvalidateFeedback()
        {
            Invalidate();
        }

        /// <summary>
        /// Invalidates the entire surface of the control and causes both the
        /// content and the feedback to be redrawn.
        /// </summary>
        /// <seealso cref="InvalidateFeedback"/>
        public void InvalidateContent()
        {
            contentStale = true;
            Invalidate();
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Raises the SizeChanged event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (content != null)
            {
                content.Dispose();
                content = null;
            }

            Invalidate();
        }

        /// <summary>
        /// Raises the Paint event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DesignMode)
            {
                PaintDebugMessage(e, Color.CornflowerBlue, Pens.Blue, "WYSIWIG panel");
            }
            else
            {
                var contentPaintTime = Time(() => { RepaintContentIfRequired(); });
                var contentCopyTime = Time(() => {
                    var checkpoint = e.Graphics.Save();
                    e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    e.Graphics.CompositingMode = CompositingMode.SourceCopy;
                    e.Graphics.DrawImage(content, 0, 0);
                    e.Graphics.Restore(checkpoint);
                });
                var feedbackPaintTime = Time(() => { OnPaintFeedback(e); });
                OnRenderOccurred(contentPaintTime, contentCopyTime, feedbackPaintTime);
            }
        }

        private static long Time(Action value)
        {
            var stopwatch = Stopwatch.StartNew();
            value();
            stopwatch.Stop();
            return stopwatch.ElapsedTicks;
        }

        #endregion


        #region Helper Methods

        void RepaintContentIfRequired()
        {
            if (null == content)
            {
                content = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
                contentStale = true;
            }

            if (contentStale)
            {
                using (var g = Graphics.FromImage(content))
                using (var e = new PaintEventArgs(
                    g, new Rectangle(0, 0, content.Width, content.Height)))
                {
                    OnPaintContent(e);

                    if (debugFrameCounterVisible)
                        PaintDebugFrameCounter(g);
                }

                IncrementDebugFrameCounter();
                contentStale = false;
            }
        }

        void IncrementDebugFrameCounter()
        {
            debugFrameCounter = (debugFrameCounter + 1) % 10;
        }

        void PaintDebugFrameCounter(Graphics g)
        {
            var rect = new Rectangle(0, 0, 15, 15);
            g.FillRectangle(Brushes.White, rect);
            g.DrawRectangle(Pens.Black, rect);

            using (var format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;

                using (var font = new Font(FontFamily.GenericMonospace, 15, GraphicsUnit.Pixel))
                    g.DrawString(debugFrameCounter.ToString(CultureInfo.InvariantCulture),
                        font, Brushes.Black, rect, format);
            }
        }

        void PaintDebugMessage(PaintEventArgs e, Color background, Pen border, string text)
        {
            e.Graphics.Clear(background);
            e.Graphics.DrawRectangle(border,
                e.ClipRectangle.X, e.ClipRectangle.Y,
                e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);

            e.Graphics.DrawString(text, Font, Brushes.Black, ClientRectangle);
        }

        #endregion
    }

    /// <summary>
    /// Provides data for the WysiwigPanel.RenderOccurred event.
    /// </summary>
    public class RenderPerformanceEventArgs : EventArgs
    {
        /// <summary>
        /// How long it took to paint the content.
        /// </summary>
        public long ContentPaintTime { get; private set; }

        /// <summary>
        /// How long it took to copy the content to the frame buffer.
        /// </summary>
        public long ContentCopyTime { get; private set; }

        /// <summary>
        /// How long it took to paint the feedback.
        /// </summary>
        public long FeedbackPaintTime { get; private set; }

        /// <summary>
        /// Initializes a new instance of the RenderPerformanceEventArgs class.
        /// </summary>
        /// <param name="contentPaintTime">How long it took to paint the content.</param>
        /// <param name="contentCopyTime">
        /// How long it took to copy the content to the frame buffer.
        /// </param>
        /// <param name="feedbackPaintTime">How long it took to paint the feedback.</param>
        public RenderPerformanceEventArgs(
            long contentPaintTime, long contentCopyTime, long feedbackPaintTime)
        {
            ContentPaintTime = contentPaintTime;
            ContentCopyTime = contentCopyTime;
            FeedbackPaintTime = feedbackPaintTime;
        }
    }
}
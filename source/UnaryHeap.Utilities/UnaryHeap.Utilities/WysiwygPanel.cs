#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ___
{
    /// <summary>
    /// Represents a display window that is optimized for efficient rendering of content
    /// (potentially expensive to render but infrequently changed) and feedback (inexpensive
    /// to render but frequently changed due to user input).
    /// </summary>
    public class WysiwygPanel : Control
    {
        #region Member Variables

        Bitmap content = null;
        bool contentStale = true;
        int debugFrameCounter = 0;
        bool debugFrameCounterVisible = false;

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
                RepaintContentIfRequired();
                e.Graphics.DrawImage(content, 0, 0);
                OnPaintFeedback(e);
            }
        }

        #endregion


        #region Helper Methods

        void RepaintContentIfRequired()
        {
            if (null == content)
            {
                content = new Bitmap(Width, Height);
                contentStale = true;
            }

            if (contentStale)
            {
                using (var g = Graphics.FromImage(content))
                {
                    OnPaintContent(new PaintEventArgs(
                        g, new Rectangle(0, 0, content.Width, content.Height)));

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

            var format = new StringFormat { Alignment = StringAlignment.Center };

            using (var font = new Font(FontFamily.GenericMonospace, 15, GraphicsUnit.Pixel))
                g.DrawString(debugFrameCounter.ToString(CultureInfo.InvariantCulture),
                    font, Brushes.Black, rect, format);
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
}

#endif
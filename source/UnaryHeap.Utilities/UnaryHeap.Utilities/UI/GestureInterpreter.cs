//#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace UnaryHeap.Utilities.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class GestureInterpreter : Component
    {
        #region Member Variables

        Control target;
        EventHandler currentStateMouseEnter;
        EventHandler currentStateMouseLeave;
        EventHandler<MouseEventArgs> currentStateMouseMove;
        EventHandler<MouseEventArgs> currentStateMouseDown;
        EventHandler<MouseEventArgs> currentStateMouseUp;
        EventHandler<KeyEventArgs> currentStateKeyDown;
        EventHandler<KeyEventArgs> currentStateKeyUp;
        GestureState currentState;
        Point? currentPos;
        Point? dragStartPos;
        MouseButtons? clickButton;
        Keys? modifierKeys;

        #endregion


        #region Events

        /// <summary>
        /// Occurs when the gesture state of the GestureInterpreter has changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when the gesture state of the GestureInterpreter has changed.")]
        public event EventHandler StateChanged;
        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        protected void OnStateChanged()
        {
            if (null != StateChanged)
                StateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when a drag gesture is input on the target control.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when a drag gesture is input on the target control.")]
        public event EventHandler<DragGestureEventArgs> DragGestured;
        /// <summary>
        /// Raises the DragGestured event.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="button"></param>
        /// <param name="modifiers"></param>
        protected void OnDragGestured(
            Point startPoint, Point endPoint, MouseButtons button, Keys modifiers)
        {
            if (null != DragGestured)
                DragGestured(this,
                    new DragGestureEventArgs(startPoint, endPoint, button, modifiers));
        }

        /// <summary>
        /// Occurs when a click gesture is input on the target control.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when a click gesture is input on the target control.")]
        public event EventHandler<ClickGestureEventArgs> ClickGestured;
        /// <summary>
        /// Raises the ClickGestured event.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="button"></param>
        /// <param name="modifiers"></param>
        protected void OnClickGestured(Point point, MouseButtons button, Keys modifiers)
        {
            if (null != ClickGestured)
                ClickGestured(this, new ClickGestureEventArgs(point, button, modifiers));
        }

        #endregion


        #region Properties

        /// <summary>
        /// The control for which the GestureInterpreter will interpret input events.
        /// </summary>
        [Category("Behavior")]
        [Description(
            "The control for which the GestureInterpreter will interpret input events.")]
        public Control Target
        {
            get
            {
                return target;
            }
            set
            {
                DetachEventHandlers();
                target = value;
                AttachEventHandlers();
                SetIdleState();
            }
        }

        /// <summary>
        /// Gets the current coordinates of the mouse cursor, relative to the Target control.
        /// Only valid when CurrentState is not Idle.
        /// </summary>
        [Browsable(false)]
        public Point CurrentPosition
        {
            get
            {
                if (GestureState.Idle == currentState)
                    throw new InvalidOperationException();

                return currentPos.Value;
            }
        }

        /// <summary>
        /// Gets the coordinates of the mouse cursor when the drag gesture was initiated.
        /// Only valid when CurrentState is Dragging.
        /// </summary>
        [Browsable(false)]
        public Point DragStartPosition
        {
            get
            {
                if (GestureState.Dragging != currentState)
                    throw new InvalidOperationException();

                return dragStartPos.Value;
            }
        }

        /// <summary>
        /// Gets the mouse button that is currently clicked. Only valid when CurrentState
        /// is either Clicking or Dragging.
        /// </summary>
        [Browsable(false)]
        public MouseButtons ClickButton
        {
            get
            {
                if (GestureState.Clicking != currentState
                    && GestureState.Dragging != currentState)
                    throw new InvalidOperationException();

                return clickButton.Value;
            }
        }

        /// <summary>
        /// Gets the modifier keys that were pressed at the time that the current click was
        /// initiated. Only valid when CurrentState is either Clicking or Dragging.
        /// </summary>
        [Browsable(false)]
        public Keys ModifierKeys
        {
            get
            {
                if (GestureState.Clicking != currentState
                    && GestureState.Dragging != currentState)
                    throw new InvalidOperationException();

                return modifierKeys.Value;
            }
        }

        /// <summary>
        /// Gets the current gesture state of the Target control.
        /// </summary>
        [Browsable(false)]
        public GestureState CurrentState
        {
            get { return currentState; }
        }

        #endregion


        #region Target Event Handling

        void DetachEventHandlers()
        {
            if (null == target)
                return;

            target.MouseDown -= target_MouseDown;
            target.MouseUp -= target_MouseUp;
            target.MouseMove -= target_MouseMove;
            target.MouseEnter -= target_MouseEnter;
            target.MouseLeave -= target_MouseLeave;
            target.KeyDown -= target_KeyDown;
            target.KeyUp -= target_KeyUp;
        }

        void AttachEventHandlers()
        {
            if (null == target)
                return;

            target.MouseDown += target_MouseDown;
            target.MouseUp += target_MouseUp;
            target.MouseMove += target_MouseMove;
            target.MouseEnter += target_MouseEnter;
            target.MouseLeave += target_MouseLeave;
            target.KeyDown += target_KeyDown;
            target.KeyUp += target_KeyUp;
        }

        void target_MouseEnter(object sender, EventArgs e)
        {
            currentStateMouseEnter(sender, e);
        }

        void target_MouseLeave(object sender, EventArgs e)
        {
            currentStateMouseLeave(sender, e);
        }

        void target_MouseMove(object sender, MouseEventArgs e)
        {
            currentStateMouseMove(sender, e);
        }

        void target_MouseDown(object sender, MouseEventArgs e)
        {
            currentStateMouseDown(sender, e);
        }

        void target_MouseUp(object sender, MouseEventArgs e)
        {
            currentStateMouseUp(sender, e);
        }

        void target_KeyDown(object sender, KeyEventArgs e)
        {
            currentStateKeyDown(sender, e);
        }

        void target_KeyUp(object sender, KeyEventArgs e)
        {
            currentStateKeyUp(sender, e);
        }

        #endregion


        #region Idle state

        void SetIdleState()
        {
            currentState = GestureState.Idle;
            currentStateMouseEnter = idleMouseEnter;
            currentStateMouseLeave = idleMouseLeave;
            currentStateMouseMove = idleMouseMove;
            currentStateMouseDown = idleMouseDown;
            currentStateMouseUp = idleMouseUp;
            currentStateKeyDown = idleKeyDown;
            currentStateKeyUp = idleKeyUp;

            currentPos = null;
            dragStartPos = null;
            clickButton = null;
            modifierKeys = null;

            OnStateChanged();
        }

        void idleMouseEnter(object sender, EventArgs e)
        {
        }

        void idleMouseLeave(object sender, EventArgs e)
        {
        }

        void idleMouseMove(object sender, MouseEventArgs e)
        {
            SetHoverState(e.Location);
        }

        void idleMouseDown(object sender, MouseEventArgs e)
        {
        }

        void idleMouseUp(object sender, MouseEventArgs e)
        {
        }

        void idleKeyDown(object sender, KeyEventArgs e)
        {
        }

        void idleKeyUp(object sender, KeyEventArgs e)
        {
        }

        #endregion


        #region Hover State

        void SetHoverState(Point pos)
        {
            currentState = GestureState.Hover;
            currentStateMouseEnter = hoverMouseEnter;
            currentStateMouseLeave = hoverMouseLeave;
            currentStateMouseMove = hoverMouseMove;
            currentStateMouseDown = hoverMouseDown;
            currentStateMouseUp = hoverMouseUp;
            currentStateKeyDown = hoverKeyDown;
            currentStateKeyUp = hoverKeyUp;

            currentPos = pos;
            dragStartPos = null;
            modifierKeys = null;

            OnStateChanged();
        }

        void hoverMouseEnter(object sender, EventArgs e)
        {
        }

        void hoverMouseLeave(object sender, EventArgs e)
        {
            SetIdleState();
        }

        void hoverMouseMove(object sender, MouseEventArgs e)
        {
            SetHoverState(e.Location);
        }

        void hoverMouseDown(object sender, MouseEventArgs e)
        {
            if (clickButton.HasValue)
                return;

            SetClickingState(e.Location, e.Button, Control.ModifierKeys);
        }

        void hoverMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == clickButton)
                clickButton = null;
        }

        void hoverKeyDown(object sender, KeyEventArgs e)
        {
        }

        void hoverKeyUp(object sender, KeyEventArgs e)
        {
        }

        #endregion


        #region Clicking State

        void SetClickingState(Point pos, MouseButtons button, Keys modifiers)
        {
            currentState = GestureState.Clicking;
            currentStateMouseEnter = clickingMouseEnter;
            currentStateMouseLeave = clickingMouseLeave;
            currentStateMouseMove = clickingMouseMove;
            currentStateMouseDown = clickingMouseDown;
            currentStateMouseUp = clickingMouseUp;
            currentStateKeyDown = clickingKeyDown;
            currentStateKeyUp = clickingKeyUp;

            currentPos = pos;
            dragStartPos = null;
            clickButton = button;
            modifierKeys = modifiers;

            OnStateChanged();
        }

        void clickingMouseEnter(object sender, EventArgs e)
        {
        }

        void clickingMouseLeave(object sender, EventArgs e)
        {
        }

        void clickingMouseMove(object sender, MouseEventArgs e)
        {
            var dragSize = SystemInformation.DragSize;
            var dragWindow = new Rectangle(
                currentPos.Value.X - dragSize.Width / 2,
                currentPos.Value.Y - dragSize.Height / 2,
                dragSize.Width, dragSize.Height);

            if (false == dragWindow.Contains(e.Location))
                SetDraggingState(currentPos.Value, e.Location);
        }

        void clickingMouseDown(object sender, MouseEventArgs e)
        {
            SetHoverState(e.Location);
        }

        void clickingMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != clickButton)
                return;

            OnClickGestured(e.Location, clickButton.Value, modifierKeys.Value);
            SetHoverState(e.Location);
            clickButton = null;
        }

        void clickingKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        void clickingKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        #endregion


        #region Dragging

        void SetDraggingState(Point start, Point pos)
        {
            currentState = GestureState.Dragging;
            currentStateMouseEnter = draggingMouseEnter;
            currentStateMouseLeave = draggingMouseLeave;
            currentStateMouseMove = draggingMouseMove;
            currentStateMouseDown = draggingMouseDown;
            currentStateMouseUp = draggingMouseUp;
            currentStateKeyDown = draggingKeyDown;
            currentStateKeyUp = draggingKeyUp;

            currentPos = pos;
            dragStartPos = start;
            OnStateChanged();
        }

        void draggingMouseEnter(object sender, EventArgs e)
        {
        }

        void draggingMouseLeave(object sender, EventArgs e)
        {
        }

        void draggingMouseMove(object sender, MouseEventArgs e)
        {
            SetDraggingState(dragStartPos.Value, e.Location);
        }

        void draggingMouseDown(object sender, MouseEventArgs e)
        {
            SetHoverState(e.Location);
        }

        void draggingMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != clickButton)
                return;

            OnDragGestured(dragStartPos.Value, e.Location, clickButton.Value, modifierKeys.Value);
            SetHoverState(e.Location);
            clickButton = null;
        }

        void draggingKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        void draggingKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum GestureState
    {
        /// <summary>
        /// 
        /// </summary>
        Idle,
        /// <summary>
        /// 
        /// </summary>
        Hover,
        /// <summary>
        /// 
        /// </summary>
        Clicking,
        /// <summary>
        /// 
        /// </summary>
        Dragging
    }

    /// <summary>
    /// 
    /// </summary>
    public class ClickGestureEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Point ClickPoint { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public MouseButtons Button { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Keys ModifierKeys { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clickPoint"></param>
        /// <param name="button"></param>
        /// <param name="modifierKeys"></param>
        public ClickGestureEventArgs(
            Point clickPoint, MouseButtons button, Keys modifierKeys)
        {
            ClickPoint = clickPoint;
            Button = button;
            ModifierKeys = modifierKeys;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DragGestureEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Point StartPoint { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Point EndPoint { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public MouseButtons Button { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Keys ModifierKeys { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="button"></param>
        /// <param name="modifierKeys"></param>
        public DragGestureEventArgs(
            Point startPoint, Point endPoint, MouseButtons button, Keys modifierKeys)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Button = button;
            ModifierKeys = modifierKeys;
        }
    }
}

//#endif
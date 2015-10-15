#if INCLUDE_WORK_IN_PROGRESS

using System;
using System.Diagnostics;
using System.Drawing;

namespace ___
{
    /// <summary>
    /// 
    /// </summary>
    public class GestureInterpreterDiagnostic
    {
        WysiwygPanel target;
        GestureInterpreter interpreter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="interpreter"></param>
        public GestureInterpreterDiagnostic(
            WysiwygPanel target, GestureInterpreter interpreter)
        {
            this.interpreter = interpreter;
            this.target = target;

            target.PaintFeedback += target_PaintFeedback;
            target.PaintContent += target_PaintContent;

            interpreter.DragGestured += interpreter_DragGestured;
            interpreter.ClickGestured += interpreter_ClickGestured;
            interpreter.StateChanged += interpreter_StateChanged;
        }

        void interpreter_StateChanged(object sender, EventArgs e)
        {
            target.InvalidateFeedback();
        }

        void interpreter_ClickGestured(object sender, ClickGestureEventArgs e)
        {
            Debug.WriteLine("{0} {1}  {2}  {3}",
                DateTime.Now, e.ClickPoint, e.Button, e.ModifierKeys);
        }

        void interpreter_DragGestured(object sender, DragGestureEventArgs e)
        {
            Debug.WriteLine("{0} {1} -> {2}  {3}  {4}",
                DateTime.Now, e.StartPoint, e.EndPoint, e.Button, e.ModifierKeys);
        }

        void target_PaintContent(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            e.Graphics.DrawRectangle(Pens.GreenYellow,
                new Rectangle(0, 0, target.Width - 1, target.Height - 1));
        }

        void target_PaintFeedback(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            switch (interpreter.CurrentState)
            {
                case GestureState.Hover:
                    {
                        var point = interpreter.CurrentPosition;

                        e.Graphics.DrawLine(Pens.DarkOrange,
                            new Point(e.ClipRectangle.Left, point.Y),
                            new Point(e.ClipRectangle.Right, point.Y));
                        e.Graphics.DrawLine(Pens.DarkOrange,
                            new Point(point.X, e.ClipRectangle.Bottom),
                            new Point(point.X, e.ClipRectangle.Top));
                    }
                    break;

                case GestureState.Clicking:
                    {
                        var point = interpreter.CurrentPosition;

                        e.Graphics.DrawRectangle(Pens.DarkOrange,
                            new Rectangle(point.X - 2, point.Y - 2, 4, 4));

                        e.Graphics.DrawString(interpreter.ClickButton.ToString(),
                            target.Font, Brushes.Black, new PointF(0f, 48f));
                        e.Graphics.DrawString(interpreter.ModifierKeys.ToString(),
                            target.Font, Brushes.Black, new PointF(0f, 64f));
                    }
                    break;

                case GestureState.Dragging:
                    {
                        var point = interpreter.CurrentPosition;

                        e.Graphics.DrawLine(Pens.DarkOrange,
                            interpreter.CurrentPosition, interpreter.DragStartPosition);
                        e.Graphics.DrawRectangle(Pens.DarkOrange,
                            new Rectangle(point.X - 1, point.Y - 1, 2, 2));

                        e.Graphics.DrawString(interpreter.ClickButton.ToString(),
                            target.Font, Brushes.Black, new PointF(0f, 48f));
                        e.Graphics.DrawString(interpreter.ModifierKeys.ToString(),
                            target.Font, Brushes.Black, new PointF(0f, 64f));
                    }
                    break;
            }



            e.Graphics.DrawString(interpreter.CurrentState.ToString(),
                target.Font, Brushes.Black, new PointF(0f, 0f));

            if (interpreter.CurrentState == GestureState.Dragging)
                e.Graphics.DrawString(interpreter.DragStartPosition.ToString(),
                    target.Font, Brushes.Black, new PointF(0f, 16f));

            if (interpreter.CurrentState != GestureState.Idle)
                e.Graphics.DrawString(interpreter.CurrentPosition.ToString(),
                    target.Font, Brushes.Black, new PointF(0f, 32f));
        }
    }
}

#endif
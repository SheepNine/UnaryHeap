using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Reversi.Forms
{
    public partial class ReversiBoardControl : Control
    {
        protected virtual void OnSquareClicked(int x, int y)
        {
            if (SquareClicked != null)
                SquareClicked(this, new SquareClickedEventArgs(x, y));
        }
        public event EventHandler<SquareClickedEventArgs> SquareClicked;


        string stateString;
        bool isActivePlayer;


        public ReversiBoardControl()
        {
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void UpdateState(string newState)
        {
            stateString = newState;
            Invalidate();
        }

        public bool IsActivePlayer
        {
            get { return isActivePlayer; }
            set
            {
                if (isActivePlayer == value)
                    return;

                isActivePlayer = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            g.Clear(Color.Gray);

            if (stateString != null)
            {
                var squareSize = Math.Min(Width, Height) / 8;

                foreach (var y in Enumerable.Range(0, 8))
                    foreach (var x in Enumerable.Range(0, 8))
                    {
                        g.SmoothingMode = SmoothingMode.None;
                        g.FillRectangle(Brushes.DarkGreen,
                            x * squareSize, y * squareSize, squareSize, squareSize);
                        g.DrawRectangle(Pens.Black,
                            x * squareSize, y * squareSize, squareSize - 1, squareSize - 1);
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        switch (stateString[x + 8 * y])
                        {
                            case '0':
                                break;
                            case '1':
                                g.FillEllipse(Brushes.White,
                                    2 + x * squareSize, 2 + y * squareSize,
                                    squareSize - 5, squareSize - 5);
                                break;
                            case '2':
                                g.FillEllipse(Brushes.Black, 
                                    2 + x * squareSize, 2 + y * squareSize,
                                    squareSize - 5, squareSize - 5);    
                                break;
                        }
                    }

                if (isActivePlayer)
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.DrawRectangle(Pens.Orange, 0, 0, Width - 1, Height - 1);
                    g.DrawRectangle(Pens.Orange, 1, 1, Width - 3, Height - 3);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            var squareSize = Math.Min(Width, Height) / 8;
            var x = e.X / squareSize;
            var y = e.Y / squareSize;

            if (x >= 0 && x < 8 && y >= 0 && y < 8)
                OnSquareClicked(x, y);
        }
    }

    public class SquareClickedEventArgs : EventArgs
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public SquareClickedEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}

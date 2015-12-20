using UnaryHeap.Utilities.Core;
using UnaryHeap.Utilities.D2;

namespace GraphPaper
{
    class GridSnapper
    {
        Rational GridSize = new Rational(1, 2);

        public Point2D Snap(Point2D p)
        {
            return new Point2D(
                GridSize * (p.X / GridSize).Rounded,
                GridSize * (p.Y / GridSize).Rounded);
        }
    }
}

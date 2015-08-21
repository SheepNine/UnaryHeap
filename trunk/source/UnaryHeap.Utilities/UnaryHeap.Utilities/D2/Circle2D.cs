#if INCLUDE_WORK_IN_PROGRESS

using UnaryHeap.Utilities.Core;

namespace UnaryHeap.Utilities.D2
{
    /// <summary>
    /// Represents a rational circle.
    /// </summary>
    public class Circle2D
    {
        Point2D center;
        Rational quadrance;

        /// <summary>
        /// Initializes a new instance of the Circle2D class with a zero quadrance.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        public Circle2D(Point2D center) : this(center, Rational.Zero) { }

        /// <summary>
        /// Initializes a new instance of the Circle2D class with a zero quadrance.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="quadrance">The quadrance of the circle.</param>
        public Circle2D (Point2D center, Rational quadrance)
        {
            this.center = center;
            this.quadrance = quadrance;
        }

        /// <summary>
        /// Computes the circle containing a given triple of points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>The circle containing all three points,
        /// or null, if the points ar colinear</returns>
        public static Circle2D Circumcircle(Point2D a, Point2D b, Point2D c)
        {
            var G = 2 * (a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y));

            if (0 == G)
                return null;

            var aQ = a.X.Squared + a.Y.Squared;
            var bQ = b.X.Squared + b.Y.Squared;
            var cQ = c.X.Squared + c.Y.Squared;

            var x = (aQ * (b.Y - c.Y) + bQ * (c.Y - a.Y) + cQ * (a.Y - b.Y)) / G;
            var y = (cQ * (b.X - a.X) + bQ * (a.X - c.X) + aQ * (c.X - b.X)) / G;

            return new Circle2D(new Point2D(x, y), (a.X - x).Squared + (a.Y - y).Squared);
        }

        /// <summary>
        /// Gets the center of this Circle2D.
        /// </summary>
        public Point2D Center
        {
            get { return center; }
        }

        /// <summary>
        /// Gets the quadrance of this Circle2D.
        /// </summary>
        public Rational Quadrance
        {
            get { return quadrance; }
        }
    }
}

#endif
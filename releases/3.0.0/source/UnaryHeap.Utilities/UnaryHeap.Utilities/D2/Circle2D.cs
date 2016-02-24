using System;
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
            if (null == center)
                throw new ArgumentNullException("center");
            if (null == quadrance)
                throw new ArgumentNullException("quadrance");
            if (0 > quadrance)
                throw new ArgumentOutOfRangeException("quadrance", "quadrance is negative.");

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
        /// or null, if the points ar colinear.</returns>
        /// <exception cref="System.ArgumentNullException">a, b, or c are null.</exception>
        public static Circle2D Circumcircle(Point2D a, Point2D b, Point2D c)
        {
            if (null == a)
                throw new ArgumentNullException("a");
            if (null == b)
                throw new ArgumentNullException("b");
            if (null == c)
                throw new ArgumentNullException("c");

            var circumcenter = Point2D.Circumcenter(a, b, c);

            if (null == circumcenter)
                return null;

            return new Circle2D(circumcenter, Point2D.Quadrance(a, circumcenter));
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
using System.Collections.Generic;

namespace UnaryHeap.Utilities.D2
{
    /// <summary>
    /// Defines a class that can be used to order Point2D objects.
    /// </summary>
    public class Point2DComparer : IComparer<Point2D>
    {
        #region Member Variables

        bool sortFirstByX;
        bool sortXDescending;
        bool sortYDescending;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Point2DComparer class that sorts first in Y,
        /// then in X, both in ascending order.
        /// </summary>
        public Point2DComparer()
            : this(false, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Point2DComparer class with the
        /// specified sort ordering.
        /// </summary>
        /// <param name="sortFirstByX">If true, points will be sorted by their X
        /// coordinate value, and the Y coordinate will be used as a tiebreaker.
        /// If false, points will be sorted by their Y coordinate value,
        /// and the X coordinate will be used as a tiebreaker.</param>
        /// <param name="sortXDescending">If true, the X coordinates will be sorted in
        /// descending order instead of ascending order.</param>
        /// <param name="sortYDescending">If true, the Y coordinates will be sorted in
        /// descending order instead of ascending order.</param>
        public Point2DComparer(bool sortFirstByX, bool sortXDescending, bool sortYDescending)
        {
            this.sortFirstByX = sortFirstByX;
            this.sortXDescending = sortXDescending;
            this.sortYDescending = sortYDescending;
        }

        #endregion


        #region Comparison

        /// <summary>
        /// Compares two Point2D objects and returns a value indicating whether one is less than,
        /// equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A negative integer, if x is less than y. Zero, if x equals y.
        /// A positive integer, if x is greater than y.</returns>
        public int Compare(Point2D x, Point2D y)
        {
            if (null == x && null == y)
                return 0;
            if (null == x)
                return -1;
            if (null == y)
                return 1;

            var xDiff = x.X.CompareTo(y.X);
            var yDiff = x.Y.CompareTo(y.Y);

            if (sortXDescending)
                xDiff *= -1;
            if (sortYDescending)
                yDiff *= -1;

            if (sortFirstByX)
                return xDiff == 0 ? yDiff : xDiff;
            else
                return yDiff == 0 ? xDiff : yDiff;
        }

        #endregion
    }
}

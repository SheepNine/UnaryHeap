using System.Collections.Generic;
using UnaryHeap.DataType;

namespace UnaryHeap.Utilities.D2
{
    /// <summary>
    /// A comparison object that orders Circle2D instances according to the least Y-value
    /// of the circles, in descending order.
    /// </summary>
    public class CircleBottomComparer : IComparer<Circle2D>
    {
        /// <summary>
        /// Compares two Circle2D objects and returns a value indicating whether one is less than,
        /// equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A negative integer, if x is less than y. Zero, if x equals y.
        /// A positive integer, if x is greater than y.</returns>
        public int Compare(Circle2D x, Circle2D y)
        {
            return CompareCircles(x, y);
        }

        /// <summary>
        /// Compares two Circle2D objects and returns a value indicating whether one is less than,
        /// equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A negative integer, if x is less than y. Zero, if x equals y.
        /// A positive integer, if x is greater than y.</returns>
        public static int CompareCircles(Circle2D x, Circle2D y)
        {
            if (null == x)
            {
                if (null == y)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (null == y)
                    return 1;
                else
                    return CompareNonNullCircles(x, y);
            }
        }

        static int CompareNonNullCircles(Circle2D x, Circle2D y)
        {
            var result = CompareCircleBottoms(x, y);

            if (result == 0)
                result = CompareCircleCenterXs(x, y);

            return result;
        }

        static int CompareCircleBottoms(Circle2D x, Circle2D y)
        {
            if (x.Quadrance == y.Quadrance)
                return y.Center.Y.CompareTo(x.Center.Y);

            Rational qS, qL, yS, yL;
            int invert;

            if (x.Quadrance < y.Quadrance)
            {
                yS = x.Center.Y;
                qS = x.Quadrance;
                yL = y.Center.Y;
                qL = y.Quadrance;
                invert = 1;
            }
            else
            {
                yS = y.Center.Y;
                qS = y.Quadrance;
                yL = x.Center.Y;
                qL = x.Quadrance;
                invert = -1;
            }

            if (yS >= yL)
            {
                return -1 * invert;
            }
            else if ((yL - yS).Squared > qL)
            {
                return 1 * invert;
            }
            else
            {
                var yC = (yS.Squared - yL.Squared + qL - qS) / (2 * (yS - yL));
                return qS.CompareTo((yC - yS).Squared) * invert;
            }
        }

        static int CompareCircleCenterXs(Circle2D x, Circle2D y)
        {
            return x.Center.X.CompareTo(y.Center.X);
        }
    }
}
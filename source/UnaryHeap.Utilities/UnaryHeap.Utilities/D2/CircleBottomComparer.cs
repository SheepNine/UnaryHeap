#if INCLUDE_WORK_IN_PROGRESS

using System.Collections.Generic;
using UnaryHeap.Utilities.Core;

namespace UnaryHeap.Utilities.D2
{
    /// <summary>
    /// A comparison object that orders Circle2D instances according to the least Y-value
    /// of the circles, in descending order.
    /// </summary>
    public class CircleBottomComparer : IComparer<Circle2D>
    {
        public int Compare(Circle2D x, Circle2D y)
        {
            return CompareBottoms(x, y);
        }

        public static int CompareBottoms(Circle2D x, Circle2D y)
        {
            var result = OrderCircleBottoms(x, y);

            if (result == 0)
                result = x.Center.X.CompareTo(y.Center.X);

            return result;
        }

        static int OrderCircleBottoms(Circle2D x, Circle2D y)
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
    }
}

#endif
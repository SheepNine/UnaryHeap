using System;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a two-dimensinal facet, e.g. a line segment.
    /// </summary>
    public class Facet2D
    {
        /// <summary>
        /// The line on which the line segment lies.
        /// </summary>
        public Hyperplane2D Plane { get; private set; }

        /// <summary>
        /// The start point of the line segment.
        /// </summary>
        public Point2D Start { get; private set; }

        /// <summary>
        /// The end point of the line segment.
        /// </summary>
        public Point2D End { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Facet2D class.
        /// </summary>
        /// <param name="plane">The line on which the line segment lies.</param>
        /// <param name="start">The start point of the line segment.</param>
        /// <param name="end">The end point of the line segment.</param>
        public Facet2D(Hyperplane2D plane, Point2D start, Point2D end)
        {
            // TODO: input checks

            Plane = plane;
            Start = start;
            End = end;
        }

        /// <summary>
        /// OSHT FIXME
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="size"></param>
        public Facet2D(Hyperplane2D plane, Rational size)
        {
            this.Plane = plane;
            if (plane.A != 0)
            {
                var sign = plane.A.Sign;
                Start = SolveForX(plane, size * sign);
                End = SolveForX(plane, -size * sign);
            }
            else // Constructor affirms that B != 0
            {
                var sign = plane.B.Sign;
                Start = SolveForY(plane, -size * sign);
                End = SolveForY(plane, size * sign);
            }
        }

        static Point2D SolveForX(Hyperplane2D plane, Rational y)
        {
            return new Point2D(-(plane.B * y + plane.C) / plane.A, y);
        }

        static Point2D SolveForY(Hyperplane2D plane, Rational x)
        {
            return new Point2D(x, -(plane.A * x + plane.C) / plane.B);
        }

        /// <summary>
        /// Computes the front and half facets which would result from splitting this
        /// instance along a line.
        /// </summary>
        /// <param name="plane">The line by which to split this instance.</param>
        /// <param name="frontFacet">The component of this facet in the front halfspace
        /// of the splitting line.</param>
        /// <param name="backFacet">The component of this facet in the back halfspace
        /// of the splitting line.</param>
        public void Split(Hyperplane2D plane,
            out Facet2D frontFacet, out Facet2D backFacet)
        {
            // TODO: input checks

            if (plane.Equals(Plane))
            {
                frontFacet = this;
                backFacet = null;
                return;
            }
            if (plane.Coplane.Equals(Plane))
            {
                frontFacet = null;
                backFacet = this;
                return;
            }

            var startDet = plane.Determinant(Start);
            var endDet = plane.Determinant(End);

            if (startDet >= 0 && endDet >= 0)
            {
                frontFacet = this;
                backFacet = null;
                return;
            }

            if (startDet <= 0 && endDet <= 0)
            {
                frontFacet = null;
                backFacet = this;
                return;
            }

            var tEnd = startDet / (startDet - endDet);
            var tStart = 1 - tEnd;

            var mid = new Point2D(
                Start.X * tStart + End.X * tEnd,
                Start.Y * tStart + End.Y * tEnd);

            if (startDet >= 0)
            {
                frontFacet = new Facet2D(Plane, Start, mid);
                backFacet = new Facet2D(Plane, mid, End);
            }
            else
            {
                backFacet = new Facet2D(Plane, Start, mid);
                frontFacet = new Facet2D(Plane, mid, End);
            }
        }
    }
}

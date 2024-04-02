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
            Plane = plane;
            Start = start;
            End = end;
        }

        /// <summary>
        /// OSHT FIXME
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="radius"></param>
        public Facet2D(Hyperplane2D plane, Rational radius)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Computes the front and half facets which would result from splitting this
        /// instance along a line.
        /// </summary>
        /// <param name="splittingPlane">The line by which to split this instance.</param>
        /// <param name="frontFacet">The component of this facet in the front halfspace
        /// of the splitting plane.</param>
        /// <param name="backFacet">The component of this facet in the back halfspace
        /// of the splitting plane.</param>
        public void Split(Hyperplane2D splittingPlane,
            out Facet2D frontFacet, out Facet2D backFacet)
        {
            if (splittingPlane.Equals(Plane))
            {
                frontFacet = this;
                backFacet = null;
                return;
            }
            if (splittingPlane.Coplane.Equals(Plane))
            {
                frontFacet = null;
                backFacet = this;
                return;
            }

            var startDet = splittingPlane.Determinant(Start);
            var endDet = splittingPlane.Determinant(End);

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

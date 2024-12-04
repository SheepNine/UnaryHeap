using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a three-dimensional facet, e.g. a polygon.
    /// </summary>
    public class Facet3D
    {
        readonly List<Point3D> points;

        /// <summary>
        /// Gets the number of points in this Facet3D's winding.
        /// </summary>
        public int NumPoints
        {
            get { return points.Count; }
        }
        
        /// <summary>
        /// The points of the polygon, in a right-handed winding around
        /// the surface normal.
        /// </summary>
        public IEnumerable<Point3D> Points
        {
            get { return points; }
        }

        /// <summary>
        /// The plane on which the polygon lies.
        /// </summary>
        public Hyperplane3D Plane { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Facet3D class.
        /// </summary>
        /// <param name="plane">The plane on which the polygon lies.</param>
        /// <param name="points">The points of the polygon.</param>
        public Facet3D(Hyperplane3D plane, IEnumerable<Point3D> points)
        {
            if (null == points)
                throw new ArgumentNullException(nameof(plane));

            Plane = plane ?? throw new ArgumentNullException(nameof(plane));
            this.points = new List<Point3D>(points);

            if (this.points.Any(p => p == null))
                throw new ArgumentNullException(nameof(points));
        }

        /// <summary>
        /// Initializes a new instance of the Facet3D class by intersecting a plane with a
        /// hypercube of given side length.
        /// </summary>
        /// <param name="plane">The plane upon which the facet should lie.</param>
        /// <param name="sideLength">The length of side of hypercube to use to find the
        /// corners of the plane.</param>
        public Facet3D(Hyperplane3D plane, Rational sideLength)
            : this(plane, ComputeWinding(plane, sideLength))
        {
        }

        static List<Point3D> ComputeWinding(Hyperplane3D plane, Rational size)
        {
            if (plane.A != 0)
            {
                var sign = plane.A.Sign;
                return new List<Point3D>()
                {
                    SolveForX(plane, size, size),
                    SolveForX(plane, -size * sign, size * sign),
                    SolveForX(plane, -size, -size),
                    SolveForX(plane, size * sign, -size * sign),
                };
            }
            else if (plane.B != 0)
            {
                var sign = plane.B.Sign;
                return new List<Point3D>()
                {
                    SolveForY(plane, size, size),
                    SolveForY(plane, size * sign, -size * sign),
                    SolveForY(plane, -size, -size),
                    SolveForY(plane, -size * sign, size * sign),
                };
            }
            else // Constructor affirms that C != 0
            {
                var sign = plane.C.Sign;
                return new List<Point3D>()
                {
                    SolveForZ(plane, size, size),
                    SolveForZ(plane, -size * sign, size * sign),
                    SolveForZ(plane, -size, -size),
                    SolveForZ(plane, size * sign, -size * sign),
                };
            }
        }

        static Point3D SolveForX(Hyperplane3D plane, Rational y, Rational z)
        {
            return new Point3D(-(plane.B * y + plane.C * z + plane.D) / plane.A, y, z);
        }

        static Point3D SolveForY(Hyperplane3D plane, Rational x, Rational z)
        {
            return new Point3D(x, -(plane.A * x + plane.C * z + plane.D) / plane.B, z);
        }

        static Point3D SolveForZ(Hyperplane3D plane, Rational x, Rational y)
        {
            return new Point3D(x, y, -(plane.A * x + plane.B * y + plane.D) / plane.C);
        }

        /// <summary>
        /// Computes the front and half polygons which would result from splitting this
        /// instance along a plane.
        /// </summary>
        /// <param name="plane">The plane by which to split this instance.</param>
        /// <param name="frontFacet">The component of this face in the front half
        /// of the splitting plane.</param>
        /// <param name="backFacet">The component of this facet in the back half
        /// of the splitting plane.</param>
        public void Split(Hyperplane3D plane, out Facet3D frontFacet, out Facet3D backFacet)
        {
            if (null == plane)
                throw new ArgumentNullException(nameof(plane));

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

            // TODO : use determinant here; save having to compute twice
            var windingPointHalfspaces = points.Select(point =>
                plane.DetermineHalfspaceOf(point)).ToList();
            var windingPoints = new List<Point3D>(points);

            if (windingPointHalfspaces.All(hs => hs >= 0))
            {
                frontFacet = this;
                backFacet = null;
                return;
            }
            if (windingPointHalfspaces.All(hs => hs <= 0))
            {
                frontFacet = null;
                backFacet = this;
                return;
            }

            for (var i = windingPoints.Count - 1; i >= 0; i--)
            {
                var j = (i + 1) % windingPoints.Count;

                if (windingPointHalfspaces[i] * windingPointHalfspaces[j] >= 0)
                    continue;

                var detI = plane.Determinant(windingPoints[i]);
                var detJ = plane.Determinant(windingPoints[j]);

                var tJ = detI / (detI - detJ);
                var tI = 1 - tJ;

                var intersectionPoint = new Point3D(
                    windingPoints[i].X * tI + windingPoints[j].X * tJ,
                    windingPoints[i].Y * tI + windingPoints[j].Y * tJ,
                    windingPoints[i].Z * tI + windingPoints[j].Z * tJ);

                windingPointHalfspaces.Insert(i + 1, 0);
                windingPoints.Insert(i + 1, intersectionPoint);
            }

            frontFacet = new Facet3D(Plane,
                Enumerable.Range(0, windingPointHalfspaces.Count)
                .Where(i => windingPointHalfspaces[i] >= 0)
                .Select(i => windingPoints[i]));
            backFacet = new Facet3D(Plane,
                Enumerable.Range(0, windingPointHalfspaces.Count)
                .Where(i => windingPointHalfspaces[i] <= 0)
                .Select(i => windingPoints[i]));
        }

        /// <summary>
        /// Compuutes point indcies for a triangulation of this Facet3D.
        /// </summary>
        /// <returns>A list of triples of points, corresponding to indices of Points.</returns>
        public List<Tuple<int, int, int>> Triangulate()
        {
            List<Tuple<int, int, int>> result = new();

            var pointIndices = Enumerable.Range(0, points.Count).ToList();

            while (pointIndices.Count > 2)
            {
                var pointIndexI = 0;

                while (true)
                {
                    var pointIndexJ = (pointIndexI + 1) % pointIndices.Count;
                    var pointIndexK = (pointIndexI + 2) % pointIndices.Count;

                    var p1 = points[pointIndices[pointIndexI]];
                    var p2 = points[pointIndices[pointIndexJ]];
                    var p3 = points[pointIndices[pointIndexK]];

                    if (!Point3D.AreIndependent(p1, p2, p3))
                    {
                        pointIndexI += 1;
                        continue;
                    }

                    var sphere = Sphere3D.Circumcircle(p1, p2, p3);

                    if (sphere == null || points.Any(p => sphere.DetermineHalfspaceOf(p) < 0))
                    {
                        pointIndexI += 1;
                        continue;
                    }

                    result.Add(Tuple.Create(
                        pointIndices[pointIndexI],
                        pointIndices[pointIndexJ],
                        pointIndices[pointIndexK]
                    ));
                    pointIndices.RemoveAt(pointIndexJ);
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Makes a copy of this Facet3D with additional edge points
        /// </summary>
        /// <param name="extraPoints">The points to add.</param>
        /// <returns>A new Facet3D, with any points in extraPoints that lie between points in
        /// the original facet added to the new facet.</returns>
        public Facet3D AddPointsToEdge(IEnumerable<Point3D> extraPoints)
        {
            var result = new Facet3D(Plane, Points.ToList());
            foreach (var extraPoint in extraPoints)
                AddPointToEdge(result.points, extraPoint);
            return result;
        }

        static void AddPointToEdge(List<Point3D> points, Point3D extraPoint)
        {
            foreach (var i in Enumerable.Range(0, points.Count))
            {
                var a = points[i];
                var b = points[(i + 1) % points.Count];

                if (PointsInLine(a, extraPoint, b))
                {
                    points.Insert(i + 1, extraPoint);
                    return;
                }
            }
        }

        /// <summary>
        /// Check if three points lie in a line
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool PointsInLine(Point3D a, Point3D b, Point3D c)
        {
            Rational commonRatio = null;
            var ratios = new List<Rational>();
            if (a.X == c.X)
            {
                if (b.X != a.X)
                    return false;
            }
            else
            {
                if (!AxisInLine(ref commonRatio, a.X, b.X, c.X))
                    return false;
            }

            if (a.Y == c.Y)
            {
                if (b.Y != a.Y)
                    return false;
            }
            else
            {
                if (!AxisInLine(ref commonRatio, a.Y, b.Y, c.Y))
                    return false;
            }

            if (a.Z == c.Z)
            {
                if (b.Z != a.Z)
                    return false;
            }
            else
            {
                if (!AxisInLine(ref commonRatio, a.Z, b.Z, c.Z))
                    return false;
            }

            return true;
        }

        private static bool AxisInLine(ref Rational commonRatio,
            Rational aX, Rational bX, Rational cX)
        {
            var ratio = (bX - aX) / (cX - aX);
            if (ratio >= 1 || ratio <= 0)
                return false;
            if (commonRatio != null && ratio != commonRatio)
                return false;
            commonRatio = ratio;
            return true;
        }

        /// <summary>
        /// Gets the facet representing this facet, if its front and back halfspaces were flipped.
        /// </summary>
        public Facet3D Cofacet
        {
            get { return new Facet3D(Plane.Coplane, Points.Reverse()); }
        }
    }
}

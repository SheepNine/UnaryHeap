using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.DataType
{
    /// <summary>
    /// Represents a three-dimensional facet, e.g. a polygon.
    /// </summary>
    public class Facet3D
    {
        readonly List<Point3D> points;
        
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
            // TODO: input checks

            Plane = plane;
            this.points = new List<Point3D>(points);
        }

        /// <summary>
        /// OSHT FIXME
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="radius"></param>
        public Facet3D(Hyperplane3D plane, Rational radius)
            : this(plane, ComputeWinding(plane, radius))
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
        public void Split(Hyperplane3D plane,
            out Facet3D frontFacet, out Facet3D backFacet)
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
        /// Constructs a new facet with additional points added to its polygon.
        /// </summary>
        /// <param name="newPoints">The points to add.</param>
        /// <returns>A new facet with the new points included.</returns>
        /// <exception cref="ArgumentException">A point is given which is already in the polygon,
        /// or not along one of the polygon edges.</exception>
        public Facet3D Heal(List<Point3D> newPoints)
        {
            var result = new Facet3D(Plane, Points);

            foreach (var point in newPoints)
            {
                if (result.points.Contains(point))
                    throw new ArgumentException("Facet already contains point");
                if (result.Plane.DetermineHalfspaceOf(point) != 0)
                    throw new ArgumentException("Point not on facet plane");
                Heal(result.points, point);
            }

            return result;
        }

        static void Heal(List<Point3D> points, Point3D newPoint)
        {
            foreach (var i in Enumerable.Range(0, points.Count))
            {
                var a = points[i];
                var b = points[(i + 1) % points.Count];
                var ratios = new List<Rational>();

                if (a.X == b.X)
                {
                    if (newPoint.X != a.X)
                        continue;
                }
                else
                {
                    ratios.Add((newPoint.X - a.X) / (b.X - a.X));
                }

                if (a.Y == b.Y)
                {
                    if (newPoint.Y != a.Y)
                        continue;
                }
                else
                {
                    ratios.Add((newPoint.Y - a.Y) / (b.Y - a.Y));
                }

                if (a.Z == b.Z)
                {
                    if (newPoint.Z != a.Z)
                        continue;
                }
                else
                {
                    ratios.Add((newPoint.Z - a.Z) / (b.Z - a.Z));
                }

                if (ratios[0] > 0 && ratios[0] < 1 && ratios.All(ratio => ratio == ratios[0]))
                {
                    points.Insert(i + 1, newPoint);
                    return;
                }
            }
            //throw new ArgumentException("Got a heal point but found nowhere to put it");
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

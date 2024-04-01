using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace Quake
{
    public class Facet3D
    {
        List<Point3D> points;
        public IEnumerable<Point3D> Points { get { return points; } }
        public Hyperplane3D Plane { get; private set; }

        public Facet3D(Hyperplane3D plane, IEnumerable<Point3D> winding)
        {
            Plane = plane;
            points = new List<Point3D>(winding);
        }

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

        public void Split(Hyperplane3D splitter,
            out Facet3D frontFacet, out Facet3D backFacet)
        {
            if (splitter.Equals(Plane))
            {
                frontFacet = this;
                backFacet = null;
                return;
            }
            if (splitter.Coplane.Equals(Plane))
            {
                frontFacet = null;
                backFacet = this;
                return;
            }

            // TODO : use determinant here; save having to compute twice
            var windingPointHalfspaces = points.Select(point =>
                splitter.DetermineHalfspaceOf(point)).ToList();
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

                var detI = splitter.Determinant(windingPoints[i]);
                var detJ = splitter.Determinant(windingPoints[j]);

                var tJ = detI / (detI - detJ);
                var tI = 1 - tJ;

                if (tI < 0 || tI > 1)
                    throw new Exception("Coefficient not right");

                var intersectionPoint = new Point3D(
                    windingPoints[i].X * tI + windingPoints[j].X * tJ,
                    windingPoints[i].Y * tI + windingPoints[j].Y * tJ,
                    windingPoints[i].Z * tI + windingPoints[j].Z * tJ);

                if (splitter.DetermineHalfspaceOf(intersectionPoint) != 0)
                {
                    throw new Exception("Point calculation isn't right");
                }

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
    }
}

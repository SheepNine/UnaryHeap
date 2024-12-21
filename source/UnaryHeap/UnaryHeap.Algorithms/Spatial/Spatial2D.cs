using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.Algorithms
{

    /// <summary>
    /// Provides 2D-specific implementations of dimensionally-agnostic algorithms.
    /// </summary>
    /// <typeparam name="TSurface">The type representing surfaces to be partitioned by
    /// the algorithm.</typeparam>
    public class Spatial2D<TSurface>
        : Spatial<TSurface, Hyperplane2D, Orthotope2D, Facet2D, Point2D>
        where TSurface: Spatial2D<TSurface>.SurfaceBase
    {
        /// <summary>
        /// Initializes a new instance of the Spatial2D class.
        /// </summary>
        public Spatial2D() : base(new Dimension()) { }

        sealed class Dimension : IDimension
        {
            public int MinBrushFacets { get { return 3; } }

            public void ClassifySurface(Facet2D facet, Hyperplane2D plane,
                out int minDeterminant, out int maxDeterminant)
            {
                if (facet.Plane.Equals(plane))
                {
                    minDeterminant = 1;
                    maxDeterminant = 1;
                    return;
                }
                if (facet.Plane.Equals(plane.Coplane))
                {
                    minDeterminant = -1;
                    maxDeterminant = -1;
                    return;
                }
                var d1 = plane.DetermineHalfspaceOf(facet.Start);
                var d2 = plane.DetermineHalfspaceOf(facet.End);

                minDeterminant = Math.Min(d1, d2);
                maxDeterminant = Math.Max(d1, d2);
            }

            public int ClassifyPoint(Point2D point, Hyperplane2D plane)
            {
                return plane.DetermineHalfspaceOf(point);
            }

            public Orthotope2D CalculateBounds(IEnumerable<Facet2D> facets)
            {
                return Orthotope2D.FromPoints(facets.Select(f => f.Start)
                    .Concat(facets.Select(f => f.End)));
            }

            public Hyperplane2D GetCoplane(Hyperplane2D plane)
            {
                return plane.Coplane;
            }

            public IEnumerable<Facet2D> MakeFacets(Orthotope2D bounds)
            {
                return bounds.MakeFacets();
            }

            public Facet2D Facetize(Hyperplane2D plane)
            {
                return new Facet2D(plane, 100000);
            }

            public Hyperplane2D GetPlane(Facet2D facet)
            {
                return facet.Plane;
            }

            public void Split(Facet2D facet, Hyperplane2D plane,
                out Facet2D front, out Facet2D back)
            {
                facet.Split(plane, out front, out back);
            }

            public Facet2D GetCofacet(Facet2D facet)
            {
                return facet.Cofacet;
            }

            public bool BoundsOverlap(Orthotope2D a, Orthotope2D b)
            {
                return a.Intersects(b);
            }

            public Orthotope2D Expand(Orthotope2D bounds)
            {
                return bounds.GetPadded(1);
            }
        }
    }
}

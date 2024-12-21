using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.Algorithms
{
    /// <summary>
    /// Provides 3D-specific implementations of dimensionally-agnostic algorithms.
    /// </summary>
    /// <typeparam name="TSurface">The type representing surfaces to be partitioned by
    /// the algorithm.</typeparam>
    public class Spatial3D<TSurface>
            : Spatial<TSurface, Hyperplane3D, Orthotope3D, Facet3D, Point3D>
        where TSurface : Spatial3D<TSurface>.SurfaceBase
    {
        /// <summary>
        /// Initializes a new instance of the Spatial3D class.
        /// </summary>
        public Spatial3D() : base(new Dimension()) { }

        class Dimension : IDimension
        {
            public int MinBrushFacets { get { return 4; } }

            public void ClassifySurface(Facet3D facet, Hyperplane3D plane,
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
                var determinants = facet.Points.Select(plane.DetermineHalfspaceOf);

                minDeterminant = determinants.Min();
                maxDeterminant = determinants.Max();
            }

            public int ClassifyPoint(Point3D point, Hyperplane3D plane)
            {
                return plane.DetermineHalfspaceOf(point);
            }

            public Orthotope3D CalculateBounds(IEnumerable<Facet3D> facets)
            {
                return Orthotope3D.FromPoints(facets.SelectMany(f => f.Points));
            }

            public IEnumerable<Facet3D> MakeFacets(Orthotope3D bounds)
            {
                return bounds.MakeFacets();
            }

            public Facet3D Facetize(Hyperplane3D plane)
            {
                return new Facet3D(plane, 100000);
            }

            public Hyperplane3D GetCoplane(Hyperplane3D plane)
            {
                return plane.Coplane;
            }

            public void Split(Facet3D facet, Hyperplane3D plane,
                out Facet3D front, out Facet3D back)
            {
                facet.Split(plane, out front, out back);
            }

            public Hyperplane3D GetPlane(Facet3D facet)
            {
                return facet.Plane;
            }

            public Facet3D GetCofacet(Facet3D facet)
            {
                return facet.Cofacet;
            }

            public bool BoundsOverlap(Orthotope3D a, Orthotope3D b)
            {
                return a.Intersects(b);
            }

            public Orthotope3D Expand(Orthotope3D bounds)
            {
                return bounds.GetPadded(1);
            }
        }
    }
}

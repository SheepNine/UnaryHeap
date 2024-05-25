using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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
        /// <param name="debug">Debugging logic.</param>
        public Spatial3D(IDebug debug) : base(new Dimension(), debug) { }

        /// <summary>
        /// Null object to disable debugging logic.
        /// </summary>
        public class NoDebug : IDebug
        {
            /// <summary>
            /// Called when binary space partitioning had to choose a splitting plan for a
            /// branch node.
            /// </summary>
            /// <param name="elapsedMilliseconds">How long the selection took.</param>
            /// <param name="surfaceCount">The number of surfaces requiring partitioning.</param>
            /// <param name="depth">The depth of the node in the tree.</param>
            /// <param name="partitionPlane">The plane chosen</param>
            public void SplittingPlaneChosen(long elapsedMilliseconds, int surfaceCount,
                int depth, Hyperplane3D partitionPlane)
            {
            }

            /// <summary>
            /// Called when outside culling marks a leaf as interior.
            /// </summary>
            /// <param name="interiorPoint">The origin point of the fill.</param>
            /// <param name="result">The current list of interior leaves.</param>
            /// <param name="leafCount">The number of leaves in the tree.</param>
            public void InsideFilled(Point3D interiorPoint,
                HashSet<BigInteger> result, int leafCount)
            {
            }
        }

        /// <summary>
        /// Dimension-specific logic for the dimensionally-agnostic algorithms.
        /// </summary>
        public class Dimension : IDimension
        {
            /// <summary>
            /// Gets the min and max determinant for a facet against a plane.
            /// If the facet is coincident with the plane, min=max=1.
            /// If the facet is coincident with the coplane, min=max=-1.
            /// Otherwise, this gives the range of determinants of the surface against the plane.
            /// </summary>
            /// <param name="facet">The facet to classify.</param>
            /// <param name="plane">The plane to classify against.</param>
            /// <param name="minDeterminant">
            /// The smallest determinant among the facet's points.</param>
            /// <param name="maxDeterminant">
            /// The greatest determinant among the facet's points.
            /// </param>
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

            /// <summary>
            /// Gets the determinant for a point against a plane.
            /// </summary>
            /// <param name="point">The point to classify.</param>
            /// <param name="plane">The plane against which to classify the point.</param>
            /// <returns>
            /// 1, if the point is in the front halfspace of the plane
            /// -1, if the point is in the back halfspace of the plane
            /// otherwise, 0
            /// </returns>
            public int ClassifyPoint(Point3D point, Hyperplane3D plane)
            {
                return plane.DetermineHalfspaceOf(point);
            }

            /// <summary>
            /// Determine a bounding box containing all of the input surfaces.
            /// </summary>
            /// <param name="facets">The facets to bound.</param>
            /// <returns>The bounding box calculated.</returns>
            public Orthotope3D CalculateBounds(IEnumerable<Facet3D> facets)
            {
                return Orthotope3D.FromPoints(facets.SelectMany(f => f.Points));
            }

            /// <summary>
            /// Calculates the set of facets corresponding to a bounding box.
            /// </summary>
            /// <param name="bounds">The boudning box from which to create facets.</param>
            /// <returns>The set of facets corresponding to bounds.</returns>
            public IEnumerable<Facet3D> MakeFacets(Orthotope3D bounds)
            {
                return bounds.MakeFacets();
            }

            /// <summary>
            /// Create a big facet for a given plane.
            /// </summary>
            /// <param name="plane">The plane of the facet.</param>
            /// <returns>A large facet on the plane.</returns>
            public Facet3D Facetize(Hyperplane3D plane)
            {
                return new Facet3D(plane, 100000);
            }

            /// <summary>
            /// Computes the coplane of the given plane.
            /// </summary>
            /// <param name="plane">The plane from which to get a coplane.</param>
            /// <returns>The complane of the given plane.</returns>
            public Hyperplane3D GetCoplane(Hyperplane3D plane)
            {
                return plane.Coplane;
            }

            /// <summary>
            /// Divide a facet to the pieces lying on either side of a plane.
            /// </summary>
            /// <param name="facet">The facet to split.</param>
            /// <param name="plane">The plane to split with.</param>
            /// <param name="front">The component of facet on the front of the plane.</param>
            /// <param name="back">The component of faet on the back of the plane.</param>
            public void Split(Facet3D facet, Hyperplane3D plane,
                out Facet3D front, out Facet3D back)
            {
                facet.Split(plane, out front, out back);
            }

            /// <summary>
            /// Gets the plane that a facet lies on.
            /// </summary>
            /// <param name="facet">The facet for which to determine a plane.</param>
            /// <returns>The plane that the facet lies on.</returns>
            public Hyperplane3D GetPlane(Facet3D facet)
            {
                return facet.Plane;
            }

            /// <summary>
            /// Get the cofacet of a given facet.
            /// </summary>
            /// <param name="facet">The facet for which to compute a cofacet.</param>
            /// <returns>The cofacet of the given facet.</returns>
            public Facet3D GetCofacet(Facet3D facet)
            {
                return facet.Cofacet;
            }

            /// <summary>
            /// Check whether two bounds overlap.
            /// </summary>
            /// <param name="a">The first bound to check.</param>
            /// <param name="b">The second bound to check.</param>
            /// <returns>true, if the bounds overlap; false otherwise.</returns>
            public bool BoundsOverlap(Orthotope3D a, Orthotope3D b)
            {
                return a.Intersects(b);
            }

            /// <summary>
            /// Calculates the determinant of a point.
            /// Where the plane normal is a unit vector, this will be the minimal distance from
            /// the plane to the point, with a sign corresponding to whether the point is in the
            /// front or back of the plane.
            /// </summary>
            /// <param name="point">The point to check.</param>
            /// <param name="plane">The plane to check against.</param>
            /// <returns>The determinant of a point.</returns>
            public double DeterminatePoint(Point3D point, Hyperplane3D plane)
            {
                return (double)plane.Determinant(point);
            }

            /// <summary>
            /// Make a bounding box a bit bigger.
            /// </summary>
            /// <param name="bounds">The bounding box to make bigger.</param>
            /// <returns>A slightly larget bounding box.</returns>
            public Orthotope3D Expand(Orthotope3D bounds)
            {
                return bounds.GetPadded(1);
            }
        }
    }
}

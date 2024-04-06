using System.Collections.Generic;
using UnaryHeap.DataType;

namespace UnaryHeap.Algorithms
{

    /// <summary>
    /// Provides 2D-specific implementations of dimensionally-agnostic algorithms.
    /// </summary>
    /// <typeparam name="TSurface">The type representing surfaces to be partitioned by
    /// the algorithm.</typeparam>
    public class Spatial2D<TSurface> : Spatial<TSurface, Hyperplane2D, Orthotope2D, Facet2D>
    {
        /// <summary>
        /// Initializes a new instance of the Spatial2D class.
        /// </summary>
        /// <param name="dimension">The specific dimension customization to use
        /// for manipulating surfaces.</param>
        public Spatial2D(Dimension dimension) : base(dimension) { }

        /// <summary>
        /// Dimension-specific logic for the dimensionally-agnostic algorithms.
        /// </summary>
        public abstract class Dimension : IDimension
        {
            /// <summary>
            /// Gets the plane of a surface.
            /// </summary>
            /// <param name="surface">The surface from which to get the plane.</param>
            /// <returns>The plane of the surface.</returns>
            public abstract Hyperplane2D GetPlane(TSurface surface);


            /// <summary>
            /// Gets the min and max determinant for a surface against a plane.
            /// If the surface is coincident with the plane, min=max=1.
            /// If the surface is coincident with the coplane, min=max=-1.
            /// Otherwise, this gives the range of determinants of the surface against the plane.
            /// </summary>
            /// <param name="surface">The surface to classify.</param>
            /// <param name="plane">The plane to classify against.</param>
            /// <param name="minDeterminant">
            /// The smallest determinant among the surface's points.</param>
            /// <param name="maxDeterminant">
            /// The greatest determinant among the surface's points.
            /// </param>
            public abstract void ClassifySurface(TSurface surface, Hyperplane2D plane,
                out int minDeterminant, out int maxDeterminant);

            /// <summary>
            /// Checks if a surface is a 'hint surface' used to speed up the first few levels
            /// of BSP partitioning by avoiding an exhaustive search for a balanced plane.
            /// </summary>
            /// <param name="surface">The surface to check.</param>
            /// <param name="depth">The current depth of the BSP tree.</param>
            /// <returns>True of this surface should be used for a partitioning plane
            /// (and discarded from the final BSP tree), false otherwise.</returns>
            public abstract bool IsHintSurface(TSurface surface, int depth);

            /// <summary>
            /// Splits a surface into two subsurfaces lying on either side of a
            /// partitioning plane.
            /// If surface lies on the partitioningPlane, it should be considered in the
            /// front halfspace of partitioningPlane if its front halfspace is identical
            /// to that of partitioningPlane. Otherwise, it should be considered in the 
            /// back halfspace of partitioningPlane.
            /// </summary>
            /// <param name="surface">The surface to split.</param>
            /// <param name="partitioningPlane">The plane used to split surface.</param>
            /// <param name="frontSurface">The subsurface of surface lying in the front
            /// halfspace of partitioningPlane, or null, if surface is entirely in the
            /// back halfspace of partitioningPlane.</param>
            /// <param name="backSurface">The subsurface of surface lying in the back
            /// halfspace of partitioningPlane, or null, if surface is entirely in the
            /// front halfspace of partitioningPlane.</param>
            public abstract void Split(TSurface surface, Hyperplane2D partitioningPlane,
                out TSurface frontSurface, out TSurface backSurface);

            /// <summary>
            /// Determine a bounding box containing all of the input surfaces.
            /// </summary>
            /// <param name="surfaces">The surfaces to bound.</param>
            /// <returns>The bounding box calculated.</returns>
            public abstract Orthotope2D CalculateBounds(IEnumerable<TSurface> surfaces);

            /// <summary>
            /// Computes the coplane of the given plane.
            /// </summary>
            /// <param name="plane">The plane from which to get a coplane.</param>
            /// <returns>The complane of the given plane.</returns>
            public Hyperplane2D GetCoplane(Hyperplane2D plane)
            {
                return plane.Coplane;
            }

            /// <summary>
            /// Calculates the set of facets corresponding to a bounding box.
            /// </summary>
            /// <param name="bounds">The boudning box from which to create facets.</param>
            /// <returns>The set of facets corresponding to bounds.</returns>
            public IEnumerable<Facet2D> MakeFacets(Orthotope2D bounds)
            {
                return bounds.MakeFacets();
            }

            /// <summary>
            /// Create a big facet for a given plane.
            /// </summary>
            /// <param name="plane">The plane of the facet.</param>
            /// <returns>A large facet on the plane.</returns>
            public Facet2D Facetize(Hyperplane2D plane)
            {
                return new Facet2D(plane, 100000);
            }

            /// <summary>
            /// Gets the plane that a facet lies on.
            /// </summary>
            /// <param name="facet">The facet for which to determine a plane.</param>
            /// <returns>The plane that the facet lies on.</returns>
            public Hyperplane2D GetPlane(Facet2D facet)
            {
                return facet.Plane;
            }

            /// <summary>
            /// Divide a facet to the pieces lying on either side of a plane.
            /// </summary>
            /// <param name="facet">The facet to split.</param>
            /// <param name="plane">The plane to split with.</param>
            /// <param name="front">The component of facet on the front of the plane.</param>
            /// <param name="back">The component of faet on the back of the plane.</param>
            public void Split(Facet2D facet, Hyperplane2D plane,
                out Facet2D front, out Facet2D back)
            {
                facet.Split(plane, out front, out back);
            }

            /// <summary>
            /// Calculate the union of two bounding boxes.
            /// </summary>
            /// <param name="a">The first box.</param>
            /// <param name="b">The second box.</param>
            /// <returns>The union of a and b.</returns>
            public Orthotope2D UnionBounds(Orthotope2D a, Orthotope2D b)
            {
                return new Orthotope2D(
                    Rational.Min(a.X.Min, b.X.Min),
                    Rational.Min(a.Y.Min, b.Y.Min),
                    Rational.Max(a.X.Max, b.X.Max),
                    Rational.Max(a.Y.Max, b.Y.Max)
                );
            }

            /// <summary>
            /// Get the cofacet of a given facet.
            /// </summary>
            /// <param name="facet">The facet for which to compute a cofacet.</param>
            /// <returns>The cofacet of the given facet.</returns>
            public Facet2D GetCofacet(Facet2D facet)
            {
                return new Facet2D(facet.Plane.Coplane, facet.End, facet.Start);
            }
        }
    }
}

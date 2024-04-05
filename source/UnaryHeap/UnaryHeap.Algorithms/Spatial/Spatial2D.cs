using System.Collections.Generic;
using UnaryHeap.DataType;

namespace UnaryHeap.Algorithms
{

    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="TSurface"></typeparam>
    public class Spatial2D<TSurface> : Spatial<TSurface, Hyperplane2D, Orthotope2D, Facet2D>
    {
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="partitioner"></param>
        public Spatial2D(Dimension dimension) : base(dimension) { }

        /// <summary>
        /// TODO
        /// </summary>
        /// <typeparam name="TSurface"></typeparam>
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

            public abstract Orthotope2D CalculateBounds(IEnumerable<TSurface> surfaces);

            public Hyperplane2D GetCoplane(Hyperplane2D partitionPlane)
            {
                return partitionPlane.Coplane;
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

            public void Split(Facet2D facet, Hyperplane2D splitter,
                out Facet2D front, out Facet2D back)
            {
                facet.Split(splitter, out front, out back);
            }

            public Orthotope2D UnionBounds(Orthotope2D a, Orthotope2D b)
            {
                return new Orthotope2D(
                    Rational.Min(a.X.Min, b.X.Min),
                    Rational.Min(a.Y.Min, b.Y.Min),
                    Rational.Max(a.X.Max, b.X.Max),
                    Rational.Max(a.Y.Max, b.Y.Max)
                );
            }

            public Facet2D GetCofacet(Facet2D facet)
            {
                return new Facet2D(facet.Plane.Coplane, facet.End, facet.Start);
            }
        }
    }
}

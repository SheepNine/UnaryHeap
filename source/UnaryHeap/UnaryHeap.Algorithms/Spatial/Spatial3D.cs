using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.Algorithms
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="TSurface"></typeparam>
    public class Spatial3D<TSurface> : Spatial<TSurface, Hyperplane3D, Orthotope3D, Facet3D>
    {
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="partitioner"></param>
        protected Spatial3D(Dimension dimension) : base(dimension) { }

        /// <summary>
        /// TODO
        /// </summary>
        public abstract class Dimension : IDimension
        {
            /// <summary>
            /// Gets the plane of a surface.
            /// </summary>
            /// <param name="surface">The surface from which to get the plane.</param>
            /// <returns>The plane of the surface.</returns>
            public abstract Hyperplane3D GetPlane(TSurface surface);


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
            public abstract void ClassifySurface(TSurface surface, Hyperplane3D plane,
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
            public abstract void Split(TSurface surface, Hyperplane3D partitioningPlane,
                out TSurface frontSurface, out TSurface backSurface);

            public abstract Orthotope3D CalculateBounds(IEnumerable<TSurface> surfaces);

            public Orthotope3D UnionBounds(Orthotope3D a, Orthotope3D b)
            {
                return new Orthotope3D(
                    Rational.Min(a.X.Min, b.X.Min),
                    Rational.Min(a.Y.Min, b.Y.Min),
                    Rational.Min(a.Z.Min, b.Z.Min),
                    Rational.Max(a.X.Max, b.X.Max),
                    Rational.Max(a.Y.Max, b.Y.Max),
                    Rational.Max(a.Z.Max, b.Z.Max)
                );
            }

            public IEnumerable<Facet3D> MakeFacets(Orthotope3D bounds)
            {
                return bounds.MakeFacets();
            }

            public Facet3D Facetize(Hyperplane3D plane)
            {
                return new Facet3D(plane, 100000);
            }

            public Hyperplane3D GetCoplane(Hyperplane3D partitionPlane)
            {
                return partitionPlane.Coplane;
            }

            public void Split(Facet3D facet, Hyperplane3D splitter,
                out Facet3D front, out Facet3D back)
            {
                facet.Split(splitter, out front, out back);
            }

            public Hyperplane3D GetPlane(Facet3D facet)
            {
                return facet.Plane;
            }

            public Facet3D GetCofacet(Facet3D facet)
            {
                return new Facet3D(facet.Plane.Coplane, facet.Points.Reverse());
            }
        }
    }
}

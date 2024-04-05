using System.Collections.Generic;
using System;

namespace UnaryHeap.Algorithms
{
    /// <summary>
    /// Provides an implementation of the binary space partitioning algorithm that is
    /// dimensionally-agnostic.
    /// </summary>
    /// <typeparam name="TSurface">The type representing surfaces to be partitioned by
    /// the algorithm.</typeparam>
    /// <typeparam name="TPlane">The type representing the partitioning planes to be
    /// chosen by the algorithm.</typeparam>
    /// <typeparam name="TBounds"></typeparam>
    /// <typeparam name="TFacet"></typeparam>
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet>
        where TPlane : IEquatable<TPlane>
    {
        /// <summary>
        /// TODO
        /// </summary>
        public interface IDimension
        {

            /// <summary>
            /// Gets the plane of a surface.
            /// </summary>
            /// <param name="surface">The surface from which to get the plane.</param>
            /// <returns>The plane of the surface.</returns>
            TPlane GetPlane(TSurface surface);

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
            void ClassifySurface(TSurface surface, TPlane plane,
                out int minDeterminant, out int maxDeterminant);

            /// <summary>
            /// Checks if a surface is a 'hint surface' used to speed up the first few levels
            /// of BSP partitioning by avoiding an exhaustive search for a balanced plane.
            /// </summary>
            /// <param name="surface">The surface to check.</param>
            /// <param name="depth">The current depth of the BSP tree.</param>
            /// <returns>True of this surface should be used for a partitioning plane
            /// (and discarded from the final BSP tree), false otherwise.</returns>
            bool IsHintSurface(TSurface surface, int depth);

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
            void Split(TSurface surface, TPlane partitioningPlane,
                out TSurface frontSurface, out TSurface backSurface);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="surfaces"></param>
            /// <returns></returns>
            TBounds CalculateBounds(IEnumerable<TSurface> surfaces);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            TBounds UnionBounds(TBounds a, TBounds b);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="bounds"></param>
            /// <returns></returns>
            IEnumerable<TFacet> MakeFacets(TBounds bounds);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="plane"></param>
            /// <returns></returns>
            TFacet Facetize(TPlane plane);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="partitionPlane"></param>
            /// <returns></returns>
            TPlane GetCoplane(TPlane partitionPlane);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="facet"></param>
            /// <param name="splitter"></param>
            /// <param name="front"></param>
            /// <param name="back"></param>
            void Split(TFacet facet, TPlane splitter, out TFacet front, out TFacet back);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="facet"></param>
            /// <returns></returns>
            TPlane GetPlane(TFacet facet);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="facet"></param>
            /// <returns></returns>
            TFacet GetCofacet(TFacet facet);
        }

        readonly IDimension dimension;

        /// <summary>
        /// Initializes a new instance of the BinarySpacePartitioner class.
        /// </summary>
        /// <param name="dimension">Dimensional logic.</param>
        /// <param name="partitioner">The partitioner used to select partitioning
        /// planes for a set of surfaces.</param>
        /// <exception cref="System.ArgumentNullException">partitioner is null.</exception>
        protected Spatial(IDimension dimension)
        {
            if (null == dimension)
                throw new ArgumentNullException(nameof(dimension));

            this.dimension = dimension;
        }
    }
}

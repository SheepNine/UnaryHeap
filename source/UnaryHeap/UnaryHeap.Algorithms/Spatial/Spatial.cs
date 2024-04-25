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
    /// <typeparam name="TBounds">The type representing an axially-aligned bounding box for 
    /// the dimension.</typeparam>
    /// <typeparam name="TFacet">The type representing a facet for the dimension.</typeparam>
    /// <typeparam name="TPoint">The type representing a point for the dimension.</typeparam>
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
    {
        /// <summary>
        /// Dimension-specific logic for the dimensionally-agnostic algorithms.
        /// </summary>
        public interface IDimension
        {
            /// <summary>
            ///  Gets the facet of a surface.
            /// </summary>
            /// <param name="surface">The surface from which to get the facet.</param>
            /// <returns>The facet of the surface.</returns>
            TFacet GetFacet(TSurface surface);

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
            void ClassifySurface(TFacet facet, TPlane plane,
                out int minDeterminant, out int maxDeterminant);

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
            int ClassifyPoint(TPoint point, TPlane plane);

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
            /// Determine a bounding box containing all of the input surfaces.
            /// </summary>
            /// <param name="surfaces">The surfaces to bound.</param>
            /// <returns>The bounding box calculated.</returns>
            TBounds CalculateBounds(IEnumerable<TSurface> surfaces);

            /// <summary>
            /// Calculate the union of two bounding boxes.
            /// </summary>
            /// <param name="a">The first box.</param>
            /// <param name="b">The second box.</param>
            /// <returns>The union of a and b.</returns>
            TBounds UnionBounds(TBounds a, TBounds b);

            /// <summary>
            /// Calculates the set of facets corresponding to a bounding box.
            /// </summary>
            /// <param name="bounds">The boudning box from which to create facets.</param>
            /// <returns>The set of facets corresponding to bounds.</returns>
            IEnumerable<TFacet> MakeFacets(TBounds bounds);

            /// <summary>
            /// Create a big facet for a given plane.
            /// </summary>
            /// <param name="plane">The plane of the facet.</param>
            /// <returns>A large facet on the plane.</returns>
            TFacet Facetize(TPlane plane);

            /// <summary>
            /// Computes the coplane of the given plane.
            /// </summary>
            /// <param name="plane">The plane from which to get a coplane.</param>
            /// <returns>The complane of the given plane.</returns>
            TPlane GetCoplane(TPlane plane);

            /// <summary>
            /// Divide a facet to the pieces lying on either side of a plane.
            /// </summary>
            /// <param name="facet">The facet to split.</param>
            /// <param name="plane">The plane to split with.</param>
            /// <param name="front">The component of facet on the front of the plane.</param>
            /// <param name="back">The component of faet on the back of the plane.</param>
            void Split(TFacet facet, TPlane plane, out TFacet front, out TFacet back);

            /// <summary>
            /// Gets the plane that a facet lies on.
            /// </summary>
            /// <param name="facet">The facet for which to determine a plane.</param>
            /// <returns>The plane that the facet lies on.</returns>
            TPlane GetPlane(TFacet facet);

            /// <summary>
            /// Get the cofacet of a given facet.
            /// </summary>
            /// <param name="facet">The facet for which to compute a cofacet.</param>
            /// <returns>The cofacet of the given facet.</returns>
            TFacet GetCofacet(TFacet facet);

            /// <summary>
            /// Makes a copy of a surface, with the front material replaced.
            /// </summary>
            /// <param name="surface">The surface to copy.</param>
            /// <param name="material">The material to fill in the front.</param>
            /// <returns>The copied surface.</returns>
            TSurface FillFront(TSurface surface, int material);

            /// <summary>
            /// Creates a copy of a surface with the front and back sides reversed.
            /// </summary>
            /// <param name="surface">The surface to copy.</param>
            /// <returns>A new surface with the front and back sides reversed.</returns>
            TSurface GetCosurface(TSurface surface);

            /// <summary>
            /// Check whether two bounds overlap.
            /// </summary>
            /// <param name="a">The first bound to check.</param>
            /// <param name="b">The second bound to check.</param>
            /// <returns>true, if the bounds overlap; false otherwise.</returns>
            bool BoundsOverlap(TBounds a, TBounds b);

            /// <summary>
            /// Get the front material of a surface.
            /// </summary>
            /// <param name="surface">The surface to query.</param>
            /// <returns>The front material of the surface.</returns>
            public abstract int GetFrontMaterial(TSurface surface);

            /// <summary>
            /// Get the back material of a surface.
            /// </summary>
            /// <param name="surface">The surface to query.</param>
            /// <returns>The back material of the surface.</returns>
            public abstract int GetBackMaterial(TSurface surface);

            /// <summary>
            /// Checks whether a plane is an axial plane (e.g. normal vector has N-1 zero values)
            /// </summary>
            /// <param name="p">The plane to check.</param>
            /// <returns>True, if the plane has only one non-zero normal component.</returns>
            bool IsAxial(TPlane p);

            /// <summary>
            /// Calculates the center point of a bounding box.
            /// </summary>
            /// <param name="bounds">The box to check.</param>
            /// <returns>The point in the middle of the bounds.</returns>
            TPoint FindCenterPoint(TBounds bounds);
            
            /// <summary>
            /// Calculates the determinant of a point.
            /// Where the plane normal is a unit vector, this will be the minimal distance from
            /// the plane to the point, with a sign corresponding to whether the point is in the
            /// front or back of the plane.
            /// </summary>
            /// <param name="point">The point to check.</param>
            /// <param name="plane">The plane to check against.</param>
            /// <returns>The determinant of a point.</returns>
            double DeterminatePoint(TPoint point, TPlane plane);

            /// <summary>
            /// Make a bounding box a bit bigger.
            /// </summary>
            /// <param name="bounds">The bounding box to make bigger.</param>
            /// <returns>A slightly larget bounding box.</returns>
            TBounds Expand(TBounds bounds);

            /// <summary>
            /// Checks if a surface is two-sided or not.
            /// </summary>
            /// <param name="surface">The surface to check.</param>
            /// <returns>True if the surface is two-sided (i.e. both its front halfspace
            /// and back halfspace are 'inside'.</returns>
            bool IsTwoSided(TSurface surface);
        }

        readonly IDimension dimension;

        /// <summary>
        /// Initializes a new instance of the BinarySpacePartitioner class.
        /// </summary>
        /// <param name="dimension">Dimensional logic.</param>
        /// <param name="debug">Debugging logic.</param>
        /// <exception cref="System.ArgumentNullException">partitioner is null.</exception>
        protected Spatial(IDimension dimension, IDebug debug)
        {
            this.dimension = dimension ?? throw new ArgumentNullException(nameof(dimension));
            this.debug = debug;
        }
    }
}

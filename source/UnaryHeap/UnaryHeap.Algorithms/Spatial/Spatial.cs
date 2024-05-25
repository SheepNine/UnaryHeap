using System.Collections.Generic;
using System;
using System.Linq;
using System.Numerics;

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
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
    {
        /// <summary>
        /// Base class for surfaces.
        /// </summary>
        public abstract class SurfaceBase
        {
            /// <summary>
            /// The facet for the surface.
            /// </summary>
            public TFacet Facet { get; private set; }

            /// <summary>
            /// The density value of the front half of the surface.
            /// </summary>
            public int FrontMaterial { get; private set; }
            
            /// <summary>
            /// The density value of the back half of the surface.
            /// </summary>
            public int BackMaterial { get; private set; }

            /// <summary>
            /// Whether this surface is two-sided (i.e. both its front and back halves are
            /// interior spaces.
            /// </summary>
            public abstract bool IsTwoSided { get; }

            /// <summary>
            /// Gets a copy of a surface with the front and back sides reversed.
            /// </summary>
            public abstract TSurface Cosurface { get; }

            /// <summary>
            /// Initializes a new instance of the SurfaceBase class.
            /// </summary>
            /// <param name="facet">The facet for the surface.</param>
            /// <param name="frontDensity">
            /// The density value of the front half of the surface.</param>
            /// <param name="backDensity">
            /// The density value of the back half of the surface.</param>
            protected SurfaceBase(TFacet facet, int frontDensity, int backDensity)
            {
                Facet = facet;
                FrontMaterial = frontDensity;
                BackMaterial = backDensity;
            }

            /// <summary>
            /// Checks if this surface is a 'hint surface' used to speed up the first few levels
            /// of BSP partitioning by avoiding an exhaustive search for a balanced plane.
            /// </summary>
            /// <param name="depth">The current depth of the BSP tree.</param>
            /// <returns>True of this surface should be used for a partitioning plane
            /// (and discarded from the final BSP tree), false otherwise.</returns>
            public abstract bool IsHintSurface(int depth);

            /// <summary>
            /// Splits a surface into two subsurfaces lying on either side of a
            /// partitioning plane.
            /// If surface lies on the partitioningPlane, it should be considered in the
            /// front halfspace of partitioningPlane if its front halfspace is identical
            /// to that of partitioningPlane. Otherwise, it should be considered in the 
            /// back halfspace of partitioningPlane.
            /// </summary>
            /// <param name="partitioningPlane">The plane used to split surface.</param>
            /// <param name="frontSurface">The subsurface of surface lying in the front
            /// halfspace of partitioningPlane, or null, if surface is entirely in the
            /// back halfspace of partitioningPlane.</param>
            /// <param name="backSurface">The subsurface of surface lying in the back
            /// halfspace of partitioningPlane, or null, if surface is entirely in the
            /// front halfspace of partitioningPlane.</param>
            public abstract void Split(TPlane partitioningPlane,
                out TSurface frontSurface, out TSurface backSurface);

            /// <summary>
            /// Makes a copy of a surface, with the front material replaced.
            /// </summary>
            /// <param name="material">The material to fill in the front.</param>
            /// <returns>The copied surface.</returns>
            public abstract TSurface FillFront(int material);

            /// <summary>
            /// Makes a copy of a surface, with any edges between the given facets and this
            /// surface's facets healed.
            /// </summary>
            /// <param name="facets">The other surfaces that are potentially adjacent
            /// to this surface.</param>
            /// <returns>A new Surface that has no cracks with the input facets.</returns>
            public abstract TSurface HealEdges(List<TFacet> facets);
        }

        /// <summary>
        /// Dimension-specific logic for the dimensionally-agnostic algorithms.
        /// </summary>
        public interface IDimension
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
            /// Determine a bounding box containing all of the input surfaces.
            /// </summary>
            /// <param name="facets">The facets to bound.</param>
            /// <returns>The bounding box calculated.</returns>
            TBounds CalculateBounds(IEnumerable<TFacet> facets);

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
            /// Check whether two bounds overlap.
            /// </summary>
            /// <param name="a">The first bound to check.</param>
            /// <param name="b">The second bound to check.</param>
            /// <returns>true, if the bounds overlap; false otherwise.</returns>
            bool BoundsOverlap(TBounds a, TBounds b);
            
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

        /// <summary>
        /// Modifies a BSP to ensure that all of the facets meet on mutually-shared edges.
        /// For the 3D case, this solves T-join intersections.
        /// </summary>
        /// <param name="tree">The tree to heal.</param>
        /// <returns>A copy of the tree with edges healed.</returns>
        public IBspTree HealEdges(IBspTree tree)
        {
            var facets = new List<TFacet>();

            tree.InOrderTraverse(index =>
            {
                if (tree.IsLeaf(index))
                {
                    facets.AddRange(tree.Surfaces(index).Select(s => s.Surface.Facet));
                }
            });

            var result = new BspTree(tree as BspTree);
            HealEdges(result, 0, facets);
            return result;
        }

        private void HealEdges(BspTree tree, BigInteger index, List<TFacet> facets)
        {
            if (tree.IsLeaf(index))
            {
                foreach (var surface in tree.Surfaces(index).Cast<BspSurface>())
                    surface.Surface = surface.Surface.HealEdges(facets);
            }
            else
            {
                Partition(facets, tree.PartitionPlane(index),
                    out List<TFacet> frontFacets, out List<TFacet> backFacets);
                HealEdges(tree, index.FrontChildIndex(), frontFacets);
                HealEdges(tree, index.BackChildIndex(), backFacets);
            }
        }

        private void Partition(List<TFacet> facets, TPlane plane,
            out List<TFacet> frontFacets, out List<TFacet> backFacets)
        {
            frontFacets = new();
            backFacets = new();
            foreach (var facet in facets)
            {
                dimension.ClassifySurface(facet, plane, out int minDeterminant,
                    out int maxDeterminant);
                if (minDeterminant < 1)
                    backFacets.Add(facet);
                if (maxDeterminant > -1)
                    frontFacets.Add(facet);
            }
        }
    }
}

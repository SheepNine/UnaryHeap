﻿using System;
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
    {
        /// <summary>
        /// Initializes a new instance of the Spatial3D class.
        /// </summary>
        /// <param name="dimension">The specific dimension customization to use
        /// for manipulating surfaces.</param>
        /// <param name="debug">Debugging logic.</param>
        public Spatial3D(Dimension dimension, IDebug debug) : base(dimension, debug) { }

        /// <summary>
        /// Null object to disable debugging logic.
        /// </summary>
        public class NoDebug : IDebug
        {
            /// <summary>
            /// Called when binary space partitioning partitions a set of surfaces.
            /// </summary>
            /// <param name="partitionPlane">The splitting plane.</param>
            /// <param name="frontSurfaces">
            /// The resulting surfaces on the front of the plane.</param>
            /// <param name="backSurfaces">
            /// The resulting surfaces on the back of the plane.</param>
            public void PartitionOccurred(Hyperplane3D partitionPlane,
                List<TSurface> frontSurfaces, List<TSurface> backSurfaces)
            {
            }
        }

        /// <summary>
        /// Dimension-specific logic for the dimensionally-agnostic algorithms.
        /// </summary>
        public abstract class Dimension : IDimension
        {
            /// <summary>
            ///  Gets the facet of a surface.
            /// </summary>
            /// <param name="surface">The surface from which to get the facet.</param>
            /// <returns>The facet of the surface.</returns>
            public abstract Facet3D GetFacet(TSurface surface);

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
                if (facet.Plane == plane)
                {
                    minDeterminant = 1;
                    maxDeterminant = 1;
                    return;
                }
                if (facet.Plane == plane.Coplane)
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

            /// <summary>
            /// Determine a bounding box containing all of the input surfaces.
            /// </summary>
            /// <param name="surfaces">The surfaces to bound.</param>
            /// <returns>The bounding box calculated.</returns>
            public abstract Orthotope3D CalculateBounds(IEnumerable<TSurface> surfaces);

            /// <summary>
            /// Makes a copy of a surface, with the front material replaced.
            /// </summary>
            /// <param name="surface">The surface to copy.</param>
            /// <param name="material">The material to fill in the front.</param>
            /// <returns>The copied surface.</returns>
            public abstract TSurface FillFront(TSurface surface, int material);

            /// <summary>
            /// Creates a copy of a surface with the front and back sides reversed.
            /// </summary>
            /// <param name="surface">The surface to copy.</param>
            /// <returns>A new surface with the front and back sides reversed.</returns>
            public abstract TSurface GetCosurface(TSurface surface);

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
            /// Calculate the union of two bounding boxes.
            /// </summary>
            /// <param name="a">The first box.</param>
            /// <param name="b">The second box.</param>
            /// <returns>The union of a and b.</returns>
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
        }
    }
}

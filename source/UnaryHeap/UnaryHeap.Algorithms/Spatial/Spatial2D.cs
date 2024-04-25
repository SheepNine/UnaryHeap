using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        /// <param name="dimension">The specific dimension customization to use
        /// for manipulating surfaces.</param>
        /// <param name="debug">Debugging logic.</param>
        public Spatial2D(Dimension dimension, IDebug debug) : base(dimension, debug) { }

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
            /// <param name="surfaces">The input set of surfaces requiring partitioning.</param>
            /// <param name="depth">The depth of the node in the tree.</param>
            /// <param name="partitionPlane">The plane chosen</param>
            public void SplittingPlaneChosen(long elapsedMilliseconds, List<TSurface> surfaces,
                int depth, Hyperplane2D partitionPlane)
            {
            }

            /// <summary>
            /// Called when binary space partitioning partitions a set of surfaces.
            /// </summary>
            /// <param name="elapsedTimeMs">How long the partitioning took.</param>
            /// <param name="surfacesToPartition">
            /// The input set of surfaces requiring partitioning.</param>
            /// <param name="partitionPlane">The splitting plane.</param>
            /// <param name="frontSurfaces">
            /// The resulting surfaces on the front of the plane.</param>
            /// <param name="backSurfaces">
            /// The resulting surfaces on the back of the plane.</param>
            /// <param name="depth">The depth of the node in the tree.</param>
            public void PartitionOccurred(long elapsedTimeMs, List<TSurface> surfacesToPartition,
                int depth, Hyperplane2D partitionPlane,
                List<TSurface> frontSurfaces, List<TSurface> backSurfaces)
            {
            }

            /// <summary>
            /// Called when outside culling marks a leaf as interior.
            /// </summary>
            /// <param name="interiorPoint">The origin point of the fill.</param>
            /// <param name="result">The current list of interior leaves.</param>
            /// <param name="leafCount">The number of leaves in the tree.</param>
            public void InsideFilled(Point2D interiorPoint,
                HashSet<BspNode> result, int leafCount)
            {

            }
        }

        /// <summary>
        /// Dimension-specific logic for the dimensionally-agnostic algorithms.
        /// </summary>
        public abstract class Dimension : IDimension
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
            public void ClassifySurface(Facet2D facet, Hyperplane2D plane,
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
                var d1 = plane.DetermineHalfspaceOf(facet.Start);
                var d2 = plane.DetermineHalfspaceOf(facet.End);

                minDeterminant = Math.Min(d1, d2);
                maxDeterminant = Math.Max(d1, d2);
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
            public int ClassifyPoint(Point2D point, Hyperplane2D plane)
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
            public abstract void Split(TSurface surface, Hyperplane2D partitioningPlane,
                out TSurface frontSurface, out TSurface backSurface);

            /// <summary>
            /// Determine a bounding box containing all of the input surfaces.
            /// </summary>
            /// <param name="facets">The facets to bound.</param>
            /// <returns>The bounding box calculated.</returns>
            public Orthotope2D CalculateBounds(IEnumerable<Facet2D> facets)
            {
                return Orthotope2D.FromPoints(facets.Select(f => f.Start)
                    .Concat(facets.Select(f => f.End)));
            }

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
                return facet.Cofacet;
            }

            /// <summary>
            /// Check whether two bounds overlap.
            /// </summary>
            /// <param name="a">The first bound to check.</param>
            /// <param name="b">The second bound to check.</param>
            /// <returns>true, if the bounds overlap; false otherwise.</returns>
            public bool BoundsOverlap(Orthotope2D a, Orthotope2D b)
            {
                return a.Intersects(b);
            }

            /// <summary>
            /// Checks whether a plane is an axial plane (e.g. normal vector has N-1 zero values)
            /// </summary>
            /// <param name="p">The plane to check.</param>
            /// <returns>True, if the plane has only one non-zero normal component.</returns>
            public bool IsAxial(Hyperplane2D p)
            {
                return p.A == 0 || p.B == 0;
            }

            /// <summary>
            /// Calculates the center point of a bounding box.
            /// </summary>
            /// <param name="bounds">The box to check.</param>
            /// <returns>The point in the middle of the bounds.</returns>
            public Point2D FindCenterPoint(Orthotope2D bounds)
            {
                return bounds.Center;
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
            public double DeterminatePoint(Point2D point, Hyperplane2D plane)
            {
                return (double)plane.Determinant(point);
            }

            /// <summary>
            /// Make a bounding box a bit bigger.
            /// </summary>
            /// <param name="bounds">The bounding box to make bigger.</param>
            /// <returns>A slightly larget bounding box.</returns>
            public Orthotope2D Expand(Orthotope2D bounds)
            {
                return bounds.GetPadded(1);
            }
        }
    }
}

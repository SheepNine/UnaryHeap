﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.DataType;

namespace UnaryHeap.Algorithms
{
    /// <summary>
    /// Interface defining a strategy for partitioning sets of surfaces.
    /// </summary>
    /// <typeparam name="TSurface">The type representing surfaces to be partitioned by
    /// the algorithm.</typeparam>
    /// <typeparam name="TPlane">The type representing the partitioning planes to be
    /// chosen by the algorithm.</typeparam>
    /// <typeparam name="TBounds"></typeparam>
    /// <typeparam name="TFacet"></typeparam>
    public interface IPartitioner<TSurface, TPlane, TBounds, TFacet>
    {
        /// <summary>
        /// Selects a partitioning plane to be used to partition a set of surfaces.
        /// </summary>
        /// <param name="surfacesToPartition">The set of surfaces to partition.</param>
        /// <returns>The selected plane.</returns>
        TPlane SelectPartitionPlane(IEnumerable<TSurface> surfacesToPartition);
    }

    /// <summary>
    /// Implements a partitioning strategy that, at every step, checks all available splitting
    /// planes and chooses the optimal one according to weights assigned to tree balance and
    /// minimizing surface splits.
    /// </summary>
    /// <typeparam name="TSurface">The type representing surfaces to be partitioned by
    /// the algorithm.</typeparam>
    /// <typeparam name="TPlane">The type representing the partitioning planes to be
    /// chosen by the algorithm.</typeparam>
    /// <typeparam name="TBounds"></typeparam>
    /// <typeparam name="TFacet"></typeparam>
    public class ExhaustivePartitioner<TSurface, TPlane, TBounds, TFacet>
        : IPartitioner<TSurface, TPlane, TBounds, TFacet>
    {
        readonly IDimension<TSurface, TPlane, TBounds, TFacet> dimension;
        readonly int imbalanceWeight;
        readonly int splitWeight;

        /// <summary>
        /// Initializes a new instance of the ExhaustivePartitioner class.
        /// </summary>
        /// <param name="dimension">Dimensional logic.</param>
        /// <param name="imbalanceWeight">How many points to subtract for the computed difference
        /// in surfaces on the front and back halves of a splitting plane.</param>
        /// <param name="splitWeight">How many points to subtract for the number of surfaces
        /// that must be split by a splitting plane.</param>
        public ExhaustivePartitioner(IDimension<TSurface, TPlane, TBounds, TFacet> dimension,
            int imbalanceWeight, int splitWeight)
        {
            this.dimension = dimension;
            this.imbalanceWeight = imbalanceWeight;
            this.splitWeight = splitWeight;
        }

        /// <summary>
        /// Selects a partitioning plane to be used to partition a set of surfaces.
        /// </summary>
        /// <param name="surfacesToPartition">The set of surfaces to partition.</param>
        /// <returns>The selected plane.</returns>
        public TPlane SelectPartitionPlane(IEnumerable<TSurface> surfacesToPartition)
        {
            return surfacesToPartition
                .Select(dimension.GetPlane)
                .Distinct()
                .Select(h => ComputeSplitResult(h, surfacesToPartition))
                .Where(splitResult => splitResult != null)
                .OrderBy(splitResult => splitResult.ComputeScore(imbalanceWeight, splitWeight))
                .First()
                .splitter;
        }

        SplitResult ComputeSplitResult(TPlane splitter,
            IEnumerable<TSurface> surfacesToPartition)
        {
            int splits = 0;
            int front = 0;
            int back = 0;

            foreach (var surface in surfacesToPartition)
            {
                dimension.ClassifySurface(surface, splitter,
                    out int minDeterminant, out int maxDeterminant);

                if (maxDeterminant > 0)
                    front += 1;
                if (minDeterminant < 0)
                    back += 1;
                if (maxDeterminant > 0 && minDeterminant < 0)
                    splits += 1;
            }

            if (splits == 0 && (front == 0 || back == 0))
                return null;
            else
                return new SplitResult(splitter, front, back, splits);
        }

        class SplitResult
        {
            public int back;
            public int front;
            public int splits;
            public TPlane splitter;

            public SplitResult(TPlane splitter, int front, int back, int splits)
            {
                this.splitter = splitter;
                this.front = front;
                this.back = back;
                this.splits = splits;
            }

            public int ComputeScore(int imbalanceWeight, int splitWeight)
            {
                return Math.Abs(front - back) * imbalanceWeight + splits * splitWeight;
            }
        }
    }

    /// <summary>
    /// XXXXXX
    /// </summary>
    /// <typeparam name="TSurface"></typeparam>
    public class ExhaustivePartitioner2D<TSurface>
        : ExhaustivePartitioner<TSurface, Hyperplane2D, Orthotope2D, Facet2D>
    {
        /// <summary>
        /// XXXXXX
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="imbalanceWeight"></param>
        /// <param name="splitWeight"></param>
        public ExhaustivePartitioner2D(
            Dimension2D<TSurface> dimension, int imbalanceWeight, int splitWeight)
            : base(dimension, imbalanceWeight, splitWeight)
        {
        }
    }

    /// <summary>
    /// XXXXXX
    /// </summary>
    /// <typeparam name="TSurface"></typeparam>
    public class ExhaustivePartitioner3D<TSurface>
        : ExhaustivePartitioner<TSurface, Hyperplane3D, Orthotope3D, Facet3D>
    {
        /// <summary>
        /// XXXXXX
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="imbalanceWeight"></param>
        /// <param name="splitWeight"></param>
        public ExhaustivePartitioner3D(Dimension3D<TSurface> dimension,
            int imbalanceWeight, int splitWeight)
            : base(dimension, imbalanceWeight, splitWeight)
        {
        }
    }

    /// <summary>
    /// Callback deletage for IBspNode's traversal methods.
    /// </summary>
    /// <param name="target">The BSP node currently being visited.</param>
    /// <typeparam name="TSurface">The type representing surfaces to be partitioned by
    /// the algorithm.</typeparam>
    /// <typeparam name="TPlane">The type representing the partitioning planes to be
    /// chosen by the algorithm.</typeparam>
    public delegate void BspIteratorCallback<TSurface, TPlane>(
        IBspNode<TSurface, TPlane> target);

    /// <summary>
    /// Interface representing a node in a BSP tree.
    /// </summary>
    /// <typeparam name="TSurface">The type representing surfaces to be partitioned by
    /// the algorithm.</typeparam>
    /// <typeparam name="TPlane">The type representing the partitioning planes to be
    /// chosen by the algorithm.</typeparam>
    public interface IBspNode<TSurface, TPlane>
    {
        /// <summary>
        /// Gets whether this node is a leaf node or a branch node.
        /// </summary>
        bool IsLeaf { get; }

        /// <summary>
        /// Gets the partitioning plane of a branch node. Returns null for leaf nodes.
        /// </summary>
        TPlane PartitionPlane { get; }

        /// <summary>
        /// Gets the front child of a branch node. Returns null for leaf nodes.
        /// </summary>
        IBspNode<TSurface, TPlane> FrontChild { get; }

        /// <summary>
        /// Gets the back child of a branch node. Returns null for leaf nodes.
        /// </summary>
        IBspNode<TSurface, TPlane> BackChild { get; }

        /// <summary>
        /// Gets the surfaces in a leaf node. Returns null for branch nodes.
        /// </summary>
        IEnumerable<TSurface> Surfaces { get; }

        /// <summary>
        /// Gets the number of surfaces in a leaf node. Returns 0 fro branch nodes.
        /// </summary>
        int SurfaceCount { get; }

        /// <summary>
        /// Counts the number of nodes in a BSP tree.
        /// </summary>
        int NodeCount { get; }

        /// <summary>
        /// Gets the depth of this node in the BSP tree.
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// Iterates a BSP tree in pre-order.
        /// </summary>
        /// <param name="callback">The callback to run for each node traversed.</param>
        /// <exception cref="System.ArgumentNullException">callback is null.</exception>
        void PreOrderTraverse(BspIteratorCallback<TSurface, TPlane> callback);

        /// <summary>
        /// Iterates a BSP tree in in-order.
        /// </summary>
        /// <param name="callback">The callback to run for each node traversed.</param>
        /// <exception cref="System.ArgumentNullException">callback is null.</exception>
        void InOrderTraverse(BspIteratorCallback<TSurface, TPlane> callback);

        /// <summary>
        /// Iterates a BSP tree in post-order.
        /// </summary>
        /// <param name="callback">The callback to run for each node traversed.</param>
        /// <exception cref="System.ArgumentNullException">callback is null.</exception>
        void PostOrderTraverse(BspIteratorCallback<TSurface, TPlane> callback);
    }

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
    public class BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>
        where TPlane : class
    {
        IDimension<TSurface, TPlane, TBounds, TFacet> dimension;
        IPartitioner<TSurface, TPlane, TBounds, TFacet> partitioner;

        /// <summary>
        /// Initializes a new instance of the BinarySpacePartitioner class.
        /// </summary>
        /// <param name="dimension">Dimensional logic.</param>
        /// <param name="partitioner">The partitioner used to select partitioning
        /// planes for a set of surfaces.</param>
        /// <exception cref="System.ArgumentNullException">partitioner is null.</exception>
        protected BinarySpacePartitioner(IDimension<TSurface, TPlane, TBounds, TFacet> dimension,
            IPartitioner<TSurface, TPlane, TBounds, TFacet> partitioner)
        {
            if (null == dimension)
                throw new ArgumentNullException(nameof(dimension));
            if (null == partitioner)
                throw new ArgumentNullException(nameof(partitioner));

            this.dimension = dimension;
            this.partitioner = partitioner;
        }

        /// <summary>
        /// Constructs a BSP tree for a set of input surfaces.
        /// </summary>
        /// <param name="inputSurfaces">The surfaces to partition.</param>
        /// <returns>The root node of the resulting BSP tree.</returns>
        public IBspNode<TSurface, TPlane> ConstructBspTree(IEnumerable<TSurface> inputSurfaces)
        {
            if (null == inputSurfaces)
                throw new ArgumentNullException(nameof(inputSurfaces));

            var surfaces = inputSurfaces.ToList();

            if (0 == surfaces.Count)
                throw new ArgumentException("No surfaces to partition.");

            return ConstructBspNode(surfaces, 0);
        }

        BspNode ConstructBspNode(List<TSurface> surfaces, int depth)
        {
            if (AllConvex(surfaces))
                return BspNode.LeafNode(surfaces, depth);

            var hintSurface = FindHintSurface(surfaces, depth);

            TPlane partitionPlane;
            if (null != hintSurface)
            {
                partitionPlane = dimension.GetPlane(hintSurface);
                surfaces.Remove(hintSurface);
            }
            else
            {
                partitionPlane = partitioner.SelectPartitionPlane(surfaces);
            }

            if (null == partitionPlane)
                throw new InvalidOperationException("Failed to select partition plane.");

            List<TSurface> frontSurfaces, backSurfaces;
            Partition(surfaces, partitionPlane, out frontSurfaces, out backSurfaces);

            if (0 == frontSurfaces.Count || 0 == backSurfaces.Count)
                throw new InvalidOperationException(
                    "Partition plane selected does not partition surfaces.");

            var frontChild = ConstructBspNode(frontSurfaces, depth + 1);
            var backChild = ConstructBspNode(backSurfaces, depth + 1);
            return BspNode.BranchNode(partitionPlane, depth, frontChild, backChild);
        }

        bool AllConvex(List<TSurface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == AreConvex(surfaces[i], surfaces[j]))
                        return false;

            return true;
        }

        TSurface FindHintSurface(List<TSurface> surfaces, int depth)
        {
            return surfaces.FirstOrDefault(surface => dimension.IsHintSurface(surface, depth));
        }

        void Partition(List<TSurface> surfaces, TPlane partitionPlane,
            out List<TSurface> frontSurfaces, out List<TSurface> backSurfaces)
        {
            frontSurfaces = new List<TSurface>();
            backSurfaces = new List<TSurface>();

            foreach (var surface in surfaces)
            {
                TSurface frontSurface, backSurface;
                dimension.Split(surface, partitionPlane, out frontSurface, out backSurface);

                if (null != frontSurface)
                    frontSurfaces.Add(frontSurface);
                if (null != backSurface)
                    backSurfaces.Add(backSurface);
            }
        }

        /// <summary>
        /// Checks whether two surfaces are mutually convex (that is, neither one is
        /// behind the other). Surfaces which are convex do not need to be partitioned.
        /// </summary>
        /// <param name="a">The first surface to check.</param>
        /// <param name="b">The second surface to check.</param>
        /// <returns>True, if a is in the front halfspace of b and vice versa;
        /// false otherwise.</returns>
        protected bool AreConvex(TSurface a, TSurface b)
        {
            if (null == a)
                throw new ArgumentNullException(nameof(a));
            if (null == b)
                throw new ArgumentNullException(nameof(b));

            dimension.ClassifySurface(a, dimension.GetPlane(b), out int aMin, out _);
            dimension.ClassifySurface(b, dimension.GetPlane(a), out int bMin, out _);

            return aMin >= 0 && bMin >= 0;
        }

        class BspNode : IBspNode<TSurface, TPlane>
        {
            TPlane partitionPlane;
            BspNode frontChild;
            BspNode backChild;
            List<TSurface> surfaces;
            int depth;

            private BspNode() { }

            public static BspNode LeafNode(IEnumerable<TSurface> surfaces, int depth)
            {
                return new BspNode()
                {
                    partitionPlane = null,
                    frontChild = null,
                    backChild = null,
                    surfaces = surfaces.ToList(),
                    depth = depth
                };
            }

            public static BspNode BranchNode(TPlane splitter, int depth,
                BspNode frontChild, BspNode backChild)
            {
                return new BspNode()
                {
                    partitionPlane = splitter,
                    frontChild = frontChild,
                    backChild = backChild,
                    surfaces = null,
                    depth = depth
                };
            }

            public bool IsLeaf
            {
                get { return surfaces != null; }
            }

            public TPlane PartitionPlane
            {
                get { return partitionPlane; }
            }

            public IBspNode<TSurface, TPlane> FrontChild
            {
                get { return frontChild; }
            }

            public IBspNode<TSurface, TPlane> BackChild
            {
                get { return backChild; }
            }

            public IEnumerable<TSurface> Surfaces
            {
                get { return surfaces; }
            }

            public int SurfaceCount
            {
                get { return surfaces == null ? 0 : surfaces.Count; }
            }

            public int Depth
            {
                get { return depth; }
            }

            public int NodeCount
            {
                get
                {
                    if (IsLeaf)
                        return 1;
                    else
                        return 1 + frontChild.NodeCount + backChild.NodeCount;
                }
            }

            public void PreOrderTraverse(BspIteratorCallback<TSurface, TPlane> callback)
            {
                if (null == callback)
                    throw new ArgumentNullException(nameof(callback));

                if (IsLeaf)
                {
                    callback(this);
                }
                else
                {
                    callback(this);
                    frontChild.PreOrderTraverse(callback);
                    backChild.PreOrderTraverse(callback);
                }
            }

            public void InOrderTraverse(BspIteratorCallback<TSurface, TPlane> callback)
            {
                if (null == callback)
                    throw new ArgumentNullException(nameof(callback));

                if (IsLeaf)
                {
                    callback(this);
                }
                else
                {
                    frontChild.InOrderTraverse(callback);
                    callback(this);
                    backChild.InOrderTraverse(callback);
                }
            }

            public void PostOrderTraverse(BspIteratorCallback<TSurface, TPlane> callback)
            {
                if (null == callback)
                    throw new ArgumentNullException(nameof(callback));

                if (IsLeaf)
                {
                    callback(this);
                }
                else
                {
                    frontChild.PostOrderTraverse(callback);
                    backChild.PostOrderTraverse(callback);
                    callback(this);
                }
            }
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="TSurface"></typeparam>
    public class BinarySpacePartitioner2D<TSurface>
        : BinarySpacePartitioner<TSurface, Hyperplane2D, Orthotope2D, Facet2D>
    {
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="partitioner"></param>
        public BinarySpacePartitioner2D(Dimension2D<TSurface> dimension,
            IPartitioner<TSurface, Hyperplane2D, Orthotope2D, Facet2D> partitioner)
            : base(dimension, partitioner)
        {
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="TSurface"></typeparam>
    public class BinarySpacePartitioner3D<TSurface>
        : BinarySpacePartitioner<TSurface, Hyperplane3D, Orthotope3D, Facet3D>
    {
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="partitioner"></param>
        protected BinarySpacePartitioner3D(Dimension3D<TSurface> dimension,
            IPartitioner<TSurface, Hyperplane3D, Orthotope3D, Facet3D> partitioner)
            : base(dimension, partitioner)
        {
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="TSurface"></typeparam>
    /// <typeparam name="TPlane"></typeparam>
    /// <typeparam name="TBounds"></typeparam>
    /// <typeparam name="TFacet"></typeparam>
    public interface IDimension<TSurface, TPlane, TBounds, TFacet>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool Equals(TPlane x, TPlane y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int GetHashCode(TPlane obj);
    }


    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="TSurface"></typeparam>
    public abstract class Dimension2D<TSurface>
        : IDimension<TSurface, Hyperplane2D, Orthotope2D, Facet2D>
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

        public bool Equals(Hyperplane2D x, Hyperplane2D y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Hyperplane2D obj)
        {
            return obj.GetHashCode();
        }

        public Facet2D GetCofacet(Facet2D facet)
        {
            return new Facet2D(facet.Plane.Coplane, facet.End, facet.Start);
        }
    }


    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="TSurface"></typeparam>
    public abstract class Dimension3D<TSurface>
        : IDimension<TSurface, Hyperplane3D, Orthotope3D, Facet3D>
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

        public bool Equals(Hyperplane3D x, Hyperplane3D y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Hyperplane3D obj)
        {
            return obj.GetHashCode();
        }

        public Facet3D GetCofacet(Facet3D facet)
        {
            return new Facet3D(facet.Plane.Coplane, facet.Points.Reverse());
        }
    }
}

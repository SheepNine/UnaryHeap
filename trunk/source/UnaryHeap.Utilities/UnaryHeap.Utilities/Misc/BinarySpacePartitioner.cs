using System;
using System.Collections.Generic;
using System.Linq;

namespace Partitioner
{
    /// <summary>
    /// Interface defining a strategy for partitioning sets of surfaces.
    /// </summary>
    /// <typeparam name="TSurface">The type representing surfaces to be partitioned by
    /// the algorithm.</typeparam>
    /// <typeparam name="TPlane">The type representing the partitioning planes to be
    /// chosen by the algorithm.</typeparam>
    public interface IPartitioner<TSurface, TPlane>
    {
        /// <summary>
        /// Selects a partitioning plane to be used to partition a set of points.
        /// </summary>
        /// <param name="surfacesToPartition">The set of points to partition.</param>
        /// <returns>The selected plane.</returns>
        TPlane SelectPartitionPlane(IEnumerable<TSurface> surfacesToPartition);
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
        /// Counts the number of nodes in a BSP tree.
        /// </summary>
        int NodeCount { get; }

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
    public abstract class BinarySpacePartitioner<TSurface, TPlane>
        where TPlane : class
    {
        IPartitioner<TSurface, TPlane> partitioner;

        /// <summary>
        /// Initializes a new instance of the BinarySpacePartitioner class.
        /// </summary>
        /// <param name="partitioner">The partitioner used to select partitioning
        /// planes for a set of surfaces.</param>
        /// <exception cref="System.ArgumentNullException">partitioner is null.</exception>
        protected BinarySpacePartitioner(IPartitioner<TSurface, TPlane> partitioner)
        {
            if (null == partitioner)
                throw new ArgumentNullException("partitioner");

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
                throw new ArgumentNullException("inputSurfaces");

            var surfaces = inputSurfaces.ToList();

            if (0 == surfaces.Count)
                throw new ArgumentException("No surfaces to partition.");

            return ConstructBspNode(surfaces);
        }

        BspNode ConstructBspNode(List<TSurface> surfaces)
        {
            if (AllConvex(surfaces))
                return BspNode.LeafNode(surfaces);

            var partitionPlane = partitioner.SelectPartitionPlane(surfaces);

            if (null == partitionPlane)
                throw new InvalidOperationException("Failed to select partition plane.");

            List<TSurface> frontSurfaces, backSurfaces;
            Partition(surfaces, partitionPlane, out frontSurfaces, out backSurfaces);

            if (0 == frontSurfaces.Count || 0 == backSurfaces.Count)
                throw new InvalidOperationException(
                    "Partition plane selected does not partition surfaces.");

            var frontChild = ConstructBspNode(frontSurfaces);
            var backChild = ConstructBspNode(backSurfaces);
            return BspNode.BranchNode(partitionPlane, frontChild, backChild);
        }

        bool AllConvex(List<TSurface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == AreConvex(surfaces[i], surfaces[j]))
                        return false;

            return true;
        }

        void Partition(List<TSurface> surfaces, TPlane partitionPlane,
            out List<TSurface> frontSurfaces, out List<TSurface> backSurfaces)
        {
            frontSurfaces = new List<TSurface>();
            backSurfaces = new List<TSurface>();

            foreach (var surface in surfaces)
            {
                TSurface frontSurface, backSurface;
                Split(surface, partitionPlane, out frontSurface, out backSurface);

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
        protected abstract bool AreConvex(TSurface a, TSurface b);

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", "CA1021:AvoidOutParameters",
            Justification = "This is a reasonable use of out parameters.")]
        protected abstract void Split(TSurface surface, TPlane partitioningPlane,
            out TSurface frontSurface, out TSurface backSurface);


        class BspNode : IBspNode<TSurface, TPlane>
        {
            TPlane partitionPlane;
            BspNode frontChild;
            BspNode backChild;
            List<TSurface> surfaces;

            private BspNode() { }

            public static BspNode LeafNode(IEnumerable<TSurface> surfaces)
            {
                return new BspNode()
                {
                    partitionPlane = null,
                    frontChild = null,
                    backChild = null,
                    surfaces = surfaces.ToList()
                };
            }

            public static BspNode BranchNode(TPlane splitter,
                BspNode frontChild, BspNode backChild)
            {
                return new BspNode()
                {
                    partitionPlane = splitter,
                    frontChild = frontChild,
                    backChild = backChild,
                    surfaces = null
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
                    throw new ArgumentNullException("callback");

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
                    throw new ArgumentNullException("callback");

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
                    throw new ArgumentNullException("callback");

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
}

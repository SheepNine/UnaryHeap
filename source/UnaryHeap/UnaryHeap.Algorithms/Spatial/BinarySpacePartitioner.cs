using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet>
        where TPlane : IEquatable<TPlane>
    {
        /// <summary>
        /// Interface defining a strategy for partitioning sets of surfaces.
        /// </summary>
        public interface IPartitionStrategy
        {
            /// <summary>
            /// Selects a partitioning plane to be used to partition a set of surfaces.
            /// </summary>
            /// <param name="surfacesToPartition">The set of surfaces to partition.</param>
            /// <returns>The selected plane.</returns>
            TPlane SelectPartitionPlane(IEnumerable<TSurface> surfacesToPartition);
        }

        /// <summary>
        /// Provides a partitioning strategy which evaluates all available splitting planes.
        /// </summary>
        /// <param name="imbalanceWeight">How many points to deduct for imbalance between
        /// the number of surfaces on the front and back halves of a splitting plane.</param>
        /// <param name="splitWeight">How many points to deduct for each surface that is split
        /// by a splitting plane.</param>
        /// <returns>A partitioner with the given settings.</returns>
        public IPartitionStrategy ExhaustivePartitionStrategy(
            int imbalanceWeight, int splitWeight)
        {
            return new ExhaustivePartitioner(dimension, imbalanceWeight, splitWeight);
        }

        class ExhaustivePartitioner : IPartitionStrategy
        {
            readonly IDimension dimension;
            readonly int imbalanceWeight;
            readonly int splitWeight;

            public ExhaustivePartitioner(IDimension dimension,
                int imbalanceWeight, int splitWeight)
            {
                this.dimension = dimension;
                this.imbalanceWeight = imbalanceWeight;
                this.splitWeight = splitWeight;
            }

            public TPlane SelectPartitionPlane(IEnumerable<TSurface> surfacesToPartition)
            {
                return surfacesToPartition
                    .Select(dimension.GetPlane)
                    .Distinct()
                    .Select(h => ComputeSplitResult(h, surfacesToPartition))
                    .Where(splitResult => splitResult != null)
                    .OrderBy(splitResult =>
                        splitResult.ComputeScore(imbalanceWeight, splitWeight))
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
        /// Constructs a BSP tree for a set of input surfaces.
        /// </summary>
        /// <param name="strategy">The strategy to use to select a partitioning plane.</param>
        /// <param name="inputSurfaces">The surfaces to partition.</param>
        /// <returns>The root node of the resulting BSP tree.</returns>
        public BspNode ConstructBspTree(IPartitionStrategy strategy,
            IEnumerable<TSurface> inputSurfaces)
        {
            if (null == inputSurfaces)
                throw new ArgumentNullException(nameof(inputSurfaces));

            var surfaces = inputSurfaces.ToList();

            if (0 == surfaces.Count)
                throw new ArgumentException("No surfaces to partition.");

            return ConstructBspNode(strategy, surfaces, 0);
        }

        BspNode ConstructBspNode(IPartitionStrategy partitioner,
            List<TSurface> surfaces, int depth)
        {
            if (AllConvex(surfaces))
                return new BspNode(surfaces, depth);

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

            var frontChild = ConstructBspNode(partitioner, frontSurfaces, depth + 1);
            var backChild = ConstructBspNode(partitioner, backSurfaces, depth + 1);
            return new BspNode(partitionPlane, depth, frontChild, backChild);
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

        /// <summary>
        /// Class representing a node in a BSP tree.
        /// </summary>
        public class BspNode
        {
            /// <summary>
            /// Callback deletage for IBspNode's traversal methods.
            /// </summary>
            /// <param name="target">The BSP node currently being visited.</param>
            public delegate void IteratorCallback(BspNode target);

            readonly TPlane partitionPlane;
            readonly BspNode frontChild;
            readonly BspNode backChild;
            readonly List<TSurface> surfaces;
            readonly int depth;

            /// <summary>
            /// Initializes a new instance of the BspNode class for a leaf.
            /// </summary>
            /// <param name="depth">The depth of the leaf in the tree.</param>
            /// <param name="surfaces">The set of surfaces in the leaf.</param>
            public BspNode(IEnumerable<TSurface> surfaces, int depth)
            {
                this.partitionPlane = default;
                this.frontChild = null;
                this.backChild = null;
                this.surfaces = surfaces.ToList();
                this.depth = depth;
            }

            /// <summary>
            /// Initializes a new instance of the BspNode class for a branch.
            /// </summary>
            /// <param name="depth">The depth of the leaf in the tree.</param>
            /// <param name="splitter">The splitting plane of the branch.</param>
            /// <param name="frontChild">The BSP subtree on the front half of the plane.</param>
            /// <param name="backChild">The BSP subtree on the back half of the plane.</param>
            public BspNode(TPlane splitter, int depth, BspNode frontChild, BspNode backChild)
            {
                this.partitionPlane = splitter;
                this.frontChild = frontChild;
                this.backChild = backChild;
                this.surfaces = null;
                this.depth = depth;
            }

            /// <summary>
            /// Gets whether this node is a leaf node or a branch node.
            /// </summary>
            public bool IsLeaf
            {
                get { return surfaces != null; }
            }

            /// <summary>
            /// Gets the partitioning plane of a branch node. Returns null for leaf nodes.
            /// </summary>
            public TPlane PartitionPlane
            {
                get { return partitionPlane; }
            }

            /// <summary>
            /// Gets the front child of a branch node. Returns null for leaf nodes.
            /// </summary>
            public BspNode FrontChild
            {
                get { return frontChild; }
            }

            /// <summary>
            /// Gets the back child of a branch node. Returns null for leaf nodes.
            /// </summary>
            public BspNode BackChild
            {
                get { return backChild; }
            }

            /// <summary>
            /// Gets the surfaces in a leaf node. Returns null for branch nodes.
            /// </summary>
            public IEnumerable<TSurface> Surfaces
            {
                get { return surfaces; }
            }

            /// <summary>
            /// Gets the number of surfaces in a leaf node. Returns 0 fro branch nodes.
            /// </summary>
            public int SurfaceCount
            {
                get { return surfaces == null ? 0 : surfaces.Count; }
            }

            /// <summary>
            /// Gets the depth of this node in the BSP tree.
            /// </summary>
            public int Depth
            {
                get { return depth; }
            }

            /// <summary>
            /// Counts the number of nodes in a BSP tree.
            /// </summary>
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

            /// <summary>
            /// Iterates a BSP tree in pre-order.
            /// </summary>
            /// <param name="callback">The callback to run for each node traversed.</param>
            /// <exception cref="System.ArgumentNullException">callback is null.</exception>
            public void PreOrderTraverse(IteratorCallback callback)
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

            /// <summary>
            /// Iterates a BSP tree in in-order.
            /// </summary>
            /// <param name="callback">The callback to run for each node traversed.</param>
            /// <exception cref="System.ArgumentNullException">callback is null.</exception>
            public void InOrderTraverse(IteratorCallback callback)
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

            /// <summary>
            /// Iterates a BSP tree in post-order.
            /// </summary>
            /// <param name="callback">The callback to run for each node traversed.</param>
            /// <exception cref="System.ArgumentNullException">callback is null.</exception>
            public void PostOrderTraverse(IteratorCallback callback)
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
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
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

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            TPlane partitionPlane;
            if (null != hintSurface)
            {
                partitionPlane = dimension.GetPlane(hintSurface.Facet);
                surfaces.Remove(hintSurface);
            }
            else
            {
                partitionPlane = partitioner.SelectPartitionPlane(surfaces);
            }
            stopwatch.Stop();
            debug.SplittingPlaneChosen(stopwatch.ElapsedMilliseconds,
                surfaces, depth, partitionPlane);

            if (null == partitionPlane)
                throw new InvalidOperationException("Failed to select partition plane.");

            stopwatch.Restart();
            Partition(surfaces, partitionPlane, out List<TSurface> frontSurfaces,
                out List<TSurface> backSurfaces);
            stopwatch.Stop();

            debug.PartitionOccurred(stopwatch.ElapsedMilliseconds, surfaces, depth,
                partitionPlane, frontSurfaces, backSurfaces);

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
                    if (false == AreConvex(surfaces[i].Facet, surfaces[j].Facet))
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
                dimension.Split(surface, partitionPlane, out TSurface frontSurface,
                    out TSurface backSurface);

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
        protected bool AreConvex(TFacet a, TFacet b)
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

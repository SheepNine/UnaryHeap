using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
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

        class BspSurface
        {
            public TSurface Surface;
            public int FrontNodeIndex;
            public int BackNodeIndex;
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
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy));
            if (inputSurfaces == null)
                throw new ArgumentNullException(nameof(inputSurfaces));

            var surfaces = inputSurfaces.Select(s => new BspSurface()
            {
                Surface = s,
                FrontNodeIndex = 0,
                BackNodeIndex = dimension.IsTwoSided(s) ? 0 : -1
            }).ToList();
            var planes = new List<TPlane>();

            if (surfaces.Count == 0)
                throw new ArgumentException("No surfaces to partition.");

            PartitionSurfaces(strategy, surfaces, planes, 0, 0);

            return AssembleTree(surfaces, planes, 0, 0);
        }

        void PartitionSurfaces(IPartitionStrategy strategy, List<BspSurface> surfaces,
            List<TPlane> planes, int nodeIndex, int depth)
        {
            while (planes.Count <= nodeIndex)
                planes.Add(default);

            var leftChildIndex = LeftChildIndex(nodeIndex);
            var rightChildIndex = RightChildIndex(nodeIndex);

            var allNodeSurfaces = Enumerable.Range(0, surfaces.Count).Where(i =>
                surfaces[i].FrontNodeIndex == nodeIndex || surfaces[i].BackNodeIndex == nodeIndex
            ).ToList();
            var nodeFrontSurfaces = surfaces.Where(s => s.FrontNodeIndex == nodeIndex)
                .Select(s => s. Surface).ToList();

            if (AllConvex(nodeFrontSurfaces))
                return;

            var hintSurface = FindHintSurface(nodeFrontSurfaces, depth);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            TPlane partitionPlane;
            if (null != hintSurface)
            {
                partitionPlane = dimension.GetPlane(dimension.GetFacet(hintSurface));
                var u = surfaces.FindIndex(s => Equals(s.Surface, hintSurface));
                surfaces.RemoveAt(u);
                allNodeSurfaces = allNodeSurfaces.Select(s => s >= u ? s - 1 : s).ToList();
            }
            else
            {
                partitionPlane = strategy.SelectPartitionPlane(nodeFrontSurfaces);
            }
            stopwatch.Stop();
            debug.SplittingPlaneChosen(stopwatch.ElapsedMilliseconds,
                nodeFrontSurfaces, depth, partitionPlane);

            if (null == partitionPlane)
                throw new InvalidOperationException("Failed to select partition plane.");

            planes[nodeIndex] = partitionPlane;

            var frontCount = 0;
            var backCount = 0;
            stopwatch.Restart();
            foreach (var i in allNodeSurfaces)
            {
                var surface = surfaces[i];
                var facet = dimension.GetFacet(surface.Surface);
                var plane = dimension.GetPlane(facet);

                if (partitionPlane.Equals(plane))
                {
                    frontCount += 1;
                    surfaces[i].FrontNodeIndex = leftChildIndex;
                    if (surfaces[i].BackNodeIndex != -1)
                        surfaces[i].BackNodeIndex = rightChildIndex;
                }
                else if (partitionPlane.Equals(dimension.GetCoplane(plane)))
                {
                    backCount += 1;
                    surfaces[i].FrontNodeIndex = rightChildIndex;
                    if (surfaces[i].BackNodeIndex != -1)
                        surfaces[i].BackNodeIndex = leftChildIndex;
                }
                else
                {
                    dimension.Split(surface.Surface, partitionPlane, out TSurface frontSurface,
                        out TSurface backSurface);

                    if (backSurface == null)
                    {
                        frontCount += 1;
                        if (surfaces[i].FrontNodeIndex == nodeIndex)
                            surfaces[i].FrontNodeIndex = leftChildIndex;
                        if (surfaces[i].BackNodeIndex == nodeIndex)
                            surfaces[i].BackNodeIndex = leftChildIndex;
                    }
                    else if (frontSurface == null)
                    {
                        backCount += 1;
                        if (surfaces[i].FrontNodeIndex == nodeIndex)
                            surfaces[i].FrontNodeIndex = rightChildIndex;
                        if (surfaces[i].BackNodeIndex == nodeIndex)
                            surfaces[i].BackNodeIndex = rightChildIndex;
                    }
                    else
                    {
                        frontCount += 1;
                        backCount += 1;
                        var j = surfaces.Count;
                        surfaces[i].Surface = frontSurface;
                        surfaces.Add(new BspSurface()
                        {
                            Surface = backSurface,
                            FrontNodeIndex = surfaces[i].FrontNodeIndex,
                            BackNodeIndex = surfaces[i].BackNodeIndex,
                        });

                        if (surfaces[i].FrontNodeIndex == nodeIndex)
                            surfaces[i].FrontNodeIndex = leftChildIndex;
                        if (surfaces[i].BackNodeIndex == nodeIndex)
                            surfaces[i].BackNodeIndex = leftChildIndex;

                        if (surfaces[j].FrontNodeIndex == nodeIndex)
                            surfaces[j].FrontNodeIndex = rightChildIndex;
                        if (surfaces[j].BackNodeIndex == nodeIndex)
                            surfaces[j].BackNodeIndex = rightChildIndex;
                    }
                }
            }
            stopwatch.Stop();

            debug.PartitionOccurred(stopwatch.ElapsedMilliseconds,
                surfaces.Select(s => s.Surface).ToList(),
                depth,
                partitionPlane,
                surfaces.Where(
                    s => s.FrontNodeIndex == leftChildIndex || s.BackNodeIndex == leftChildIndex
                ).Select(s => s.Surface).ToList(),
                surfaces.Where(
                    s => s.FrontNodeIndex == rightChildIndex || s.BackNodeIndex == rightChildIndex
                ).Select(s => s.Surface).ToList());

            if (0 == frontCount || 0 == backCount)
                throw new InvalidOperationException(
                    "Partition plane selected does not partition surfaces.");

            PartitionSurfaces(strategy, surfaces, planes, leftChildIndex, depth + 1);
            PartitionSurfaces(strategy, surfaces, planes, rightChildIndex, depth + 1);
        }

        static BspNode AssembleTree(List<BspSurface> surfaces,
            List<TPlane> planes, int nodeIndex, int depth)
        {
            if (planes[nodeIndex] == null)
            {
                return new BspNode(
                    surfaces.Where(s => s.FrontNodeIndex == nodeIndex).Select(s => s.Surface),
                    depth);
            }
            else
            {
                return new BspNode(planes[nodeIndex], depth,
                    AssembleTree(surfaces, planes, LeftChildIndex(nodeIndex), depth + 1),
                    AssembleTree(surfaces, planes, RightChildIndex(nodeIndex), depth + 1));
            }
        }

        static int LeftChildIndex(int nodeIndex)
        {
            return (nodeIndex << 1) + 1;
        }

        static int RightChildIndex(int nodeIndex)
        {
            return (nodeIndex + 1) << 1;
        }

        bool AllConvex(List<TSurface> surfaces)
        {
            foreach (var i in Enumerable.Range(0, surfaces.Count))
                foreach (var j in Enumerable.Range(i + 1, surfaces.Count - i - 1))
                    if (false == AreConvex(dimension.GetFacet(surfaces[i]),
                            dimension.GetFacet(surfaces[j])))
                        return false;

            return true;
        }

        TSurface FindHintSurface(List<TSurface> surfaces, int depth)
        {
            return surfaces.FirstOrDefault(surface => dimension.IsHintSurface(surface, depth));
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

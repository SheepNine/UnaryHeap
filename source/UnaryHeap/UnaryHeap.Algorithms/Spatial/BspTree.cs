using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
    {
        /// <summary>
        /// Class representing a BSP tree.
        /// </summary>
        public interface IBspTree
        {
            /// <summary>
            /// Counts the number of nodes in a BSP tree.
            /// </summary>
            int NodeCount { get; }

            /// <summary>
            /// Checks if a given node is a leaf or a branch.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>true, if the node is a leaf; false otherwise.</returns>
            bool IsLeaf(int index);

            /// <summary>
            /// Gets the partitioning plane of a branch node.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>The partitioning plane of the node.</returns>
            TPlane PartitionPlane(int index);

            /// <summary>
            /// Gets the number of surfaces of a leaf node.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>The number of surfaces in the node.</returns>
            int SurfaceCount(int index);

            /// <summary>
            /// Gets the surfaces of a leaf node.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>The surfaces of the node.</returns>
            IEnumerable<TSurface> Surfaces(int index);

            /// <summary>
            /// Iterates a BSP tree in pre-order.
            /// </summary>
            /// <param name="callback">The callback to run for each node traversed.</param>
            /// <exception cref="System.ArgumentNullException">callback is null.</exception>
            void PreOrderTraverse(Action<int> callback);

            /// <summary>
            /// Iterates a BSP tree in in-order.
            /// </summary>
            /// <param name="callback">The callback to run for each node traversed.</param>
            /// <exception cref="System.ArgumentNullException">callback is null.</exception>
            void InOrderTraverse(Action<int> callback);

            /// <summary>
            /// Iterates a BSP tree in post-order.
            /// </summary>
            /// <param name="callback">The callback to run for each node traversed.</param>
            /// <exception cref="System.ArgumentNullException">callback is null.</exception>
            void PostOrderTraverse(Action<int> callback);
        }

        class BspTree : IBspTree
        {
            public int NodeCount { get; private set; }
            readonly List<TPlane> branchPlanes = new();
            readonly List<List<TSurface>> leafSurfaces = new();

            public void AddBranch(int index, TPlane plane)
            {
                // TODO: AddRange(Enumerable.Repeat(...))
                while (branchPlanes.Count < index + 1)
                    branchPlanes.Add(default);
                branchPlanes[index] = plane;
                NodeCount += 1;
            }

            public void AddLeaf(int index, IEnumerable<TSurface> surfaces)
            {
                // TODO: AddRange(Enumerable.Repeat(...))
                while (leafSurfaces.Count < index + 1)
                    leafSurfaces.Add(null);
                leafSurfaces[index] = surfaces.ToList();
                NodeCount += 1;
            }

            public bool IsLeaf(int index)
            {
                return leafSurfaces[index] != null;
            }

            public TPlane PartitionPlane(int index)
            {
                return branchPlanes[index];
            }

            public IEnumerable<TSurface> Surfaces(int index)
            {
                return leafSurfaces[index];
            }

            public int SurfaceCount(int index)
            {
                return leafSurfaces[index].Count;
            }

            // public int Depth
            public void PreOrderTraverse(Action<int> callback)
            {
                PreOrderTraverse(callback, 0);
            }

            void PreOrderTraverse(Action<int> callback, int index)
            {
                if (null == callback)
                    throw new ArgumentNullException(nameof(callback));

                if (IsLeaf(index))
                {
                    callback(index);
                }
                else
                {
                    callback(index);
                    PreOrderTraverse(callback, index.FrontChildIndex());
                    PreOrderTraverse(callback, index.BackChildIndex());
                }
            }

            public void InOrderTraverse(Action<int> callback)
            {
                InOrderTraverse(callback, 0);
            }

            void InOrderTraverse(Action<int> callback, int index)
            {
                if (null == callback)
                    throw new ArgumentNullException(nameof(callback));

                if (IsLeaf(index))
                {
                    callback(index);
                }
                else
                {
                    InOrderTraverse(callback, index.FrontChildIndex());
                    callback(index);
                    InOrderTraverse(callback, index.BackChildIndex());
                }
            }

            public void PostOrderTraverse(Action<int> callback)
            {
                PostOrderTraverse(callback, 0);
            }

            void PostOrderTraverse(Action<int> callback, int index)
            {
                if (null == callback)
                    throw new ArgumentNullException(nameof(callback));

                if (IsLeaf(index))
                {
                    callback(index);
                }
                else
                {
                    PostOrderTraverse(callback, index.FrontChildIndex());
                    PostOrderTraverse(callback, index.BackChildIndex());
                    callback(index);
                }
            }
        }
    }

    /// <summary>
    /// Utility methods for computing indices in a heap data structure.
    /// </summary>
    public static class HeapIndex
    {
        /// <summary>
        /// Gets the index of the front child of an index.
        /// </summary>
        /// <param name="index">The parent index.</param>
        /// <returns>The front child index.</returns>
        public static int FrontChildIndex(this int index)
        {
            return (index << 1) + 1;
        }

        /// <summary>
        /// Gets the index of the back child of an index.
        /// </summary>
        /// <param name="index">The parent index.</param>
        /// <returns>The back child index.</returns>
        public static int BackChildIndex(this int index)
        {
            return (index + 1) << 1;
        }

        /// <summary>
        /// Gets the index of the parent of an index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <returns>The parent index, or -1 for child index 0.</returns>
        public static int ParentIndex(this int index)
        {
            return (index - 1) >> 1;
        }

        /// <summary>
        /// Gets the 0-based depth of an index.
        /// </summary>
        /// <param name="index">The index for which to compute.</param>
        /// <returns>The depth of the index in the tree, starting with zero.</returns>
        public static int Depth(this int index)
        {
            var result = 0;

            while (index >= (2 << result) - 1)
                result += 1;

            return result;
        }
    }
}

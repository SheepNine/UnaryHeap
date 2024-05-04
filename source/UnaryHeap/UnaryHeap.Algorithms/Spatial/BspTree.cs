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
            /// Compute the back index of a node.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>The node index of that node's back child.</returns>
            /// 
            int BackChildIndexOf(int index);
            /// <summary>
            /// Compute the front child index of a node.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>The node index of that node's front child.</returns>
            /// 
            int FrontChildIndexOf(int index);
            /// <summary>
            /// Checks if a given node is a leaf or a branch.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>true, if the node is a leaf; false otherwise.</returns>
            /// 
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

            public int FrontChildIndexOf(int index)
            {
                return (index << 1) + 1;
            }

            public int BackChildIndexOf(int index)
            {
                return (index + 1) << 1;
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
                    PreOrderTraverse(callback, FrontChildIndexOf(index));
                    PreOrderTraverse(callback, BackChildIndexOf(index));
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
                    InOrderTraverse(callback, FrontChildIndexOf(index));
                    callback(index);
                    InOrderTraverse(callback, BackChildIndexOf(index));
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
                    PostOrderTraverse(callback, FrontChildIndexOf(index));
                    PostOrderTraverse(callback, BackChildIndexOf(index));
                    callback(index);
                }
            }
        }
    }
}

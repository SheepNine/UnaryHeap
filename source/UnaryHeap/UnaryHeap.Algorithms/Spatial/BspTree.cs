using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            /// Computes the bounding box containing all of the tree's surfaces.
            /// </summary>
            /// <returns>The bounding box containing all of the tree's surfaces.</returns>
            TBounds CalculateBoundingBox();

            /// <summary>
            /// Find the index of the leaf node containing a given point.
            /// </summary>
            /// <param name="point">The point to check against.</param>
            /// <returns>The leaf containing a point, so long as it is in the front halfspace
            /// of that leaf's surfaces; NullNodeIndex otherwise.</returns>
            public int FindLeafContaining(TPoint point);

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

        /// <summary>
        /// Class representing a surface in a BSP tree.
        /// </summary>
        public interface IBspSurface
        {
            /// <summary>
            /// Index of the front leaf of the tree for this surface.
            /// </summary>
            public int FrontLeaf { get; }

            /// <summary>
            /// Index of the back leaf of the tree of this surface.
            /// </summary>
            public int BackLeaf { get; }
        
            /// <summary>
            /// The underlying surface of this BspSurface.
            /// </summary>
            public TSurface Surface { get; }
        }

        class BspSurface : IBspSurface
        {
            public int FrontLeaf { get; set; }
            public int BackLeaf { get; set; }
            public TSurface Surface { get; set; }
        }

        /// <summary>
        /// The node index for 'not a node'.
        /// </summary>
        public const int NullNodeIndex = -1;

        class BspTree : IBspTree
        {
            public int NodeCount { get; private set; }
            readonly List<TPlane> branchPlanes = new();
            readonly List<List<TSurface>> leafSurfaces = new();
            readonly BitArray validNodes = new(0);
            readonly IDimension dimension;

            public BspTree(IDimension dimension)
            {
                this.dimension = dimension;
            }

            public BspTree(BspTree clone)
                :this(clone.dimension)
            {
                NodeCount = clone.NodeCount;
                branchPlanes = new(clone.branchPlanes);
                leafSurfaces = new(clone.leafSurfaces.Select(
                    ss => ss == null ? null : new List<TSurface>(ss)));
                validNodes = new(clone.validNodes);
            }

            void SizeToFit(int index)
            {
                // TODO: AddRange(Enumerable.Repeat(...))
                while (branchPlanes.Count < index + 1)
                    branchPlanes.Add(default);
                while (leafSurfaces.Count < index + 1)
                    leafSurfaces.Add(null);
                if (validNodes.Length < branchPlanes.Count)
                    validNodes.Length = branchPlanes.Count;
            }

            public void AddBranch(int index, TPlane plane)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                SizeToFit(index);

                if (validNodes[index])
                    throw new InvalidOperationException("Node already exists");

                branchPlanes[index] = plane;
                validNodes[index] = true;
                NodeCount += 1;
            }

            public void AddLeaf(int index, IEnumerable<TSurface> surfaces)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                SizeToFit(index);

                if (validNodes[index])
                    throw new InvalidOperationException("Node already exists");

                leafSurfaces[index] = surfaces.ToList();
                validNodes[index] = true;
                NodeCount += 1;
            }

            public bool IsLeaf(int index)
            {
                CheckIndex(index);
                return leafSurfaces[index] != null;
            }

            public TPlane PartitionPlane(int index)
            {
                CheckIndex(index);
                return branchPlanes[index];
            }

            public IEnumerable<TSurface> Surfaces(int index)
            {
                CheckIndex(index);
                return leafSurfaces[index];
            }

            public int SurfaceCount(int index)
            {
                CheckIndex(index);
                return leafSurfaces[index].Count;
            }

            private void CheckIndex(int index)
            {
                if (!IsValid(index))
                    throw new ArgumentOutOfRangeException(nameof(index));
            }

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

            public TBounds CalculateBoundingBox()
            {
                return dimension.CalculateBounds(
                    leafSurfaces.Where(s => s != null).SelectMany(s => s).Select(s => s.Facet)
                );
            }

            public void CheckIntegrity()
            {
                if (leafSurfaces.Count != branchPlanes.Count
                        || branchPlanes.Count != validNodes.Count)
                    throw new InvalidDataException("Array size mismatch");

                var validNodeCount = 0;
                foreach (var index in Enumerable.Range(0, validNodes.Count))
                {
                    if (validNodes[index])
                    {
                        validNodeCount += 1;
                        if (!(leafSurfaces[index] == null ^ branchPlanes[index] == null))
                        {
                            throw new InvalidDataException("Leaf is mutant!");
                        }
                        var iter = index.ParentIndex();
                        while (iter != -1)
                        {
                            if (!IsValid(iter))
                                throw new InvalidDataException("Ancestor of node is invalid");
                            if (IsLeaf(iter))
                                throw new InvalidDataException("Ancestor is a leaf");
                            iter = iter.ParentIndex();
                        }

                        if (IsLeaf(index))
                        {
                            if (IsValid(index.FrontChildIndex()))
                                throw new InvalidDataException("Leaf with child");
                            if (IsValid(index.BackChildIndex()))
                                throw new InvalidDataException("Leaf with child");
                        }
                        else
                        {
                            if (!IsValid(index.FrontChildIndex()))
                                throw new InvalidDataException("Branch missing child");
                            if (!IsValid(index.BackChildIndex()))
                                throw new InvalidDataException("Branch missing child");
                        }
                    }
                    else
                    {
                        if (leafSurfaces[index] != null || branchPlanes[index] != null)
                        {
                            throw new InvalidDataException("Orphan data in invalid node");
                        }
                    }
                }

                if (validNodeCount != NodeCount)
                    throw new InvalidDataException("Node count tracking desync");
            }

            bool IsValid(int index)
            {
                return index >= 0 && index < validNodes.Count && validNodes[index];
            }

            public void CullOutside(HashSet<int> interiorLeaves)
            {
                CullOutside(0, interiorLeaves);
                CheckIntegrity();
            }

            void CullOutside(int index, HashSet<int> interiorLeaves)
            {
                if (!IsValid(index))
                    return;

                if (IsLeaf(index))
                {
                    if (!interiorLeaves.Contains(index))
                    {
                        validNodes[index] = false;
                        leafSurfaces[index] = null;
                        NodeCount -= 1;
                    }
                }
                else
                {
                    var frontIndex = index.FrontChildIndex();
                    var backIndex = index.BackChildIndex();
                    CullOutside(frontIndex, interiorLeaves);
                    CullOutside(backIndex, interiorLeaves);

                    if (!IsValid(frontIndex) && !IsValid(backIndex))
                    {
                        validNodes[index] = false;
                        branchPlanes[index] = default;
                        NodeCount -= 1;
                    }
                    else if (!IsValid(frontIndex))
                    {
                        PullUp(backIndex, index, false);
                        NodeCount -= 1;
                    }
                    else if (!IsValid(backIndex))
                    {
                        PullUp(frontIndex, index, true);
                        NodeCount -= 1;
                    }
                }
            }

            void PullUp(int fromIndex, int toIndex, bool backFirst)
            {
                if (IsLeaf(fromIndex))
                {
                    leafSurfaces[toIndex] = leafSurfaces[fromIndex];
                    leafSurfaces[fromIndex] = null;
                    branchPlanes[toIndex] = default;
                    validNodes[fromIndex] = false;
                    validNodes[toIndex] = true;
                }
                else
                {
                    branchPlanes[toIndex] = branchPlanes[fromIndex];
                    branchPlanes[fromIndex] = default;
                    leafSurfaces[toIndex] = null;
                    validNodes[fromIndex] = false;
                    validNodes[toIndex] = true;
                    if (backFirst)
                        PullUp(fromIndex.BackChildIndex(), toIndex.BackChildIndex(), backFirst);
                    PullUp(fromIndex.FrontChildIndex(), toIndex.FrontChildIndex(), backFirst);
                    if (!backFirst)
                        PullUp(fromIndex.BackChildIndex(), toIndex.BackChildIndex(), backFirst);
                }
            }

            public int FindLeafContaining(TPoint point)
            {
                return FindLeafContaining(0, point);
            }

            int FindLeafContaining(int index, TPoint point)
            {
                if (IsLeaf(index))
                {
                    var clipPlanes = leafSurfaces[index]
                        .Where(s => !s.IsTwoSided)
                        .Select(s => dimension.GetPlane(s.Facet))
                        .Distinct();
                    if (clipPlanes.Any(plane => dimension.ClassifyPoint(point, plane) < 0))
                        return NullNodeIndex;

                    return index;
                }
                else
                {
                    var classification = dimension.ClassifyPoint(point, branchPlanes[index]);
                    if (classification > 0)
                        return FindLeafContaining(index.FrontChildIndex(), point);
                    else if (classification < 0)
                        return FindLeafContaining(index.BackChildIndex(), point);
                    else
                    {
                        var result = FindLeafContaining(index.FrontChildIndex(), point);
                        if (result == NullNodeIndex)
                            result = FindLeafContaining(index.BackChildIndex(), point);
                        return result;
                    }
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

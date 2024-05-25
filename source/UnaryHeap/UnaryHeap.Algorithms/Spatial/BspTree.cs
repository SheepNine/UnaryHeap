using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

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
            bool IsLeaf(BigInteger index);

            /// <summary>
            /// Gets the partitioning plane of a branch node.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>The partitioning plane of the node.</returns>
            TPlane PartitionPlane(BigInteger index);

            /// <summary>
            /// Gets the number of surfaces of a leaf node.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>The number of surfaces in the node.</returns>
            int SurfaceCount(BigInteger index);

            /// <summary>
            /// Gets the surfaces of a leaf node.
            /// </summary>
            /// <param name="index">The node index.</param>
            /// <returns>The surfaces of the node.</returns>
            IEnumerable<IBspSurface> Surfaces(BigInteger index);

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
            public BigInteger FindLeafContaining(TPoint point);

            /// <summary>
            /// Iterates a BSP tree in pre-order.
            /// </summary>
            /// <param name="callback">The callback to run for each node traversed.</param>
            /// <exception cref="System.ArgumentNullException">callback is null.</exception>
            void PreOrderTraverse(Action<BigInteger> callback);

            /// <summary>
            /// Iterates a BSP tree in in-order.
            /// </summary>
            /// <param name="callback">The callback to run for each node traversed.</param>
            /// <exception cref="System.ArgumentNullException">callback is null.</exception>
            void InOrderTraverse(Action<BigInteger> callback);

            /// <summary>
            /// Iterates a BSP tree in post-order.
            /// </summary>
            /// <param name="callback">The callback to run for each node traversed.</param>
            /// <exception cref="System.ArgumentNullException">callback is null.</exception>
            void PostOrderTraverse(Action<BigInteger> callback);
        }

        /// <summary>
        /// Class representing a surface in a BSP tree.
        /// </summary>
        public interface IBspSurface
        {
            /// <summary>
            /// Index of the front leaf of the tree for this surface.
            /// </summary>
            public BigInteger FrontLeaf { get; }

            /// <summary>
            /// Index of the back leaf of the tree of this surface.
            /// </summary>
            public BigInteger BackLeaf { get; }
        
            /// <summary>
            /// The underlying surface of this BspSurface.
            /// </summary>
            public TSurface Surface { get; }
        }

        [DebuggerDisplay("{FrontLeaf}:{BackLeaf} {Surface}")]
        class BspSurface : IBspSurface
        {
            public BigInteger FrontLeaf { get; set; }
            public BigInteger BackLeaf { get; set; }
            public TSurface Surface { get; set; }
        }

        /// <summary>
        /// The node index for 'not a node'.
        /// </summary>
        public const int NullNodeIndex = -1;

        class BspTree : IBspTree
        {
            public int NodeCount { get { return validNodes.Count; } }
            readonly Dictionary<BigInteger, TPlane> branchPlanes = new();
            readonly Dictionary<BigInteger, List<BspSurface>> leafSurfaces = new();
            readonly SortedSet<BigInteger> validNodes = new();
            readonly IDimension dimension;

            public BspTree(IDimension dimension)
            {
                this.dimension = dimension;
            }

            public BspTree(BspTree clone)
                :this(clone.dimension)
            {
                branchPlanes = clone.branchPlanes.ToDictionary(kv => kv.Key, kv => kv.Value);
                leafSurfaces = clone.leafSurfaces.ToDictionary(kv => kv.Key,
                    kv => kv.Value.ToList());
                validNodes = new(clone.validNodes);
            }

            public void AddBranch(BigInteger index, TPlane plane)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (validNodes.Contains(index))
                    throw new InvalidOperationException("Node already exists");


                branchPlanes[index] = plane;
                validNodes.Add(index);
            }

            public void AddLeaf(BigInteger index, IEnumerable<BspSurface> surfaces)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (validNodes.Contains(index))
                    throw new InvalidOperationException("Node already exists");

                leafSurfaces[index] = surfaces.ToList();
                validNodes.Add(index);
            }

            public bool IsLeaf(BigInteger index)
            {
                CheckIndex(index);
                return leafSurfaces.ContainsKey(index);
            }

            public TPlane PartitionPlane(BigInteger index)
            {
                CheckIndex(index);
                return branchPlanes[index];
            }

            public IEnumerable<IBspSurface> Surfaces(BigInteger index)
            {
                CheckIndex(index);
                return leafSurfaces[index];
            }

            public int SurfaceCount(BigInteger index)
            {
                CheckIndex(index);
                return leafSurfaces[index].Count;
            }

            private void CheckIndex(BigInteger index)
            {
                if (!IsValid(index))
                    throw new ArgumentOutOfRangeException(nameof(index));
            }

            public void PreOrderTraverse(Action<BigInteger> callback)
            {
                PreOrderTraverse(callback, 0);
            }

            void PreOrderTraverse(Action<BigInteger> callback, BigInteger index)
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

            public void InOrderTraverse(Action<BigInteger> callback)
            {
                InOrderTraverse(callback, 0);
            }

            void InOrderTraverse(Action<BigInteger> callback, BigInteger index)
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

            public void PostOrderTraverse(Action<BigInteger> callback)
            {
                PostOrderTraverse(callback, 0);
            }

            void PostOrderTraverse(Action<BigInteger> callback, BigInteger index)
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
                return dimension.CalculateBounds(leafSurfaces.SelectMany(kv => kv.Value)
                    .Select(s => s.Surface.Facet)
                );
            }

            bool IsValid(BigInteger index)
            {
                return index >= 0 && validNodes.Contains(index);
            }

            public void CullOutside(HashSet<BigInteger> interiorLeaves)
            {
                CullOutside(0, interiorLeaves);
            }

            void CullOutside(BigInteger index, HashSet<BigInteger> interiorLeaves)
            {
                if (!IsValid(index))
                    return;

                if (IsLeaf(index))
                {
                    if (!interiorLeaves.Contains(index))
                    {
                        validNodes.Remove(index);
                        leafSurfaces.Remove(index);
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
                        validNodes.Remove(index);
                        branchPlanes.Remove(index);
                    }
                    else if (!IsValid(frontIndex))
                    {
                        PullUp(backIndex, index, false);
                    }
                    else if (!IsValid(backIndex))
                    {
                        PullUp(frontIndex, index, true);
                    }
                }
            }

            void PullUp(BigInteger fromIndex, BigInteger toIndex, bool backFirst)
            {
                if (IsLeaf(fromIndex))
                {
                    leafSurfaces[toIndex] = leafSurfaces[fromIndex];
                    leafSurfaces.Remove(fromIndex);
                    branchPlanes.Remove(toIndex);
                    validNodes.Remove(fromIndex);
                    validNodes.Add(toIndex);
                }
                else
                {
                    branchPlanes[toIndex] = branchPlanes[fromIndex];
                    branchPlanes.Remove(fromIndex);
                    leafSurfaces.Remove(toIndex);
                    validNodes.Remove(fromIndex);
                    validNodes.Add(toIndex);
                    if (backFirst)
                        PullUp(fromIndex.BackChildIndex(), toIndex.BackChildIndex(), backFirst);
                    PullUp(fromIndex.FrontChildIndex(), toIndex.FrontChildIndex(), backFirst);
                    if (!backFirst)
                        PullUp(fromIndex.BackChildIndex(), toIndex.BackChildIndex(), backFirst);
                }
            }

            public BigInteger FindLeafContaining(TPoint point)
            {
                return FindLeafContaining(0, point);
            }

            BigInteger FindLeafContaining(BigInteger index, TPoint point)
            {
                if (IsLeaf(index))
                {
                    var clipPlanes = leafSurfaces[index]
                        .Where(s => !s.Surface.IsTwoSided)
                        .Select(s => dimension.GetPlane(s.Surface.Facet))
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

            public void Populate(Dictionary<BigInteger, TPlane> branchPlanes,
                List<BspSurface> surfaces)
            {
                Populate(branchPlanes, surfaces, 0);
            }

            private void Populate(Dictionary<BigInteger, TPlane> branchPlanes,
                List<BspSurface> surfaces, BigInteger index)
            {
                if (branchPlanes.ContainsKey(index))
                {
                    AddBranch(index, branchPlanes[index]);
                    Populate(branchPlanes, surfaces, index.FrontChildIndex());
                    Populate(branchPlanes, surfaces, index.BackChildIndex());
                }
                else
                {
                    AddLeaf(index, surfaces.Where(s => s.FrontLeaf == index));
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
        public static BigInteger FrontChildIndex(this BigInteger index)
        {
            return (index << 1) + 1;
        }

        /// <summary>
        /// Gets the index of the back child of an index.
        /// </summary>
        /// <param name="index">The parent index.</param>
        /// <returns>The back child index.</returns>
        public static BigInteger BackChildIndex(this BigInteger index)
        {
            return (index + 1) << 1;
        }

        /// <summary>
        /// Gets the index of the parent of an index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <returns>The parent index, or -1 for child index 0.</returns>
        public static BigInteger ParentIndex(this BigInteger index)
        {
            return (index - 1) >> 1;
        }

        /// <summary>
        /// Gets the 0-based depth of an index.
        /// </summary>
        /// <param name="index">The index for which to compute.</param>
        /// <returns>The depth of the index in the tree, starting with zero.</returns>
        public static int Depth(this BigInteger index)
        {
            var result = 0;

            while (index >= (new BigInteger(2) << result) - 1)
                result += 1;

            return result;
        }
    }
}

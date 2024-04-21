using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
    {
        /// <summary>
        /// Cull leaves of a BSP tree which are not interior spaces.
        /// </summary>
        /// <param name="root">The BSP tree to cull.</param>
        /// <param name="portals">Portals between leaf nodes in the tree.</param>
        /// <param name="interiorPoints">Locations in the tree which are considered interior.
        /// </param>
        /// <param name="solidPredicate">Funtion to check if a given surface is 'solid'
        /// (i.e. the space behind it is not space that a valid interior point can occupy).
        /// </param>
        /// <returns>A new BSP with only leaves which are interior, or are connected
        /// to interior spaces.</returns>
        public BspNode CullOutside(BspNode root,
            IEnumerable<Portal> portals,
            IEnumerable<TPoint> interiorPoints,
            Func<TSurface, bool> solidPredicate)
        {
            return CullOutside(root,
                FindInteriorLeaves(root, portals, interiorPoints, solidPredicate));
        }

        HashSet<BspNode> FindInteriorLeaves(BspNode root,
            IEnumerable<Portal> portals, IEnumerable<TPoint> interiorPoints,
            Func<TSurface, bool> solidPredicate)
        {
            var leafCount = (root.NodeCount + 1) >> 1;

            var result = new HashSet<BspNode>();
            foreach (var interiorPoint in interiorPoints)
            {
                MarkInteriorSpace(result, portals,
                    FindLeafContaining(root, interiorPoint, solidPredicate));
                debug.InsideFilled(interiorPoint, result, leafCount);
            }
            return result;
        }

        static void MarkInteriorSpace(HashSet<BspNode> interiorNodes,
            IEnumerable<Portal> portals, BspNode leaf)
        {
            if (interiorNodes.Contains(leaf))
                return;

            interiorNodes.Add(leaf);

            foreach (var portal in portals)
            {
                if (portal.Front == leaf)
                    MarkInteriorSpace(interiorNodes, portals, portal.Back);
                if (portal.Back == leaf)
                    MarkInteriorSpace(interiorNodes, portals, portal.Front);
            }
        }

        BspNode FindLeafContaining(BspNode node, TPoint point,
            Func<TSurface, bool> solidPredicate)
        {
            if (node.IsLeaf)
            {
                var clipPlanes = node.Surfaces
                    .Where(solidPredicate)
                    .Select(s => dimension.GetPlane(dimension.GetFacet(s)))
                    .Distinct();
                if (clipPlanes.Any(plane => dimension.ClassifyPoint(point, plane) < 0))
                    return null;

                return node;
            }
            else
            {
                var classification = dimension.ClassifyPoint(point, node.PartitionPlane);
                if (classification > 0)
                    return FindLeafContaining(node.FrontChild, point, solidPredicate);
                else if (classification < 0)
                    return FindLeafContaining(node.BackChild, point, solidPredicate);
                else
                    return FindLeafContaining(node.FrontChild, point, solidPredicate)
                        ?? FindLeafContaining(node.BackChild, point, solidPredicate);
            }
        }

        static BspNode CullOutside(BspNode node, HashSet<BspNode> interiorLeaves)
        {
            if (node.IsLeaf)
            {
                return interiorLeaves.Contains(node) ? node : null;
            }
            else
            {
                var culledFront = CullOutside(node.FrontChild, interiorLeaves);
                var culledBack = CullOutside(node.BackChild, interiorLeaves);

                if (culledFront == null && culledBack == null)
                {
                    return null;
                }
                else if (culledFront == null)
                {
                    if (culledBack.IsLeaf)
                    {
                        return new BspNode(culledBack.Surfaces, node.Depth);
                    }
                    else
                    {
                        return new BspNode(culledBack.PartitionPlane, node.Depth,
                            culledBack.FrontChild, culledBack.BackChild);
                    }
                }
                else if (culledBack == null)
                {
                    if (culledFront.IsLeaf)
                    {
                        return new BspNode(culledFront.Surfaces, node.Depth);
                    }
                    else
                    {
                        return new BspNode(culledFront.PartitionPlane, node.Depth,
                            culledFront.FrontChild, culledFront.BackChild);
                    }
                }
                else
                {
                    return new BspNode(node.PartitionPlane, node.Depth, culledFront, culledBack);
                }
            }
        }

        /// <summary>
        /// Determine the size of connected sets of leaves.
        /// </summary>
        /// <param name="root">The BSP tree to analyze.</param>
        /// <param name="portals">The portals connecting leaves.</param>
        /// <returns>A list of the sizes of subsets.</returns>
        public List<int> LeafSubsets(BspNode root, IEnumerable<Portal> portals)
        {
            var result = new List<int>();
            var foundNodes = new HashSet<BspNode>();

            root.InOrderTraverse(node =>
            {
                if (!node.IsLeaf || foundNodes.Contains(node)) return;
                var startCount = foundNodes.Count;
                MarkInteriorSpace(foundNodes, portals, node);
                var endCount = foundNodes.Count;
                result.Add(endCount - startCount);
            });

            result.Sort();
            return result;
        }
    }
}

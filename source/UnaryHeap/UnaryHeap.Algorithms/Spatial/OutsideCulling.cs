﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
        where TSurface : Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>.SurfaceBase
    {
        /// <summary>
        /// Cull leaves of a BSP tree which are not interior spaces.
        /// </summary>
        /// <param name="root">The BSP tree to cull.</param>
        /// <param name="portals">Portals between leaf nodes in the tree.</param>
        /// <param name="interiorPoints">Locations in the tree which are considered interior.
        /// </param>
        /// <returns>A new BSP with only leaves which are interior, or are connected
        /// to interior spaces.</returns>
        public IBspTree CullOutside(IBspTree root,
            IEnumerable<Portal> portals,
            IEnumerable<TPoint> interiorPoints)
        {
            var result = new BspTree(root as BspTree);
            result.CullOutside(FindInteriorLeaves(root, portals, interiorPoints));
            return result;
        }

        HashSet<int> FindInteriorLeaves(IBspTree root,
            IEnumerable<Portal> portals, IEnumerable<TPoint> interiorPoints)
        {
            var leafCount = (root.NodeCount + 1) >> 1;

            var result = new HashSet<int>();
            foreach (var interiorPoint in interiorPoints)
            {
                MarkInteriorSpace(result, portals,
                    root.FindLeafContaining(interiorPoint));
                debug.InsideFilled(interiorPoint, result, leafCount);
            }
            return result;
        }

        static void MarkInteriorSpace(HashSet<int> interiorNodes,
            IEnumerable<Portal> portals, int leaf)
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

        /// <summary>
        /// Determine the size of connected sets of leaves.
        /// </summary>
        /// <param name="tree">The BSP tree to analyze.</param>
        /// <param name="portals">The portals connecting leaves.</param>
        /// <returns>A list of the sizes of subsets.</returns>
        public List<int> LeafSubsets(IBspTree tree, IEnumerable<Portal> portals)
        {
            var result = new List<int>();
            var foundNodes = new HashSet<int>();

            tree.InOrderTraverse(nodeIndex =>
            {
                if (!tree.IsLeaf(nodeIndex) || foundNodes.Contains(nodeIndex)) return;
                var startCount = foundNodes.Count;
                MarkInteriorSpace(foundNodes, portals, nodeIndex);
                var endCount = foundNodes.Count;
                result.Add(endCount - startCount);
            });

            result.Sort();
            return result;
        }
    }
}

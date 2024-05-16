using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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

        HashSet<BigInteger> FindInteriorLeaves(IBspTree root,
            IEnumerable<Portal> portals, IEnumerable<TPoint> interiorPoints)
        {
            var leafCount = (root.NodeCount + 1) >> 1;

            var result = new HashSet<BigInteger>();
            foreach (var interiorPoint in interiorPoints)
            {
                MarkInteriorSpace(result, portals,
                    root.FindLeafContaining(interiorPoint));
                debug.InsideFilled(interiorPoint, result, leafCount);
            }
            return result;
        }

        static void MarkInteriorSpace(HashSet<BigInteger> interiorNodes,
            IEnumerable<Portal> portals, BigInteger leaf)
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
            var foundNodes = new HashSet<BigInteger>();

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

        // =====================================================================================

        /// <summary>
        /// Adds missing points to facets to fix any T-joins (in 3D case).
        /// </summary>
        /// <param name="tree">The BSP tree to heal.</param>
        /// <param name="portals">The tree's portalization.</param>
        /// <returns>The healed tree.</returns>
        public IBspTree HealTIntersections(IBspTree tree, IEnumerable<Portal> portals)
        {
            var portalList = portals.ToList();
            var result = new BspTree(tree as BspTree);

            result.InOrderTraverse(index =>
            {
                if (!result.IsLeaf(index)) return;
                var leafSurfaces = result.Surfaces(index).Cast<BspSurface>();

                foreach (var portal in portalList
                    .Where(p => p.Front == index || p.Back == index))
                {
                    foreach (var surface  in leafSurfaces)
                    {
                        surface.Surface = surface.Surface.HealWith(portal.Facet);
                    }
                }

                result.RepalceLeaf(index, leafSurfaces);
            });

            return result;
        }
    }
}

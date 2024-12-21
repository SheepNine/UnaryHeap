using System;
using System.Collections.Generic;
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
            IEnumerable<Portal<TFacet>> portals,
            IEnumerable<TPoint> interiorPoints)
        {
            var result = new BspTree(root as BspTree);
            result.CullOutside(FindInteriorLeaves(root, portals, interiorPoints));
            return result;
        }

        HashSet<BigInteger> FindInteriorLeaves(IBspTree root,
            IEnumerable<Portal<TFacet>> portals, IEnumerable<TPoint> interiorPoints)
        {
            var leafCount = (root.NodeCount + 1) >> 1;

            var result = new HashSet<BigInteger>();
            foreach (var interiorPoint in interiorPoints)
            {
                MarkInteriorSpace(result, portals,
                    root.FindLeafContaining(interiorPoint));
                OnInsideFilled(interiorPoint, result, leafCount);
            }
            return result;
        }

        static void MarkInteriorSpace(HashSet<BigInteger> interiorNodes,
            IEnumerable<Portal<TFacet>> portals, BigInteger leaf)
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
    }
}

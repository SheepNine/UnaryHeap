using System;
using System.Collections.Generic;
using UnaryHeap.DataType;

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
        /// <returns>A new BSP with only leaves which are interior, or are connected
        /// to interior spaces.</returns>
        public BspNode CullOutside(BspNode root,
            IEnumerable<Portal> portals,
            IEnumerable<TPoint> interiorPoints)
        {
            // TODO: implement me
            return root;
        }
    }
}

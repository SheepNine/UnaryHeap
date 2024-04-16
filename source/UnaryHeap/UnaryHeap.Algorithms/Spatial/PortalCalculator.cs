using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet, TPoint>
        where TPlane : IEquatable<TPlane>
    {
        class Portalization
        {
            readonly IDimension dimension;
            List<Portal> portals;
            public IEnumerable<Portal> Portals { get { return portals; } }

            public Portalization(IDimension dimension, IEnumerable<Portal> initialPortals)
            {
                this.dimension = dimension;
                portals = initialPortals.ToList();
            }

            public void SplitCell(BspNode cell, TPlane splittingPlane,
                BspNode newFrontCell, BspNode newBackCell)
            {
                var cellPortals = new List<Portal>();
                var otherPortals = new List<Portal>();
                foreach (var portal in portals)
                {
                    if (portal.Front == cell || portal.Back == cell)
                        cellPortals.Add(portal);
                    else
                        otherPortals.Add(portal);
                }

                var splitFacet = dimension.Facetize(splittingPlane);
                foreach (var portal in cellPortals)
                {
                    if (portal.Front == cell)
                        dimension.Split(splitFacet, dimension.GetPlane(portal.Facet),
                            out splitFacet, out _);
                    else
                        dimension.Split(splitFacet, dimension.GetPlane(portal.Facet),
                            out _, out splitFacet);

                    if (splitFacet == null)
                        throw new InvalidOperationException(
                            "Split portal was clipped away by cell portals; should not happen");
                }
                var splitPortal = new Portal(splitFacet, newFrontCell, newBackCell);

                var splitCellPortals = cellPortals.SelectMany(cellPortal =>
                        SplitPortal(cellPortal, splittingPlane, cell, newFrontCell, newBackCell))
                    .ToList();

                portals = otherPortals
                    .Concat(splitCellPortals)
                    .Append(splitPortal)
                    .ToList();
            }

            private IEnumerable<Portal> SplitPortal(Portal portal, TPlane plane, BspNode cell,
                BspNode newFrontCell, BspNode newBackCell)
            {
                var result = new List<Portal>();
                dimension.Split(portal.Facet, plane, out TFacet frontFacet, out TFacet backFacet);
                if (frontFacet != null)
                {
                    result.Add(new Portal(frontFacet,
                        portal.Front == cell ? newFrontCell : portal.Front,
                        portal.Back == cell ? newFrontCell : portal.Back
                    ));
                }
                if (backFacet != null)
                {
                    result.Add(new Portal(backFacet,
                        portal.Front == cell ? newBackCell : portal.Front,
                        portal.Back == cell ? newBackCell : portal.Back
                    ));
                }
                return result;
            }
        }

        Portalization MakePortalization(BspNode root)
        {
            var bounds = dimension.Expand(CalculateBoundingBox(root));
            var boundsFacets = dimension.MakeFacets(bounds);

            return new Portalization(dimension,
                boundsFacets.Select(facet => new Portal(facet, root, null)));
        }

        /// <summary>
        /// Calculate the set of portals between BSP leaves.
        /// </summary>
        /// <param name="root">The BSP tree to portalize.</param>
        /// <param name="solidPredicate">Function to determine whether a surface is 'solid'
        /// and the back halfspace considered not part of a leaf.
        /// </param>
        /// <returns>The set of portals connecting the leaves of the BSP tree.</returns>
        public IEnumerable<Portal> Portalize(BspNode root, Func<TSurface, bool> solidPredicate)
        {
            var portalization = MakePortalization(root);

            root.PreOrderTraverse((node) =>
            {
                if (node.IsLeaf)
                {
                    var clipPlanes = node.Surfaces
                        .Where(solidPredicate)
                        .Select(s => dimension.GetPlane(dimension.GetFacet(s)))
                        .Distinct();
                    foreach (var plane in clipPlanes)
                        portalization.SplitCell(node, plane, node, null);
                }
                else
                {
                    portalization.SplitCell(node, node.PartitionPlane,
                        node.FrontChild, node.BackChild);
                }
            });

            return portalization.Portals.Where(p => p.Front != null && p.Back != null);
        }

        /// <summary>
        /// Calculate the bounding box of a BSP tree.
        /// </summary>
        /// <param name="tree">The tree for which to calculte bounds.</param>
        /// <returns>A bounding box containing all the surfaces in the tree.</returns>
        public TBounds CalculateBoundingBox(BspNode tree)
        {
            if (tree.IsLeaf)
            {
                return dimension.CalculateBounds(tree.Surfaces);
            }
            else
            {
                return dimension.UnionBounds(
                    CalculateBoundingBox(tree.FrontChild),
                    CalculateBoundingBox(tree.BackChild)
                );
            }
        }

        /// <summary>
        /// Represents a portal facet between two BSP leaves.
        /// </summary>
        public class Portal
        {
            /// <summary>
            /// The facet defining the portal.
            /// </summary>
            public TFacet Facet { get; private set; }

            /// <summary>
            /// The BSP leaf on the front side of the facet.
            /// </summary>
            public BspNode Front { get; private set; }

            /// <summary>
            /// The BSP leaf on the back side of the facet.
            /// </summary>
            public BspNode Back { get; private set; }

            /// <summary>
            /// Initializes a new instance of the Portal class.
            /// </summary>
            /// <param name="facet">The facet defining the portal.</param>
            /// <param name="front">The BSP leaf on the front side of the facet.</param>
            /// <param name="back">The BSP leaf on the back side of the facet.</param>
            public Portal(TFacet facet, BspNode front, BspNode back)
            {
                if (facet == null)
                    throw new ArgumentNullException(nameof(facet));
                Facet = facet;
                Front = front;
                Back = back;
            }
        }
    }
}

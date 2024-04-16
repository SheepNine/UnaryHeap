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

                portals = otherPortals
                    .Concat(cellPortals.SelectMany(cellPortal =>
                        SplitPortal(cellPortal, splittingPlane, cell, newFrontCell, newBackCell)))
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
                    result.Add(new Portal(frontFacet,
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
                boundsFacets.Select(facet => new Portal(facet, root, null))) ;
        }

        /// <summary>
        /// Calculate the set of portals between BSP leaves.
        /// </summary>
        /// <param name="root">The BSP tree to portalize.</param>
        /// <returns>The set of portals connecting the leaves of the BSP tree.</returns>
        public IEnumerable<Portal> Portalize(BspNode root)
        {
            var bounds = CalculateBoundingBox(root);
            var boundsFacets = dimension.MakeFacets(bounds);
            var endingPortals = FragmentPortals(root, Enumerable.Empty<Portal>(),
                boundsFacets.Select(dimension.GetPlane));
            return endingPortals;
        }

        IEnumerable<Portal> FragmentPortals(BspNode node,
            IEnumerable<Portal> startingPortals, IEnumerable<TPlane> parentSplittingPlanes)
        {
            if (node.IsLeaf)
            {
                var clipSurfaces = node.Surfaces.Select(
                        s => dimension.GetPlane(dimension.GetFacet(s))
                    ).Distinct();

                var splitPortals = startingPortals
                    .Where(p => p.Front == node || p.Back == node)
                    .Select(p => GetPortalRemainder(FaceTowards(p, node), clipSurfaces))
                    .Where(p => p != null);
                var portalsToIgnore = startingPortals
                    .Where(p => p.Front != node && p.Back != node).ToList();

                return portalsToIgnore.Concat(splitPortals).ToList();
            }
            else
            {
                var splitPortals = startingPortals.Where(p => p.Front == node || p.Back == node)
                    .SelectMany(portal => SplitAndReassign(portal, node)).ToList();
                var portalsToIgnore = startingPortals
                    .Where(p => p.Front != node && p.Back != node).ToList();

                var newPortalFacet = dimension.Facetize(node.PartitionPlane);
                foreach (var splitter in parentSplittingPlanes)
                {
                    if (newPortalFacet == null) break;
                    newPortalFacet = Clip(newPortalFacet, splitter);
                }
                if (newPortalFacet != null)
                    splitPortals.Add(new Portal(newPortalFacet, node.FrontChild, node.BackChild));

                var alpha = splitPortals.Concat(portalsToIgnore).ToList();
                var beta = FragmentPortals(node.FrontChild, alpha,
                    parentSplittingPlanes.Append(node.PartitionPlane));
                var gamma = FragmentPortals(node.BackChild, beta,
                    parentSplittingPlanes.Append(dimension.GetCoplane(node.PartitionPlane)));
                return gamma;
            }
        }

        private Portal FaceTowards(Portal portal, BspNode node)
        {
            return portal.Back == node
                ? new Portal(dimension.GetCofacet(portal.Facet), portal.Back, portal.Front)
                : portal;
        }

        private Portal GetPortalRemainder(Portal portal, IEnumerable<TPlane> clipSurfaces)
        {
            var facet = portal.Facet;
            foreach (var clipSurface in clipSurfaces)
            {
                if (facet == null)
                    break;
                facet = Clip(facet, clipSurface);
            }
            return facet == null ? null : new Portal(facet, portal.Front, portal.Back);
        }

        private IEnumerable<Portal> SplitAndReassign(Portal portal, BspNode node)
        {
            dimension.Split(portal.Facet, node.PartitionPlane,
                out TFacet frontFacet, out TFacet backFacet);
            return new[]
            {
                frontFacet == null ? null : new Portal(frontFacet,
                    portal.Front == node ? node.FrontChild : portal.Front,
                    portal.Back == node ? node.FrontChild : portal.Back),
                backFacet == null ? null : new Portal(backFacet,
                    portal.Front == node ? node.BackChild : portal.Front,
                    portal.Back == node ? node.BackChild : portal.Back)
            }.Where(portal => portal != null);
        }

        private TFacet Clip(TFacet facet, TPlane plane)
        {
            dimension.Split(facet, plane, out TFacet result, out _);
            return result;
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
                Facet = facet;
                Front = front;
                Back = back;
            }
        }
    }
}

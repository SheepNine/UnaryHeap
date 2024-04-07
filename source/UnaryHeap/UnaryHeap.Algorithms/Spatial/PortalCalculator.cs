using System;
using System.Collections.Generic;
using System.Linq;

namespace UnaryHeap.Algorithms
{
    public partial class Spatial<TSurface, TPlane, TBounds, TFacet>
        where TPlane : IEquatable<TPlane>
    {
        /// <summary>
        /// Calculate the set of portals between BSP leaves.
        /// </summary>
        /// <param name="root">The BSP tree to portalize.</param>
        /// <returns>The set of portals connecting the leaves of the BSP tree.</returns>
        public IEnumerable<Portal> Portalize(BspNode root)
        {
            var bounds = CalculateBoundingBox(root);
            var boundsFacets = dimension.MakeFacets(bounds);
            // TODO: forego facets; just make planes?
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

        private TBounds CalculateBoundingBox(BspNode root)
        {
            if (root.IsLeaf)
            {
                return dimension.CalculateBounds(root.Surfaces);
            }
            else
            {
                return dimension.UnionBounds(
                    CalculateBoundingBox(root.FrontChild),
                    CalculateBoundingBox(root.BackChild)
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

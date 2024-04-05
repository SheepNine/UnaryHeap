using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;
using UnaryHeap.Graph;

namespace Quake
{
    class Portalizer<TSurface, TPlane, TFacet, TBounds>
        : IEqualityComparer<TPlane> where TPlane : class
    {
        IDimension<TSurface, TPlane, TBounds, TFacet> dimension;

        public Portalizer(IDimension<TSurface, TPlane, TBounds, TFacet> dimension)
        {
            this.dimension = dimension;
        }

        public IEnumerable<Portal> Portalize(
            BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>.BspNode root)
        {
            var bounds = CalculateBoundingBox(root);
            var boundsFacets = dimension.MakeFacets(bounds);
            // TODO: forego facets; just make planes?
            var endingPortals = FragmentPortals(root, Enumerable.Empty<Portal>(),
                boundsFacets.Select(dimension.GetPlane));
            return endingPortals;
        }

        IEnumerable<Portal> FragmentPortals(
            BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>.BspNode node,
            IEnumerable<Portal> startingPortals, IEnumerable<TPlane> parentSplittingPlanes)
        {
            if (node.IsLeaf)
            {
                var clipSurfaces = node.Surfaces.Select(dimension.GetPlane).Distinct(this);

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
                    newPortalFacet = Clip(newPortalFacet, splitter);
                var newPortal = new Portal(newPortalFacet, node.FrontChild, node.BackChild);

                var alpha = splitPortals.Concat(portalsToIgnore).Append(newPortal).ToList();
                var beta = FragmentPortals(node.FrontChild, alpha,
                    parentSplittingPlanes.Append(node.PartitionPlane));
                var gamma = FragmentPortals(node.BackChild, beta,
                    parentSplittingPlanes.Append(dimension.GetCoplane(node.PartitionPlane)));
                return gamma;
            }
        }

        private Portal FaceTowards(Portal portal,
            BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>.BspNode node)
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

        private IEnumerable<Portal> SplitAndReassign(Portal portal,
            BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>.BspNode node)
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

        private TBounds CalculateBoundingBox(
            BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>.BspNode root)
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

        public bool Equals(TPlane x, TPlane y)
        {
            return dimension.Equals(x, y);
        }

        public int GetHashCode(TPlane obj)
        {
            return dimension.GetHashCode(obj);
        }

        public class Portal
        {
            public TFacet Facet { get; private set; }
            public BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>.BspNode
                Front { get; private set; }
            public BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>.BspNode
                Back { get; private set; }

            public Portal(TFacet facet,
                BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>.BspNode front,
                BinarySpacePartitioner<TSurface, TPlane, TBounds, TFacet>.BspNode back)
            {
                Facet = facet;
                Front = front;
                Back = back;
            }
        }
    }

    class GraphPortalizer : Portalizer<GraphSegment, Hyperplane2D, Facet2D, Orthotope2D>
    {
        public GraphPortalizer() : base(GraphDimension.Instance)
        {
        }
    }

    class QuakePortalizer : Portalizer<QuakeSurface, Hyperplane3D, Facet3D, Orthotope3D>
    {
        public QuakePortalizer() : base(QuakeDimension.Instance)
        {
        }
    }
}

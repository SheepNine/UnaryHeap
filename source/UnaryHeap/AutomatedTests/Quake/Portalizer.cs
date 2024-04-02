using AutomatedTests.Quake;
using System;
using System.Collections.Generic;
using System.Linq;
using UnaryHeap.Algorithms;
using UnaryHeap.DataType;
using UnaryHeap.Graph;

namespace Quake
{
    abstract class Portalizer<TSurface, TPlane, TFacet, TBounds>
    {
        public void Portalize(IBspNode<TSurface, TPlane> root)
        {
            var bounds = CalculateBoundingBox(root);
            var startingPortals = Facetize(bounds)
                .Select(facet => new Portal(facet, root, null)).ToList();
            var endingPortals = FragmentPortals(root, startingPortals,
                startingPortals.Select(p => GetPlane(p.Facet)));
        }

        IEnumerable<Portal> FragmentPortals(IBspNode<TSurface, TPlane> node,
            IEnumerable<Portal> startingPortals, IEnumerable<TPlane> parentSplittingPlanes)
        {
            if (node.IsLeaf)
            {
                // TODO: subdivide by node surfaces; setting back plane to undefined
                // e.g. p.Split(surface.Facet.Plane, node, null);
                throw new NotImplementedException();
            }
            else
            {
                var splitPortals = startingPortals.Where(p => p.Front == node || p.Back == node)
                    .SelectMany(portal => SplitAndReassign(portal, node)).ToList();
                var portalsToIgnore = startingPortals
                    .Where(p => p.Front != node || p.Back != node).ToList();

                var newPortalFacet = Facetize(node.PartitionPlane);
                foreach (var splitter in parentSplittingPlanes)
                    Split(newPortalFacet, splitter, out newPortalFacet, out _);
                var newPortal = new Portal(newPortalFacet, node.FrontChild, node.BackChild);

                var alpha = splitPortals.Concat(portalsToIgnore).Append(newPortal).ToList();
                var beta = FragmentPortals(node.FrontChild, alpha,
                    parentSplittingPlanes.Append(node.PartitionPlane));
                var gamma = FragmentPortals(node.BackChild, beta,
                    parentSplittingPlanes.Append(Coplane(node.PartitionPlane)));
                return gamma;
            }
        }

        private IEnumerable<Portal> SplitAndReassign(Portal portal,
            IBspNode<TSurface, TPlane> node)
        {
            Split(portal.Facet, node.PartitionPlane, out TFacet frontFacet, out TFacet backFacet);
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

        private TBounds CalculateBoundingBox(IBspNode<TSurface, TPlane> root)
        {
            if (root.IsLeaf)
            {
                return UnionBounds(
                    CalculateBoundingBox(root.FrontChild),
                    CalculateBoundingBox(root.BackChild)
                );
            }
            else
            {
                return CalculateBounds(root.Surfaces);
            }
        }

        protected abstract TBounds CalculateBounds(IEnumerable<TSurface> surfaces);
        protected abstract TBounds UnionBounds(TBounds a, TBounds b);
        protected abstract IEnumerable<TFacet> Facetize(TBounds bounds);
        protected abstract TFacet Facetize(TPlane plane);
        protected abstract TPlane Coplane(TPlane partitionPlane);
        protected abstract void Split(TFacet facet, TPlane splitter,
            out TFacet front, out TFacet back);
        protected abstract TPlane GetPlane(TFacet facet);

        class Portal
        {
            public TFacet Facet { get; private set; }
            public IBspNode<TSurface, TPlane> Front { get; private set; }
            public IBspNode<TSurface, TPlane> Back { get; private set; }

            public Portal(TFacet facet, IBspNode<TSurface, TPlane> front,
                IBspNode<TSurface, TPlane> back)
            {
                Facet = facet;
                Front = front;
                Back = back;
            }
        }
    }

    class GraphPortalizer : Portalizer<GraphSegment, Hyperplane2D, Facet2D, Orthotope2D>
    {
        protected override Orthotope2D CalculateBounds(IEnumerable<GraphSegment> surfaces)
        {
            return Orthotope2D.FromPoints(surfaces.Select(surface => surface.Facet.Start)
                .Concat(surfaces.Select(surface => surface.Facet.End)));
        }

        protected override Hyperplane2D Coplane(Hyperplane2D partitionPlane)
        {
            return partitionPlane.Coplane;
        }

        protected override IEnumerable<Facet2D> Facetize(Orthotope2D bounds)
        {
            return bounds.Facetize();
        }

        protected override Facet2D Facetize(Hyperplane2D plane)
        {
            return new Facet2D(plane, 100000);
        }

        protected override Hyperplane2D GetPlane(Facet2D facet)
        {
            return facet.Plane;
        }

        protected override void Split(Facet2D facet, Hyperplane2D splitter,
            out Facet2D front, out Facet2D back)
        {
            facet.Split(splitter, out front, out back);
        }

        protected override Orthotope2D UnionBounds(Orthotope2D a, Orthotope2D b)
        {
            return new Orthotope2D(
                Rational.Min(a.X.Min, b.X.Min),
                Rational.Min(a.Y.Min, b.Y.Min),
                Rational.Max(a.X.Max, b.X.Max),
                Rational.Max(a.Y.Max, b.Y.Max)
            );
        }
    }

    class QuakePortalizer : Portalizer<QuakeSurface, Hyperplane3D, Facet3D, Orthotope3D>
    {
        protected override Orthotope3D CalculateBounds(IEnumerable<QuakeSurface> surfaces)
        {
            return Orthotope3D.FromPoints(surfaces.SelectMany(surface => surface.Facet.Points));
        }

        protected override Orthotope3D UnionBounds(Orthotope3D a, Orthotope3D b)
        {
            return new Orthotope3D(
                Rational.Min(a.X.Min, b.X.Min),
                Rational.Min(a.Y.Min, b.Y.Min),
                Rational.Min(a.Z.Min, b.Z.Min),
                Rational.Max(a.X.Max, b.X.Max),
                Rational.Max(a.Y.Max, b.Y.Max),
                Rational.Max(a.Z.Max, b.Z.Max)
            );
        }

        protected override IEnumerable<Facet3D> Facetize(Orthotope3D bounds)
        {
            return bounds.Facetize();
        }

        protected override Facet3D Facetize(Hyperplane3D plane)
        {
            return new Facet3D(plane, 100000);
        }

        protected override Hyperplane3D Coplane(Hyperplane3D plane)
        {
            return plane.Coplane;
        }

        protected override void Split(Facet3D facet, Hyperplane3D splitter,
            out Facet3D front, out Facet3D back)
        {
            facet.Split(splitter, out front, out back);
        }

        protected override Hyperplane3D GetPlane(Facet3D facet)
        {
            return facet.Plane;
        }
    }
}
